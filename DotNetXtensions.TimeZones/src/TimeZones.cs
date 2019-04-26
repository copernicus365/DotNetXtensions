using System;
using System.Collections.Generic;

namespace DotNetXtensions.Globalization
{
	public class TimeZones
	{
		private static Dictionary<string, string> __TZDictionary;
		private static Dictionary<string, TZKeyValues> __WinDictionary;

		public static Dictionary<string, string> TZDictionary
		{
			get
			{
				if (__TZDictionary == null)
					__Init();
				return __TZDictionary;
			}
		}
		public static Dictionary<string, TZKeyValues> WinDictionary
		{
			get
			{
				if (__WinDictionary == null)
					__Init();
				return __WinDictionary;
			}
		}

		#region Initialize

		public static void Initialize()
		{
			if (TZDictionary == null || WinDictionary == null) {
				TimeZonesGenerator.GetDictionaries(out __WinDictionary, out __TZDictionary);
			}
		}

		public static void __Init()
		{
			TimeZonesGenerator.GetDictionaries(out __WinDictionary, out __TZDictionary);
		}

		#endregion

		public static TimeZoneInfo GetTimeZoneInfoFromTZId(string tzId)
		{
			if (tzId == null)
				return null;
			if (TZDictionary == null)
				__Init();

			string winZone;
			if (!TZDictionary.TryGetValue(tzId, out winZone))
				return null;

			TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(winZone);
			return tz;
		}

		public static TZKeyValues GetTZValuesFromWindowsTimeZoneId(string winTZId)
		{
			if (winTZId == null)
				return null;
			if (WinDictionary == null)
				__Init();

			TZKeyValues winZone;
			if (!WinDictionary.TryGetValue(winTZId, out winZone))
				return null;

			return winZone;
		}

		public static string GetTZValueFromWindowsTimeZoneId(string winTZId)
		{
			var kv = GetTZValuesFromWindowsTimeZoneId(winTZId);
			return kv == null ? null : kv.Value;
		}

	}

	public static class TimeZonesX
	{
		/// <summary>
		/// Gets the first TZ Id value for this TimeZoneInfo. This is an indirection 
		/// call to DotNetXtensions.Time.GetTZValueFromWindowsTimeZoneId.
		/// </summary>
		/// <param name="tzi">TimeZoneInfo</param>
		public static string TZId(this TimeZoneInfo tzi)
		{
			if (tzi == null)
				return null;
			return TimeZones.GetTZValueFromWindowsTimeZoneId(tzi.Id);
		}

		/// <summary>
		/// Gets the all TZ id values (often there is only one) for this TimeZoneInfo. 
		/// This is an indirection call to DotNetXtensions.Time.GetTZValuesFromWindowsTimeZoneId.
		/// </summary>
		/// <param name="tzi">TimeZoneInfo</param>
		public static TZKeyValues TZIdValues(this TimeZoneInfo tzi)
		{
			if (tzi == null)
				return null;
			return TimeZones.GetTZValuesFromWindowsTimeZoneId(tzi.Id);
		}
	}
}
