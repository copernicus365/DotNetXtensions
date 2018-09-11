using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;

namespace DotNetXtensions.Json
{
	public class KeyNameMapper
	{
		/// <summary>
		/// Contains all keys that had one or more values.
		/// </summary>
		public Dictionary<string, List<string>> NamesToCustomDict { get; set; }

		/// <summary>
		/// Contains all key-values from <see cref="NamesToCustomDict"/> that 
		/// actually had more than one value.
		/// </summary>
		public Dictionary<string, List<string>> NamesToCustomDictWithMultiples { get; set; }

		/// <summary>
		/// All of the custom values stored in this dictionary as keys, with the original
		/// key listed as a value. This can be considered a reverse
		/// of <see cref="NamesToCustomDict"/>. But if there were any multiple-values per key,
		/// it will mean there will be that many extra keys in this dictionary.
		/// (Note that there can never be a value of the same name more than once).
		/// </summary>
		public Dictionary<string, string> CustomNamesDict { get; set; }

		/// <summary>
		/// All of the input keys stored in a dictionary.
		/// </summary>
		public Dictionary<string, bool> KeysDict { get; set; }

		/// <summary>
		/// Contains all keys that were marked to be ignored. This 
		/// can be indicated within a map-file by making the value of 
		/// a given key: "IGNORE" (upper-case).
		/// </summary>
		public Dictionary<string, bool> IgnoreKeys { get; set; }


		/// <summary>
		/// Contains any keys that *ever* appeared at all within the parsed map-file.
		/// This includes: <para />
		/// 1) any keys that had no value so that they were skipped, and <para />
		/// 2) any keys that were explicitly marked to be IGNOREd (see <see cref="IgnoreKeys"/>)
		/// <para />
		/// In both of those cases, the key never even registers with <see cref="NamesToCustomDict"/>
		/// and etc. The utility of persisting such ever-occurred in the map-file keys
		/// is when one needs to know which keys a user may need to have displayed to
		/// them as extra available key options.
		/// </summary>
		public Dictionary<string, bool> MapFileKeys { get; set; }

		/// <summary>
		/// Gets any keys from <see cref="KeysDict"/> which are not in
		/// <see cref="MapFileKeys"/> (see notes there).
		/// </summary>
		public string[] GetKeysNeverAppearedInMapFile() 
			=> KeysDict.Keys.Where(k => MapFileKeys.NoKey(k)).ToArray();

		/// <summary>
		/// (Currently in use, but note: this may need to be removed and make it 
		/// always case-sensitive, as JSON is).
		/// </summary>
		public bool IgnoreCase { get; set; }


		public KeyNameMapper() { }



		public (bool success, string reason) InitMap(string keysCommaSeparated, string map)
		{
			string[] keys = keysCommaSeparated?.SplitAndRemoveWhiteSpaceEntries(',').NullIfEmpty();
			return InitMap(keys, map);
		}

		public (bool success, string reason) InitMap(string[] keys, string map)
		{
			CustomNamesDict = null;

			if (map.IsNulle())
				throw new ArgumentNullException(nameof(map), "");

			if (keys.IsNulle())
				throw new ArgumentOutOfRangeException(nameof(keys), "");

			IgnoreKeys = new Dictionary<string, bool>();
			MapFileKeys = new Dictionary<string, bool>();
			KeysDict = keys.ToDictionaryIgnoreDuplicateKeys(k => k, k => false,
				comparer: IgnoreCase ? StringComparer.OrdinalIgnoreCase : null);

			Dictionary<string, bool> _anyKeysEver = new Dictionary<string, bool>();

			string[] lines = map.SplitLines(trimLines: true, removeEmptyLines: true);

			if (lines.IsNulle())
				return (false, "Null or empty input");

			NamesToCustomDict = new Dictionary<string, List<string>>(lines.Length);
			NamesToCustomDictWithMultiples = new Dictionary<string, List<string>>();

			int multipleNamesInitialLen = (int)(lines.Length * 1.2);
			CustomNamesDict = new Dictionary<string, string>(multipleNamesInitialLen);

			for (int i = 0; i < lines.Length; i++) {
				string line = lines[i];

				if (line.StartsWith("//"))
					continue; // ignore comment

				int commIdx = line.IndexOf("//");
				if (commIdx > 0) // don't worry about validating if // existed otherwise, alphanum checks still to come
					line = line.Substring(0, commIdx);

				string[] vals = line.Split(_colonSplit, StringSplitOptions.RemoveEmptyEntries);

				if (vals.Length != 2) {
					if (vals.Length == 1) {
						string _key = vals[0].NullIfEmptyTrimmed();

						if (_key == null)
							continue;

						else if (KeysDict.ContainsKey(_key)) {
							// key was already present with no value, ignore. 
							_anyKeysEver[_key] = true;
							continue;
						}
					}
					return (false, $"One of the lines is invalid, does not have a key-value set.");
				}

				string key = vals[0].NullIfEmptyTrimmed();
				string val = vals[1].NullIfEmptyTrimmed();

				if (key == null || key.Length > 15 || !key.IsAsciiAlphaNumeric())
					return (false, $"One of the keys is invalid ('{key?.SubstringMax(10, "...")}')");

				_anyKeysEver[key] = true;

				//if (NamesToCustomDict.Count == 0 // can't just check `i == 0` because first line(s) could have been comments (see above)
				//	&& removeTopKeyIfMatch.NotNulle()
				//	&& (IgnoreCase ? key.EqualsIgnoreCase(removeTopKeyIfMatch) : key == removeTopKeyIfMatch)) {
				//	TopKeyValue = (key, val);
				//	continue;
				//}

				if (!KeysDict.ContainsKey(key))
					return (false, $"Invalid (unrecognized) key ('{key}')");

				if (val == "IGNORE") { // MUST check this before duplicate key check below! CAN have a doubled duplicate in case of IGNORE
					IgnoreKeys[key] = true;
					continue;
				}

				if (NamesToCustomDict.ContainsKey(key))
					return (false, $"Duplicate key value encountered '{key}'");

				string badValueMsg = $"Value for key '{key}' is invalid (e.g. too long, or null, or invalid characters, etc)";

				if (val.IsNulle() || key == val)
					continue;

				if (val.Length > 256)
					return (false, badValueMsg);

				List<string> valsList = new List<string>();
				const int maxValLen = 30;

				if (key.Length == val.Length && (IgnoreCase ? key.EqualsIgnoreCase(val) : key == val))
					continue;

				if (val.IsAsciiAlphaNumeric()) {
					if (val.Length > maxValLen)
						return (false, badValueMsg);
					valsList.Add(val);
				}
				else {
					string[] mVals = val.SplitAndRemoveWhiteSpaceEntries('|'); //.Split(_barSplit);
					if (mVals == null || mVals.Length < 2)
						return (false, badValueMsg);

					for (int j = 0; j < mVals.Length; j++) {
						string v1 = mVals[j];
						if (v1.Length > maxValLen || !v1.IsAsciiAlphaNumeric())
							return (false, badValueMsg);
						valsList.Add(v1);
					}
				}

				bool isMultiValues = valsList.Count > 1;

				NamesToCustomDict.Add(key, valsList);
				if (isMultiValues)
					NamesToCustomDictWithMultiples.Add(key, valsList.ToList());

				for (int j = 0; j < valsList.Count; j++) { // string mVal in valsList)
					string mVal = valsList[j];
					if (CustomNamesDict.ContainsKey(mVal)) {
						return (false, $"Value {mVal.SubstringMax(15)} was entered more than once.");
					}

					CustomNamesDict.Add(mVal, key);
					//if (isMultiValues) {
					//	MultipleCustomNamesDict[mVal] = valsList.ToList(); //.Where((vv, idxx) => vv.NotNulle()).ToList();
					//}
				}
			}

			foreach (string key in KeysDict.Keys)
				if (_anyKeysEver.ContainsKey(key))
					MapFileKeys[key] = true;

			return (true, null);
		}

		static char[] _colonSplit = { ':' };
		static char[] _barSplit = { '|' };

	}
}