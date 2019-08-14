// XDateTimes_InRange

using System;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XDateTimes
	{
		// --- Min / Max ---

		public static DateTime Min(this DateTime val1, DateTime val2) { return val1 <= val2 ? val1 : val2; }

		public static DateTimeOffset Min(this DateTimeOffset val1, DateTimeOffset val2) { return val1 <= val2 ? val1 : val2; }

		public static DateTime Max(this DateTime val1, DateTime val2) { return val1 >= val2 ? val1 : val2; }

		public static DateTimeOffset Max(this DateTimeOffset val1, DateTimeOffset val2) { return val1 >= val2 ? val1 : val2; }

		public static TimeSpan Min(this TimeSpan val1, TimeSpan val2) { return val1 <= val2 ? val1 : val2; }

		public static TimeSpan Max(this TimeSpan val1, TimeSpan val2) { return val1 >= val2 ? val1 : val2; }


		// InRange / NotInRange
		public static bool InRange(this TimeSpan ts, TimeSpan min, TimeSpan max)
			=> ts >= min && ts <= max;

		public static bool InRange(this TimeSpan? ts, TimeSpan min, TimeSpan max)
			=> ts != null && ts >= min && ts <= max;

		public static bool InRangeForTimeOfDay(this TimeSpan ts)
			=> ts >= TimeSpan.Zero && ts < TimeSpan.FromHours(24);


		// --- 

		public static bool InRange(this DateTime dt, DateTime min, DateTime max)
		{
			return dt >= min && dt <= max;
		}
		public static bool InRange(this DateTime dt, DateTime min, TimeSpan timeAfterMin)
		{
			if(dt >= min) {
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
			if(dt >= min) {
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

	}
}