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
		{
			return dt == null || dt.Value == DateTime.MinValue
				? true
				: false;
		}

		public static bool IsNullOrEmpty(this DateTime? dt)
		{
			return dt == null || dt.Value == DateTime.MinValue
				? true
				: false;
		}

		public static bool IsEmpty(this DateTime dt)
		{
			return dt == DateTime.MinValue
				? true
				: false;
		}

		public static bool IsNulle(this DateTimeOffset? dt)
		{
			return dt == null || dt.Value == DateTimeOffset.MinValue;
		}

		public static bool NotNulle(this DateTimeOffset? dt)
		{
			return !dt.IsNulle();
		}

		#endregion
	}
}
