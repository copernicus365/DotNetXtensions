using System;
using System.Collections.Generic;

namespace DotNetXtensions.Globalization
{
	/// <summary>
	/// TimeZone helper, allowing conversions from the common TZ ids, to .NET / Windows
	/// <see cref="TimeZoneInfo"/>s.
	/// </summary>
	public class TimeZones
	{
		private static Dictionary<string, string> __TZDictionary;
		private static Dictionary<string, TZKeyValues> __WinDictionary;

		public static Dictionary<string, string> TZDictionary {
			get {
				if(__TZDictionary == null)
					Init();
				return __TZDictionary;
			}
		}
		public static Dictionary<string, TZKeyValues> WinDictionary {
			get {
				if(__WinDictionary == null)
					Init();
				return __WinDictionary;
			}
		}

		#region Initialize

		//public static void Initialize()
		//{
		//	if (TZDictionary == null || WinDictionary == null) {
		//		TimeZonesGenerator.GetDictionaries(out __WinDictionary, out __TZDictionary);
		//	}
		//}

		public static void Init()
		{
			TimeZonesGenerator.GetDictionaries(out __WinDictionary, out __TZDictionary);
		}

		#endregion

		public static TimeZoneInfo GetTimeZoneInfoFromTZId(string tzId)
		{
			if(tzId == null)
				return null;

#if NET
			if(OperatingSystem.IsWindows())
				return _Windows_GetTimeZoneInfoFromTZId(tzId);
			else {
				return TimeZoneInfo.FindSystemTimeZoneById(tzId);
			}
#else
			return _Windows_GetTimeZoneInfoFromTZId(tzId);
#endif
		}

		static TimeZoneInfo _Windows_GetTimeZoneInfoFromTZId(string tzId)
		{
			if(TZDictionary == null)
				Init();

			if(!TZDictionary.TryGetValue(tzId, out string winZone))
				return null;

			TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(winZone);
			return tz;
		}

		public static TZKeyValues GetTZValuesFromWindowsTimeZoneId(string winTZId)
		{
			if(winTZId == null)
				return null;
			if(WinDictionary == null)
				Init();

			if(WinDictionary.TryGetValue(winTZId, out TZKeyValues winZone))
				return winZone;

			return null;
		}

		public static string GetTZValueFromWindowsTimeZoneId(string winTZId)
		{
			var kv = GetTZValuesFromWindowsTimeZoneId(winTZId);
			return kv?.Value;
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
			if(tzi == null)
				return null;

			string tz = TimeZones.GetTZValueFromWindowsTimeZoneId(tzi.Id);
			return tz;
		}

		/// <summary>
		/// Gets the all TZ id values (often there is only one) for this TimeZoneInfo. 
		/// This is an indirection call to DotNetXtensions.Time.GetTZValuesFromWindowsTimeZoneId.
		/// </summary>
		/// <param name="tzi">TimeZoneInfo</param>
		public static TZKeyValues TZIdValues(this TimeZoneInfo tzi)
		{
			if(tzi == null)
				return null;
			return TimeZones.GetTZValuesFromWindowsTimeZoneId(tzi.Id);
		}
	}
}
