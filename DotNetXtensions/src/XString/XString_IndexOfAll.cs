using System;
using System.Collections.Generic;

namespace DotNetXtensions
{
	public static partial class XString
	{
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
			if(s == null) throw new ArgumentNullException("s");
			if(searchValue == null) throw new ArgumentNullException("searchValue");

			if(s.Length == 0) return new int[0]; // must do this before next; bec if s.Length = 0, it would throw exception

			if((index < 0) || (length < 0) || (index >= s.Length) || (length > (s.Length - index)))
				throw new ArgumentOutOfRangeException();

			List<int> finds = new List<int>();

			int currentPos = index;

			for(int runningLen = length; runningLen >= 0; runningLen = length - (++currentPos - index)) {
				currentPos = s.IndexOf(searchValue, currentPos, runningLen, comparisonType);

				if(currentPos < 0)
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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");

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
			if(s == null) throw new ArgumentNullException("s");

			return __IndexOfAll(s, searchValue, index, length, comparisonType);
		}

	}
}
