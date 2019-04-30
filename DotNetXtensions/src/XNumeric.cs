using System;
using System.Text;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for numbers, math, and related functions.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XNumeric
	{

		#region Min / Max

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

		#endregion


		#region RoundTo

		public static double Round(this double d, int decimalsAfter = 2)
		{
			if (d == 0.0) return d;

			int extraDecimals = 0;
			if (d > .0999)
				extraDecimals = 0;
			else if (d > .00999)
				extraDecimals = 1;
			else if (d > .000999)
				extraDecimals = 2;
			else if (d > .0000999)
				extraDecimals = 3;
			else if (d > .00000999)
				extraDecimals = 4;
			else if (d > .00000099)
				extraDecimals = 5;
			else if (d > .00000009)
				extraDecimals = 6;
			else
				return d;

			return Math.Round(d, decimalsAfter + extraDecimals);
		}

		public static decimal Round(this decimal d, int decimalsAfter = 2)
		{
			if (d == 0.0M) return d;

			int extraDecimals = 0;
			if (d > .0999M)
				extraDecimals = 0;
			else if (d > .00999M)
				extraDecimals = 1;
			else if (d > .000999M)
				extraDecimals = 2;
			else if (d > .0000999M)
				extraDecimals = 3;
			else if (d > .00000999M)
				extraDecimals = 4;
			else if (d > .00000099M)
				extraDecimals = 5;
			else if (d > .00000009M)
				extraDecimals = 6;
			else
				return d;

			return Math.Round(d, decimalsAfter + extraDecimals);
		}

		public static double Pos(this double d)
		{
			return d < 0 ? -d : d;
		}
		public static int Pos(this int d)
		{
			return d < 0 ? -d : d;
		}
		public static decimal Pos(this decimal d)
		{
			return d < 0 ? -d : d;
		}
		public static long Pos(this long d)
		{
			return d < 0 ? -d : d;
		}

		#endregion

		#region InRange

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

		#endregion


		#region --- DivideUp ---

		public static int DivideUp(this int dividend, int divisor)
		{
			int r = (dividend + (divisor - 1)) / divisor;
			return r;
			// note also: int result; int remainder = Math.DivRem(dividend, divisor, out result);
		}

		#endregion


		#region  ToHex / ToIntFromHex
		//no null check category for one-liners

		/*
		 * A note concerning hexidecimal conversions.  Consider the following: 
		 * 
			int i = -33;
			string hex = i.ToString("X"); 
			// hex == FFFFFFDF.  As a positive number, == 4294967263, as following shows:
 
			uint uInt = UInt32.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
			//uInt == 4294967263

			int sInt = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber); 
			//sInt == -33 (!)
		 * 
		 * Now clearly, (-33 == 4294967263) = false!  But in base2, it's true!
		 * Convert.ToString(i, 2) == 11111111111111111111111111011111; (true for: i = -33, and i = 4294967263)
		 * 
		 * What this shows, and what the programmer must be aware of, is that such hexidecimal
		 * conversions (both to and from numeric types) within these .NET functions always 
		 * treat the numeric type as if it were an unsigned numeric type.  They could have 
		 * avoided this, if desired (I don't know if it would be though?), simply by checking,
		 * when converting signed numeric types, that the value was not negative, and if it 
		 * was to throw an ArgumentException (or ArgumentOutOfRangeException).
		 * We could do such checking here, but as elsewhere, we follow .NET's convention
		 * (which was probably the best decision anyways).
		*/

		#region ToIntFromHex

		/// <summary>
		/// Parses this hexadecimal string to its corresponding integer value.
		/// Leading and trailing white space are ignored.  If the parse fails a 
		/// FormatException is thrown.
		/// <example><code>
		/// <![CDATA[
		/// // --- Example 1 ---
		/// int i = 123456;
		/// 
		/// string hex = i.ToHex();  // hex == 1E240
		/// int j = hex.ToIntFromHex();  // j == 123456
		/// 
		/// // --- Example 2 ---
		/// 
		/// int y = "1E24G".ToIntFromHex();  // ..FormatException..
		/// // note: "1E24G" is an invalid hex (with "G")
		/// ]]></code></example>
		/// </summary>
		/// <param name="hexString">This hexadecimal string literal.</param>
		/// <returns>The hexadecimal integer value.</returns>
		public static int ToIntFromHex(this string hexString)
		{
			return Int32.Parse(hexString, NumberStyles.HexNumber);
		}

		/// <summary>
		/// Converts this hexadecimal string to its corresponding integer value.
		/// Leading and trailing white space are ignored.  If the parse fails, 
		/// <i>parseFailReturnValue</i> is returned, or -1 if null is entered for that parameter.
		/// </summary>
		/// <param name="hexString">This hexadecimal string literal.</param>
		/// <param name="parseFailReturnValue">The value to return if the parse fails.</param>
		/// <returns>The hexadecimal integer value or -1.</returns>
		public static int ToIntFromHex(this string hexString, int? parseFailReturnValue)
		{
			if (hexString == null) throw new ArgumentNullException("hexString");

			int hexInt;

			if (Int32.TryParse(hexString, NumberStyles.HexNumber, null, out hexInt))
				return hexInt;

			if (parseFailReturnValue == null)
				return -1;
			else
				return (int)parseFailReturnValue;
		}

		#endregion

		#region ToHex

		public static string ToHex(this byte thisByte, bool upperCase = true)
		{
			return thisByte.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this short thisShort, bool upperCase = true)
		{
			return thisShort.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this char thisChar, bool upperCase = true)
		{
			return ((UInt16)thisChar).ToString(upperCase ? "X" : "x");
			// see notes above, due to which, ironically, casting to Int16 woud produce the same result
			// ironic because an Int16 can only hold half (32767) of the values that a char type can (65535)
			// (the signed type (Int16) is treated by this function as if it were an UInt16)
		}

		public static string ToHex(this int thisInt, bool upperCase = true)
		{
			return thisInt.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this long thisLong, bool upperCase = true)
		{
			return thisLong.ToString(upperCase ? "X" : "x");
		}

		#endregion

		#endregion



		#region --- bool ---

		public static int ToBit(this bool b)
		{
			return b ? 1 : 0;
		}

		//public static string ToBit(this bool b, string t, string f)
		//{
		//	return b ? t : f;
		//}

		public static string ToBitString(this bool b, string t = "T", string f = "F")
		{
			return b ? t : f;
		}

		/// <summary>
		/// Converts this boolean to a lower case 'true' / 'false' representation.
		/// </summary>
		/// <param name="b">bool</param>
		public static string ToStringLower(this bool b)
		{
			return b ? "true" : "false";
		}

		/// <summary>
		/// Converts this boolean to a 'Yes' / 'No' representation.
		/// </summary>
		/// <param name="b">bool</param>
		/// <param name="lower">True to have a lowercase result returned.</param>
		public static string ToStringYesNo(this bool b, bool lower = false)
		{
			if (lower)
				return b ? "yes" : "no";
			return b ? "Yes" : "No";
		}

		#endregion


		#region --- KeyValuePair ---

		public static TKey GetKey<TKey, TValue>(this KeyValuePair<TKey, TValue>? kv)
		{
			if (kv == null)
				return default(TKey);
			return ((KeyValuePair<TKey, TValue>)kv).Key;
		}
		public static TValue GetValue<TKey, TValue>(this KeyValuePair<TKey, TValue>? kv)
		{
			if (kv == null)
				return default(TValue);
			return ((KeyValuePair<TKey, TValue>)kv).Value;
		}

		#endregion

	}
}