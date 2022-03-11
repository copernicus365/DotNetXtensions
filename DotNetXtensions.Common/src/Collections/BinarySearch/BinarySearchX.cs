using System;
using System.Collections.Generic;

namespace DotNetXtensions.Collections
{
	public static class BinarySearchX
	{
		/// <summary>
		/// Finds the range of matches that are greater or equal than low value and lesser or equal to
		/// high value within the sorted sequence by means of a binary search. 
		/// See BinarySearch[T].FindRange for further details. 
		/// </summary>
		public static IndexRangeWindow BinarySearchFindRange<T>(IList<T> arr, T lowValue, T highValue,
			bool reverse = false, int index = 0, int? length = null, Func<T, T, int> comparer = null)
		{
			var bs = new BinarySearch<T>(arr, reverse, comparer);
			IndexRangeWindow result = bs.FindRange(lowValue, highValue, index, length);
			return result;
		}

		/// <summary>
		/// Finds the range of matches of a single value within the sorted sequence
		/// by means of a binary search, see BinarySearch[T].FindSingleRange for further details. 
		/// </summary>
		public static IndexRangeWindow BinarySearchFindRange<T>(IList<T> arr, T value,
			bool reverse = false, int index = 0, int? length = null, Func<T, T, int> comparer = null)
		{
			var bs = new BinarySearch<T>(arr, reverse, comparer);
			IndexRangeWindow result = bs.FindSingleRange(value, index, length);
			return result;
		}

		public static int BinarySearchFindLowRange<T>(IList<T> arr, T value,
			bool reverse = false, int index = 0, int? length = null, Func<T, T, int> comparer = null)
		{
			var bs = new BinarySearch<T>(arr, reverse, comparer);
			int high = bs._GetHighFromInputLength(index, length);
			int result = bs.FindLowRange(value, index, high);
			return result;
		}

		public static int BinarySearchFindHighRange<T>(IList<T> arr, T value,
			bool reverse = false, int index = 0, int? length = null, Func<T, T, int> comparer = null)
		{
			var bs = new BinarySearch<T>(arr, reverse, comparer);
			int high = bs._GetHighFromInputLength(index, length);
			int result = bs.FindHighRange(value, index, high);
			return result;
		}

	}
}
