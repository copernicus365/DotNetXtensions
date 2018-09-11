using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
#if DNXPublic
	public
#endif
	class StringStartsWithComparer : IComparer<string>, IEqualityComparer<string>
	{
		public bool IgnoreCase { get; set; }

		public int Compare(string x, string y)
		{
			if(x == null)
				return y == null ? 0 : -1;
			if(y == null)
				return 1;
			if(x.Length == 0)
				return y.Length == 0 ? 0 : -1;
			if(y.Length == 0)
				return 1;

			//// if y is longer than x, x obv can't start with y, so use normal comparer
			//if(y.Length >= x.Length)
			//	return string.Compare(x, y, IgnoreCase);

			// compare with y (which we know is shorter now), as it doesn't allow length stop then negative result
			StringComparison _case = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return string.Compare(x, 0, y, 0, Math.Min(x.Length, y.Length), _case);
			// x.StartsWith(y, StringComparison.OrdinalIgnoreCase);
		}

		public bool Equals(string x, string y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(string obj)
		{
			if(obj == null)
				return 0;
			return obj.GetHashCode();
		}
	}
}
