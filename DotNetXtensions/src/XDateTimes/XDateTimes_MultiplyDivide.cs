// XDateTimes_MultiplyDivide

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
		// --- TimeSpan.Multiply / Divide ---

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
	}
}