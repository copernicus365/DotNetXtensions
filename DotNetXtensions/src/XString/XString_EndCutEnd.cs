using System;
using System.Linq;

namespace DotNetXtensions
{
	public static partial class XString
	{
		// --- End and Cut End Functions ---

		/// <summary>
		/// Gets the char from teh specified count *from the end* of the string.
		/// Returns <see cref="char.MinValue"/> if index was outside of range 
		/// of the string.
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="indexFromEnd">Index from end of string to get the char from.
		/// Consider the last char index '0', the second to last index '1', etc.</param>
		/// <returns></returns>
		public static char CharFromEnd(this string s, int indexFromEnd)
		{
			int len = s?.Length ?? -1;
			if(len > 0 && indexFromEnd < len && indexFromEnd >= 0) {
				int idx = len - indexFromEnd - 1;
				return s[idx];
			}
			return char.MinValue;
		}

		public static string ReplaceEnd(this string s, int cutFromEnd, string newEnd)
		{
			int origLen = s?.Length ?? 0;
			string val = s.CutEnd(cutFromEnd);
			if(val?.Length != origLen)
				val += newEnd;
			return val;
		}

		/// <summary>
		/// Removes the specified length of chars from the end of source string 
		/// and returns the substring result. Gracefully handles if the cut length is 
		/// greater than or equal to the source string, in which case returns an empty 
		/// string.
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="cutFromEnd">Count of chars to remove (cut) from the end of source string</param>
		public static string CutEnd(this string s, int cutFromEnd)
		{
			if(s.IsNulle() || cutFromEnd < 0) return s;
			if(cutFromEnd >= s.Length)
				return "";

			string value = s.Substring(0, s.Length - cutFromEnd);
			return value;
		}

		/// <summary>
		/// Removes the final substring from source string *if* it actually ends with the specified
		/// <paramref name="endString"/>. Else returns full string. If <paramref name="checkDoesEndWith"/>
		/// is true, then no check is even done to see that it actually ends with <paramref name="endString"/>.
		/// This is useful when one has already checked, so the extra check does not have to be
		/// reperformed. Gracefully ignores any null or empty strings (by returning source string), 
		/// or when end is longer than source (by returning empty).
		/// </summary>
		/// <param name="s">Source string.</param>
		/// <param name="endString">End string expected to occur at end of source string. 
		/// If not, will return source string.</param>
		/// <param name="checkDoesEndWith">True (by default) to check the end does actually end
		/// the source. Specify false to ignore this check if you already know it is a match.</param>
		/// <param name="ignoreCase">True to have the end check done case-insensitive (OrdinalIgnoreCase).</param>
		public static string CutEndIfEndsWith(this string s, string endString, bool checkDoesEndWith = true, bool ignoreCase = false)
		{
			if(s.IsNulle() || endString.IsNulle())
				return s;

			if(checkDoesEndWith && endString.Length <= s.Length) {
				if(!s.EndsWith(endString, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
					return s;
			}

			return s.CutEnd(endString.Length);
		}

		/// <summary>
		/// Removes the final character from string *if* it ends with the specified
		/// <paramref name="endChar"/>. Else returns full string.
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="endChar">End character to remove if it exists as last char.</param>
		/// <param name="ignoreCase">True to ignore case.</param>
		public static string CutLastCharIfEndsWith(this string s, char endChar, bool ignoreCase = false)
		{
			if(s.IsNulle())
				return s;

			char last = s.Last();

			if(ignoreCase) {
				last = char.ToLower(last);
				endChar = char.ToLower(endChar);
			}

			if(last != endChar)
				return s;

			return s.Substring(0, s.Length - 1);
		}

		/// <summary>
		/// Gets the end substring of the source string by the specified length.
		/// *NO* exception is thrown if the length was longer than string, 
		/// just returns full string then.
		/// </summary>
		/// <param name="s">String</param>
		/// <param name="length">Count of characters from end to get.</param>
		public static string End(this string s, int length)
		{
			if(s == null || s.Length <= length || length < 1)
				return s;

			return s.Substring(s.Length - length, length);
		}

	}
}
