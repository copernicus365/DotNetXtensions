// XNumeric_Hex

#if !DNXPrivate
using System;
using System.Globalization;

namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XNumeric
	{
		// --- ToHex / ToIntFromHex ---

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
			return int.Parse(hexString, NumberStyles.HexNumber);
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
			if(hexString == null) throw new ArgumentNullException("hexString");

			int hexInt;

			if(Int32.TryParse(hexString, NumberStyles.HexNumber, null, out hexInt))
				return hexInt;

			if(parseFailReturnValue == null)
				return -1;
			else
				return (int)parseFailReturnValue;
		}

		#endregion

		#region ToHex

		public static string ToHex(this byte n, bool upperCase = true)
		{
			return n.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this short n, bool upperCase = true)
		{
			return n.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this char c, bool upperCase = true)
		{
			return ((UInt16)c).ToString(upperCase ? "X" : "x");
			// see notes above, due to which, ironically, casting to Int16 woud produce the same result
			// ironic because an Int16 can only hold half (32767) of the values that a char type can (65535)
			// (the signed type (Int16) is treated by this function as if it were an UInt16)
		}

		public static string ToHex(this int n, bool upperCase = true)
		{
			return n.ToString(upperCase ? "X" : "x");
		}

		public static string ToHex(this long n, bool upperCase = true)
		{
			return n.ToString(upperCase ? "X" : "x");
		}

		#endregion
	}
}
