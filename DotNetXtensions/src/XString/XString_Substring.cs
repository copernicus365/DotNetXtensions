using System;

namespace DotNetXtensions
{
	public static partial class XString
	{
		public static string SubstringMax(this string str, int maxLength, string ellipsis = null, bool tryBreakOnWord = false)
			=> SubstringMax(str, 0, maxLength, ellipsis: ellipsis, tryBreakOnWord: tryBreakOnWord);

		/// <summary>
		/// Returns a substring of the input string where instead of specifying
		/// the exact length of the return string (which in .NET's string.Substring
		/// cannot be specified out of range), one specifies a maxLength, meaning
		/// maxLength can be out of range, in which case the substring from index
		/// to the end of the string is returned.
		/// <para />
		/// If the string is NULL or EMPTY, the same is immediately returned, NO exceptions
		/// (Null or OutOfRange) will be thrown.
		/// <para/>
		/// This nicely solves the problem when one simply wants the first n length
		/// of characters from a string, but without having to write a bunch of
		/// code to make sure they do not go out of range in case, for instance, the string was shorter
		/// than expected.
		/// </summary>
		/// <param name="str">String</param>
		/// <param name="index">Start Index</param>
		/// <param name="maxLength">Maximum length of the return substring.</param>
		/// <param name="ellipsis">...</param>
		/// <param name="tryBreakOnWord"></param>
		public static string SubstringMax(this string str, int index, int maxLength, string ellipsis = null, bool tryBreakOnWord = false)
		{
			const int maxWordBreakSize = 15;
			const int minCheckWordBreak = 7;

			if(str == null || str.Length == 0) return str;
			int strLen = str.Length;
			if(index >= strLen) throw new ArgumentOutOfRangeException();

			if(index == 0 && strLen <= maxLength)
				return str;

			int finalLen = strLen - index;
			bool useEllipsis = ellipsis.NotNulle();

			if(maxLength < finalLen)
				finalLen = maxLength;
			else
				useEllipsis = false; // was true up to here if wasn't nulle

			// WOULD BE MORE PERFORMANT TO DO THE WORD-BREAK SEARCH HERE,
			// NOT NEED AN EXTRA STRING ALLOC. WANNA DO IT?! GO AHEAD, MAKE MY DAY!

			int postIdx = index + finalLen;
			string result = str.Substring(index, finalLen);

			// WORD-BREAK SEARCH
			if(tryBreakOnWord && postIdx < strLen && char.IsLetterOrDigit(str, postIdx) && result.Length > minCheckWordBreak) {
				int i = 0;
				int x = result.Length - 1;
				for(; i < maxWordBreakSize && x >= minCheckWordBreak; i++, x--) {
					if(char.IsWhiteSpace(result[x]))
						break;
				}
				if(i > 0 && i < maxWordBreakSize && x >= minCheckWordBreak)
					result = result.Substring(0, x + 1);
			}

			if(useEllipsis)
				result += ellipsis;

			return result;
		}




		/// <summary>
		/// Tests if the source string starts with the input <paramref name="startsWithValue"/>,
		/// in which case returns the remainder of the string after that point. 
		/// While one could typically just do this: <c>startsWithValue.Substring(str.Length);</c>,
		/// this method handles testing if source string actually starts with the input value,
		/// and also gracefully handles null or empty values (for any null or empty input values
		/// the method returns null).
		/// </summary>
		/// <example><c>var val = "bundleId:1234"; var bId = val.SubstringAfter("bundleId:"); // bId == "1234"</c></example>
		/// <param name="str">Source string, which for a success, should be longer than input value.</param>
		/// <param name="startsWithValue">The start value.</param>
		public static string SubstringAfterStartsWith(this string str, string startsWithValue)
		{
			if(str.IsNulle() || startsWithValue.IsNulle() || str.Length <= startsWithValue.Length)
				return null;

			if(!str.StartsWith(startsWithValue))
				return null;

			string remainder = str.Substring(startsWithValue.Length); //, startsWithValue.Length - str.Length);
			return remainder;
		}

		/// <summary>
		/// Gets a substring of text following the search value. If not found
		/// null is returned. An empty string is returned if
		/// the input string ends with the search val. *Graceful, no exceptions.
		/// <code>
		/// "Hello world out there!".SubstringAfter("world ") == "out there!"
		/// </code>
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="val">Value to search for.</param>
		/// <param name="includeSearchVal">True to include the search value in the substring result.</param>
		public static string SubstringAfter(this string str, string val, bool includeSearchVal = false)
		{
			if(str.IsNulle() || val.IsNulle())
				return str;

			int idx = str.IndexOf(val);
			if(idx < 0)
				return null;
			int finalIdx = idx + (includeSearchVal ? 0 : val.Length);
			return str.Substring(finalIdx);
		}

		/// <summary>
		/// Gets a substring of text before the search value. If not found,
		/// null is returned. An empty string is returned if
		/// the input string starts with the search val. *Graceful, no exceptions.
		/// <code>
		/// "Hello world out there!".SubstringBefore(" world") == "Hello"
		/// </code>
		/// </summary>
		/// <param name="str">String.</param>
		/// <param name="val">Value to search for.</param>
		/// <param name="includeSearchVal">True to include the search value in the substring result.</param>
		public static string SubstringBefore(this string str, string val, bool includeSearchVal = false)
		{
			if(str.IsNulle() || val.IsNulle())
				return str;

			int idx = str.IndexOf(val);
			if(idx < 0)
				return null;

			return str.Substring(0, (includeSearchVal ? idx + val.Length : idx));
		}
	}
}
