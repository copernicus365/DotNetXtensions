using System;

namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for DateTimes / DateTimeOffsets and for TimeSpans.
	/// </summary>
	public static partial class XDateTimes
	{
		#region --- Nulle ---

		public static bool Nulle(this DateTime? dt)
			=> dt == null || dt.Value == DateTime.MinValue;

		public static bool IsNullOrEmpty(this DateTime? dt)
			=> dt == null || dt.Value == DateTime.MinValue;

		public static bool IsEmpty(this DateTime dt)
			=> dt == DateTime.MinValue;

		public static bool IsNulle(this DateTimeOffset? dt)
			=> dt == null || dt.Value == DateTimeOffset.MinValue;

		public static bool NotNulle(this DateTimeOffset? dt)
			=> !dt.IsNulle();

		#endregion
	}
}
