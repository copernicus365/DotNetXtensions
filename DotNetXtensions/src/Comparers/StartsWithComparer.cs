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
		class StartsWithComparer : IComparer<string>
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
