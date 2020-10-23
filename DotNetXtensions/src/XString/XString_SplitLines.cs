using System;
using System.Linq;

namespace DotNetXtensions
{
	public static partial class XString
	{
		// --- SplitN ---

		/// <summary>
		/// Calls string.Split, but returns an empty sequence for null or empty input, and assumes
		/// by default to removeEmptyEntries (default param is true).
		/// </summary>
		/// <param name="s">input</param>
		/// <param name="separator">separator</param>
		/// <param name="count">Maximum items to return</param>
		/// <param name="removeEmptyEntries">True (default) to remove nullorempty items.</param>
		public static string[] SplitN(this string s, char separator, int? count = null, bool removeEmptyEntries = true)
		{
			return SplitN(s, new char[] { separator }, count, removeEmptyEntries);
		}

		public static string[] SplitN(this string s, char[] separators, int? count = null, bool removeEmptyEntries = true)
		{
			if(s.IsNulle() || (count != null && count < 1))
				return new string[0];

			StringSplitOptions opt = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
			return count != null
				? s.Split(separators, (int)count, opt)
				: s.Split(separators, opt);
		}

		public static string[] SplitN(this string s, string separator, int? count = null, bool removeEmptyEntries = true)
		{
			return SplitN(s, new string[] { separator }, count, removeEmptyEntries);
		}

		public static string[] SplitN(this string s, string[] separators, int? count = null, bool removeEmptyEntries = true)
		{
			if(s.IsNulle() || (count != null && count < 1))
				return new string[0];

			StringSplitOptions opt = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
			return count != null
				? s.Split(separators, (int)count, opt)
				: s.Split(separators, opt);
		}

		public static string[] SplitAndRemoveWhiteSpaceEntries(this string val, char separator, Predicate<string> pred = null)
		{
			return SplitAndRemoveWhiteSpaceEntries(
				val,
				separator == ',' ? _commaCharArr : new char[] { separator });
		}

		/// <summary>
		/// Trims all entries and removes whitespace entries.
		/// </summary>
		public static string[] SplitAndRemoveWhiteSpaceEntries(this string val, char[] separator, Predicate<string> pred = null)
		{
			if(val.IsNulle())
				return _emptyStrArr;

			var arr = val.Split(separator, StringSplitOptions.RemoveEmptyEntries);

			if(arr.IsNulle())
				return _emptyStrArr;

			bool usePred = pred != null;

			arr = arr
				.Select(s => s.TrimN())
				.Where(s => s.NotNulle() && usePred ? pred(s) : true)
				.ToArray();

			return arr;
		}

		static char[] _commaCharArr = { ',' };
		static string[] _emptyStrArr = { };


		/// <summary>
		/// Splits the string into lines, both "\r\n", "\n" are checked.
		/// Optionally trims the lines and removes empty lines.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="trimLines"></param>
		/// <param name="removeEmptyLines"></param>
		/// <returns></returns>
		public static string[] SplitLines(this string str, bool trimLines = false, bool removeEmptyLines = false)
		{
			if(str == null)
				return null;
			if(str.Length == 0)
				return new string[0];

			string[] lines = str.Split(___splitLinesArray,
				removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);

			if(lines.Length == 0 || !trimLines && !removeEmptyLines)
				return lines;

			for(int i = 0; i < lines.Length; i++) {
				string l = lines[i];
				if(l != null) {
					if(trimLines && l.Length > 0)
						l = l.Trim();
					if(removeEmptyLines && l.Length == 0)
						l = null;
					lines[i] = l;
				}
			}
			if(removeEmptyLines)
				lines = lines.Where(l => l != null).ToArray();
			return lines;
		}

		private static string[] ___splitLinesArray = new string[] { "\r\n", "\n" };
	}
}
