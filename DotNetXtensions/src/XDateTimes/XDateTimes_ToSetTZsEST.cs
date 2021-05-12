// XDateTimes_ToSetTZsEST

using System;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
		static TimeZoneInfo _tzi_EST;


		static TimeZoneInfo tzi_EST {
			get {
				// Linux OS probs, we could fix like we did w/Dnx.Globalization with multi-target,
				// but that's a big change to make for this fundamental lib, for this little need,
				// so for moment following works: allows 1 caught exception if not run on Windows
				if(_tzi_EST == null) {
					try {
						_tzi_EST = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
					}
					catch {
						_tzi_EST = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
					}
				}
				return _tzi_EST;
			}
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
