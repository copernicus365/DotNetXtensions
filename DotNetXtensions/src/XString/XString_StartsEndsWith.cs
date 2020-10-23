using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static partial class XString
	{
		public static bool StartsWithN(this string s, char c)
		{
			if(s.IsNulle()) return false;
			return s[0] == c;
		}

		public static bool StartsWithN(this string s, string val)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.StartsWith(val);
		}

		public static bool StartsWithN(this string s, string val, StringComparison comparison)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.StartsWith(val, comparison);
		}


		public static bool EndsWithN(this string s, char c)
		{
			if(s.IsNulle()) return false;
			return s[s.Length - 1] == c;
		}

		public static bool EndsWithN(this string s, string val)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.EndsWith(val);
		}

		public static bool EndsWithN(this string s, string val, StringComparison comparison)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.EndsWith(val, comparison);
		}


		public static bool StartsWithIgnore(this string s, string val, Predicate<char> ignore, StringComparison sc = StringComparison.Ordinal)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			int i = 0;
			for(; i < s.Length; i++) {
				if(!ignore(s[i]))
					break;
			}
			if(i >= s.Length)
				return false;
			if(i > 0)
				s = s.Substring(i);
			return s.StartsWith(val, sc);
		}

		public static bool StartsWithIgnoreWhite(this string s, string val, StringComparison sc = StringComparison.Ordinal)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.StartsWithIgnore(val, c => char.IsWhiteSpace(c), sc);
		}

		public static bool StartsWithIgnoreCase(this string s, string val)
		{
			if(s.IsNulle()) return val == null;
			if(val == null) return false;
			return s.StartsWith(val, StringComparison.OrdinalIgnoreCase);
		}

		public static bool StartsWithIgnoreCaseIndex(this string s, string val, int startIndex)
		{
			throw new NotImplementedException();
			//if (s.IsNulle() || startIndex >= s.Length) return val == null;

			//int len = s.Length - startIndex;
			//if (val == null || val.Length > len) return false;

			//for (int i = startIndex, j = 0; i < len; i++) {
			//	if (char.ToLower(s[i]) == char.ToLower(val[i]))
			//		return i;
			//         }
			//return s.StartsWith(val, StringComparison.OrdinalIgnoreCase);
		}



		public static bool FollowsWith(this string a, string b, int startIndex)
		{
			if(a == null) return b == null;
			if(b == null) return false;

			if(startIndex + b.Length > a.Length)
				return false;

			for(int i = 0; i < b.Length; i++)
				if(a[i + startIndex] != b[i])
					return false;

			return true;
		}

	}
}
