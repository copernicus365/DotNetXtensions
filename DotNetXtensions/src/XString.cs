using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Extension methods for strings.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XString
	{
		#region All

		/* Inclusion of an extension method named "All" upon type string with any signature
		 * surprisingly automatically makes available one "All" extension method, as follows:
		 *
		 * // bool IEnumerable<char>.All<char>(Func<char, bool> predicate);
		 * // "Determines whether all elements of a sequence satisfy a condition."
		 *
		 * The reason for this is somehow due to that fact that System.String implements
		 * IEnumerable<char>.
		*/

		/// <summary>
		/// Determines whether all the chars within the specified range of this string
		/// (from index to string end) satisfy the given condition.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="index">The start index.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static bool All(this string s, int index, Func<char, bool> predicate)
		{
			if (s == null) throw new ArgumentNullException("s");

			return All(s, index, s.Length - index, predicate);
		}

		/// <summary>
		/// Determines whether all the chars within the specified range of this string
		/// satisfy the given condition.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="index">The start index.</param>
		/// <param name="length">The length following index.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static bool All(this string s, int index, int length, Func<char, bool> predicate)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (predicate == null) throw new ArgumentNullException("predicate");

			if ((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			for (int i = index; i < length; i++)
				if (!predicate(s[i]))
					return false;

			return true;
		}

		#endregion All

		#region Any

		/* Inclusion of an extension method named "Any" upon type string with any signature
		 * surprisingly automatically makes available two "Any" extension methods, as follows:
		 * // bool IEnumerable<char>.Any<char>();
		 * // "Determines whether a sequence contains any elements."
		 *
		 * // bool IEnumerable<char>.Any<char>(Func<char, bool> predicate);
		 * // "Determines whether any element of a sequence satisfies a condition."
		 *
		 * The reason for this is somehow due to that fact that System.String implements
		 * IEnumerable<char>.
		*/

		/// <summary>
		/// Determines whether any of the chars within the specified range of this string
		/// (from index to string end) satisfy the given condition.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="index">The start index.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static bool Any(this string s, int index, Func<char, bool> predicate)
		{
			if (s == null) throw new ArgumentNullException("s");

			return Any(s, index, s.Length - index, predicate);
		}

		/// <summary>
		/// Determines whether any the chars within the specified range of this string
		/// satisfy the given condition.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="index">The start index.</param>
		/// <param name="length">The length following index.</param>
		/// <param name="predicate">The predicate.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static bool Any(this string s, int index, int length, Func<char, bool> predicate)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (predicate == null) throw new ArgumentNullException("predicate");

			if ((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			for (int i = index; i < length; i++)
				if (predicate(s[i]))
					return true;

			return false;
		}

		#endregion Any

		#region FormatX

		/// <summary>
		/// Replaces the format items in this string with the text equivalent
		/// of the values of the cooresponding objects.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="arg0">An object to format.</param>
		/// <returns>The formatted string.</returns>
		[DebuggerStepThrough]
		public static string FormatX(this string s, object arg0)
		{
			return string.Format(s, arg0);
		}

		/// <summary>
		/// Replaces the format items in this string with the text equivalent
		/// of the values of the cooresponding objects.
		/// <example><code>
		/// <![CDATA[
		/// string s = "Hi there {0}, you are {1} years old.";
		/// string person = "Joe";
		/// int age = 32;
		///
		/// string greeting = s.FormatX(person, age); // == Hi there Joe, you are 32 years old.
		/// ]]></code></example>
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="arg0">An object to format.</param>
		/// <param name="arg1">An object to format.</param>
		/// <returns>The formatted string.</returns>
		[DebuggerStepThrough]
		public static string FormatX(this string s, object arg0, object arg1)
		{
			return string.Format(s, arg0, arg1);
		}

		/// <summary>
		/// Replaces the format items in this string with the text equivalent
		/// of the values of the cooresponding objects.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="arg0">An object to format.</param>
		/// <param name="arg1">An object to format.</param>
		/// <param name="arg2">An object to format.</param>
		/// <returns>The formatted string.</returns>
		[DebuggerStepThrough]
		public static string FormatX(this string s, object arg0, object arg1, object arg2)
		{
			return string.Format(s, arg0, arg1, arg2);
		}

		/// <summary>
		/// Replaces the format items in this string with the text equivalent
		/// of the values of the cooresponding objects.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="args">An object array containing zero or more objects to format.</param>
		/// <returns>The formatted string.</returns>
		[DebuggerStepThrough]
		public static string FormatX(this string s, params object[] args)
		{
			return string.Format(s, args);
		}

		#endregion FormatX

		#region Trim

		/// <summary>
		/// Indicates if this string can be trimmed. Null and Empty values ARE valid (will return false).
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTrimmable(this string s)
		{
			if (s != null) {
				int len = s.Length;
				if (len > 1)
					return char.IsWhiteSpace(s[0]) || char.IsWhiteSpace(s[len - 1]);
				return len == 0 || char.IsWhiteSpace(s[0]);
			}
			return false;
		}

		/// <summary>
		/// Trims the string only if it is needed. Value CAN be Null or Empty.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TrimIfNeeded(ref string s)
		{
			if (s.IsTrimmable()) {
				s = s.Trim();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Trims the string only if it is needed. Value CAN be Null or Empty.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string TrimIfNeeded(this string s)
		{
			if (s.IsTrimmable())
				return s.Trim();
			return s;
		}

		/// <summary>
		/// Trims the string if it is not null, else returns null.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string TrimN(this string s)
		{
			return s == null ? null : s.Trim();
		}

		#endregion Trim

		#region GetBytes / GetByteCount

		/// <summary>
		/// Converts this string into a UTF-8 encoded byte array.
		/// <example><code>
		/// <![CDATA[
		/// byte[] bytes = "Hello World!".GetBytes(); // bytes == 72, 101, 108, ...
		/// ]]></code></example>
		/// </summary>
		/// <param name="s">This string.</param>
		/// <returns></returns>
		public static byte[] GetBytes(this string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		/// <summary>
		/// Returns the number of bytes that would result from encoding this string into
		/// UTF-8 encoding.
		/// <example><code>
		/// <![CDATA[
		/// int byteCount = "abcαβγ".GetByteCount();  // byteCount == 9
		/// ]]></code></example>
		/// </summary>
		/// <param name="s">This string.</param>
		/// <returns></returns>
		public static int GetByteCount(this string s)
		{
			return Encoding.UTF8.GetByteCount(s);
		}

		public static int GetByteCount(this string s, int index, int length)
		{
			return Encoding.UTF8.GetByteCount(s.Substring(index, length));
		}

		#endregion GetBytes

		#region IndexOfAll

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found within the specified range of this string, while employing the specified comparison type,
		/// or if none is specified, the default StringComparison.CurrentCulture is used (see notes in code
		/// below: this is the verified default used by String.IndexOf() when no StringComparison is supplied).
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="index">The start index.</param>
		/// <param name="length">The number of characters following start index on which to search.</param>
		/// <param name="comparisonType">A StringComparison value.</param>
		/// <returns>The index positions of the found values.</returns>
		private static int[] __IndexOfAll(string s, string searchValue, int index, int length, StringComparison comparisonType)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (searchValue == null) throw new ArgumentNullException("searchValue");

			if (s.Length == 0) return new int[0]; // must do this before next; bec if s.Length = 0, it would throw exception

			if ((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			List<int> finds = new List<int>();

			int currentPos = index;

			for (int runningLen = length; runningLen >= 0; runningLen = length - (++currentPos - index)) {
				currentPos = s.IndexOf(searchValue, currentPos, runningLen, comparisonType);

				if (currentPos < 0)
					break;

				finds.Add(currentPos);
			}

			return finds.ToArray();

			#region NOTES

			// 1) On StringComparison, if none was specified we send in StringComparison.CurrentCulture.
			// Extensive check was made to verify that this is the default that is used for IndexOf when
			// no StringComparison value is specified.  MSDN says:
			// "This method performs a word (case-sensitive and culture-sensitive) search using the current culture."
			// Digging into compiled assembly clearly shows this to be the case as well, where the following is called:
			// CultureInfo.CurrentCulture.CompareInfo.IndexOf(...);  The thing to note is the "CurrentCulture" part
			//
			// 2)
			// Must increment currentPos before setting length! bec currentPos is used to set length
			// Note: currentPos is always *increasing*, length is always *decreasing*
			//
			// 3)
			// Let's unpack the final equation: two of the 4 values used in this last equation were original params
			// that are not changing: [[ length and index ]]; the other two are the changing counterparts:
			// [[ runningLen and currentPos ]].  These considerations should help make better sense
			// of this all.  We are essentially just staying in the bounds that were set for us.

			#endregion NOTES
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found in this string.
		/// <example><code>
		/// <![CDATA[
		/// string text = "How much wood could a wood chuck chuck if a woodchuck could chuck wood?";
		///
		/// int[] finds = text.IndexOfAll("wood");
		///
		/// Console.WriteLine(finds.JoinToString()); // --> 9, 22, 44, 66
		///
		/// // *JoinToString() is a DotNetExtensions extension method.
		/// ]]></code></example>
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, 0, s.Length, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found in this string following index.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="index">The start index.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue, int index)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, index, (s.Length - index), StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found within the specified range of this string.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="index">The start index.</param>
		/// <param name="length">The number of characters following start index on which to search.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue, int index, int length)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, index, length, StringComparison.CurrentCulture);
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found in this string, while employing the specified comparison type.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="comparisonType">A StringComparison value.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue, StringComparison comparisonType)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, 0, s.Length, comparisonType);
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found in this string following index, while employing the specified comparison type.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="index">The start index.</param>
		/// <param name="comparisonType">A StringComparison value.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue, int index, StringComparison comparisonType)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, index, (s.Length - index), comparisonType);
		}

		/// <summary>
		/// Returns the index positions all the occurences of <i>searchValue</i>
		/// found within the specified range of this string, while employing the specified comparison type.
		/// </summary>
		/// <param name="s">This string.</param>
		/// <param name="searchValue">The value to search for.</param>
		/// <param name="index">The start index.</param>
		/// <param name="length">The number of characters following start index on which to search.</param>
		/// <param name="comparisonType">A StringComparison value.</param>
		/// <returns>The index positions of the found values.</returns>
		public static int[] IndexOfAll(this string s, string searchValue, int index, int length, StringComparison comparisonType)
		{
			if (s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, index, length, comparisonType);
		}

		#endregion IndexOfAll

		/// <summary>
		/// Returns the index *after* the found IndexOf result, or if not found,
		/// returns -1;
		/// </summary>
		/// <param name="s"></param>
		/// <param name="value"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static int IndexAfter(this string s, string value, int startIndex = 0)
		{
			if (s == null || value == null)
				return -1;
			int idx = s.IndexOf(value, startIndex);
			if (idx < 0)
				return -1;
			return idx + value.Length;
		}

		public static char CharFromEnd(this string s, int indexFromEnd)
		{
			int len = s?.Length ?? -1;
			if (len > 0 && indexFromEnd < len && indexFromEnd >= 0) {
				int idx = len - indexFromEnd - 1;
				return s[idx];
			}
			return char.MinValue;
		}

		public static string ReplaceEnd(this string s, int cutFromEnd, string newEnd)
		{
			int origLen = s?.Length ?? 0;
			string val = s.CutEnd(cutFromEnd);
			if (val?.Length != origLen)
				val += newEnd;
			return val;
		}

		public static string CutEnd(this string s, int cutFromEnd)
		{
			if (s.IsNulle() || cutFromEnd < 0) return s;
			if (cutFromEnd >= s.Length)
				return "";

			string value = s.Substring(0, s.Length - cutFromEnd);
			return value;
		}

		/// <summary>
		/// Cuts the source string by the length of the specified endString.
		/// By default checks first that the source string does end with that value
		/// (if not, returns source string). Gracefully ignores any null or empty
		/// strings (by returning source string), or when end is longer than source (by returning empty).
		/// </summary>
		/// <param name="s">Source string.</param>
		/// <param name="endString">End string expected to occur at end of source string. 
		/// If not, will return source string.</param>
		/// <param name="checkDoesEndWith">True (by default) to check the end does actually end
		/// the source. Specify false to ignore this check if you already know it is a match.</param>
		/// <param name="ignoreCase">True to have the end check does case-insensitive (OrdinalIgnoreCase).</param>
		public static string CutEnd(this string s, string endString, bool checkDoesEndWith = true, bool ignoreCase = false)
		{
			if (s.IsNulle() || endString.IsNulle())
				return s;

			if (checkDoesEndWith && endString.Length <= s.Length) {
				if (!s.EndsWith(endString, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
					return s;
			}

			return s.CutEnd(endString.Length);
		}

		public static string CutEnd(this string s, char endChar, bool ignoreCase = false)
		{
			if (s.IsNulle())
				return s;

			char last = s.Last();

			if (ignoreCase) {
				last = char.ToLower(last);
				endChar = char.ToLower(endChar);
			}

			if (last != endChar)
				return s;

			return s.Substring(0, s.Length - 1);
		}

		///// <summary>
		///// Finds the last index in string of <paramref name="cutAtValue"/>
		///// and returns a substring which is cut off at that point. If the value is never found,
		///// source string is returned. Often <paramref name="cutAtValue"/> will simply be 
		///// the very end of the source string (e.g. <c>CutEnd("file.txt", ".txt")</c> returns <c>"file"</c>),
		///// but it can be anywhere in the string.
		///// </summary>
		///// <param name="s">Source string.</param>
		///// <param name="cutAtValue">The string to find the last instance of this value
		///// in the source string (may be the very end of the string).</param>
		//public static string CutEnd(this string s, string cutAtValue)
		//{
		//	if(s.IsNulle() || cutAtValue.IsNulle()) return s;
		//	return CutEnd(s, s.LastIndexOf(cutAtValue));
		//}

		public static string End(this string s, int length)
		{
			if (s == null || s.Length <= length || length < 1)
				return s;

			return s.Substring(s.Length - length, length);
		}

		public static bool PreviousIndexIsMatch(this string s, int idx, Func<char, bool?> matchOrBreakOrContinuePredicate)
		{
			return s.PreviousIndexOf(idx, matchOrBreakOrContinuePredicate) >= 0;
		}

		public static int PreviousIndexOf(this string s, int idx, Func<char, bool?> matchOrBreakOrContinuePredicate)
		{
			if (s.IsNulle() || idx >= s.Length)
				return -1;

			bool? result = null;
			while (idx >= 0) {

				result = matchOrBreakOrContinuePredicate(s[idx]);

				if (result != null) {
					if (result == true)
						return idx;
					break;
				}
				idx--;
			}
			return -1;
		}

		/// <summary>
		/// By 'CodePascalCase' we mean a code name like this very method's name:
		/// <see cref="ToCamelCaseFromCodePascalCase"/>, i.e. a valid C# member name
		/// whose first letter is capitalized as in the case (typically) of public
		/// class members. This function assumes the input is valid as such and does nothing
		/// to verify any of this. It simply lowercases the first char if it determined to 
		/// already be a lowercase letter, with the additional check first that the string
		/// must not be null and lenght >= 2 (else returns input string with no error).
		/// </summary>
		/// <param name="s">Input string expected to already be a valid code name in PascalCase.</param>
		public static string ToCamelCaseFromCodePascalCase(this string s)
		{
			if (s == null || s.Length < 2) return s;

			if (char.IsUpper(s[0]))
				s = char.ToLower(s[0]) + s.Substring(1);

			return s;
		}

		#region SplitN

		/// <summary>
		/// Calls string.Split, but returns an empty sequence for null or empty input, and assumes
		/// by default to removeEmptyEntries (default param is true).
		/// </summary>
		/// <param name="s">input</param>
		/// <param name="separator">separator</param>
		/// <param name="count">Maximum items to return</param>
		/// <param name="removeEmptyEntries">True (default) to remove nullorempty items.</param>
		public static string[] SplitN(this string s, char separator, int? count = null, bool removeEmptyEntries = true)
		{
			return SplitN(s, new char[] { separator }, count, removeEmptyEntries);
		}

		public static string[] SplitN(this string s, char[] separators, int? count = null, bool removeEmptyEntries = true)
		{
			if (s.IsNulle() || (count != null && count < 1))
				return new string[0];

			StringSplitOptions opt = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
			return count != null
				? s.Split(separators, (int)count, opt)
				: s.Split(separators, opt);
		}

		public static string[] SplitN(this string s, string separator, int? count = null, bool removeEmptyEntries = true)
		{
			return SplitN(s, new string[] { separator }, count, removeEmptyEntries);
		}

		public static string[] SplitN(this string s, string[] separators, int? count = null, bool removeEmptyEntries = true)
		{
			if (s.IsNulle() || (count != null && count < 1))
				return new string[0];

			StringSplitOptions opt = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
			return count != null
				? s.Split(separators, (int)count, opt)
				: s.Split(separators, opt);
		}

		public static string[] SplitAndRemoveWhiteSpaceEntries(this string val, char separator, Predicate<string> pred = null)
		{
			return SplitAndRemoveWhiteSpaceEntries(
				val,
				separator == ',' ? _commaCharArr : new char[] { separator });
		}

		/// <summary>
		/// Trims all entries and removes whitespace entries.
		/// </summary>
		public static string[] SplitAndRemoveWhiteSpaceEntries(this string val, char[] separator, Predicate<string> pred = null)
		{
			if (val.IsNulle())
				return _emptyStrArr;

			var arr = val.Split(separator, StringSplitOptions.RemoveEmptyEntries);

			if (arr.IsNulle())
				return _emptyStrArr;

			bool usePred = pred != null;

			arr = arr
				.Select(s => s.TrimN())
				.Where(s => s.NotNulle() && usePred ? pred(s) : true)
				.ToArray();

			return arr;
		}

		static char[] _commaCharArr = { ',' };
		static string[] _emptyStrArr = { };

		#endregion

		/// <summary>
		/// Splits the string into lines, both "\r\n", "\n" are checked.
		/// Optionally trims the lines and removes empty lines.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="trimLines"></param>
		/// <param name="removeEmptyLines"></param>
		/// <returns></returns>
		public static string[] SplitLines(this string str, bool trimLines = false, bool removeEmptyLines = false)
		{
			if (str == null)
				return null;
			if (str.Length == 0)
				return new string[0];

			string[] lines = str.Split(___splitLinesArray,
				removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

			if (lines.Length == 0 || !trimLines && !removeEmptyLines)
				return lines;

			for (int i = 0; i < lines.Length; i++) {
				string l = lines[i];
				if (l != null) {
					if (trimLines && l.Length > 0)
						l = l.Trim();
					if (removeEmptyLines && l.Length == 0)
						l = null;
					lines[i] = l;
				}
			}
			if (removeEmptyLines)
				lines = lines.Where(l => l != null).ToArray();
			return lines;
		}

		private static string[] ___splitLinesArray = new string[] { "\r\n", "\n" };

		#region --- InsertLineBreaks ---

		/// <summary>
		/// 
		/// </summary>
		/// <param name="s"></param>
		/// <param name="lengthTillBreak"></param>
		/// <param name="breakVal">If null, will be set to <see cref="Environment.NewLine"/>.</param>
		/// <param name="endWithBreak">True to have the break inserted after the last line.</param>
		/// <returns></returns>
		public static string InsertBreaks(this string s, int lengthTillBreak, string breakVal = null, bool endWithBreak = false)
		{
			if (s == null) throw new ArgumentNullException();
			if (lengthTillBreak < 1) throw new ArgumentOutOfRangeException();

			if (breakVal == null)
				breakVal = Environment.NewLine;

			int len = s.Length;
			int numLines = len.DivideUp(lengthTillBreak);
			int totalLenMax = (numLines * breakVal.Length) + numLines * lengthTillBreak; // includes "\r\n" per line

			StringBuilder sb = new StringBuilder(totalLenMax);

			for (int i = 0; i < len; i += lengthTillBreak) {
				int lineLen = Math.Min(len - i, lengthTillBreak);
				sb.Append(s, i, lineLen);
				sb.Append(breakVal);
			}

			return endWithBreak || sb.Length < 1
				? sb.ToString()
				: sb.ToString(0, sb.Length - breakVal.Length);
		}

		#endregion

		#region IsNullOrEmpty | EmptyIfNull

		/// <summary>
		/// Checks if string is null or empty. This is an interesting
		/// abbreviated form of 'IsNullOrEmpty', which is a char short of half
		/// the length of the latter. Use it if you want to! Don't if don't. 
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNulle(this string str)
		{
			return str == null || str.Length == 0;
		}

		/// <summary>
		/// Checks if string is NOT null or empty.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotNulle(this string str)
		{
			return str != null && str.Length != 0;
		}

		/// <summary>
		/// Checks if string is null or empty.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty(this string str)
		{
			return str == null || str.Length == 0;
		}

		/// <summary>
		/// Checks if string is null, empty, or only has whitespace.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// Returns an empty string if input string is null.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EmptyIfNull(this string str)
		{
			return str == null ? "" : str;
		}

		/// <summary>
		/// Returns null if input is an empty string, else returns the input.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullIfEmpty(this string s)
		{
			return s == "" ? null : s;
		}

		/// <summary>
		/// Returns null if input is an empty or whitespace string, 
		/// else returns the (trimmed if needed) input.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullIfEmptyTrimmed(this string s)
		{
			s = s.TrimIfNeeded();
			return s == "" ? null : s;
		}


		#endregion

		#region QQQ

		//[DebuggerStepThrough]
		//public static string QQ(this string str, string value2, string value3)
		//{
		//	return (str != null)
		//		? str
		//		: (value2 != null)
		//			? value2
		//			: value3;
		//}

		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string is returned.
		/// This is a corrolary to the ?? operator, which only checks if
		/// the first string is null, not if it is also not empty.
		/// </summary>
		/// <param name="str">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string QQQ(this string str, string value2)
		{
			return (str != null && str.Length > 0) ? str : value2;
		}

		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string itself is checked 
		/// if it is NULL or EMPTY, if so it is returned. Else <paramref name="value3"/>
		/// is returned.
		/// </summary>
		/// <param name="str">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		/// <param name="value3">String to return if BOTH previous strings were both NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string QQQ(this string str, string value2, string value3)
		{
			return (str != null && str.Length > 0)
				? str
				: (value2 != null && value2.Length > 0)
					? value2
					: value3;
		}





		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string is returned.
		/// This is a corrolary to the ?? operator, which only checks if
		/// the first string is null, not if it is also not empty.
		/// </summary>
		/// <param name="str">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FirstNotNulle(this string str, string value2)
		{
			return (str != null && str.Length > 0) ? str : value2;
		}

		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string itself is checked 
		/// if it is NULL or EMPTY, if so it is returned. Else <paramref name="value3"/>
		/// is returned.
		/// </summary>
		/// <param name="str">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		/// <param name="value3">String to return if BOTH previous strings were both NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FirstNotNulle(this string str, string value2, string value3)
		{
			return (str != null && str.Length > 0)
				? str
				: (value2 != null && value2.Length > 0)
					? value2
					: value3;
		}

		#endregion

		#region SubstringMax

		public static string SubstringMax(this string str, int maxLength, string ellipsis = null, bool tryBreakOnWord = false)
		{
			return SubstringMax(str, 0, maxLength, ellipsis: ellipsis, tryBreakOnWord: tryBreakOnWord);
		}

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

			if (str == null || str.Length == 0) return str;
			int strLen = str.Length;
			if (index >= strLen) throw new ArgumentOutOfRangeException();

			if (index == 0 && strLen <= maxLength)
				return str;

			int finalLen = strLen - index;
			bool useEllipsis = ellipsis.NotNulle();

			if (maxLength < finalLen)
				finalLen = maxLength;
			else
				useEllipsis = false; // was true up to here if wasn't nulle

			// WOULD BE MORE PERFORMANT TO DO THE WORD-BREAK SEARCH HERE,
			// NOT NEED AN EXTRA STRING ALLOC. WANNA DO IT?! GO AHEAD, MAKE MY DAY!

			int postIdx = index + finalLen;
			string result = str.Substring(index, finalLen);

			// WORD-BREAK SEARCH
			if (tryBreakOnWord && postIdx < strLen && char.IsLetterOrDigit(str, postIdx) && result.Length > minCheckWordBreak) {
				int i = 0;
				int x = result.Length - 1;
				for (; i < maxWordBreakSize && x >= minCheckWordBreak; i++, x--) {
					if (char.IsWhiteSpace(result[x]))
						break;
				}
				if (i > 0 && i < maxWordBreakSize && x >= minCheckWordBreak)
					result = result.Substring(0, x + 1);
			}

			if (useEllipsis)
				result += ellipsis;

			return result;
		}

		#endregion

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
			if (str.IsNulle() || startsWithValue.IsNulle() || str.Length <= startsWithValue.Length)
				return null;

			if (!str.StartsWith(startsWithValue))
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
			if (str.IsNulle() || val.IsNulle())
				return str;

			int idx = str.IndexOf(val);
			if (idx < 0)
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
			if (str.IsNulle() || val.IsNulle())
				return str;

			int idx = str.IndexOf(val);
			if (idx < 0)
				return null;

			return str.Substring(0, (includeSearchVal ? idx + val.Length : idx));
		}

		public static bool EqualsIgnoreCase(this string str1, string str2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		{
			if (str1 == null)
				return str2 == null;
			else if (str2 == null)
				return false;
			return str1.Equals(str2, comparison);
		}

		public static bool ContainsIgnoreCase(this string str1, string value)
		{
			if (str1 == null)
				return value == null;
			else if (value == null)
				return false;

			return ContainsN(str1, value, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ContainsN(this string str, string value, StringComparison comparison)
		{
			if (str == null || value.IsNulle()) return false;
			int idx = str.IndexOf(value, comparison);
			return idx >= 0;
		}

		public static bool ContainsN(this string str, string value)
		{
			return str == null ? false : str.Contains(value);
		}

		/// <summary>
		/// Returns true if the search string occurs *anywhere* within the maximum
		/// range (see <paramref name="maxCount"/>) from starting point (see <paramref name="startIndex"/>, 
		/// 0 by default). See <see cref="IndexOfMax(string, string, int, int, StringComparison?)"/> for further
		/// details.
		/// </summary>
		public static bool ContainsMax(this string src, string value, int maxCount, int startIndex = 0, StringComparison? comparisonType = null)
		{
			int idx = IndexOfMax(src, value, maxCount, startIndex, comparisonType);
			return idx >= 0;
		}

		/// <summary>
		/// Returns the index of search string if it occurs *anywhere* within the maximum
		/// range (see <paramref name="maxCount"/>) from starting point (see <paramref name="startIndex"/>, 
		/// 0 by default).
		/// Makes for a much more efficient search, when you want to search from the beginning of a string,
		/// or from a starting point in the string, where you know you can apply a maximum beyond which 
		/// the search *should discontinue*. For instance, if you have a long
		/// text (many paragraphs or more), and you want to know if a certain string exists within just the 
		/// first 20 characters, this allows you to search only within that range, rather than a full 
		/// string.Contains search searching the entire string.
		/// </summary>
		/// <param name="src"></param>
		/// <param name="value"></param>
		/// <param name="maxCount">The maximum count after start index to perform the search.</param>
		/// <param name="startIndex"></param>
		/// <param name="comparisonType"></param>
		public static int IndexOfMax(this string src, string value, int maxCount, int startIndex = 0, StringComparison? comparisonType = null)
		{
			if (startIndex < 0 || maxCount < 0) throw new ArgumentOutOfRangeException();

			const int noFind = -1;

			if (maxCount == 0)
				return noFind;

			if (src == null)
				return noFind;

			int len = src.Length;

			if (len == 0)
				return value == "" ? 0 : noFind;

			if (value.IsNulle())
				return noFind;

			// make sure both int values were non-negative already
			int _maxFull = len - startIndex;

			if (_maxFull < 1)
				return noFind;

			if (maxCount > _maxFull)
				maxCount = _maxFull;

			if (value.Length > maxCount)
				return noFind;

			int idx = comparisonType == null
				? src.IndexOf(value, startIndex, maxCount)
				: src.IndexOf(value, startIndex, maxCount, comparisonType.Value);

			return idx;
		}


		/// <summary>
		/// Returns string.ToLower, but null if input string is null.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static string ToLowerN(this string str)
		{
			return str == null ? null : str.ToLower();
		}

		public static string Replace(this string s, Func<char, char> replaceChar)
		{
			if (s.IsNulle()) return s;
			if (replaceChar == null) throw new ArgumentNullException();

			char[] arr = new char[s.Length];
			bool crunch = false;
			int len = s.Length;
			for (int i = 0; i < len; i++) {
				char c = replaceChar(s[i]);
				if (c > 0)
					arr[i] = c;
				else if (crunch != true)
					crunch = true;
			}

			if (crunch) {
				arr = arr.Where(c => c != 0).ToArray();
				if (arr.Length == 0) return "";
			}
			string result = new string(arr);
			return result;
		}

		/// <summary>
		/// Returns a string.Replace result *only* after first validating that at least one instance
		/// of the specified 'find' string exists in the source string (by means of <see cref="String.IndexOf(string)"/>).
		/// The benefit to this is, if none exists, the same string is then returned, which means that NO new string
		/// allocations were ever needed or incurred, but only a single IndexOf operation. This method is useful in
		/// scenarios where you suspect a majority of times no replacement was needed. Otherwise, it will add a small
		/// extra overhead of conducting an extra IndexOf operation, so use this accordingly.
		/// </summary>
		/// <param name="s">Source string.</param>
		/// <param name="find">The string to be replaced, if it exists</param>
		/// <param name="replace">The string to replace the find string with, if it exists</param>
		public static string ReplaceIfNeeded(this string s, string find, string replace)
		{
			if (s.IsNulle()) return s;

			int idx = s.IndexOf(find);

			if (idx > 0)
				s = s.Replace(find, replace);

			return s;
		}

		public static string ReplaceIfNeeded(this string s, char find, char replace)
		{
			if (s.IsNulle()) return s;

			int idx = s.IndexOf(find);

			if (idx > 0)
				s = s.Replace(find, replace);

			return s;
		}

		public static bool FollowsWith(this string a, string b, int startIndex)
		{
			if (a == null) return b == null;
			if (b == null) return false;

			if (startIndex + b.Length > a.Length)
				return false;

			for (int i = 0; i < b.Length; i++)
				if (a[i + startIndex] != b[i])
					return false;

			return true;
		}

		#region -- StartsWith / EndsWith --

		public static bool StartsWithN(this string s, char c)
		{
			if (s.IsNulle()) return false;
			return s[0] == c;
		}

		public static bool StartsWithN(this string s, string val)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.StartsWith(val);
		}

		public static bool StartsWithN(this string s, string val, StringComparison comparison)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.StartsWith(val, comparison);
		}


		public static bool EndsWithN(this string s, char c)
		{
			if (s.IsNulle()) return false;
			return s[s.Length - 1] == c;
		}

		public static bool EndsWithN(this string s, string val)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.EndsWith(val);
		}

		public static bool EndsWithN(this string s, string val, StringComparison comparison)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.EndsWith(val, comparison);
		}


		public static bool StartsWithIgnore(this string s, string val, Predicate<char> ignore, StringComparison sc = StringComparison.Ordinal)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			int i = 0;
			for (; i < s.Length; i++) {
				if (!ignore(s[i]))
					break;
			}
			if (i >= s.Length)
				return false;
			if (i > 0)
				s = s.Substring(i);
			return s.StartsWith(val, sc);
		}

		public static bool StartsWithIgnoreWhite(this string s, string val, StringComparison sc = StringComparison.Ordinal)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.StartsWithIgnore(val, c => char.IsWhiteSpace(c), sc);
		}

		public static bool StartsWithIgnoreCase(this string s, string val)
		{
			if (s.IsNulle()) return val == null;
			if (val == null) return false;
			return s.StartsWith(val, StringComparison.OrdinalIgnoreCase);
		}

		public static bool StartsWithIgnoreCaseIndex(this string s, string val, int startIndex)
		{
			throw new NotImplementedException();
			//if (s.IsNulle() || startIndex >= s.Length) return val == null;

			//int len = s.Length - startIndex;
			//if (val == null || val.Length > len) return false;

			//for (int i = startIndex, j = 0; i < len; i++) {
			//	if (char.ToLower(s[i]) == char.ToLower(val[i]))
			//		return i;
			//         }
			//return s.StartsWith(val, StringComparison.OrdinalIgnoreCase);
		}

		#endregion


		#region --- EndsWithAny ---

		public static bool EndsWithAny(this string s, params string[] vals)
		{
			return EndsWithAnyIndex(s, vals, null) >= 0;
		}

		public static bool EndsWithAny(this string s, StringComparison? comparison, params string[] vals)
		{
			return EndsWithAnyIndex(s, vals, comparison) >= 0;
		}

		public static bool EndsWithAny(this string s, IEnumerable<string> vals, StringComparison? comparison = null)
		{
			return EndsWithAnyIndex(s, vals, comparison) >= 0;
		}

		public static int EndsWithAnyIndex(this string s, IEnumerable<string> vals, StringComparison? comparison = null)
		{
			int i = 0;
			if (s.NotNulle() && vals != null) {
				StringComparison _comparison = comparison ?? StringComparison.Ordinal;
				foreach (var val in vals)
					if (val != null && s.EndsWith(val, _comparison))
						return i;
			}
			return -1;
		}

		#endregion

	}
}
