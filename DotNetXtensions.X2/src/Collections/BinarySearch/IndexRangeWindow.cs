using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetXtensions.Collections
{
	public struct IndexRangeWindow
	{
		public const int noneFound = -1;

		public static readonly IndexRangeWindow NoneFound = new IndexRangeWindow(noneFound, noneFound);

		int low;
		int high;
		int count;

		public int Low { get { return low; } }
		public int High { get { return high; } }
		public int Count { get { return count; } }

		/// <summary>
		/// Represents the high / low range result of a BinarySearchV2.FindWithinRange search.
		/// If none were in range, Count will be 0 and Low will (always?) be one higher than
		/// High.
		/// </summary>
		/// <param name="low">The low index. If none found, will be one greater than high, but Count will be set to 0.</param>
		/// <param name="high">The high index.</param>
		public IndexRangeWindow(int low, int high)
		{
			if(low < 0 || high < 0)
				low = high = noneFound;

			this.low = low;
			this.high = high;
			this.count = low < 0 || high < low
				? 0
				: high - low + 1;
		}

		/// <summary>
		/// Gets the range of items specified in this range 
		/// from the input source IList.
		/// </summary>
		/// <param name="list">The source list.</param>
		public IEnumerable<T> GetItems<T>(IList<T> list)
		{
			if(count < 1 || list.IsNulle())
				yield break;
			if(list == null) throw new ArgumentNullException(nameof(list));
			if(high >= list.Count) throw new ArgumentOutOfRangeException(nameof(list), "IList count is smaller than the high range index.");

			for(int i = low; i <= high; i++) {
				yield return list[i];
			}
		}

		public IEnumerable<int> GetIndexes()
		{
			for(int i = low; i <= high; i++) {
				yield return i;
			}
		}

		public override string ToString()
		{
			return $"[{low},{high}:{count}]";
		}
	}
}
