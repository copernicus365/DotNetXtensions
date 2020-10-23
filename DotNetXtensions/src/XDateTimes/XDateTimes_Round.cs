// XDateTimes_Round

using System;

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{

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
			if(m >= 0.5)
				f = 1;
			long val = ((dtTicks / roundTicks) + f) * roundTicks;
			return val;
		}

	}
}
