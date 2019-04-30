/* MAINTAINANCE NOTE!!

1) I wrote a huge portion of these functions nearly 10 years ago, ... yeah, prior
to even the day when optional parameters existed! So would be good to go through
and cleanup a lot of these overrides that can be combined.

2) Some of the functions here focused on high performance finding (see SequenceIsEqual), 
may best just be deleted now as not quite worth it.

Till then...
Nicholas Petersen
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for arrays.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XArray
	{

		#region BinarySearch

		/// <summary>
		/// Searches this array for the specified item using a rapid binary chop 
		/// search, which presumes that this array is sorted (the search will fail if it is not).
		/// The default IComparable&lt;T&gt; generic interface is implemented by each element 
		/// of this array type in comparing items.
		/// </summary>
		public static int BinarySearch<T>(this T[] arr, T value)
		{
			return Array.BinarySearch<T>(arr, value);
		}

		public static int BinarySearch<T>(this T[] arr, T value, IComparer<T> comparer)
		{
			return Array.BinarySearch<T>(arr, value, comparer);
		}

		public static int BinarySearch<T>(this T[] arr, int index, int length, T value)
		{
			return Array.BinarySearch<T>(arr, index, length, value);
		}

		public static int BinarySearch<T>(this T[] arr, int index, int length, T value, IComparer<T> comparer)
		{
			return Array.BinarySearch<T>(arr, index, length, value, comparer);
		}

		// Non-Generic

		public static int BinarySearch(this Array arr, object value)
		{
			return Array.BinarySearch(arr, value);
		}

		public static int BinarySearch(this Array arr, object value, IComparer comparer)
		{
			return Array.BinarySearch(arr, value, comparer);
		}

		public static int BinarySearch(this Array arr, int index, int length, object value)
		{
			return Array.BinarySearch(arr, index, length, value);
		}

		public static int BinarySearch(this Array arr, int index, int length, object value, IComparer comparer)
		{
			return Array.BinarySearch(arr, index, length, value, comparer);
		}

		#endregion

		#region Copy

		public static T[] Copy<T>(this T[] arr)
		{
			if(arr == null) return null;

			T[] copiedArray = new T[arr.Length];
			Array.Copy(arr, 0, copiedArray, 0, arr.Length);
			return copiedArray;
		}

		public static T[] Copy<T>(this T[] arr, int start)
		{
			return Copy<T>(arr, start, arr.Length - start);
		}

		public static T[] Copy<T>(this T[] arr, int start, int length)
		{
			if (arr == null) return null;

			if ((start < 0) || (length < 0) || (start >= arr.Length) || (length > (arr.Length - start)))
				throw new ArgumentOutOfRangeException();

			T[] copiedArray = new T[length];
			Array.Copy(arr, start, copiedArray, 0, length);
			return copiedArray;
		}

		public static T[] ToArray<T>(this List<T> list, int start)
		{
			return ToArray<T>(list, start, list.Count - start);
		}

		public static T[] ToArray<T>(this List<T> list, int start, int length)
		{
			if(list == null) return null;

			if ((start < 0) || (length < 0) || (start >= list.Count) || (length > (list.Count - start)))
				throw new ArgumentOutOfRangeException();

			T[] copiedArray = new T[length];
			list.CopyTo(start, copiedArray, 0, length);
			return copiedArray;
		}


		#endregion

		#region Foreach

		public static IEnumerable<T> Foreach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if(source == null) return source;
			if (action == null) throw new ArgumentNullException("action");

			foreach (T item in source)
				action(item);

			return source;
		}

		public static IEnumerable<T> Foreach<T>(this IEnumerable<T> source, Action<T, int> action)
		{
			if(source == null) return source;
			if (action == null) throw new ArgumentNullException("action");

			int i = 0;
			foreach (T item in source)
				action(item, i++);
			
			return source;
		}
		
		#endregion

		#region IndexOf -- Generic

		public static int IndexOf<T>(this T[] arr, T value)
		{
			return Array.IndexOf<T>(arr, value);
		}

		public static int IndexOf<T>(this T[] arr, T value, int start)
		{
			return Array.IndexOf<T>(arr, value, start);
		}

		public static int IndexOf<T>(this T[] arr, T value, int start, int length)
		{
			return Array.IndexOf<T>(arr, value, start, length);
		}

		#endregion

		#region IndexOfAll

		/// <summary>
		/// Returns the index positions of all occurences of <paramref name="value"/>
		/// that are found in this array.
		/// <example><code>
		/// <![CDATA[
		/// int[] ints = { 66, 95, 32, 120, 66, 89, 66, 32 };
		/// 
		/// int[] indexes = ints.IndexOfAll(66); // indexes = { 0, 4, 6 }
		/// ]]></code></example>
		/// </summary>
		/// <typeparam name="T">This type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="value">The value (i.e. array element) to search for.</param>
		/// <returns>The index positions of all occurences of <paramref name="value"/>,
		/// or an empty integer array (not a null array).</returns>
		public static int[] IndexOfAll<T>(this T[] arr, T value)
		{
			if (arr == null) return null;

			List<int> indexes = new List<int>();

			int indexHit = Array.IndexOf(arr, value);  // arr.IndexOf(value);

			while (indexHit >= 0)
			{
				indexes.Add(indexHit);
				indexHit = Array.IndexOf(arr, value, indexHit + 1); // arr.IndexOf(value, indexHit + 1);
			}
			if (indexes.Count < 1)
				return new int[0];
			else
				return indexes.ToArray();
		}

		#endregion

		#region LastIndexOf -- Generic / Non-Generic

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in this array, or -1 if it is not located.
		/// <example><code>
		/// <![CDATA[
		/// int[] years = { 1912, 2009, 1999, 1017, 2009, 1278, 1953 };
		/// 
		/// int index = years.LastIndexOf(2009); // index == 4
		/// ]]></code></example>
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf<T>(this T[] arr, T value)
		{
			return Array.LastIndexOf<T>(arr, value);
		}

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in the specified range of this array, or -1 if it is not located.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <param name="start">The zero-based starting index of the backward search.</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf<T>(this T[] arr, T value, int start)
		{
			return Array.LastIndexOf<T>(arr, value, start);
		}

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in the specified range of this array, or -1 if it is not located.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <param name="start">The zero-based starting index of the backward search.</param>
		/// <param name="length">The number of elements to search on following start.</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf<T>(this T[] arr, T value, int start, int length)
		{
			return Array.LastIndexOf<T>(arr, value, start, length);
		}

		//

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in this array, or -1 if it is not located.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf(this Array arr, object value)
		{
			return Array.LastIndexOf(arr, value);
		}

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in the specified range of this array, or -1 if it is not located.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <param name="start">The zero-based starting index of the backward search.</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf(this Array arr, object value, int start)
		{
			return Array.LastIndexOf(arr, value, start);
		}

		/// <summary>
		/// Searches for the specified object and returns the index of the last ocurrence
		/// in the specified range of this array, or -1 if it is not located.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="value">The object to locate in this array</param>
		/// <param name="start">The zero-based starting index of the backward search.</param>
		/// <param name="length">The number of elements to search on following start.</param>
		/// <returns>The index of the located object, or -1 if it is not found.</returns>
		public static int LastIndexOf(this Array arr, object value, int start, int length)
		{
			return Array.LastIndexOf(arr, value, start, length);
		}

		#endregion

		#region IndexOfSequence

		/// <summary>
		/// Returns the index position where the find sequence is found
		/// within this array.
		/// <example><code>
		/// <![CDATA[
		/// byte[] ints1 = { 66, 95, 32, 120, 66, 89, 66, 32 };
		/// 
		/// int index = ints1.IndexOfSequence(new byte[] { 32, 120, 66 }); // index == 2
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <typeparam name="T">The sequence type.</typeparam>
		/// <param name="srcArray">The source array to search on.</param>
		/// <param name="findSeq">The sequence to search for.</param>
		/// <returns>The index of the find, or -1 if it is not found.</returns>
		public static int IndexOfSequence<T>(this T[] srcArray, T[] findSeq) where T : IEquatable<T>
		{
			if (srcArray == null) throw new ArgumentNullException("srcArray");

			return IndexOfSequence(srcArray, findSeq, 0, srcArray.Length);
		}

		/// <summary>
		/// Returns the index position where the find sequence is found
		/// within the specified range of this array.
		/// </summary>
		/// <typeparam name="T">The sequence type.</typeparam>
		/// <param name="srcArray">The source array to search on.</param>
		/// <param name="findSeq">The sequence to search for.</param>
		/// <param name="start">The zero based starting index.</param>
		/// <returns>The index of the find, or -1 if it is not found.</returns>
		public static int IndexOfSequence<T>(this T[] srcArray, T[] findSeq, int start) where T : IEquatable<T>
		{
			if (srcArray == null) throw new ArgumentNullException("srcArray");

			return IndexOfSequence(srcArray, findSeq, start, (srcArray.Length - start));
		}

		/// <summary>
		/// Returns the index position where the find sequence is found
		/// within the specified range of this array.
		/// </summary>
		/// <typeparam name="T">The sequence type.</typeparam>
		/// <param name="srcArray">The source array to search on.</param>
		/// <param name="findSeq">The sequence to search for.</param>
		/// <param name="start">The zero based starting index of the search.</param>
		/// <param name="length">The maximum number of items to search following start.</param>
		/// <returns>The index of the find, or -1 if it is not found.</returns>
		public static int IndexOfSequence<T>(this T[] srcArray, T[] findSeq, int start, int length) where T : IEquatable<T>
		{
			if (srcArray == null) throw new ArgumentNullException("srcArray");
			if (findSeq == null) throw new ArgumentNullException("findSeq");

			if (srcArray.Length == 0) return -1;

			if ((start < 0) || (length < 0) || (start >= srcArray.Length) || (length > (srcArray.Length - start)))
				throw new ArgumentOutOfRangeException();

			int findLen = findSeq.Length;

			if (length < findLen)
				return -1;

			int findLenMinus1 = findSeq.Length - 1;

			T firstItem = findSeq[0];
			T lastItem = findSeq[findLenMinus1];

			int len = length - findLen;
			int i = 0;
			int currentPos = Array.IndexOf(srcArray, firstItem, start);

			for (; currentPos >= 0 && currentPos <= len; currentPos = Array.IndexOf(srcArray, firstItem, currentPos + 1))
			{
				if (srcArray[currentPos + findLenMinus1].Equals(lastItem)) // see notes
				{
					for (i = 1; i < findLen; i++)
						if (!findSeq[i].Equals(srcArray[currentPos + i]))
							break;

					if (i == findLen)
						return currentPos;
				}
			}
			return -1;

			#region NOTES

			// 1) The first if statement inside the main loop does the following:
			// After the first item is determined to be a match, this if determines if the last 
			// item in the find sequence is a match.  If not, and most the time IT IS NOT, we immediately
			// continue the another round.  This is faster than immediately entering a loop, with its
			// initializations, etc.  Further, take the case of a byte[].  In unicode, very frequently we'll
			// have two bytes representing a char.  So checking an item in the find sequence other than
			// the second item has benefits.
			// 
			// 2) Following is a different way to do this that includes specifying the length/count 
			// in the call to Array.IndexOf.  This has a small benefit for a large disadvantage of 
			// having far more bounds checking for EVERY loop.  In practise, the timing was just
			// barely slower, but we'd rather keep this as trimmed as possible.
			// 
			//   int currentPos = start;
			//   int runningLen = length;
			//   while (runningLen >= 0) {
			//       currentPos = Array.IndexOf(srcArray, firstItem, currentPos, runningLen);
			//       if (currentPos < 0) break;
			//
			//       for (i = 1; i < findLen; i++)
			//           if (!findSeq[i].Equals(srcArray[currentPos + i])) break;
			//
			//       if (i == findLen) return currentPos;
			//       runningLen = length - (++currentPos - start);
			//   }

			#endregion
		}

		#endregion

		#region RemoveSequentialValues

		/// <summary>
		/// See overload.
		/// </summary>
		public static T[] RemoveSequentialValues<T>(this T[] arr, T dup, bool trimEnds = false) where T : IEquatable<T>
		{
			if (arr == null) throw new ArgumentNullException();
			int len = arr.Length;
			if (len == 0) return arr;
			List<T> list = new List<T>(len);
			T t = default(T);
			bool lastWas = false;

			for (int i = 0; i < len; i++) {

				t = arr[i];
				if (!t.Equals(dup)) {
					list.Add(t);
					lastWas = false;
				}
				else {
					if (!lastWas)
						list.Add(t);
					lastWas = true;
				}
			}

			if (trimEnds) {
				len = list.Count;
				if (len == 0)
					return new T[0];
				else if (len == 1 && list[0].Equals(dup))
					return new T[0];
				else {
					int start = list[0].Equals(dup) ? 1 : 0;
					int cutLen = len - start - (list[len - 1].Equals(dup) ? 1 : 0);
					if (cutLen != len)
						return list.ToArray(start, cutLen);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// Trims any occurences of the specified duplicate value T to only one (1) occurence.
		/// Optionally trims the beginning and end of this value from the return result.
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="arr">Array</param>
		/// <param name="dup">Duplicate value to trim to one when two or more occur in the sequence.</param>
		/// <param name="trimEnds">True to trim the end result of the duplicate value if occuring.</param>
		public static T[] RemoveSequentialValues<T>(this IEnumerable<T> arr, T dup, bool trimEnds = false) where T : IEquatable<T>
		{
			return RemoveSequentialValues(arr.ToArray(), dup, trimEnds);
		}

		#endregion

		#region ReverseArray

		/// <summary>
		/// Reverses the sequence of elements in this array.
		/// <example><code>
		/// <![CDATA[
		/// string[] liftoff = { "ten", "nine", "eight", "seven" };
		/// 
		/// liftoff.ReverseArray();  // liftoff == { "seven", "eight", "nine", "ten" }
		/// ]]></code></example>
		/// <example><code>
		/// <![CDATA[
		/// int[] years = { 1912, 1847, 1999, 1017, 2009, 1278, 1953 };
		/// 
		/// int[] sortedYearsReversed = years.Copy().Sort().ReverseArray();
		/// // sortedYearsReversed == { 2009, 1999, 1953, 1912, 1847, 1278, 1017 }
		/// 
		/// // Note that by including "Copy()" in "years.Copy()...", the original array "years"
		/// // remains unaltered, while the new (and desired) sequence gets returned.
		/// ]]></code></example>
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <returns></returns>
		public static T[] ReverseArray<T>(this T[] arr)
		{
			Array.Reverse(arr);
			return arr;
		}

		/// <summary>
		/// Reverses the sequence of elements in the specified range of this array.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="start">The starting index at which to begin.</param>
		/// <returns></returns>
		public static T[] ReverseArray<T>(this T[] arr, int start)
		{
			if (arr == null) return null;

			Array.Reverse(arr, start, (arr.Length - start));
			return arr;
		}

		/// <summary>
		/// Reverses the sequence of elements in the specified range of this array.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="start">The starting index at which to begin.</param>
		/// <param name="length">The number of elements to reverse following start.</param>
		/// <returns></returns>
		public static T[] ReverseArray<T>(this T[] arr, int start, int length)
		{
			Array.Reverse(arr, start, length);
			return arr;
		}

		#endregion

		#region SequenceIsEqual

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// <example><code>
		/// <![CDATA[
		/// byte[] seq1 = { 120, 93, 248 };
		/// 
		/// byte[] seq2 = { 120, 93, 248 };
		/// 
		/// bool isEqual = seq1.SequenceIsEqual(seq2);  // isEqual == true
		/// ]]></code></example>
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this byte[] srcArray, byte[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if (srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;

			//int len = srcArray.Length;
			//if (len != comparisonArray.Length)
			//    return false;

			//if (len == 0)
			//    return true;

			//int i = 0;
			//while (i < len)
			//    if (srcArray[i] != comparisonArray[i++])
			//        return false;

			//return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this char[] srcArray, char[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if (srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this short[] srcArray, short[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this int[] srcArray, int[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this long[] srcArray, long[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this double[] srcArray, double[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this decimal[] srcArray, decimal[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual(this string[] srcArray, string[] comparisonArray)
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (srcArray[pos] != comparisonArray[pos++]) return false;

			if (iterations == 0) return true;

			do
			{
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
				if (srcArray[pos] != comparisonArray[pos++]) return false;
			}
			while (--iterations > 0);

			return true;
		}

		/// <summary>
		/// Compares this array with <paramref name="comparisonArray"/> by iterating 
		/// through each item to see if they are equivalent.
		/// <example><code>
		/// <![CDATA[
		/// byte[] seq1 = { 120, 93, 248 };
		/// 
		/// byte[] seq2 = { 120, 93, 248 };
		/// 
		/// bool isEqual = seq1.SequenceIsEqual(seq2);  // isEqual == true
		/// ]]></code></example>
		/// </summary>
		/// <param name="srcArray">This array.</param>
		/// <param name="comparisonArray">The array to compare.</param>
		/// <returns>The start index of the found sequence, or -1 if it is not found.</returns>
		public static bool SequenceIsEqual<T>(this T[] srcArray, T[] comparisonArray) where T : IEquatable<T>
		{
			if(srcArray == null) return comparisonArray == null;
			if(comparisonArray == null) return false;

			if(srcArray.Length != comparisonArray.Length)
				return false;

			int loops = 8;
			int remainder = srcArray.Length % loops;
			int iterations = (srcArray.Length - remainder) / loops;
			int pos = 0;

			for (int i = 0; i < remainder; i++)
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;

			if (iterations == 0) return true;

			do
			{
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
				if (!srcArray[pos].Equals(comparisonArray[pos++])) return false;
			}
			while (--iterations > 0);

			return true;
		}

		#endregion

		#region Sort

		/// <summary>
		/// Sorts the elements in this array using the default IComparable&lt;T&gt;
		/// implementation.<para/><para/>
		/// <example><code>
		/// <![CDATA[
		/// int[] years = { 1912, 1847, 1999, 1017, 2009, 1278, 1953 };
		/// 
		/// int[] sortedYears = years.Copy().Sort();
		/// // sortedYears == { 1017, 1278, 1847, 1912, 1953, 1999, 2009 }
		/// 
		/// // Note that by including "Copy()" in "years.Copy()...", the original array "years"
		/// // remains unaltered, while the new (and desired) sequence gets returned.
		/// ]]></code></example>
		/// <example><code>
		/// <![CDATA[
		/// string[] names = "Fernando, Alice, Zachery, Joe, Niel, Robert, Sandra".RxSplit(@"\s*,\s*");
		/// // names == { "Fernando", "Alice", "Zachery", "Joe", "Niel", "Robert", "Sandra" }
		/// 
		/// string[] sorted = names.Copy().Sort();
		/// // sorted == { "Alice", "Fernando", "Joe", "Niel", "Robert", "Sandra", "Zachery" }
		/// 
		/// // * RxSplit() is a DotNetExtensions extension method.
		/// ]]></code></example>
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		public static T[] Sort<T>(this T[] arr)
		{
			Array.Sort<T>(arr);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in this array using the specified Comparison&lt;T&gt;.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="comparison">The Comparison&lt;T&gt; to use when comparing elements.</param>
		public static T[] Sort<T>(this T[] arr, Comparison<T> comparison)
		{
			Array.Sort<T>(arr, comparison);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in this array using the specified IComparer&lt;T&gt; 
		/// interface.
		/// <example><code>
		/// <![CDATA[
		/// // To see the full declaration of this IComparer<T>, see the full entry of this method overload)
		/// PresidentsComparer presCompare = new PresidentsComparer();
		/// 
		/// string[] somePresidents = { "Truman", "Garfield", "Clinton", "Lincoln", "Reagan", "Roosevelt" };
		/// 
		/// string[] alphaSorted = somePresidents.Copy().Sort();
		/// // A regular sort:  alphaSorted == { Clinton,Garfield,Lincoln,Reagan,Roosevelt,Truman }
		/// 
		/// string[] historySorted = somePresidents.Copy().Sort(presCompare);
		/// // Custom comparer sorted:  historySorted == { Lincoln,Garfield,Roosevelt,Truman,Reagan,Clinton }
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <example><code>
		/// <![CDATA[
		/// // A simple comparer that allows US presidents to be historically sorted
		/// public class PresidentsComparer : IComparer<string>
		/// {
		///    public string[] presidents;
		/// 
		///    // CONSTRUCTOR:
		///    public PresidentsComparer()
		///    {
		///       presidents = new string[] { 
		///          "Washington",  "Adams",  "Jefferson",  "Madison", "Monroe",  "Adams II",  
		///          "Jackson",  "Van Buren", "Harrison",  "Tyler",  "Polk",  "Taylor", "Fillmore",  
		///          "Pierce",  "Buchanan",  "Lincoln", "Johnson", "Grant", "Hayes", "Garfield",  
		///          "Arthur",  "Cleveland",  "Harrison",  "Cleveland II", "McKinley", "Roosevelt", 
		///          "Taft",  "Wilson", "Harding", "Coolidge", "Hoover", "Roosevelt", "Truman",  
		///          "Eisenhower",  "Kennedy",  "Johnson", "Nixon",  "Ford",  "Carter", "Reagan",  
		///          "Bush",  "Clinton",  "Bush II",  "Obama" };
		///    }
		/// 
		/// public int Compare(string pres1, string pres2)
		/// {
		///    int a = presidents.IndexOf(pres1);
		///    int b = presidents.IndexOf(pres2);
		/// 
		///    if (a < 0 || b < 0) throw new Exception("The person you entered was never a president!");
		/// 
		///    if (a < b) return -1;
		/// 
		///    else if (a > b) return 1;
		/// 
		///    else if (a == b) return 0;
		/// 
		///    throw new Exception("We shouldn't get here.");
		/// }
		/// ]]></code></example>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static T[] Sort<T>(this T[] arr, IComparer<T> comparer)
		{
			Array.Sort<T>(arr, comparer);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in the specified range of this array using the default 
		/// IComparable&lt;T&gt; implementation.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="index">The starting index of the range to sort.</param>
		/// <param name="length">The number of elements to sort in the specified range.</param>
		public static T[] Sort<T>(this T[] arr, int index, int length)
		{
			Array.Sort<T>(arr, index, length);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in the specified range of this array using the specified 
		/// IComparer&lt;T&gt; interface.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="arr">This array.</param>
		/// <param name="index">The starting index of the range to sort.</param>
		/// <param name="length">The number of elements to sort in the specified range.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static T[] Sort<T>(this T[] arr, int index, int length, IComparer<T> comparer)
		{
			Array.Sort<T>(arr, index, length, comparer);
			return arr;
		}

		//

		/// <summary>
		/// Sorts the elements in this array using the default IComparable
		/// implementation.
		/// </summary>
		/// <param name="arr">This array.</param>
		public static Array Sort(this Array arr)
		{
			Array.Sort(arr);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in this array using the specified IComparer.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="comparer">The IComparer implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static Array Sort(this Array arr, IComparer comparer)
		{
			Array.Sort(arr, comparer);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in the specified range of this array using the default 
		/// IComparable implementation.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="index">The starting index of the range to sort.</param>
		/// <param name="length">The number of elements to sort in the specified range.</param>
		public static Array Sort(this Array arr, int index, int length)
		{
			Array.Sort(arr, index, length);
			return arr;
		}

		/// <summary>
		/// Sorts the elements in the specified range of this array using the specified 
		/// IComparer.
		/// </summary>
		/// <param name="arr">This array.</param>
		/// <param name="index">The starting index of the range to sort.</param>
		/// <param name="length">The number of elements to sort in the specified range.</param>
		/// <param name="comparer">The IComparer implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static Array Sort(this Array arr, int index, int length, IComparer comparer)
		{
			Array.Sort(arr, index, length, comparer);
			return arr;
		}

		/// <summary>
		/// Sorts an IList T in place.
		/// Source: http://www.velir.com/blog/index.php/2011/02/17/ilistt-sorting-a-better-way/
		/// </summary>
		public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
		{
			// This DOES work, if we want to 
			//List<T> listT = list as List<T>;
			//if(listT != null) {
			//	listT.Sort(comparison);
			//}
			//else {
			ArrayList.Adapter((IList)list).Sort(comparison.ToIComparer());
		}

		public static List<TSource> Sort<TSource, TKey>(
			this List<TSource> list,
			Func<TSource, TKey> keySelector,
			bool reverse = false)
		{
			if(list != null && list.Count > 1) {
				var comparer = new ComparerX<TSource, TKey>(keySelector, null, null, reverse);
				list.Sort(comparer);
			}
			return list;
		}

		#endregion

		#region SortPair

		/// <summary>
		/// Sorts this array (<paramref name="baseArray"/>, often called the <i>keys</i> array) 
		/// while sorting a paired array (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the default IComparable&lt;T&gt;
		/// interface.  Both arrays should be of the same length.
		/// <example><code>
		/// <![CDATA[
		/// int[] familyAges = { 33, 6, 3, 32 };
		/// 
		/// string[] familyNames = { "Todd", "Cloey", "Joey", "Teressa" };
		/// 
		/// familyAges.SortPair(familyNames);
		/// 
		/// // To see results: 
		/// string family = "";
		/// for (int i = 0; i < familyAges.Length; i++)
		///    family += familyNames[i] + " - " + familyAges[i] + "; ";
		/// 
		/// // family == "Joey - 3; Cloey - 6; Teressa - 32; Todd - 33; "
		/// ]]></code></example><para />
		/// </summary>
		/// <typeparam name="TBase">The base array type.</typeparam>
		/// <typeparam name="TPair">The paired array type.</typeparam>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		public static void SortPair<TBase, TPair>(this TBase[] baseArray, TPair[] pairedArray)
		{
			Array.Sort(baseArray, pairedArray);
		}

		/// <summary>
		/// Sorts this array (<paramref name="baseArray"/>, often called the <i>keys</i> array) 
		/// while sorting a paired array (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the specified IComparer&lt;T&gt; 
		/// interface.  Both arrays should be of the same length.
		/// <example><code>
		/// <![CDATA[
		/// // To see the full declaration of this IComparer<T>, see the full entry of this method overload)
		/// PresidentsComparer presCompare = new PresidentsComparer();
		/// 
		/// string[] somePresidents = { "Clinton", "Garfield", "Lincoln", "Reagan", "Roosevelt", "Truman" };
		/// 
		/// int[] inaugYear = { 1993, 1881, 1861, 1981, 1933, 1945 };
		/// 
		/// somePresidents.SortPair(inaugYear, presCompare);
		/// 
		/// // Results:
		/// string results = "";
		/// for (int i = 0; i < somePresidents.Length; i++)
		///    results += "{0}-{1}; ".FormatX(somePresidents[i], inaugYear[i]);
		/// // results == "Lincoln-1861; Garfield-1881; Roosevelt-1933; Truman-1945; Reagan-1981; Clinton-1993; "
		/// 
		/// // * FormatX() is a DotNetExtensions extension method
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <example><code>
		/// <![CDATA[
		/// // A simple comparer that allows US presidents to be historically sorted
		/// public class PresidentsComparer : IComparer<string>
		/// {
		///    public string[] presidents;
		/// 
		///    // CONSTRUCTOR:
		///    public PresidentsComparer()
		///    {
		///       presidents = new string[] { 
		///          "Washington",  "Adams",  "Jefferson",  "Madison", "Monroe",  "Adams II",  
		///          "Jackson",  "Van Buren", "Harrison",  "Tyler",  "Polk",  "Taylor", "Fillmore",  
		///          "Pierce",  "Buchanan",  "Lincoln", "Johnson", "Grant", "Hayes", "Garfield",  
		///          "Arthur",  "Cleveland",  "Harrison",  "Cleveland II", "McKinley", "Roosevelt", 
		///          "Taft",  "Wilson", "Harding", "Coolidge", "Hoover", "Roosevelt", "Truman",  
		///          "Eisenhower",  "Kennedy",  "Johnson", "Nixon",  "Ford",  "Carter", "Reagan",  
		///          "Bush",  "Clinton",  "Bush II",  "Obama" };
		///    }
		/// 
		/// public int Compare(string pres1, string pres2)
		/// {
		///    int a = presidents.IndexOf(pres1);
		///    int b = presidents.IndexOf(pres2);
		/// 
		///    if (a < 0 || b < 0) throw new Exception("The person you entered was never a president!");
		/// 
		///    if (a < b) return -1;
		/// 
		///    else if (a > b) return 1;
		/// 
		///    else if (a == b) return 0;
		/// 
		///    throw new Exception("We shouldn't get here.");
		/// }
		/// ]]></code></example>
		/// <typeparam name="TBase">The base array type.</typeparam>
		/// <typeparam name="TPair">The paired array type.</typeparam>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static void SortPair<TBase, TPair>(this TBase[] baseArray, TPair[] pairedArray, IComparer<TBase> comparer)
		{
			Array.Sort(baseArray, pairedArray, comparer);
		}

		/// <summary>
		/// Sorts the specified range of this array (<paramref name="baseArray"/>, often called the 
		/// <i>keys</i> array) while sorting the same range of a paired array 
		/// (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the default IComparable&lt;T&gt; 
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <typeparam name="TBase">The base array type.</typeparam>
		/// <typeparam name="TPair">The paired array type.</typeparam>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="start">The zero-based starting index of the sort.</param>
		/// <param name="length">The number of elements to sort following start.</param>
		public static void SortPair<TBase, TPair>(this TBase[] baseArray, TPair[] pairedArray, int start, int length)
		{
			Array.Sort(baseArray, pairedArray, start, length);
		}

		/// <summary>
		/// Sorts the specified range of this array (<paramref name="baseArray"/>, often called the 
		/// <i>keys</i> array) while sorting the same range of a paired array 
		/// (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the specified IComparer&lt;T&gt; 
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <typeparam name="TBase">The base array type.</typeparam>
		/// <typeparam name="TPair">The paired array type.</typeparam>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="start">The zero-based starting index of the sort.</param>
		/// <param name="length">The number of elements to sort following start.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static void SortPair<TBase, TPair>(this TBase[] baseArray, TPair[] pairedArray, int start, int length, IComparer<TBase> comparer)
		{
			Array.Sort(baseArray, pairedArray, start, length, comparer);
		}

		//

		/// <summary>
		/// Sorts this array (<paramref name="baseArray"/>, often called the <i>keys</i> array) 
		/// while sorting a paired array (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the default IComparable
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		public static void SortPair(this Array baseArray, Array pairedArray)
		{
			Array.Sort(baseArray, pairedArray);
		}

		/// <summary>
		/// Sorts this array (<paramref name="baseArray"/>, often called the <i>keys</i> array) 
		/// while sorting a paired array (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the specified IComparer  
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static void SortPair(this Array baseArray, Array pairedArray, IComparer comparer)
		{
			Array.Sort(baseArray, pairedArray, comparer);
		}

		/// <summary>
		/// Sorts the specified range of this array (<paramref name="baseArray"/>, often called the 
		/// <i>keys</i> array) while sorting the same range of a paired array 
		/// (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the default IComparable 
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="start">The zero-based starting index of the sort.</param>
		/// <param name="length">The number of elements to sort following start.</param>
		public static void SortPair(this Array baseArray, Array pairedArray, int start, int length)
		{
			Array.Sort(baseArray, pairedArray, start, length);
		}

		/// <summary>
		/// Sorts the specified range of this array (<paramref name="baseArray"/>, often called the 
		/// <i>keys</i> array) while sorting the same range of a paired array 
		/// (<paramref name="pairedArray"/>, which is often called 
		/// the <i>items</i> array) in tandem with it.  The <paramref name="baseArray"/> is what 
		/// determines the sorted order which is sorted by the specified IComparer 
		/// interface.  Both arrays should be of the same length.
		/// </summary>
		/// <param name="baseArray">The base array which determines the sorted 
		/// order of the pair.</param>
		/// <param name="pairedArray">The paired array which will be sorted in tandem 
		/// with <paramref name="baseArray"/>.</param>
		/// <param name="start">The zero-based starting index of the sort.</param>
		/// <param name="length">The number of elements to sort following start.</param>
		/// <param name="comparer">The IComparer&lt;T&gt; implementation to use when 
		/// comparing elements, or null to use the default implementation.</param>
		public static void SortPair(this Array baseArray, Array pairedArray, int start, int length, IComparer comparer)
		{
			Array.Sort(baseArray, pairedArray, start, length, comparer);
		}

		#endregion

	}
}
