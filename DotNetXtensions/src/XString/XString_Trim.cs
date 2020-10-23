using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static partial class XString
	{
		/// <summary>
		/// Indicates if this string can be trimmed. Null and Empty values ARE valid (will return false).
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsTrimmable(this string s)
		{
			if(s != null) {
				int len = s.Length;
				if(len > 1)
					return char.IsWhiteSpace(s[0]) || char.IsWhiteSpace(s[len - 1]);
				return len == 0 || char.IsWhiteSpace(s[0]);
			}
			return false;
		}

		/// <summary>
		/// Trims the string only if it is needed. Value CAN be Null or Empty.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TrimIfNeeded(ref string s)
		{
			if(s.IsTrimmable()) {
				s = s.Trim();
				return true;
			}
			return false;
		}

		/// <summary>
		/// Trims the string only if it is needed. Value CAN be Null or Empty.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string TrimIfNeeded(this string s)
		{
			if(s.IsTrimmable())
				return s.Trim();
			return s;
		}

		/// <summary>
		/// Trims the string if it is not null, else returns null.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string TrimN(this string s)
		{
			return s == null ? null : s.Trim();
		}

		/// <summary>
		/// Returns null if input is an empty or whitespace string, 
		/// else returns the (trimmed if needed) input.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullIfEmptyTrimmed(this string s)
		{
			s = s.TrimIfNeeded();
			return s == "" ? null : s;
		}

	}
}
