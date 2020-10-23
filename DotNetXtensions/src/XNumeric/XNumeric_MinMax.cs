// XNumeric_MinMax

using System;

namespace DotNetXtensions
{
	public static partial class XNumeric
	{
		// --- Min / Max ---

		public static int Min(this byte val1, byte val2) { return Math.Min(val1, val2); }
		public static decimal Min(this decimal val1, decimal val2) { return Math.Min(val1, val2); }
		public static double Min(this double val1, double val2) { return Math.Min(val1, val2); }
		public static float Min(this float val1, float val2) { return Math.Min(val1, val2); }
		public static int Min(this int val1, int val2) { return Math.Min(val1, val2); }
		public static long Min(this long val1, long val2) { return Math.Min(val1, val2); }
		public static short Min(this short val1, short val2) { return Math.Min(val1, val2); }


		public static int Max(this byte val1, byte val2) { return Math.Max(val1, val2); }
		public static decimal Max(this decimal val1, decimal val2) { return Math.Max(val1, val2); }
		public static double Max(this double val1, double val2) { return Math.Max(val1, val2); }
		public static float Max(this float val1, float val2) { return Math.Max(val1, val2); }
		public static int Max(this int val1, int val2) { return Math.Max(val1, val2); }
		public static long Max(this long val1, long val2) { return Math.Max(val1, val2); }
		public static short Max(this short val1, short val2) { return Math.Max(val1, val2); }

		/// <summary>
		/// Returns the input value if it is within the min / max range, else returns
		/// min if value is below min or the max if value is above max.
		/// </summary>
		/// <param name="val">Input value.</param>
		/// <param name="min">Minimum value that will be returned.</param>
		/// <param name="max">Maximum value that will be returned.</param>
		public static int MinMax(this int val, int min, int max) { return val.Max(min).Min(max); }
		public static int MinMax(this byte val, byte min, byte max) { return val.Max(min).Min(max); }
		public static decimal MinMax(this decimal val, decimal min, decimal max) { return val.Max(min).Min(max); }
		public static double MinMax(this double val, double min, double max) { return val.Max(min).Min(max); }
		public static float MinMax(this float val, float min, float max) { return val.Max(min).Min(max); }
		public static long MinMax(this long val, long min, long max) { return val.Max(min).Min(max); }
		public static short MinMax(this short val, short min, short max) { return val.Max(min).Min(max); }

	}
}
