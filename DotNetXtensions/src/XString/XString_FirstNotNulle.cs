using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static partial class XString
	{
		// --- FirstNotNulle ---

		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string is returned.
		/// This is a corrolary to the ?? operator, which only checks if
		/// the first string is null, not if it is also not empty.
		/// </summary>
		/// <param name="value1">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FirstNotNulle(this string value1, string value2)
		{
			if(value1 != null && value1.Length > 0)
				return value1;

			if(value2 != null && value2.Length > 0)
				return value2;

			if(value1 != null || value2 != null)
				return "";

			return null;
		}

		/// <summary>
		/// If source string is not NULL or EMPTY, source string is returned,
		/// else the inputed <paramref name="value2"/> string itself is checked 
		/// if it is NULL or EMPTY, if so it is returned. Else <paramref name="value3"/>
		/// is returned.
		/// </summary>
		/// <param name="value1">Source string.</param>
		/// <param name="value2">String to return if source string is NULL or EMPTY.</param>
		/// <param name="value3">String to return if BOTH previous strings were both NULL or EMPTY.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string FirstNotNulle(this string value1, string value2, string value3)
		{
			if(value1 != null && value1.Length > 0)
				return value1;

			if(value2 != null && value2.Length > 0)
				return value2;

			if(value3 != null && value3.Length > 0)
				return value3;

			if(value1 != null || value2 != null || value3 != null)
				return "";

			return null;
		}

	}
}
