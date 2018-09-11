using System;
using System.Collections.Generic;
using System.Diagnostics;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Extension methods for DateTimes and TimeSpans.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XDateTimes
	{


		#region --- Min / Max ---

		public static DateTime Min(this DateTime val1, DateTime val2) { return val1 <= val2 ? val1 : val2; }

		public static DateTimeOffset Min(this DateTimeOffset val1, DateTimeOffset val2) { return val1 <= val2 ? val1 : val2; }

		public static DateTime Max(this DateTime val1, DateTime val2) { return val1 >= val2 ? val1 : val2; }

		public static DateTimeOffset Max(this DateTimeOffset val1, DateTimeOffset val2) { return val1 >= val2 ? val1 : val2; }

		public static TimeSpan Min(this TimeSpan val1, TimeSpan val2) { return val1 <= val2 ? val1 : val2; }

		public static TimeSpan Max(this TimeSpan val1, TimeSpan val2) { return val1 >= val2 ? val1 : val2; }

		#endregion

		public static bool InRange(this TimeSpan ts, TimeSpan min, TimeSpan max)
			=> ts >= min && ts <= max;

		public static bool InRange(this TimeSpan? ts, TimeSpan min, TimeSpan max)
			=> ts != null && ts >= min && ts <= max;

		public static bool InRangeForTimeOfDay(this TimeSpan ts)
			=> ts >= TimeSpan.Zero && ts < TimeSpan.FromHours(24);

		#region --- DateTime.Round ---



		/// <summary>
		/// Rounds the DateTime to the nearest specified interval.
		/// <para/>
		/// Thanks to DevSal on http://stackoverflow.com/questions/7029353/c-sharp-round-up-time-to-nearest-x-minutes.
		/// </summary>
		/// <param name="dt">DateTime to round.</param>
		/// <param name="roundBy">TimeSpan to round to.</param>
		public static DateTime Round(this DateTime dt, TimeSpan roundBy)
		{
			return new DateTime(_RoundTicks(roundBy, dt.Ticks));
		}

		/// <summary>
		/// Rounds the DateTimeOffset to the nearest specified interval.
		/// <para/>
		/// Thanks to DevSal on http://stackoverflow.com/questions/7029353/c-sharp-round-up-time-to-nearest-x-minutes.
		/// </summary>
		/// <param name="dt">DateTime to round.</param>
		/// <param name="roundBy">TimeSpan to round to.</param>
		public static DateTimeOffset Round(this DateTimeOffset dt, TimeSpan roundBy)
		{
			return new DateTimeOffset(_RoundTicks(roundBy, dt.Ticks), dt.Offset);
		}



		// http://stackoverflow.com/questions/7029353/how-can-i-round-up-the-time-to-the-nearest-x-minutes

		public static DateTime RoundUp(this DateTime dt, TimeSpan d)
		{
			long delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
			return new DateTime(dt.Ticks + delta, dt.Kind);
		}

		public static DateTimeOffset RoundUp(this DateTimeOffset dt, TimeSpan d)
		{
			long delta = (d.Ticks - (dt.Ticks % d.Ticks)) % d.Ticks;
			return new DateTimeOffset(dt.Ticks + delta, dt.Offset);
		}



		public static DateTime RoundDown(this DateTime dt, TimeSpan d)
		{
			long delta = dt.Ticks % d.Ticks;
			return new DateTime(dt.Ticks - delta, dt.Kind);
		}

		public static DateTimeOffset RoundDown(this DateTimeOffset dt, TimeSpan d)
		{
			long delta = dt.Ticks % d.Ticks;
			return new DateTimeOffset(dt.Ticks - delta, dt.Offset);
		}



		public static DateTime RoundToNearest(this DateTime dt, TimeSpan d)
		{
			long delta = dt.Ticks % d.Ticks;
			bool roundUp = delta > d.Ticks / 2;
			return roundUp ? dt.RoundUp(d) : dt.RoundDown(d);
		}

		public static DateTimeOffset RoundToNearest(this DateTimeOffset dt, TimeSpan d)
		{
			long delta = dt.Ticks % d.Ticks;
			bool roundUp = delta > d.Ticks / 2;
			return roundUp ? dt.RoundUp(d) : dt.RoundDown(d);
		}


		static long _RoundTicks(TimeSpan roundBy, long dtTicks)
		{
			long roundTicks = roundBy.Ticks;
			int f = 0;
			double m = (double)(dtTicks % roundTicks) / roundTicks;
			if (m >= 0.5)
				f = 1;
			long val = ((dtTicks / roundTicks) + f) * roundTicks;
			return val;
		}


		#endregion


		const long TICKS_AT_EPOCH = 621355968000000000L;

		/// <summary>
		/// Converts Java Unix Time (milliseconds since Epoch) to
		/// .NET ticks (java.util.Date/Calendar, date.getTime()).
		/// From http://stackoverflow.com/a/29668694/264031
		/// </summary>
		/// <param name="value">Java Unix time since Epoch in milliseconds.</param>
		public static long JavaUnixTimeToDNetTicks(this long value)
		{
			long ticks = TICKS_AT_EPOCH + (value * 10000);
			return ticks;
		}

		public static long SecondsSinceEpochToDNetTicks(this long value)
		{
			long ticks = TICKS_AT_EPOCH + (value * 10000000);
			return ticks;
		}

		//new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(issuedAt_unixTimeInSecs) 

		#region --- InRange / NotInRange 

		public static bool InRange(this DateTime dt, DateTime min, DateTime max)
		{
			return dt >= min && dt <= max;
		}
		public static bool InRange(this DateTime dt, DateTime min, TimeSpan timeAfterMin)
		{
			if (dt >= min) {
				DateTime max = min + timeAfterMin;
				return dt <= max;
			}
			return false;
		}
		public static bool NotInRange(this DateTime dt, DateTime min, DateTime max)
		{
			return !dt.InRange(min, max);
		}
		public static bool NotInRange(this DateTime dt, DateTime min, TimeSpan timeAfterMin)
		{
			return !dt.InRange(min, timeAfterMin);
		}

		public static bool InRange(this DateTimeOffset dt, DateTimeOffset min, DateTimeOffset max)
		{
			return dt >= min && dt <= max;
		}
		public static bool InRange(this DateTimeOffset dt, DateTimeOffset min, TimeSpan timeAfterMin)
		{
			if (dt >= min) {
				DateTimeOffset max = min + timeAfterMin;
				return dt <= max;
			}
			return false;
		}
		public static bool NotInRange(this DateTimeOffset dt, DateTimeOffset min, DateTimeOffset max)
		{
			return !dt.InRange(min, max);
		}
		public static bool NotInRange(this DateTimeOffset dt, DateTimeOffset min, TimeSpan timeAfterMin)
		{
			return !dt.InRange(min, timeAfterMin);
		}

		public static bool IsNulle(this DateTimeOffset? dt)
		{
			return dt == null || dt.Value == DateTimeOffset.MinValue;
		}

		public static bool NotNulle(this DateTimeOffset? dt)
		{
			return !dt.IsNulle();
		}

		public static DateTimeOffset SetTime(this DateTimeOffset dt, int? hours = null, int? mins = null, int? secs = null)
		{
			var time = new TimeSpan(hours ?? dt.Hour, mins ?? dt.Minute, secs ?? dt.Second);
			DateTime dt1 = dt.Date + time;
			return new DateTimeOffset(dt1, dt.Offset);
		}
		public static DateTime SetTime(this DateTime dt, int? hours = null, int? mins = null, int? secs = null)
		{
			var time = new TimeSpan(hours ?? dt.Hour, mins ?? dt.Minute, secs ?? dt.Second);
			DateTime dt1 = dt.Date + time;
			return dt1;
		}

		#endregion


		#region --- Parse DateTime Lenient ---

		public static DateTime ParseDateTimeLenientDefault(string date)
		{
			bool success = TryParseDateTimeLenient(date, out DateTime result);
			return result;
		}

		public static DateTime ParseDateTimeLenientDefault(string date, DateTime defaultTime)
		{
			return TryParseDateTimeLenient(date, out DateTime result)
				? result
				: defaultTime;
		}

		public static bool TryParseDateTimeLenient(string date, out DateTime result)
		{
			if (date == null) {
				result = DateTime.MinValue;
				return false;
			}
			if (DateTime.TryParse(date, out result))
				return true;
			return TryParseDateTimeLenient(date, out result);
		}

		public static bool TryParseDateTimeLenientAfterFail(string date, out DateTime result)
		{
			(string date1, double? usOffset) = TryParseDateTimeWithSpecialUSTZs(date);

			if (date1.NotNulle()) {
				if (DateTime.TryParse(date1, out result)) {
					if (usOffset != null)
						result = result.Add(TimeSpan.FromHours(usOffset.Value));
					return true;
				}
			}
			result = DateTime.MinValue;
			return false;
		}



		public static DateTimeOffset ParseDateTimeOffsetLenientDefault(string date)
		{
			bool success = TryParseDateTimeOffsetLenient(date, out DateTimeOffset result);
			return result;
		}

		public static DateTimeOffset ParseDateTimeOffsetLenientDefault(string date, DateTimeOffset defaultTime)
		{
			return TryParseDateTimeOffsetLenient(date, out DateTimeOffset result)
				? result
				: defaultTime;
		}

		public static bool TryParseDateTimeOffsetLenient(string date, out DateTimeOffset result)
		{
			if (date == null) {
				result = DateTime.MinValue;
				return false;
			}
			if (DateTimeOffset.TryParse(date, out result))
				return true;
			return TryParseDateTimeOffsetLenientAfterFail(date, out result);
		}

		public static bool TryParseDateTimeOffsetLenientAfterFail(string date, out DateTimeOffset result)
		{
			(string date1, double? usOffset) = TryParseDateTimeWithSpecialUSTZs(date);
			return _tryParseDateTimeOffset_WithUSTZInfo(date1, usOffset, out result);
		}

		static bool _tryParseDateTimeOffset_WithUSTZInfo(string date1, double? usOffset, out DateTimeOffset result)
		{
			if (date1.NotNulle()) {
				if (DateTime.TryParse(date1, out DateTime dt)) {
					if (usOffset != null)
						result = new DateTimeOffset(dt, TimeSpan.FromHours(usOffset.Value));
					else {
						// this may be bad, but this will give them at least the date, but the TZ lost
						result = new DateTimeOffset(dt, TimeSpan.Zero);
					}
					return true;
				}
			}
			result = DateTimeOffset.MinValue;
			return false;
		}


		public static (string date, double? usOffset) TryParseDateTimeWithSpecialUSTZs(string date)
		{
			int len = date?.Length ?? 0;
			if (len > 8) { // usually will be, so can do variable above
				if (date[3] == ','
					// note: This single first char check makes this function extremely performant on fails, 
					// most non matches would fail at this point, making a length check and a single char check
					// the main work done

					&& date[4] == ' '
					&& date[len - 4] == ' '
					&& date[len - 1] == 'T') {
					date = date.Substring(5, len - 5);
					len -= 5;

					string usTz = date.Substring(len - 3, 3);
					date = date.Substring(0, len - 4);
					if (usTz.Length == 3 // .InRange(2, 3) 
						&& date.NotNulle()) {

						return USTimeZoneDateTimeOffsets.TryGetValue(usTz, out double offset)
							? (date, offset)
							: (date, (double?)null);
					}
				}
			}
			return (null, null);
		}

		// http://www.timeanddate.com/library/abbreviations/timezones/
		static readonly Dictionary<string, double> USTimeZoneDateTimeOffsets = new Dictionary<string, double>() {
			{ "CDT", -5.0 },
			{ "CST", -6.0 },
			{ "EDT", -4.0 },
			{ "EST", -5.0 },
			{ "MDT", -6.0 },
			{ "MST", -7.0 },
			{ "PDT", -7.0 },
			{ "PST", -8.0 },
			{ "GMT", -0.0 },
			//{ "UT", -0.0 } -- looks like our code was NOT accounting for length:2 usTzs, only 3
		};

		/// <summary>
		/// Determines if the input date-time string has a offset value specified,
		/// either a "Z" zulu ending, indicating UTC, or a plus or minus offset at 
		/// the end (inclusive of a zero offset, e.g. `+00:00`). 
		/// Note that NO validation is done on the input string that it is in fact  
		/// a valid datetime (that is up to the caller), though a quick length range check is performed. 
		/// The purpose of this function is to answer in the most performant way possible  
		/// this single question alone. As for the need for this function, see the documetation of
		/// <see cref="ParseDateTimeWithOffsetInfo"/>, which calls this method.
		/// </summary>
		/// <param name="dtStr">DateTime string, which may or may not have an indicated offset.</param>
		/// <param name="offset">The string based offset</param>
		/// <returns></returns>
		public static bool DateTimeStringHasOffset(string dtStr, out TimeSpan offset)
		{
			if (dtStr != null) {
				int len = dtStr.Length;
				if (len > 10 && len < 40) { //2018-01-01 - len: 11, min for dt, 'Decemeber 28, 2018 23:32:32-05:00' is 33 ish before 
					if (dtStr[len - 1] == 'Z') {
						offset = TimeSpan.Zero;
						return true;
					}
					else if (dtStr[len - 3] == ':') {
						char tsSign = dtStr[len - 6];
						if (tsSign == '-' || tsSign == '+') {
							string tsStr = dtStr.Substring(len - (tsSign == '-' ? 6 : 5));
							if (TimeSpan.TryParse(tsStr, out TimeSpan ts)
								&& ts.InRange(TimeSpan.FromHours(-14.0), TimeSpan.FromHours(14.0))) {
								/*	Notes: https://msdn.microsoft.com/en-us/library/system.datetimeoffset.offset(v=vs.110).aspx
									The value of the Hours property of the returned TimeSpan object can range from -14 hours to 14 hours.
									The value of the Offset property is precise to the minute. */
								offset = ts;
								return true;
							}
						}
					}
				}
			}
			offset = TimeSpan.Zero;
			return false;
		}

		/// <summary>
		/// Parses the datetime or datetimeoffset string, while indicating if the original 
		/// string had an offset. Both a zero offset (+00:00) and a 'Z' ('zulu' time) appendix are considered
		/// an offset, i.e. an explicit UTC offset indicator. 
		/// Why is this needed? Because when parsing with DateTimeOffset.Parse, there is no way to know 
		/// if the string had a offset or not. The problem is, when there is no offset in the string, 
		/// it is parsed to local (server / computer) time. 
		/// But in many cases, particularly on the server, you virtually never
		/// want the server's local time to count for anything, and rather would use another timezone offset
		/// which you know from context. However, if you simply parse with DateTimeOffset, there is simply
		/// no way of know if it didn't find an offset, it just makes assumptions! Even if you specify
		/// the right assumption for it to make, UTC or Local, that's not good enough, you need to know
		/// if it was neither, in which case you'll be able to, for instance,
		/// supply your own timezone offset, etc.
		/// Lastly, one might parse with <see cref="DateTime.Parse(string)"/>, but that makes it's own 
		/// assumptions as well, again you won't know which was which. Even stranger is the fact, if 
		/// an offset was indicated, that is *converted* and the time changes accordingly 
		/// to the stinking local server time. It's understandable, but still frustrating, that
		/// for instance, indicating UTC time in the string actually makes the final result not UTC! 
		/// Most importantly though is that you simply have no way of knowing if that conversion was
		/// silently made. 
		/// </summary>
		/// <param name="dateStr">DateTime string</param>
		/// <param name="addOffsetIfNone">TimeSpan to add, if any, if no offset was indicated.</param>
		/// <returns>Returns if was success, if offset was indicated, and then for the parsed dtOffset,
		/// if offsetIndicated or if addOffsetIfNone was not null, returns the expected datetimeoffset with 
		/// expected offset. Otherwise, the returned value will be the exact time as parsed NOT converted
		/// to local time, i.e. while we use <see cref="DateTimeOffset.Parse(string)"/>, we in that case
		/// will return the UtcDateTime of the parsed value.</returns>
		public static (bool success, bool offsetIndicated, DateTimeOffset dtOffset)
			ParseDateTimeWithOffsetInfo(string dateStr, TimeSpan? addOffsetIfNone = null)
		{
			if (dateStr.NotNulle()) {

				// --- Try if had USTz DateTime --- 
				//    (don't worry, is like 1 char check in most cases for no-match)
				(string dt_hadUSTz, double? usOffset)
					= TryParseDateTimeWithSpecialUSTZs(dateStr);

				if (dt_hadUSTz.NotNulle()) {

					if (DateTime.TryParse(dt_hadUSTz, out DateTime dt)) {

						if (usOffset != null) {
							addOffsetIfNone = TimeSpan.FromHours(usOffset.Value);
						}
						else if (addOffsetIfNone == null)
							addOffsetIfNone = TimeSpan.Zero;

						var result1 = new DateTimeOffset(dt, TimeSpan.FromHours(usOffset.Value));

						return (true, usOffset != null, result1);
					}
					else
						return (false, false, DateTimeOffset.MinValue);
				}

				bool offsetIndicated = DateTimeStringHasOffset(dateStr, out TimeSpan offset);

				if (!offsetIndicated && addOffsetIfNone != null) {
					TimeSpan tsOffset = addOffsetIfNone.Value;
					if (!tsOffset.InRange(TimeSpan.FromHours(-14), TimeSpan.FromHours(14)))
						throw new ArgumentOutOfRangeException(nameof(addOffsetIfNone), "Offset is out of range");
					dateStr = dateStr + (tsOffset.Ticks < 0 ? '-' : '+') + tsOffset.ToString(@"hh\:mm"); //"+00:01";
				}

				if (DateTimeOffset.TryParse(dateStr, out DateTimeOffset dto)) {
					if (!offsetIndicated
						&& addOffsetIfNone == null
						&& dto.Offset != TimeSpan.Zero
						&& dto != DateTimeOffset.MinValue) {
						// if there was no offset and if none was added, and if the current offset is not already zero ...
						dto = dto.LocalDateTime.ToDateTimeOffset(0);
					}
					return (true, offsetIndicated, dto);
				}
			}
			return (false, false, DateTimeOffset.MinValue);
		}

		#endregion



		#region --- XmlTime ---

		[DebuggerStepThrough]
		public static string XmlTime(this DateTime dateTime, bool appendZ = true, bool dashesForColons = false)
		{
			string val = dateTime.ToString("s");
			if (appendZ)
				val += "Z";
			if (dashesForColons)
				val = val.Replace(':', '-');
			return val;
		}

		[DebuggerStepThrough]
		public static string XmlTime(this DateTimeOffset dateTime, bool appendZ = true, bool dashesForColons = false)
		{
			string val = dateTime.ToString("s");
			if (appendZ)
				val += "Z";
			if (dashesForColons)
				val = val.Replace(':', '-');
			return val;
		}

		#endregion


		#region --- TimeSpan.Multiply / Divide ---

		/// <summary>
		/// Multiplies a timespan by an integer value 
		/// (source: Stephen Hewlett: http://stackoverflow.com/a/14285561/264031)
		/// </summary>
		public static TimeSpan Multiply(this TimeSpan ts, int multiplier)
		{
			return TimeSpan.FromTicks(ts.Ticks * multiplier);
		}

		/// <summary>
		/// Multiplies a timespan by a double value
		/// </summary>
		public static TimeSpan Multiply(this TimeSpan ts, double multiplier)
		{
			return TimeSpan.FromTicks((long)(ts.Ticks * multiplier));
		}

		public static TimeSpan Multiply(this TimeSpan ts, TimeSpan multiplier)
		{
			return TimeSpan.FromTicks(ts.Ticks * multiplier.Ticks);
		}


		public static TimeSpan Divide(this TimeSpan ts, int divider)
		{
			return TimeSpan.FromTicks(ts.Ticks / divider);
		}

		public static TimeSpan Divide(this TimeSpan ts, double divider)
		{
			return TimeSpan.FromTicks((long)(ts.Ticks / divider));
		}

		public static TimeSpan Divide(this TimeSpan ts, TimeSpan divider)
		{
			return TimeSpan.FromTicks(ts.Ticks / divider.Ticks);
		}

		//public static int DivideCount(this TimeSpan ts, TimeSpan divider)
		//{
		//	return (int)(ts.Ticks / divider.Ticks);
		//}

		public static double DivideCount(this TimeSpan ts, TimeSpan divider)
		{
			return ts.TotalMilliseconds / divider.TotalMilliseconds;
		}

		public static int DivideRoundDown(this TimeSpan ts, TimeSpan divider)
		{
			double val = ts.TotalMilliseconds / divider.TotalMilliseconds;
			return (int)Math.Truncate(val);
		}

		public static int DivideRoundUp(this TimeSpan ts, TimeSpan divider)
		{
			double val = ts.TotalMilliseconds / divider.TotalMilliseconds;
			return (int)Math.Ceiling(val);
		}

		#endregion

		public static string ToTotalMinutesString(this TimeSpan ts)
		{
			string val = $"{((int)ts.TotalMinutes).ToString("00")}:{ts.Seconds.ToString("00")}.{ts.Milliseconds.ToString().SubstringMax(2)}";
			return val;
		}


		public static string ToStringBest(this TimeSpan ts, string apprxSymbol = "~ ", int roundPlace = 2, bool shortLabel = true)
		{
			double days = ts.TotalDays;
			if (days >= 1) {
				if (days > 30.41666) {
					if (days >= 365) {
						return apprxSymbol + (days / 365.24).Round(roundPlace) + (shortLabel ? "yr" : " years");
					}
					return apprxSymbol + (days / 30.4166).Round(roundPlace) + (shortLabel ? "mon" : " months");
				}
				return days.Round(roundPlace) + (shortLabel ? "dy" : " days");
			}

			double mins = ts.TotalMinutes;
			if (mins < 60) {
				if (mins < 1) {
					return ts.TotalSeconds.Round(roundPlace) + (shortLabel ? "s" : " seconds");
				}
				return mins.Round(roundPlace) + (shortLabel ? "m" : " minutes");
			}

			return ts.TotalHours.Round(roundPlace) + (shortLabel ? "h" : " hours");
		}


		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this DateTime dt, double offsetInHours = 0)
			=> ToDateTimeOffset(dt, offsetInHours == 0 ? TimeSpan.Zero : TimeSpan.FromHours(offsetInHours));

		/// <summary>
		/// Converts a DateTime to a DateTimeOffset, without risking any onerous exceptions
		/// the framework quite unfortunately throws within the DateTimeOffset constructor, 
		/// such as they do when the source DateTime's Kind is not set to UTC. The best and 
		/// most performant way around this, which we do herein, is to simply construct the 
		/// new DateTimeOffset with the overload that excepts Ticks. Also, we will simply 
		/// return <see cref="DateTimeOffset.MinValue"/> if the source DateTime was 
		/// <see cref="DateTime.MinValue"/>.
		/// </summary>
		/// <param name="dt">Source DateTime.</param>
		/// <param name="offset">Offset</param>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dt, TimeSpan offset)
		{
			// adding negative offset to a min-datetime will throw, this is a 
			// sufficient catch. Note however that a DateTime of just a few hours can still throw
			if (dt == DateTime.MinValue)
				return DateTimeOffset.MinValue;

			return new DateTimeOffset(dt.Ticks, offset);
		}



		#region --- ToEst ---

		static TimeZoneInfo tzi_EST = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

		/// <summary>
		/// Converts the DateTimeOffset to Eastern Standard Time.
		/// </summary>
		public static DateTimeOffset ToEST(this DateTimeOffset dt)
		{
			if (dt.Ticks > TimeSpan.TicksPerDay)
				return dt.ToDateTimeOffset(tzi_EST.GetUtcOffset(dt));
			return dt;
		}

		/// <summary>
		/// Converts the DateTime to Eastern Standard Time.
		/// </summary>
		public static DateTime ToEST(this DateTime dt)
		{
			if (dt.Ticks > TimeSpan.TicksPerDay)
				return dt.Add(tzi_EST.GetUtcOffset(dt)); // estOffset);
			return dt;
		}

		#endregion


















		#region -- EST --

		///// <summary>
		///// Converts the DateTimeOffset to Eastern Standard Time.
		///// </summary>
		//public static DateTimeOffset ToEST_OLD(this DateTimeOffset dt)
		//{
		//	if(dt.Ticks > TimeSpan.TicksPerDay)
		//		return dt.Add(tzi_EST.GetUtcOffset(dt)); //estOffset);
		//	return dt;
		//}

		/// <summary>
		/// Converts the DateTime to Eastern Standard Time.
		/// </summary>
		public static string ToDateTimeStringEST(this DateTime dt, bool time = true, bool secs = true, bool msecs = false)
		{
			return ToDateTimeString(dt, est: true, time: time, secs: secs, msecs: msecs);
		}

		#endregion

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
			if (est)
				dt = dt.ToEST();
			return dt.DateTime.ToDateTimeString(false, time, secs, year, date, month, msecs);
		}

		/// <summary>
		/// See overload.
		/// </summary>
		public static string ToDateTimeString(this DateTime dt, bool est = false, bool time = true, bool secs = true, bool year = true, bool date = true, bool month = true, bool msecs = false)
		{
			if (msecs) secs = true;
			if (est)
				dt = dt.ToEST();
			string v = null;

			if (date) {
				if (month)
					v = dt.ToString(year ? "MMM d, yyyy" : "MMM d");
				else
					v = dt.ToString("d"); // if no month, year doesn't make sense
			}

			if (time) {
				bool am = dt.Hour < 12;
				int hour = am ? dt.Hour : dt.Hour - 12;

				v += " " + hour.ToString("D") + ":" + dt.Minute.ToString("D2");

				if (secs) {
					v += ':' + dt.Second.ToString("D2");
					if (msecs)
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

			if (dt >= now)
				return dt.ToString(); // just not finished yet

			TimeSpan ts = (now - dt);

			if (ts.Ticks < TimeSpan.TicksPerDay * 60) {
				if (ts.Ticks < TimeSpan.TicksPerHour)
					return ts.TotalMinutes.Round(2) + " minutes ago";
				if (ts.Ticks < TimeSpan.TicksPerDay)
					return ts.TotalHours.Round(2) + " hours ago";
				return ts.TotalDays.Round(2) + " days ago";
			}

			if (ts.Ticks < TimeSpan.TicksPerDay * 180)
				return (ts.TotalDays / 30.42).Round(2) + " months ago";
			return (ts.TotalDays / 365.25).Round(2) + " years ago";
		}










		static long minTicksForDateTimeIsSet = TimeSpan.FromDays(1).Ticks;

		/// <summary>
		/// Generates a new DateTimeOffset which by default (when <paramref name="keepUtcTime" /> 
		/// is left TRUE) will have the exact same UTC time as the input 
		/// dt.UtcDateTime value, but with a new Offset derived from the TimeZoneInfo. In other words,
		/// the 'local' time (dt.DateTime, but please ignore dt.LocalDateTime) is what changes,
		/// while the UtcDateTime remains the same. Since DateTimeOffset equality is based on UTC, 
		/// the return value in that case will always be equal to the input value. 
		/// <para />
		/// Otherwise, if <paramref name="keepUtcTime" /> is set to false,
		/// the current time (dt.DateTime) is the value that will remain unchanged 
		/// while the UtcDateTime is the value that will change.
		/// </summary>
		/// <param name="dt">Input DateTimeOffset value.</param>
		/// <param name="tzInfo">The TimeZoneInfo to get a new Offset from.</param>
		/// <param name="keepUtcTime">True to keep the same UtcDateTime value 
		/// (so the DateTime current value will change), 
		/// false to keep the current DateTime value (the UtcDateTime value will change).</param>
		//[Obsolete("Thinking this should be replaced with methods that only send in the desired DateTime and offset, this gets too confusing.")]
		public static DateTimeOffset ToDateTimeOffset(this DateTimeOffset dt, TimeZoneInfo tzInfo, bool keepUtcTime = true)
		{
			if (tzInfo != null && dt.Ticks > minTicksForDateTimeIsSet) {
				return ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), keepUtcTime);
			}
			return dt;
		}

		//TimeZoneIfOffsetMissing.GetUtcOffset(dtOffset.UtcDateTime)



		//public static DateTimeOffset ChangeUtcTimeKeepLocalTime(this DateTimeOffset dt, TimeZoneInfo tzInfo)
		//{
		//	if (tzInfo != null && dt.Ticks > minTicksForDateTimeIsSet) {
		//		return ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), keepUtcTime: false);
		//	}
		//	return dt;
		//}
		//public static DateTimeOffset ChangeLocalTimeKeepUtcTime(this DateTimeOffset dt, TimeZoneInfo tzInfo)
		//{
		//	if (tzInfo != null && dt.Ticks > minTicksForDateTimeIsSet) {
		//		return ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), keepUtcTime: true);
		//	}
		//	return dt;
		//}

		static long minDTTicks = DateTimeOffset.MinValue.Ticks;
		static long maxDTTicks = DateTimeOffset.MaxValue.Ticks;

		/// <summary>
		/// Generates a new DateTimeOffset which by default (when <paramref name="keepUtcTime" /> 
		/// is left TRUE) will have the exact same UTC time as the input 
		/// dt.UtcDateTime value, but with a new Offset value. In other words,
		/// the 'local' time (dt.DateTime, but please ignore dt.LocalDateTime) is what changes,
		/// while the UtcDateTime remains the same. Since DateTimeOffset equality is based on UTC, 
		/// the return value in that case will always be equal to the input value. 
		/// <para />
		/// Otherwise, if <paramref name="keepUtcTime" /> is set to false,
		/// the current time (dt.DateTime) is the value that will remain unchanged 
		/// while the UtcDateTime is the value that will change. This is what
		/// DateTimeOffset.ToOffset actually does, but unfortunately, the intellisense
		/// documentation is obscure on this point.
		/// </summary>
		/// <param name="dt">Input DateTimeOffset value.</param>
		/// <param name="offset">The new Offset.</param>
		/// <param name="keepUtcTime">True to keep the same UtcDateTime value 
		/// (so the DateTime current value will change), 
		/// false to keep the current DateTime value (the UtcDateTime value will change).</param>
		//[Obsolete("Thinking this should be replaced with methods that only send in the desired DateTime and offset, this gets too confusing.")]
		public static DateTimeOffset ToDateTimeOffset(this DateTimeOffset dt, TimeSpan offset, bool keepUtcTime = true)
		{
			if (dt.Ticks <= minTicksForDateTimeIsSet)
				return dt;

			long ticks = keepUtcTime
				? dt.UtcDateTime.Ticks + offset.Ticks
				: dt.DateTime.Ticks;

			if (ticks <= minDTTicks)
				return DateTimeOffset.MinValue;
			else if (ticks >= maxDTTicks)
				return DateTimeOffset.MaxValue;

			DateTimeOffset val = new DateTimeOffset(ticks, offset);
			return val;
		}



		/// <summary>
		/// Converts input DateTime to a new DateTimeOffset with the specified Offset value. 
		/// Importantly, the Kind property on the input DateTime *is ignored* 
		/// (the frameworks terribly throws an exception if <c>dt.Kind == DateTimeKind.Utc</c>; 
		/// for purist reasons, but terribly inconvenient).
		/// Set the <paramref name="isUtc"/> value (default is true) to indicate if the input DateTime is
		/// already UTC time or if it should be treated as already a local time (any non-UTC time). So if 
		/// <paramref name="isUtc"/> is false, the returned d.DateTime will equal the input 
		/// value, else d.UtcDateTime will equal the input value.
		/// </summary>
		/// <param name="dt">DateTime</param>
		/// <param name="offset">Offset</param>
		/// <param name="isUtc">Indicates if the input DateTime is already a UTC value 
		/// or else a Local value (any time that is not UTC).</param>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dt, TimeSpan offset, bool isUtc = true)
		{
			if (dt.Ticks <= minTicksForDateTimeIsSet)
				return dt;

			long ticks = isUtc
				? dt.Ticks + offset.Ticks // dt is utc
				: dt.Ticks; // dt is local time

			if (ticks < 0)
				return DateTimeOffset.MinValue;

			DateTimeOffset val = new DateTimeOffset(ticks, offset);
			return val;
		}

		public static DateTimeOffset ConvertLocalTimeToDateTimeOffset(this DateTime dt, TimeZoneInfo tzInfo)
			=> ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), isUtc: false);

		public static DateTimeOffset ConvertUtcTimeToDateTimeOffset(this DateTime dt, TimeZoneInfo tzInfo)
			=> ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), isUtc: true);

		public static DateTimeOffset ToDateTimeOffsetNow(this TimeZoneInfo tzi)
		{
			if (tzi != null)
				return DateTimeOffset.UtcNow.ToDateTimeOffset(tzi, keepUtcTime: true);
			return DateTimeOffset.UtcNow;
		}

		public static DateTime ToTZTimeFromUtc(this DateTime dt, TimeZoneInfo tzi)
			=> tzi.ToTZTimeFromUtc(dt);

		public static DateTime ToTZTimeFromUtc(this TimeZoneInfo tzi, DateTime dt)
		{
			if (dt == DateTime.MinValue)
				return DateTime.MinValue;
			if (tzi == null)
				throw new ArgumentNullException(nameof(tzi));

			dt = TimeZoneInfo.ConvertTimeFromUtc(dt, tzi);
			return dt;
		}

		public static DateTime ToUtcTimeFromTZ(this DateTime dt, TimeZoneInfo tzi)
		{
			////return TimeZoneInfo.ConvertTimeToUtc(dt, tzi);
			// this throws the hideous exception: System.ArgumentException: 'The conversion could not 
			// be completed because the supplied DateTime did not have the Kind property set correctly. 
			// For example, when the Kind property is DateTimeKind.Local, the source time zone must be TimeZoneInfo.Local. Parameter name: sourceTimeZone'
			return dt.ToUtcTimeOffsetFromTZ(tzi).UtcDateTime;
		}

		public static DateTimeOffset ToUtcTimeOffsetFromTZ(this DateTime dt, TimeZoneInfo tzi)
		{
			if (tzi == null)
				throw new ArgumentNullException(nameof(tzi));
			DateTimeOffset dto = dt.ToDateTimeOffset(tzi.GetUtcOffset(dt), isUtc: false);
			return dto;
		}

		public static DateTimeOffset ToDateTimeOffsetNow(this TimeSpan offset)
		{
			return DateTimeOffset.UtcNow.ToOffset(offset);
		}

	}
}