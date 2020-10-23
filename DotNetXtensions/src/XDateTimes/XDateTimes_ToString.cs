// XDateTimes_ToString

using System;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
		/// <summary>
		/// Simply returns dt.ToString("d").
		/// </summary>
		public static string ToShortDateString(this DateTimeOffset dt)
		{
			return dt.ToString("d");
			//public string ToShortDateString() {
			//    return DateTimeFormat.Format(this, "d", DateTimeFormatInfo.CurrentInfo);
		}

		/// <summary>
		/// Displays a nice, human readable time display, and 
		/// also favoring shorter displays ("Nov" instead of "November"),
		/// while also allowing a number of options about whether different
		/// parts of the time should be included in the display
		/// (e.g. whether to include seconds or not and so forth).
		/// </summary>
		public static string ToDateTimeString(this DateTimeOffset dt, bool est = false, bool time = true, bool secs = true, bool year = true, bool date = true, bool month = true, bool msecs = false)
		{
			if(est)
				dt = dt.ToEST();
			return dt.DateTime.ToDateTimeString(false, time, secs, year, date, month, msecs);
		}

		/// <summary>
		/// See overload.
		/// </summary>
		public static string ToDateTimeString(this DateTime dt, bool est = false, bool time = true, bool secs = true, bool year = true, bool date = true, bool month = true, bool msecs = false)
		{
			if(msecs) secs = true;
			if(est)
				dt = dt.ToEST();
			string v = null;

			if(date) {
				if(month)
					v = dt.ToString(year ? "MMM d, yyyy" : "MMM d");
				else
					v = dt.ToString("d"); // if no month, year doesn't make sense
			}

			if(time) {
				bool am = dt.Hour < 12;
				int hour = am ? dt.Hour : dt.Hour - 12;

				v += " " + hour.ToString("D") + ":" + dt.Minute.ToString("D2");

				if(secs) {
					v += ':' + dt.Second.ToString("D2");
					if(msecs)
						v += '.' + dt.Millisecond.ToString("D3");
				}

				v += " " + (am ? "AM" : "PM");
			}
			return v;
		}


		/// <summary>
		/// Gets an intelligent, human readable display of how long ago
		/// the one time is from now, for instance, if its 3 minutes ago, 
		/// it returns "3 minutes ago" (not 00:00:03.000 for instance).
		/// This is particularlly useful for many front facing human displays.
		/// ALPHA, could be worked on further, and, does not yet handle future 
		/// times.
		/// </summary>
		public static string TimeFromNow(this DateTime dt, DateTime? _now = null)
		{
			DateTime now = _now ?? DateTime.Now;

			if(dt >= now)
				return dt.ToString(); // just not finished yet

			TimeSpan ts = (now - dt);

			if(ts.Ticks < TimeSpan.TicksPerDay * 60) {
				if(ts.Ticks < TimeSpan.TicksPerHour)
					return ts.TotalMinutes.Round(2) + " minutes ago";
				if(ts.Ticks < TimeSpan.TicksPerDay)
					return ts.TotalHours.Round(2) + " hours ago";
				return ts.TotalDays.Round(2) + " days ago";
			}

			if(ts.Ticks < TimeSpan.TicksPerDay * 180)
				return (ts.TotalDays / 30.42).Round(2) + " months ago";
			return (ts.TotalDays / 365.25).Round(2) + " years ago";
		}




		// --- TimeSpan: Until we make a XTimeSpan.cs ... ---

		public static string ToTotalMinutesString(this TimeSpan ts)
		{
			string val = $"{((int)ts.TotalMinutes).ToString("00")}:{ts.Seconds.ToString("00")}.{ts.Milliseconds.ToString().SubstringMax(2)}";
			return val;
		}


		public static string ToStringBest(this TimeSpan ts, string apprxSymbol = "~ ", int roundPlace = 2, bool shortLabel = true)
		{
			double days = ts.TotalDays;
			if(days >= 1) {
				if(days > 30.41666) {
					if(days >= 365) {
						return apprxSymbol + (days / 365.24).Round(roundPlace) + (shortLabel ? "yr" : " years");
					}
					return apprxSymbol + (days / 30.4166).Round(roundPlace) + (shortLabel ? "mon" : " months");
				}
				return days.Round(roundPlace) + (shortLabel ? "dy" : " days");
			}

			double mins = ts.TotalMinutes;
			if(mins < 60) {
				if(mins < 1) {
					return ts.TotalSeconds.Round(roundPlace) + (shortLabel ? "s" : " seconds");
				}
				return mins.Round(roundPlace) + (shortLabel ? "m" : " minutes");
			}

			return ts.TotalHours.Round(roundPlace) + (shortLabel ? "h" : " hours");
		}


	}
}
