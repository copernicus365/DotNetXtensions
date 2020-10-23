using System.Collections.Generic;

namespace DotNetXtensions
{
	public class StartsWithComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			if(y == null)
				return x == null ? 0 : 1;

			if(y.StartsWith(x))
				return 0;

			return string.CompareOrdinal(x, y);
		}
	}
}
