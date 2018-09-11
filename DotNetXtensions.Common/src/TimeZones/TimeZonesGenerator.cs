using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DotNetXtensions.Globalization
{
	public class TimeZonesGenerator
	{
		public static string URL_TZToWindowsZonesXml = "http://unicode.org/repos/cldr/trunk/common/supplemental/windowsZones.xml";

		public static XElement GetXML_TzToWindowsTimeZonesOnline()
		{
			XElement xml = XElement.Load(URL_TZToWindowsZonesXml, LoadOptions.None);
			return xml;
		}

		public static void GetValuesFromOnline(
			out TZKeyValues[] windowsTimeZones,
			out KeyValuePair<string, string>[] tzTimeZones)
		{
			var xml = GetXML_TzToWindowsTimeZonesOnline();
			GetValuesFromXml(xml, out windowsTimeZones, out tzTimeZones);
		}

		public static void GetValuesFromXml(
			XElement tzWindowsZonesXml,
			out TZKeyValues[] windowsTimeZones,
			out KeyValuePair<string, string>[] tzTimeZones)
		{
			XElement[] zones = tzWindowsZonesXml
				.Elements("windowsZones")
				.Elements("mapTimezones")
				.Elements("mapZone")
				.ToArray();

			if (zones == null || zones.Length == 0)
				throw new ArgumentException();

			string[] tzIds = zones.Select(e => {
				string val = e.Attribute("type").Value;
				if (e.Attribute("territory").Value == "001")
					val = "001_" + val;
				return val;
			}).ToArray();

			string[] winIds = zones.Select(e => e.Attribute("other").Value).ToArray();

			if (tzIds == null || winIds == null || tzIds.Length != winIds.Length)
				throw new ArgumentException();

			Array.Sort(tzIds, winIds);

			int len = tzIds.Length;
			var kvs = new List<TZKeyValues>(len + 50);
			string tzid = null;
			string winId = null;
			char[] spaceSeperator = { ' ' };

			for (int i = 0; i < len; i++) {

				winId = winIds[i];
				tzid = tzIds[i];

				if (tzid.IndexOf(' ') < 0)
					kvs.Add(new TZKeyValues(winId, tzid));
				else {
					string[] _tzIds = tzid.Split(spaceSeperator, StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < _tzIds.Length; j++)
						kvs.Add(new TZKeyValues(winId, _tzIds[j]));
				}
			}

			var kvsArr = kvs.OrderBy(v => v.Key).ToArray();

			Dictionary<string, bool> tempD = new Dictionary<string, bool>();
			List<string> group = null;
			int lastI = 0;
			for (int i = 1; i < kvsArr.Length; i++) {
				if (kvsArr[lastI].Key == kvsArr[i].Key) {
					if (group == null)
						group = new List<string>();
					group.Add(kvsArr[i].Value);
					kvsArr[i].Key = null;
				}
				else {
					if (group != null && group.Count > 0)
						kvsArr[lastI].Values = group.ToArray();
					lastI = i;
					group = null;
				}
			}

			if (group != null && group.Count > 0)
				kvsArr[kvsArr.Length - 1].Values = group.ToArray();

			kvsArr = kvsArr
				.Where(v => v.Key != null)
				.OrderBy(kv => kv.Key)
				.ToArray();

			for (int i = 0; i < kvsArr.Length; i++) {

				string val = kvsArr[i].Value;
				var values = kvsArr[i].Values;

				if (!val.StartsWith("001_"))
					throw new Exception();

				if (values == null)
					continue;

				if (values.Length == 0) {
					kvsArr[i].Values = null;
					continue;
				}

				if (values.FirstOrDefault(v => v.StartsWith("001_")) != null)
					throw new Exception();

				string orig001Value = val.Substring(4, val.Length - 4);
				for (int j = 0; j < values.Length; j++)
					if (values[j] == orig001Value)
						values[j] = null;

				kvsArr[i].Values = values
					.Where(x => x != null)
					.OrderBy(v => v)
					.Distinct()
					.ToArray();

				if (kvsArr[i].Values.Length == 0)
					kvsArr[i].Values = null;

				if (kvsArr[i].Values != null && kvsArr[i].Values.FirstOrDefault((x => x == null)) != null)
					throw new Exception();
			}

			for (int i = 0; i < kvsArr.Length; i++)
				kvsArr[i].Value = kvsArr[i].Value.Substring(4, kvsArr[i].Value.Length - 4);

			windowsTimeZones = kvsArr;
			tzTimeZones = GetTZTimeZonesFromWinZones(kvsArr);
		}

		public static string GetValuesToPersistStringFromXmlOnline()
		{
			TZKeyValues[] windowsTimeZones;
			KeyValuePair<string, string>[] tzTimeZones;
			GetValuesFromOnline(out windowsTimeZones, out tzTimeZones);
			string persistStr = ValuesToPersistString(windowsTimeZones);
			return persistStr;
		}

		public static string ValuesToPersistString(TZKeyValues[] windowsTimeZones)
		{
			if (windowsTimeZones == null)
				throw new ArgumentNullException();

			var sb = new StringBuilder(10000);
			foreach (var kv in windowsTimeZones) {
				sb.Append('-');
				sb.Append(kv.Key);
				sb.Append("\r\n");
				sb.Append(kv.Value);

				if (kv.Values != null) {
					foreach (var val in kv.Values) {
						sb.Append("\r\n");
						sb.Append(val);
					}
				}
				sb.Append("\r\n");
			}

			string result = sb.ToString();
			return result;
		}

		public static TZKeyValues[] ReadTimeZonesPersistStringToWindowsTimeZones(string timeZonesPersistString)
		{
			if (timeZonesPersistString == null)
				throw new ArgumentNullException();

			string[] lines = timeZonesPersistString.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			int count = lines.Length;
			var kvs = new List<TZKeyValues>(100);
			var vals = new List<string>(20);
			var currKv = new TZKeyValues();

			string winTz = null;
			for (int i = 0; i < count; i++) {
				string line = lines[i];

				if (line[0] != '-')
					throw new Exception();

				winTz = line.Substring(1, line.Length - 1);
				currKv = new TZKeyValues() { Key = winTz };
				kvs.Add(currKv);

				int c = 1;
				for (; i + c < count; c++) {
					line = lines[i + c];
					if (line[0] == '-')
						break;
					vals.Add(line);
				}

				if (vals.Count < 1) throw new Exception();
				if (vals.Count == 1)
					currKv.Value = vals[0];
				else
					currKv.Values = vals.ToArray();

				i += vals.Count;
				vals.Clear();
			}
			return kvs.ToArray();
		}

		public static KeyValuePair<string, string>[] GetTZTimeZonesFromWinZones(TZKeyValues[] windowsTimeZones)
		{
			var tzDict = new Dictionary<string, string>(440);
			var tzTimeZonesList = new List<KeyValuePair<string, string>>(440);

			foreach (var v in windowsTimeZones) {
				string key = v.Key;
				string[] values = v.Values;
				tzTimeZonesList.Add(new KeyValuePair<string, string>(v.Value, key));
				tzDict.Add(v.Value, key);
				if (values != null && values.Length > 0)
					foreach (var vl in values) {
						tzTimeZonesList.Add(new KeyValuePair<string, string>(vl, key));
						tzDict.Add(vl, key);
					}
			}
			var tzTimeZones = tzTimeZonesList.ToArray();
			return tzTimeZones;
		}

		public static Dictionary<string, string> GetTZTimeZonesDictionaryFromWinZones(TZKeyValues[] windowsTimeZones)
		{
			var tzDict = new Dictionary<string, string>(440);

			foreach (var v in windowsTimeZones) {
				string key = v.Key;
				string[] values = v.Values;
				tzDict.Add(v.Value, key);
				if (values != null && values.Length > 0)
					foreach (var vl in values) {
						tzDict.Add(vl, key);
					}
			}
			return tzDict;
		}

		public static Dictionary<string, TZKeyValues> GetWindowsTimeZonesDictionary(TZKeyValues[] windowsTimeZones)
		{
			var tzDict = new Dictionary<string, TZKeyValues>(100);

			foreach (var v in windowsTimeZones)
				tzDict.Add(v.Key, v);
			
			return tzDict;
		}

		public static void GetDictionaries(
			out Dictionary<string, TZKeyValues> winDict,
			out Dictionary<string, string> tzDict)
		{
			TZKeyValues[] windowsTimeZones;
			KeyValuePair<string, string>[] tzTimeZones;

			var winKeyVals = ReadTimeZonesPersistStringToWindowsTimeZones(TimeZonesString.TZString);

			tzDict = GetTZTimeZonesDictionaryFromWinZones(winKeyVals);
			winDict = GetWindowsTimeZonesDictionary(winKeyVals);
		}

	}
}
