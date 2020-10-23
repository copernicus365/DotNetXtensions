
namespace DotNetXtensions
{
	public static class XBool
	{
		public static int ToBit(this bool b)
			=> b ? 1 : 0;

		public static string ToBitString(this bool b, string t = "T", string f = "F")
			=> b ? t : f;

		/// <summary>
		/// Converts this boolean to a lower case 'true' / 'false' representation.
		/// </summary>
		/// <param name="b">bool</param>
		public static string ToStringLower(this bool b)
			=> b ? "true" : "false";

		/// <summary>
		/// Converts this boolean to a 'Yes' / 'No' representation.
		/// </summary>
		/// <param name="b">bool</param>
		/// <param name="lower">True to have a lowercase result returned.</param>
		public static string ToStringYesNo(this bool b, bool lower = false)
		{
			if(lower)
				return b ? "yes" : "no";
			return b ? "Yes" : "No";
		}

	}
}
