using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
#if DNXPublic
	public
#endif
	static class CsvFuncs
	{
		private const string csvQUOTE = "\"";
		private const string csvESCAPED_QUOTE = "\"\"";
		private static char[] csvCHARACTERS_THAT_MUST_BE_QUOTED = { ',', '"', '\n' };

		public static string EscapeCsv(string s)
		{
			if (s == null || s.Length == 0)
				return s;

			if (s.Contains(csvQUOTE))
				s = s.Replace(csvQUOTE, csvESCAPED_QUOTE);

			if (s.IndexOfAny(csvCHARACTERS_THAT_MUST_BE_QUOTED) > -1)
				s = csvQUOTE + s + csvQUOTE;

			return s;
		}

		public static string UnescapeCsv(string s)
		{
			if (s.StartsWith(csvQUOTE) && s.EndsWith(csvQUOTE)) {
				s = s.Substring(1, s.Length - 2);

				if (s.Contains(csvESCAPED_QUOTE))
					s = s.Replace(csvESCAPED_QUOTE, csvQUOTE);
			}

			return s;
		}

		public static string[] ToCSVEscapedStrings(params object[] args)
		{
			if (args == null)
				return null;
			if (args.Length == 0)
				return new string[0];

			string[] list = new string[args.Length];

			for (int i = 0; i < args.Length; i++) {
				string s = EscapeCsv(args[i] == null ? "" : args[i].ToString());
				list[i] = s;
			}
			return list.ToArray();
		}

		public static string ToCSV<T>(IEnumerable<T> seq, Func<T, IEnumerable<object>> convertToItems = null)
		{
			if (seq == null)
				return null;

			StringBuilder sb = new StringBuilder();

			int count = 0;
			int i = -1;
			foreach (T t in seq) {
				++i;
				object[] lineItems = convertToItems(t)?.ToArray();

				if (lineItems.IsNulle())
					continue;

				string[] all = ToCSVEscapedStrings(lineItems);
				if (i == 0)
					count = all?.Length ?? 0;

				if (all.Length != count)
					throw new ArgumentOutOfRangeException($"All of the lines must produce the same number of comma separated objects / strings ({count}).");

				sb.AppendAllSeparated(",", all);
				sb.AppendLine();
			}
			string result = sb.ToString();
			return result;
		}

	}
}