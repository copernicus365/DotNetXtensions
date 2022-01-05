//XString_NewLines

namespace DotNetXtensions
{
	public static partial class XString
	{
		/// <summary>
		/// Replaces any Windows type carriage return lines ("\r\n") with Unix
		/// still single "line feed" chars ("\n"). Optionally checks first if
		/// this operation is needed. DOES allow null or empty input.
		/// </summary>
		/// <param name="s">String input</param>
		/// <param name="ifNeeded">True to check first if this operation is
		/// even needed: namely if no '\r' are found, immediately returns the same
		/// input string.</param>
		/// <returns>The string with any carriage returns replaced with
		/// single line feed chars.</returns>
		public static string ToUnixLines(this string s, bool ifNeeded = false)
		{
			if(s.IsNulle())
				return s;

			if(ifNeeded && s.IndexOf('\r') < 0)
				return s;

			return s.Replace("\r\n", "\n");
		}
	}
}
