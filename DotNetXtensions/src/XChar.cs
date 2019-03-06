using System;
using System.Runtime.CompilerServices;
using System.Text;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// ...
	/// </summary>
#if DNXPublic
	public
#endif
	static class XChar
	{

		#region GetBytes

		/// <summary>
		/// Gets the byte sequence that cooresponds to this character in UTF-8.
		/// </summary>
		/// <param name="thisChar">This char.</param>
		/// <returns></returns>
		public static byte[] GetBytes(this char thisChar)
		{
			if (thisChar < 127) // valid for UTF-8
				return new byte[] { (byte)thisChar };

			return Encoding.UTF8.GetBytes(new char[] { thisChar });
		}

		#endregion

		#region GetByteCount

		/// <summary>
		/// Gets the byte count of this character if it were encoded in UTF-8.
		/// </summary>
		/// <param name="thisChar">This char.</param>
		/// <returns></returns>
		public static int GetByteCount(this char thisChar)
		{
			if (thisChar < 127) // valid for UTF-8
				return 1;

			return Encoding.UTF8.GetByteCount(new char[] { thisChar });
		}

		#endregion

		#region GetString



		/// <summary>
		/// Converts the specified range of this char array to a string and returns the value.
		/// </summary>
		/// <param name="chars">These chars.</param>
		/// <param name="index">The zero based starting index.</param>
		/// <param name="length">The number of chars following index. </param>
		/// <returns>The new string.</returns>
		public static string GetString(this char[] chars, int index, int length)
		{
			if (chars == null) throw new ArgumentNullException("chars");

			return new string(chars, index, length);
		}

		#endregion

		#region NotImplemented

		///// <summary>
		///// Converts this string to a char.  The string must only be 1 character long.
		///// </summary>
		///// <param name="s">This string.</param>
		///// <returns></returns>
		//public static char ToChar(this string s)
		//{
		//    if (s == null) throw new ArgumentNullException("s");

		//    if (s.Length != 1) throw new FormatException();

		//    return s[0];
		//}

		#endregion

		#region IsAscii

		/// <summary>
		/// Indicates whether the char is an ascii digit (0-9 only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiDigit(this char c)
		{
			return c < 58 && c > 47;
		}

		/// <summary>
		/// Indicates whether the char is a lowercase ascii letter (a-z only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiLetter(this char c)
		{
			// note: this tests LOWERCASE first, because the overwhelming greater instances of lowercase almsot everywhere, and since one has to be tested first anyways...
			return (c > 96 && c < 123) || (c > 64 && c < 91);
		}

		/// <summary>
		/// Indicates whether the char is a lowercase ascii letter (a-z only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiLower(this char c)
		{
			return c > 96 && c < 123;
		}

		/// <summary>
		/// Indicates whether the char is an uppercase ascii letter (A-Z only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiUpper(this char c)
		{
			return c > 64 && c < 91;
		}

		/// <summary>
		/// Indicates whether the char is a lowercase ascii letter or ascii digit (a-z || 0-9 only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiLowerOrDigit(this char c)
		{
			return (c > 96 && c < 123) || (c < 58 && c > 47);
		}

		/// <summary>
		/// Indicates whether the char is an uppercase ascii letter or ascii digit (A-Z || 0-9 only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiUpperOrDigit(this char c)
		{
			return (c > 64 && c < 91) || (c < 58 && c > 47);
		}

		/// <summary>
		/// Indicates whether the char is an ascii letter or ascii digit (a-z || A-Z || 0-9 only).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiLetterOrDigit(this char c)
		{
			return (c > 96 && c < 123) || (c > 64 && c < 91) || (c < 58 && c > 47);
		}

		// **DO** keep this string function here, as it depends on XChar functions, and only XChar

		/// <summary>
		/// Indicates whether all the characters in this string are ascii letters or numbers (a-z || A-Z || 0-9 only).
		/// </summary>
		public static bool IsAsciiAlphaNumeric(this string s, int start = 0)
		{
			if (s == null)
				throw new ArgumentNullException();
			int len = s.Length;
			if (start >= len && len > 0)
				throw new ArgumentOutOfRangeException();

			for (int i = start; i < len; i++)
				if (!IsAsciiLetterOrDigit(s[i]))
					return false;

			return true;
		}


		// FUTURE: IsOkayPunctuation / IndexOfNonAsciiAlphaNumeric

		//public static bool IsOkayPunctuation(this char c)
		//{
		//	if(c < 48) { // 0


		//		}
		//	return (c > 96 && c < 123) || (c > 64 && c < 91) || (c < 58 && c > 47);
		//}


		//public static int IndexOfNonAsciiAlphaNumeric(this string s, int start = 0, char[] okayChars = null)
		//{
		//	if (s == null) throw new ArgumentNullException();
		//	int len = s.Length;
		//	if (start >= len && len > 0) throw new ArgumentOutOfRangeException();

		//	int extraCharsLen = okayChars == null ? 0 : okayChars.Length;

		//	for (int i = start; i < len; i++) {
		//		if (!IsAsciiLetterOrDigit(s[i])) {
		//			if (extraCharsLen > 0) {
		//				char c = s[i];
		//				for (int j = 0; j < extraCharsLen; j++)
		//					if (okayChars[j] == c)
		//						continue;
		//			}
		//			return i;
		//		}
		//	}
		//	return -1;
		//}

		#endregion

		public static bool EqualsIgnoreCase(this char c, char comparisonChar, bool assumeCompareCharIsLower = false)
		{
			if (c == comparisonChar)
				return true;
			c = char.ToLower(c);
			if (!assumeCompareCharIsLower)
				comparisonChar = char.ToLower(comparisonChar);
			return c == comparisonChar;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhitespace(this char c)
		{
			return char.IsWhiteSpace(c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsUpper(this char c)
		{
			return char.IsUpper(c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLower(this char c)
		{
			return char.IsLower(c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNumber(this char c)
		{
			return char.IsNumber(c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(this char c)
		{
			return c - '0';
		}

	}
}
