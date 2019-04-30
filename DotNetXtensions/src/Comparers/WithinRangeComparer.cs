using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
		class WithinRangeComparer : IComparer<int>
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
