using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Extension methods for StringBuilder.
	/// </summary>
#if DNXPublic
	public 
#endif
	static class XStringBuilder
	{

		#region --- IsNulle / NotNulle ---

		[DebuggerStepThrough]
		public static bool IsNulle(this StringBuilder sb)
		{
			return sb == null || sb.Length < 1;
		}

		[DebuggerStepThrough]
		public static bool NotNulle(this StringBuilder sb)
		{
			return sb != null && sb.Length > 0;
		}

		#endregion


		#region --- APPEND ---

		[DebuggerStepThrough]
		public static StringBuilder Append(this StringBuilder sb, StringBuilder sb2)
		{
			if (sb != null && sb2.NotNulle())
				sb.Append(sb2.ToString());
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder Append(this StringBuilder sb, bool addCondition, string item)
		{
			if (addCondition && sb != null && !item.IsNullOrEmpty())
				sb.Append(item);
			return sb;
		}




		[DebuggerStepThrough]
		public static StringBuilder Append(this StringBuilder sb, params string[] items)
		{
			return AppendMany(sb, items);
		}




		[DebuggerStepThrough]
		public static StringBuilder AppendLine(this StringBuilder sb, StringBuilder sb2)
		{
			if (sb != null && sb2.NotNulle())
				sb.AppendLine(sb2.ToString());
			return sb;
		}


		[DebuggerStepThrough]
		public static StringBuilder AppendLine(this StringBuilder sb, bool addCondition, string item)
		{
			if (addCondition && sb != null && !item.IsNullOrEmpty())
				sb.AppendLine(item);
			return sb;
		}




		[DebuggerStepThrough]
		public static StringBuilder AppendManyAppendLine(this StringBuilder sb, bool appendIF, params string[] items)
		{
			return sb.AppendMany(appendIF, items).AppendLineN();
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendManyAppendLine(this StringBuilder sb, params string[] items)
		{
			return sb.AppendMany(items).AppendLineN();
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendManyAppendLine<T>(this StringBuilder sb, IEnumerable<T> items)
		{
			return sb.AppendMany(items).AppendLineN();
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendManyAppendLine(this StringBuilder sb, params object[] items)
		{
			return sb.AppendMany(items).AppendLineN();
		}




		[DebuggerStepThrough]
		public static StringBuilder AppendLineN(this StringBuilder sb)
		{
			if (sb != null)
				sb.AppendLine();
			return sb;
		}




		[DebuggerStepThrough]
		public static StringBuilder AppendMany(this StringBuilder sb, bool appendIF, params string[] items)
		{
			if (appendIF)
				return AppendMany(sb, items);
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendMany(this StringBuilder sb, params string[] items)
		{
			if (sb == null || items == null || items.Length == 0)
				return sb;

			for (int i = 0; i < items.Length; i++)
				sb.Append(items[i]);

			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendMany<T>(this StringBuilder sb, IEnumerable<T> items)
		{
			if (sb != null && items != null) {
				foreach (var item in items)
					if (item != null)
						sb.Append(item.ToString());
			}
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendMany(this StringBuilder sb, params object[] items)
		{
			if (sb != null && items != null) {
				foreach (var item in items)
					if (item != null)
						sb.Append(item.ToString());
			}
			return sb;
		}





		// AppendMany with separator
		public static StringBuilder AppendAllSeparated(this StringBuilder sb, string separator, params object[] items)
		{
			if (!items.IsNulle())
				AppendAllSeparated(sb, separator, items.Select(i => i == null ? null : i.ToString()).ToArray());
			return sb;
		}

		public static StringBuilder AppendAllSeparated<T>(this StringBuilder sb, string separator, IEnumerable<T> items)
		{
			if (items != null)
				AppendAllSeparated(sb, separator, items.Select(i => i == null ? null : i.ToString()).ToArray());
			return sb;
		}

		public static StringBuilder AppendAllSeparated(this StringBuilder sb, string separator, params string[] items)
		{
			if (separator.IsNullOrEmpty())
				return AppendMany(sb, items);
			else if (sb != null && items.NotNulle()) {
				int len = items.Length - 1;
				for (int i = 0; i < len; i++) {
					sb.Append(items[i]);
					sb.Append(separator);
				}
				sb.Append(items[items.Length - 1]);
			}
			return sb;
		}



		[DebuggerStepThrough]
		public static StringBuilder AppendLineMany<T>(this StringBuilder sb, IEnumerable<T> items)
		{
			if (sb != null && items != null) {
				foreach (var item in items)
					if (item != null)
						sb.AppendLine(item.ToString());
			}
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendLineMany(this StringBuilder sb, params object[] items)
		{
			if (sb != null && items != null) {
				for (int i = 0; i < items.Length; i++)
					if (items[i] != null)
						sb.AppendLine(items[i].ToString());
			}
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendIfNotEmpty(this StringBuilder sb, string item)
		{
			if (sb != null && item != null && item.Length > 0)
				sb.Append(item);
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendLineIfNotEmpty(this StringBuilder sb, string item)
		{
			if (sb != null && item != null && item.Length > 0)
				sb.AppendLine(item);
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendIf(this StringBuilder sb, bool append, object item)
		{
			if (append && sb != null && item != null)
				sb.Append(item);
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendIf(this StringBuilder sb, bool append, string item)
		{
			if (append && sb != null && item != null && item.Length > 0)
				sb.Append(item);
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendIf(this StringBuilder sb, bool append, params object[] items)
		{
			if (append && sb != null && items.NotNulle()) {
				for (int i = 0; i < items.Length; i++)
					sb.Append(items[i]); // StringBuilder ignores null (or empty) fine
			}
			return sb;
		}

		[DebuggerStepThrough]
		public static StringBuilder AppendIf(this StringBuilder sb, bool append, params string[] items)
		{
			if (append && sb != null && items.NotNulle()) {
				for (int i = 0; i < items.Length; i++)
					sb.Append(items[i]); // StringBuilder ignores null (or empty) fine
			}
			return sb;
		}


		#endregion


		#region --- TRIM ---


		public static bool IsTrimmable(this StringBuilder s)
		{
			if (s != null && s.Length > 0 && (char.IsWhiteSpace(s[0]) || s.Length > 1 && char.IsWhiteSpace(s[s.Length - 1])))
				return true;
			return false;
		}



		/// <summary>
		/// Trims any whitespace from the end of this StringBuilder.
		/// If no trim is needed, returns almost immediately. Else, the
		/// operation is highly efficient, only change made is to the Length
		/// property of the StringBuilder.
		/// </summary>
		public static StringBuilder TrimEnd(this StringBuilder sb)
		{
			if (sb == null || sb.Length == 0) return sb;

			int i = sb.Length - 1;
			for (; i >= 0; i--)
				if (!char.IsWhiteSpace(sb[i]))
					break;

			if (i < sb.Length - 1)
				sb.Length = i + 1;

			return sb;
		}

		public static string TrimToString(this StringBuilder sb)
		{
			if (sb == null) return null;

			sb.TrimEnd(); // handles nulle and is very inexpensive, unlike trimstart

			if (sb.Length > 0 && char.IsWhiteSpace(sb[0])) {
				for (int i = 0; i < sb.Length; i++)
					if (!char.IsWhiteSpace(sb[i]))
						return sb.ToString(i);
				return ""; // shouldn't reach here, bec TrimEnd should have caught full whitespace strings, but ...
			}

			return sb.ToString();
		}

		public static string ToString(this StringBuilder sb, int startIndex)
		{
			if (sb == null)
				return null;

			int len = sb.Length - startIndex;

			if (len > 0)
				return sb.ToString(startIndex, sb.Length - startIndex);
			else if (len == 0)
				return "";
			else
				return null;
		}

		#endregion

	}

}