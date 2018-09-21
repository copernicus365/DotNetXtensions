using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

		/// <summary>
		/// Java / Unix time since Epoch converted to .NET ticks,
		/// see <see cref="JavaUnixTimeToDNetTicks(long)"/> and
		/// <see cref="SecondsSinceEpochToDNetTicks"/>.
		/// </summary>
		public const long TICKS_AT_EPOCH = 621355968000000000L;

		/// <summary>
		/// Converts Java Unix Time (milliseconds since Epoch) to
		/// .NET ticks (java.util.Date/Calendar, date.getTime()).
		/// From http://stackoverflow.com/a/29668694/264031
		/// </summary>
		/// <param name="value">Java Unix time since Epoch in milliseconds.</param>
		public static long JavaUnixTimeToDNetTicks(this long value)
			=> TICKS_AT_EPOCH + (value * 10000);

		public static long SecondsSinceEpochToDNetTicks(this long value)
			=> TICKS_AT_EPOCH + (value * 10000000);

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


		#region --- ParseDateTimeWithOffsetInfo / DateTimeStringHasOffset / etc  ---

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
			offset = TimeSpan.Zero;

			if (dtStr == null)
				return false;

			int len = dtStr.Length;
			if (len.NotInRange(14, 50)) {
				// MIN: `2018-01-01` is ~ 10 long, but certainly has no date
				// MAX: `Decemeber 28, 2018 23:32:32-05:00` is ~ 33 ish long 
				return false;
			}

			if (dtStr.Last() == 'Z')
				return true; // offset was already set to zero (0)

			bool hasColon = dtStr[len - 3] == ':';
			int tsStrLen = hasColon ? 6 : 5;

			char tsNegOrPosSign = dtStr[len - tsStrLen];

			if (tsNegOrPosSign != '-' && tsNegOrPosSign != '+')
				return false;

			bool signIsNeg = tsNegOrPosSign == '-';

			int tsStartIndex = len - tsStrLen + 1; // -1 to skip the +/- sign char

			string tsStr = dtStr.Substring(tsStartIndex);

			// Quick check for zero offset, this is likely a massive speedup for the 
			// innumerable cases (majority?!) where offset was UTC anyways
			if (hasColon) {
				if (tsStr == "00:00")
					return true;
			}
			else {
				if (tsStr == "0000")
					return true;
			}

			TimeSpan parsedTS = TimeSpan.Zero;

			int minutes = 0;
			if (!tsStr.EndsWith("00")) {
				minutes = tsStr.End(2).ToInt(-1);
				if (minutes.NotInRange(0, 59)) {
					// INVALID offset! see 'https://www.ietf.org/rfc/rfc2822.txt', 
					// "... and the zone MUST be within the range -9959 through + 9959."
					return false;
				}
			}
			int hours = tsStr.Substring(0, 2).ToInt(-1);

			if (hours.NotInRange(-14, 14)) {
				// rfc2822 allows up to 99 hours, but not DateTimeOffset:
				// https://msdn.microsoft.com/en-us/library/system.datetimeoffset.offset(v=vs.110).aspx
				// "The value of the Hours property of the returned TimeSpan object can range from 
				// -14 hours to 14 hours. The value of the Offset property is precise to the minute."

				return false;
			}

			double totalHours = minutes == 0
				? hours
				: hours + (((double)minutes) / 60);

			if (signIsNeg)
				totalHours = -totalHours;

			parsedTS = TimeSpan.FromHours(totalHours);

			if (parsedTS.TotalHours.NotInRange(-14, 14)) {
				// redundancy check. if true would be a programmatic error though bec of checks already above
				return false;
			}

			offset = parsedTS;
			return true;
		}

		/// <summary>
		/// Parses the datetime or datetime+offset string while indicating if the original 
		/// string had an offset. The offset can take the standard forms, such as: "+05:00" or "+0500",
		/// while a zero offset (`+00:00` / `+0000`) as well as a 'Z' appendix
		/// ('zulu' time) are also considered legitimate offsets.
		/// <para />
		/// Why is this needed? Because when parsing with DateTimeOffset.Parse, there is no way to know 
		/// if the string had an offset or not. The problem is, when there is no offset in the string, 
		/// the standard parsers (e.g. <see cref="DateTimeOffset.Parse(string)"/>) implicitly convert
		/// to the local computer's time. But in many (most?) cases, particularly on the server, you virtually never
		/// want the server's local time to count for anything. Unfortunately there is simply
		/// no way of knowing if there even was an offset or not, it just makes assumptions! 
		/// <para />
		/// Lastly, one might parse with <see cref="DateTime.Parse(string)"/> (as opposed to DateTimeOffset), 
		/// but that makes it's own assumptions as well. For instance, if an offset was indicated, 
		/// that is *converted* and the parsed time is actually changed to the local server time! 
		/// The worse part of this is you simply have no way of knowing if it had no offset leading
		/// to this implicity conversion. This function attempts to remedy these problems.
		/// </summary>
		/// <param name="dateStr">DateTime string to parse.</param>
		/// <param name="localOffset">Offset to use as the date-time's offset from UTC if no offset 
		/// was indicated, or if there was an offset but it was UTC offset. Should only have this or 
		/// <paramref name="localTimeZone"/> set, not both (this takes precedence).</param>
		/// <param name="localTimeZone">Time zone to use for the date-time's local time if 
		/// no offset was indicated, or if there was an offset but it was UTC offset. 
		/// Should only have this or <paramref name="localOffset"/> set, 
		/// not both (<paramref name="localOffset"/> takes precedence).</param>
		/// <param name="treatNoOffsetAsLocalTime">When no offset existed, 
		/// set this to true to treat the input date-time string as representing 
		/// a local time (the default), else will be treated as representing UTC time.
		/// Note that either <paramref name="localOffset"/> or <paramref name="localTimeZone"/> 
		/// must be set for this to matter, otherwise input date-time will be treated as 
		/// UTC time with NO offset at all, as this function will never use the 
		/// local computer's time-zone / offset.</param>
		/// <param name="handleObsoleteUSTimeZones">True to handle
		/// the obsolete US time-zones, ex. 'EST' in 'Jan 07, 2015 12:01:00 EST'
		/// (this is done with <see cref="HasObsoleteUSTimeZone(ref string, out TimeSpan)"/>).
		/// Such formats will throw an exception otherwise. Note that the check for this 
		/// is extremely performant.</param>
		public static (bool success, bool hadOffset, DateTimeOffset result)
			ParseDateTimeWithOffsetInfo(
			string dateStr,
			TimeSpan? localOffset = null,
			TimeZoneInfo localTimeZone = null,
			bool treatNoOffsetAsLocalTime = true,
			bool handleObsoleteUSTimeZones = false)
		{
			if (dateStr.IsNulle())
				return (false, false, DateTimeOffset.MinValue);

			if (handleObsoleteUSTimeZones && HasObsoleteUSTimeZone(ref dateStr, out TimeSpan usOffset)) {
				// 1) if HasObsoleteUSTimeZone returned true, then it DOES have an offset
				// 2) dateStr now has had the time-zone stripped from it, so it is a normal dateTime, so
				// 3) can and MUST be parsed as a DateTime without any offset indicator 
				// (parsing with DateTimeOffset would do the horrible assume local time!)

				if (DateTime.TryParse(dateStr, out DateTime dt)) {
					var result1 = new DateTimeOffset(dt, usOffset);
					return (true, true, result1);
				}
				// else: invalid datetime, fall through to FALSE return
			}
			else {
				bool offsetIndicated = DateTimeStringHasOffset(dateStr, out TimeSpan offset);
				// `out TimeSpan offset` <-- we never actually use this (perhaps ironically)
				// reason being, since there is an offset, we know DateTimeOffset.TryParse will get it
				// the key is however that since there is an offset, we know we can TRUST DateTimeOffset's 
				// final offset has not used local time

				if (offsetIndicated) {

					// if has an offset, GREAT! let DateTimeOffset parse 
					// the set offset, but we still need to convert it if it's offset was
					// UTC and the input localOffset or localTimeZone were set and were not UTC
					bool success = DateTimeOffset.TryParse(dateStr, out DateTimeOffset dto);

					if (success && dto.Offset == TimeSpan.Zero) {
						
						if (localOffset != null && localOffset.Value != TimeSpan.Zero) {

							dto = dto.ToDateTimeOffset(localOffset.Value, keepUtcTime: true);
							return (true, true, dto);
						}
						else if (localTimeZone != null && localTimeZone.BaseUtcOffset != TimeSpan.Zero) {

							dto = dto.ToDateTimeOffset(localTimeZone, keepUtcTime: true);
							return (true, true, dto);
						}
					}
					return (success, true, dto);
				}
				// ELSE: NO offset (going ff) -->
				// We HAVE a date-time with NO offset, DO parse with normal DateTime.Parse (not DateTimeOffset!)

				if (!DateTime.TryParse(dateStr, out DateTime dt)) {
					// DateTime is invalid, bye bye
					return (false, false, DateTimeOffset.MinValue);
				}

				if (localOffset != null) {

					DateTimeOffset result = dt.ToDateTimeOffset(
						localOffset.Value,
						isUtc: !treatNoOffsetAsLocalTime);

					return (true, false, result);
				}
				else if (localTimeZone != null) {

					TimeSpan offset1 = localTimeZone.GetUtcOffset(dt);

					DateTimeOffset result = dt.ToDateTimeOffset(
						offset1,
						isUtc: !treatNoOffsetAsLocalTime);

					return (true, false, result);
				}
				else {
					//dateStr += "+00:00";
					DateTimeOffset dto1 = dt.ToDateTimeOffset(
						TimeSpan.Zero,
						isUtc: true);

					return (true, false, dto1);
				}
			}

			return (false, false, DateTimeOffset.MinValue);
		}

		/// <summary>
		/// Performs a high-performance check to see if the input date string
		/// has one of the obsolete US TimeZone formats (must be registered in
		/// <see cref="ObsoleteUSTimeZoneNamesAndOffsets"/>), 
		/// e.g. `"Fri, 01 Jan 2014 12:00:01 EST"`. If so, the obsolete 3 letter ending is cut off,
		/// and <paramref name="offset"/> will be set accordingly. <paramref name="offset"/> 
		/// WILL always be set if return result was true.
		/// </summary>
		/// <remarks>
		/// As specified in 4.3 of https://www.ietf.org/rfc/rfc2822.txt: "Obsolete Date and Time":
		/// "The syntax for the obsolete date format ... allows for a list of alphabetic time 
		/// zone specifications that were used in earlier versions of this standard."
		/// </remarks>
		public static bool HasObsoleteUSTimeZone(ref string dateString, out TimeSpan offset)
		{
			offset = TimeSpan.Zero;
			int len = dateString?.Length ?? 0;
			if (len > 8) { // usually will be, so can do variable above
				if (
					//dateString[3] == ','
					//// note: This single first char check makes this function extremely performant on fails, 
					//// most non matches would fail at this point, making a length check and a single char check
					//// the main work done
					//&& dateString[4] == ' '
					dateString[len - 4] == ' '
					&& dateString.Last() == 'T') { // all these end with 'T' e.g. "EST", "PST", "EDT", etc

					string usTz = dateString.End(3); //.Substring(len - 3, 3);

					if (ObsoleteUSTimeZoneNamesAndOffsets.TryGetValue(usTz, out offset)) {
						dateString = dateString.CutEnd(4);
						return true;
					}
				}
			}
			return false;
		}

		// http://www.timeanddate.com/library/abbreviations/timezones/
		static readonly Dictionary<string, TimeSpan> ObsoleteUSTimeZoneNamesAndOffsets = new Dictionary<string, TimeSpan>() {
			{ "CDT", TimeSpan.FromHours(-5.0) },
			{ "CST", TimeSpan.FromHours(-6.0) },
			{ "EDT", TimeSpan.FromHours(-4.0) },
			{ "EST", TimeSpan.FromHours(-5.0) },
			{ "MDT", TimeSpan.FromHours(-6.0) },
			{ "MST", TimeSpan.FromHours(-7.0) },
			{ "PDT", TimeSpan.FromHours(-7.0) },
			{ "PST", TimeSpan.FromHours(-8.0) },
			{ "GMT", TimeSpan.Zero },
			//{ "UT", -0.0 } -- looks like our code was NOT accounting for length:2 usTzs, only 3
		};

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










		static long _minTicksForDateTimeIsSet = TimeSpan.FromDays(1).Ticks;

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
		public static DateTimeOffset ToDateTimeOffset(this DateTimeOffset dt, TimeZoneInfo tzInfo, bool keepUtcTime = true)
		{
			if (tzInfo != null && dt.Ticks > _minTicksForDateTimeIsSet) {
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

		static long _minDTTicks = DateTimeOffset.MinValue.Ticks;
		static long _maxDTTicks = DateTimeOffset.MaxValue.Ticks;

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
		public static DateTimeOffset ToDateTimeOffset(this DateTimeOffset dt, TimeSpan offset, bool keepUtcTime = true)
		{
			if (dt.Ticks <= _minTicksForDateTimeIsSet)
				return dt;

			long ticks = keepUtcTime
				? dt.UtcDateTime.Ticks + offset.Ticks
				: dt.DateTime.Ticks;

			if (ticks <= _minDTTicks)
				return DateTimeOffset.MinValue;
			else if (ticks >= _maxDTTicks)
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
			if (dt.Ticks <= _minTicksForDateTimeIsSet)
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