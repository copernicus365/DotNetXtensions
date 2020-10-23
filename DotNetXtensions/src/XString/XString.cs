using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace DotNetXtensions
{
	public static partial class XString
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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");
			if(predicate == null) throw new ArgumentNullException("predicate");

			if((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			for(int i = index; i < length; i++)
				if(!predicate(s[i]))
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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");
			if(predicate == null) throw new ArgumentNullException("predicate");

			if((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			for(int i = index; i < length; i++)
				if(predicate(s[i]))
					return true;

			return false;
		}

		#endregion Any

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
			if(s == null || value == null)
				return -1;
			int idx = s.IndexOf(value, startIndex);
			if(idx < 0)
				return -1;
			return idx + value.Length;
		}



		public static bool PreviousIndexIsMatch(this string s, int idx, Func<char, bool?> matchOrBreakOrContinuePredicate)
		{
			return s.PreviousIndexOf(idx, matchOrBreakOrContinuePredicate) >= 0;
		}

		public static int PreviousIndexOf(this string s, int idx, Func<char, bool?> matchOrBreakOrContinuePredicate)
		{
			if(s.IsNulle() || idx >= s.Length)
				return -1;

			bool? result = null;
			while(idx >= 0) {

				result = matchOrBreakOrContinuePredicate(s[idx]);

				if(result != null) {
					if(result == true)
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
			if(s == null || s.Length < 2) return s;

			if(char.IsUpper(s[0]))
				s = char.ToLower(s[0]) + s.Substring(1);

			return s;
		}

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
			if(s == null) throw new ArgumentNullException();
			if(lengthTillBreak < 1) throw new ArgumentOutOfRangeException();

			if(breakVal == null)
				breakVal = Environment.NewLine;

			int len = s.Length;
			int numLines = len.DivideUp(lengthTillBreak);
			int totalLenMax = (numLines * breakVal.Length) + numLines * lengthTillBreak; // includes "\r\n" per line

			StringBuilder sb = new StringBuilder(totalLenMax);

			for(int i = 0; i < len; i += lengthTillBreak) {
				int lineLen = Math.Min(len - i, lengthTillBreak);
				sb.Append(s, i, lineLen);
				sb.Append(breakVal);
			}

			return endWithBreak || sb.Length < 1
				? sb.ToString()
				: sb.ToString(0, sb.Length - breakVal.Length);
		}

		#endregion

		public static bool EqualsIgnoreCase(this string str1, string str2, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
		{
			if(str1 == null)
				return str2 == null;
			else if(str2 == null)
				return false;
			return str1.Equals(str2, comparison);
		}

		public static bool ContainsIgnoreCase(this string str1, string value)
		{
			if(str1 == null)
				return value == null;
			else if(value == null)
				return false;

			return ContainsN(str1, value, StringComparison.OrdinalIgnoreCase);
		}

		public static bool ContainsN(this string str, string value, StringComparison comparison)
		{
			if(str == null || value.IsNulle()) return false;
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
			if(startIndex < 0 || maxCount < 0) throw new ArgumentOutOfRangeException();

			const int noFind = -1;

			if(maxCount == 0)
				return noFind;

			if(src == null)
				return noFind;

			int len = src.Length;

			if(len == 0)
				return value == "" ? 0 : noFind;

			if(value.IsNulle())
				return noFind;

			// make sure both int values were non-negative already
			int _maxFull = len - startIndex;

			if(_maxFull < 1)
				return noFind;

			if(maxCount > _maxFull)
				maxCount = _maxFull;

			if(value.Length > maxCount)
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
			if(s.IsNulle()) return s;
			if(replaceChar == null) throw new ArgumentNullException();

			char[] arr = new char[s.Length];
			bool crunch = false;
			int len = s.Length;
			for(int i = 0; i < len; i++) {
				char c = replaceChar(s[i]);
				if(c > 0)
					arr[i] = c;
				else if(crunch != true)
					crunch = true;
			}

			if(crunch) {
				arr = arr.Where(c => c != 0).ToArray();
				if(arr.Length == 0) return "";
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
			if(s.IsNulle()) return s;

			int idx = s.IndexOf(find);

			if(idx > 0)
				s = s.Replace(find, replace);

			return s;
		}

		public static string ReplaceIfNeeded(this string s, char find, char replace)
		{
			if(s.IsNulle()) return s;

			int idx = s.IndexOf(find);

			if(idx > 0)
				s = s.Replace(find, replace);

			return s;
		}

	}
}
