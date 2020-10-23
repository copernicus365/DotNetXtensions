using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static partial class XString
	{
		// IsNulle ... | NullIfEmptyTrimmed

		/// <summary>
		/// Checks if string is null or empty. This is an interesting
		/// abbreviated form of 'IsNullOrEmpty', which is a char short of half
		/// the length of the latter. Use it if you want to! Don't if don't. 
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNulle(this string str)
		{
			return str == null || str.Length == 0;
		}

		/// <summary>
		/// Checks if string is NOT null or empty.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool NotNulle(this string str)
		{
			return str != null && str.Length != 0;
		}

		/// <summary>
		/// Checks if string is null or empty.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty(this string str)
		{
			return str == null || str.Length == 0;
		}

		/// <summary>
		/// Checks if string is null, empty, or only has whitespace.
		/// </summary>
		/// <param name="str">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}


		// --- NullIfEmpty | NullIfEmptyTrimmed ---

		/// <summary>
		/// Returns null if input is an empty string, else returns the input.
		/// </summary>
		/// <param name="s">String</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string NullIfEmpty(this string s)
		{
			return s == "" ? null : s;
		}

	}
}

// REMOVED: public static string EmptyIfNull(this string str)
