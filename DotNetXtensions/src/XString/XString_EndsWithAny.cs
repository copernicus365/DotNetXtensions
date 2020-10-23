using System;
using System.Collections.Generic;

namespace DotNetXtensions
{
	public static partial class XString
	{
		public static bool EndsWithAny(this string s, params string[] vals)
		{
			return EndsWithAnyIndex(s, vals, null) >= 0;
		}

		public static bool EndsWithAny(this string s, StringComparison? comparison, params string[] vals)
		{
			return EndsWithAnyIndex(s, vals, comparison) >= 0;
		}

		public static bool EndsWithAny(this string s, IEnumerable<string> vals, StringComparison? comparison = null)
		{
			return EndsWithAnyIndex(s, vals, comparison) >= 0;
		}

		public static int EndsWithAnyIndex(this string s, IEnumerable<string> vals, StringComparison? comparison = null)
		{
			int i = 0;
			if(s.NotNulle() && vals != null) {
				StringComparison _comparison = comparison ?? StringComparison.Ordinal;
				foreach(var val in vals)
					if(val != null && s.EndsWith(val, _comparison))
						return i;
			}
			return -1;
		}
	}
}
