// XDateTimes_OffsetTZConversions

using System;
using System.Diagnostics;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
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
			if(tzInfo != null && dt.Ticks > _minTicksForDateTimeIsSet) {
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
			if(dt.Ticks <= _minTicksForDateTimeIsSet)
				return dt;

			long ticks = keepUtcTime
				? dt.UtcDateTime.Ticks + offset.Ticks
				: dt.DateTime.Ticks;

			if(ticks <= _minDTTicks)
				return DateTimeOffset.MinValue;
			else if(ticks >= _maxDTTicks)
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
			if(dt.Ticks <= _minTicksForDateTimeIsSet)
				return dt;

			long ticks = isUtc
				? dt.Ticks + offset.Ticks // dt is utc
				: dt.Ticks; // dt is local time

			if(ticks < 0)
				return DateTimeOffset.MinValue;

			DateTimeOffset val = new DateTimeOffset(ticks, offset);
			return val;
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
			if(dt == DateTime.MinValue)
				return DateTimeOffset.MinValue;

			return new DateTimeOffset(dt.Ticks, offset);
		}







		public static DateTimeOffset ConvertLocalTimeToDateTimeOffset(this DateTime dt, TimeZoneInfo tzInfo)
			=> ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), isUtc: false);

		public static DateTimeOffset ConvertUtcTimeToDateTimeOffset(this DateTime dt, TimeZoneInfo tzInfo)
			=> ToDateTimeOffset(dt, tzInfo.GetUtcOffset(dt), isUtc: true);

		/// <summary>
		/// Returns the UtcNow time converted to an offset matching 
		/// input tz. Same can be performed by calling, as this does,
		/// <see cref="ToDateTimeOffset(DateTimeOffset, TimeZoneInfo, bool)"/>,
		/// with `keepUtcTime: true`.
		/// </summary>
		/// <param name="tzi"></param>
		public static DateTimeOffset ToDateTimeOffsetNow(this TimeZoneInfo tzi)
		{
			if(tzi != null)
				return DateTimeOffset.UtcNow.ToDateTimeOffset(tzi, keepUtcTime: true);
			return DateTimeOffset.UtcNow;
		}

		public static DateTime ToTZTimeFromUtc(this DateTime dt, TimeZoneInfo tzi)
			=> tzi.ToTZTimeFromUtc(dt);

		public static DateTime ToTZTimeFromUtc(this TimeZoneInfo tzi, DateTime dt)
		{
			if(dt == DateTime.MinValue)
				return DateTime.MinValue;
			if(tzi == null)
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
			if(tzi == null)
				throw new ArgumentNullException(nameof(tzi));
			DateTimeOffset dto = dt.ToDateTimeOffset(tzi.GetUtcOffset(dt), isUtc: false);
			return dto;
		}

		public static DateTimeOffset ToDateTimeOffsetNow(this TimeSpan offset)
		{
			return DateTimeOffset.UtcNow.ToOffset(offset);
		}



		static long _minTicksForDateTimeIsSet = TimeSpan.FromDays(1).Ticks;
		static long _minDTTicks = DateTimeOffset.MinValue.Ticks;
		static long _maxDTTicks = DateTimeOffset.MaxValue.Ticks;

	}
}
