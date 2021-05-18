// XDateTimes_ToSetTZsEST

using System;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
		static TimeZoneInfo _tzi_EST;


		static TimeZoneInfo tzi_EST {
			get {
				if(_tzi_EST == null)
					_tzi_EST = GetCurrentESTTimeZone();
				return _tzi_EST;
			}
		}

		/// <summary>
		/// To handle current OS-bound .NET limitations in getting time-zones.
		/// Could be handled like we did w/Dnx.Globalization using project multi-target,
		/// but that's a huge change for this puny function in this major library.
		/// Till we have bigger reasons for needing that, or at least the time,
		/// the following works: allows a single one time caught exception if not
		/// run on Windows.
		/// </summary>
		public static TimeZoneInfo GetCurrentESTTimeZone()
		{
			TimeZoneInfo tzi;
			try {
				tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
				//if(tzi.IsDaylightSavingTime(now)) ...
			}
			catch {
				tzi = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
			}
			return tzi;
		}

		/// <summary>
		/// Converts the DateTimeOffset to Eastern Standard Time.
		/// </summary>
		public static DateTimeOffset ToEST(this DateTimeOffset dt)
		{
			if(dt.Ticks > TimeSpan.TicksPerDay)
				return dt.ToDateTimeOffset(tzi_EST.GetUtcOffset(dt));
			return dt;
		}

		/// <summary>
		/// Converts the DateTime to Eastern Standard Time.
		/// </summary>
		public static DateTime ToEST(this DateTime dt)
		{
			if(dt.Ticks > TimeSpan.TicksPerDay)
				return dt.Add(tzi_EST.GetUtcOffset(dt)); // estOffset);
			return dt;
		}

		/// <summary>
		/// Converts the DateTime to Eastern Standard Time.
		/// </summary>
		public static string ToDateTimeStringEST(this DateTime dt, bool time = true, bool secs = true, bool msecs = false)
		{
			return XDateTimes.ToDateTimeString(dt, est: true, time: time, secs: secs, msecs: msecs);
		}

		public static string ToDateTimeStringEST(this DateTimeOffset dto, bool time = true, bool secs = true, bool msecs = false)
		{
			return XDateTimes.ToDateTimeString(dto, est: true, time: time, secs: secs, msecs: msecs);
		}
	}
}
