// XDateTimes_Parsing

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
		// --- ParseDateTimeWithOffsetInfo / DateTimeStringHasOffset / etc  ---

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

			if(dtStr == null)
				return false;

			int len = dtStr.Length;
			if(len.NotInRange(14, 50)) {
				// MIN: `2018-01-01` is ~ 10 long, but certainly has no date
				// MAX: `Decemeber 28, 2018 23:32:32-05:00` is ~ 33 ish long 
				return false;
			}

			if(dtStr.Last() == 'Z')
				return true; // offset was already set to zero (0)

			bool hasColon = dtStr[len - 3] == ':';
			int tsStrLen = hasColon ? 6 : 5;

			char tsNegOrPosSign = dtStr[len - tsStrLen];

			if(tsNegOrPosSign != '-' && tsNegOrPosSign != '+')
				return false;

			bool signIsNeg = tsNegOrPosSign == '-';

			int tsStartIndex = len - tsStrLen + 1; // -1 to skip the +/- sign char

			string tsStr = dtStr.Substring(tsStartIndex);

			// Quick check for zero offset, this is likely a massive speedup for the 
			// innumerable cases (majority?!) where offset was UTC anyways
			if(hasColon) {
				if(tsStr == "00:00")
					return true;
			}
			else {
				if(tsStr == "0000")
					return true;
			}

			TimeSpan parsedTS = TimeSpan.Zero;

			int minutes = 0;
			if(!tsStr.EndsWith("00")) {
				minutes = tsStr.End(2).ToInt(-1);
				if(minutes.NotInRange(0, 59)) {
					// INVALID offset! see 'https://www.ietf.org/rfc/rfc2822.txt', 
					// "... and the zone MUST be within the range -9959 through + 9959."
					return false;
				}
			}
			int hours = tsStr.Substring(0, 2).ToInt(-1);

			if(hours.NotInRange(-14, 14)) {
				// rfc2822 allows up to 99 hours, but not DateTimeOffset:
				// https://msdn.microsoft.com/en-us/library/system.datetimeoffset.offset(v=vs.110).aspx
				// "The value of the Hours property of the returned TimeSpan object can range from 
				// -14 hours to 14 hours. The value of the Offset property is precise to the minute."

				return false;
			}

			double totalHours = minutes == 0
				? hours
				: hours + (((double)minutes) / 60);

			if(signIsNeg)
				totalHours = -totalHours;

			parsedTS = TimeSpan.FromHours(totalHours);

			if(parsedTS.TotalHours.NotInRange(-14, 14)) {
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
			dateStr = dateStr.NullIfEmptyTrimmed();

			if(dateStr.IsNulle())
				return (false, false, DateTimeOffset.MinValue);

			if(handleObsoleteUSTimeZones && HasObsoleteUSTimeZone(ref dateStr, out TimeSpan usOffset)) {
				// 1) if HasObsoleteUSTimeZone returned true, then it DOES have an offset
				// 2) dateStr now has had the time-zone stripped from it, so it is a normal dateTime, so
				// 3) can and MUST be parsed as a DateTime without any offset indicator 
				// (parsing with DateTimeOffset would do the horrible assume local time!)

				if(DateTime.TryParse(dateStr, out DateTime dt)) {
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

				if(offsetIndicated) {

					// if has an offset, GREAT! let DateTimeOffset parse 
					// the set offset, but we still need to convert it if it's offset was
					// UTC and the input localOffset or localTimeZone were set and were not UTC
					bool success = DateTimeOffset.TryParse(dateStr, out DateTimeOffset dto);

					if(success && dto.Offset == TimeSpan.Zero) {

						if(localOffset != null && localOffset.Value != TimeSpan.Zero) {

							dto = dto.ToDateTimeOffset(localOffset.Value, keepUtcTime: true);
							return (true, true, dto);
						}
						else if(localTimeZone != null && localTimeZone.BaseUtcOffset != TimeSpan.Zero) {

							dto = dto.ToDateTimeOffset(localTimeZone, keepUtcTime: true);
							return (true, true, dto);
						}
					}
					return (success, true, dto);
				}
				// ELSE: NO offset (going ff) -->
				// We HAVE a date-time with NO offset, DO parse with normal DateTime.Parse (not DateTimeOffset!)

				if(!DateTime.TryParse(dateStr, out DateTime dt)) {
					// DateTime is invalid, bye bye
					return (false, false, DateTimeOffset.MinValue);
				}

				if(localOffset != null) {

					DateTimeOffset result = dt.ToDateTimeOffset(
						localOffset.Value,
						isUtc: !treatNoOffsetAsLocalTime);

					return (true, false, result);
				}
				else if(localTimeZone != null) {

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
			if(len > 8) { // usually will be, so can do variable above
				if(
					//dateString[3] == ','
					//// note: This single first char check makes this function extremely performant on fails, 
					//// most non matches would fail at this point, making a length check and a single char check
					//// the main work done
					//&& dateString[4] == ' '
					dateString[len - 4] == ' '
					&& dateString.Last() == 'T') { // all these end with 'T' e.g. "EST", "PST", "EDT", etc

					string usTz = dateString.End(3); //.Substring(len - 3, 3);

					if(ObsoleteUSTimeZoneNamesAndOffsets.TryGetValue(usTz, out offset)) {
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

	}
}
