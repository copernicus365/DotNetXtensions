using System;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Globalization;
using DotNetXtensions.Test;

namespace DNX.Test.DateTimes
{
	public class XDateTimes_DateTimeParsing : DnxTestBase
	{
		[Fact]
		public void Test_DateTimeStringHasOffset()
		{
			(string dateStr, bool hasOffset, TimeSpan offset)[] dateStrings = {
				("Sun, 09 Sep 2014 16:36:26 +0000", true, TimeSpan.Zero),
				("Sun, 09 Sep 2014 16:36:26 +0400", true, TimeSpan.FromHours(4)),
				("Supercalifrag", false, TimeSpan.Zero),
				("Decemeber 28, 2014 23:32:32-05:00", true, TimeSpan.FromHours(-5)),
				("Sun, 09 Sep 2014 16:36:26 +1500", false, TimeSpan.Zero), // is over +-14
				("Sun, 09 Sep 2014 16:36:26 -1500", false, TimeSpan.Zero), // is over +-14
				("Jan 28, 2014 23:32:32+00:00", true, TimeSpan.Zero),
				("Jan 28, 2014 23:32:32+11:22", true, new TimeSpan(11, 22, 0)),
				("Jan 28, 2014 23:32:32-11:22", true, new TimeSpan(11, 22, 0).Negate()),
			};

			for(int i = 0; i < dateStrings.Length; i++) {
				(string dateStr, bool hasOffset, TimeSpan expectedOffset) = dateStrings[i];

				bool hadOffset = XDateTimes.DateTimeStringHasOffset(dateStr, out TimeSpan offset);

				Assert.True(hadOffset == hasOffset);
				Assert.True(offset == expectedOffset);
			}
		}

		[Fact]
		public void Test_ParseDateTimeWithOffsetInfo()
		{
			var argsArray = ParseDateTimeWithOffsetInfoArgs.GetTestArgs();

			for(int i = 0; i < argsArray.Length; i++) {
				ParseDateTimeWithOffsetInfoArgs args = argsArray[i];

				args.AssertIsValid();
			}
		}


		class ParseDateTimeWithOffsetInfoArgs
		{
			public string testDesc;
			public bool shouldFail;
			public string dateStr;
			public bool hasOffset;
			public TimeSpan offset;
			public TimeSpan? localOffset;
			public TimeZoneInfo localTimeZone;
			public bool treatNoOffsetAsLocalTime = true;
			public bool handleObsoleteUSTimeZones = true;
			public DateTimeOffset expectedDTO;

			public ParseDateTimeWithOffsetInfoArgs(
				string dateStr,
				TimeSpan? offset = null,
				bool? hasOffset = null)
			{
				this.dateStr = dateStr;
				this.hasOffset = hasOffset != null
					? hasOffset.Value
					: offset != null;
				if(offset != null)
					this.offset = offset.Value;
			}

			public bool AssertIsValid()
			{
				ParseDateTimeWithOffsetInfoArgs args = this;

				try {
					(bool success, bool hadOffsetRes, DateTimeOffset result) = XDateTimes.ParseDateTimeWithOffsetInfo(
						dateStr,
						localOffset,
						localTimeZone,
						treatNoOffsetAsLocalTime,
						handleObsoleteUSTimeZones);

					PrintTestResult(success, hadOffsetRes, result);

					Assert.True(shouldFail ? !success : success);

					Assert.True(hasOffset == hadOffsetRes);

					if(hasOffset)
						Assert.True(offset == result.Offset);

					if(expectedDTO > DateTimeOffset.MinValue)
						Assert.True(expectedDTO == result && expectedDTO.ToString() == result.ToString());
				}
				catch(Exception ex) {
					Assert.True(
						shouldFail &&
						!handleObsoleteUSTimeZones); // <- latter is only one that should have actually thrown exception
				}
				return true;
			}

			public string PrintTestResult(bool success, bool hadOffsetRes, DateTimeOffset result)
			{
				string print = $@" --- {nameof(ParseDateTimeWithOffsetInfoArgs)} {(shouldFail ? "SHOULD FAIL!" : "")}---
-- dateStr: `{dateStr}`, 
-- localOffset: `{localOffset}`, 
-- localTimeZone: `{localTimeZone}`, 

Expected:   {expectedDTO}
*ACTUAL*:   {result}  ({(expectedDTO == result ? "(same)" : "*DIFF!*")})

Offset:     {offset} {(hasOffset ? "" : "(ignore)")}
*ACTUAL*:   {result.Offset}

HasOffset:  {hasOffset}
Actual:     {hadOffsetRes}  ({(hasOffset == hadOffsetRes ? "(same)" : "*DIFF!*")})

TreatNoOffsetAsLocalTime:    {treatNoOffsetAsLocalTime.ToBitString("TRUE", "FALSE")}
HandleObsoleteUSTimeZones:   {handleObsoleteUSTimeZones.ToBitString("TRUE", "FALSE")}

";
				return print.Print();
			}

			public static ParseDateTimeWithOffsetInfoArgs[] GetTestArgs()
			{
				int listedHour = 12;
				string rootTime = $"Jan 07, 2015 {listedHour.ToString("00")}:01:00";

				ParseDateTimeWithOffsetInfoArgs[] argsArray = {
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} PST", TimeSpan.FromHours(-8)) {
						 handleObsoleteUSTimeZones = true,
						 expectedDTO = DateTimeOffset.Parse($"{rootTime} -08:00")
					},

					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} PST", TimeSpan.FromHours(-8)) {
						handleObsoleteUSTimeZones = false,
						shouldFail = true,
						testDesc = "Test US time zone fail because handleObsoleteUSTimeZones was false"
					},

					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} GMT", TimeSpan.Zero, hasOffset: true) {
						handleObsoleteUSTimeZones = true,
						expectedDTO = DateTimeOffset.Parse($"{rootTime} +00:00"),
						testDesc = "Test GMT value works"
					},


					// 
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} +0000", TimeSpan.Zero) {
						expectedDTO = DateTimeOffset.Parse($"{rootTime} +0000")
					},

					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} +0500", TimeSpan.FromHours(5)) {
						expectedDTO = DateTimeOffset.Parse($"{rootTime} +05:00"),
					},
					// same as above except input time has "+05:00" instead of "+0500"
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime} +05:00", TimeSpan.FromHours(5)) {
						expectedDTO = DateTimeOffset.Parse($"{rootTime} +05:00"),
					},


					// TEST WITH localOffset
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime}", hasOffset: false){
						localOffset = TimeSpan.FromHours(-7),
						expectedDTO = DateTimeOffset.Parse($"{rootTime} -07:00"),
						handleObsoleteUSTimeZones = false,
						treatNoOffsetAsLocalTime = true
					},
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime}", hasOffset: false){
						localOffset = TimeSpan.FromHours(-7),
						expectedDTO = new DateTimeOffset(DateTime.Parse($"{rootTime}").AddHours(-7), TimeSpan.FromHours(-7)),
						handleObsoleteUSTimeZones = false,
						treatNoOffsetAsLocalTime = false // !!
					},

					// TEST WITH TimeZoneInfo
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime}", hasOffset: false){
						localTimeZone = TimeZones.GetTimeZoneInfoFromTZId("America/New_York"),
						expectedDTO = DateTimeOffset.Parse($"{rootTime} -05:00"),
						handleObsoleteUSTimeZones = false,
						treatNoOffsetAsLocalTime = true
					},
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime}", hasOffset: false){
						localTimeZone = TimeZones.GetTimeZoneInfoFromTZId("America/New_York"),
						expectedDTO = new DateTimeOffset(DateTime.Parse($"{rootTime}").AddHours(-5), TimeSpan.FromHours(-5)),
						handleObsoleteUSTimeZones = false,
						treatNoOffsetAsLocalTime = false // !!
					},


					// TEST where no offset or TZ was passed in, should default to treating input as UTC time (if string had no offset)
					new ParseDateTimeWithOffsetInfoArgs($"{rootTime}", hasOffset: false){
						expectedDTO = DateTimeOffset.Parse($"{rootTime} -00:00"),
						handleObsoleteUSTimeZones = false,
						treatNoOffsetAsLocalTime = true
					},

				};
				return argsArray;
			}

		}
	}
}
