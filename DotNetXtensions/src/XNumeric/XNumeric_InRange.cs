// XNumeric_MinMax
// XNumeric_InRange

namespace DotNetXtensions
{
	public static partial class XNumeric
	{
		/// <summary>
		/// Indicates if the length of this string is in the given range.
		/// If null returns false.
		/// </summary>
		public static bool InRange(this string val, int val1, int val2)
		{
			return val != null && val.Length >= val1 && val.Length <= val2;
		}

		public static bool InRange(this byte val, byte val1, byte val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this short val, short val1, short val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this int val, int val1, int val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this long val, long val1, long val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this double val, double val1, double val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this decimal val, decimal val1, decimal val2)
		{
			return val >= val1 && val <= val2;
		}

		// -- null versions --
		public static bool InRange(this byte? val, byte val1, byte val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this short? val, short val1, short val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this int? val, int val1, int val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this long? val, long val1, long val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this double? val, double val1, double val2)
		{
			return val >= val1 && val <= val2;
		}
		public static bool InRange(this decimal? val, decimal val1, decimal val2)
		{
			return val >= val1 && val <= val2;
		}


		public static bool NotInRange(this string val, int val1, int val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this byte val, byte val1, byte val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this short val, short val1, short val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this int val, int val1, int val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this long val, long val1, long val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this double val, double val1, double val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this decimal val, decimal val1, decimal val2) { return !val.InRange(val1, val2); }


		public static bool NotInRange(this byte? val, byte val1, byte val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this short? val, short val1, short val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this int? val, int val1, int val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this long? val, long val1, long val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this double? val, double val1, double val2) { return !val.InRange(val1, val2); }
		public static bool NotInRange(this decimal? val, decimal val1, decimal val2) { return !val.InRange(val1, val2); }

	}
}
