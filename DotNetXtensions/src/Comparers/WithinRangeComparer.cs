using System.Collections.Generic;

namespace DotNetXtensions
{
	public class WithinRangeComparer : IComparer<int>
	{
		int maxDiff;

		public WithinRangeComparer(int maxDifference)
		{
			maxDiff = maxDifference;
		}

		public int Compare(int x, int y)
		{
			int diff = x - y;
			if(diff > 0 && diff > maxDiff)
				return 1;
			if(diff < 0 && -diff >= maxDiff)
				return -1;
			return 0;
		}
	}
}
