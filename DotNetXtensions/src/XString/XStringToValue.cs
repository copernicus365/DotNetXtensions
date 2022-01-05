using System;
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static class XStringToValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ToInt(this string val, int dflt = 0)
			=> int.TryParse(val, out int v) ? v : dflt;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long ToLong(this string val, long dflt = 0)
			=> long.TryParse(val, out long v) ? v : dflt;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal ToDecimal(this string val, decimal dflt = 0)
			=> decimal.TryParse(val, out decimal v) ? v : dflt;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDouble(this string val, double dflt = 0)
			=> double.TryParse(val, out double v) ? v : dflt;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ToBool(this string val, bool dflt = false)
		{
			if(val.NotNulle()) {
				if(bool.TryParse(val, out bool i))
					return i;

				if(int.TryParse(val, out int num) && num < 2) {
					if(num == 0) return false;
					if(num == 1) return true;
				}
			}
			return dflt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime ToDateTime(this string val, DateTime? defaultVal = null)
		{
			if(val.NotNulle()) {
				// NOTE!!  parses DateTimeOffset and if true gets the DateTime...
				// big issues with frameword using computer's local time in these considerations,
				// this was probably not wise though :think: ... but change now would be breaking...
				if(DateTimeOffset.TryParse(val, out DateTimeOffset dt))
					return dt.DateTime;
			}
			return defaultVal ?? DateTime.MinValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset ToDateTimeOffset(this string val, DateTimeOffset? dflt = null)
			=> DateTimeOffset.TryParse(val, out DateTimeOffset v) ? v : (dflt ?? DateTimeOffset.MinValue);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Guid ToGuid(this string val, Guid? dflt = null)
			=> Guid.TryParse(val, out Guid v) ? v : (dflt ?? default);



		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int? ToIntN(this string val)
			=> val.NotNulle() && int.TryParse(val, out int v) ? (int?)v : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static long? ToLongN(this string val)
			=> val.NotNulle() && long.TryParse(val, out long v) ? (long?)v : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static decimal? ToDecimalN(this string val)
			=> val.NotNulle() && decimal.TryParse(val, out decimal v) ? (decimal?)v : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double? ToDoubleN(this string val)
			=> val.NotNulle() && double.TryParse(val, out double v) ? (double?)v : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool? ToBoolN(this string val)
			=> val.IsNulle() ? (bool?)null : ToBool(val); // must use ToBool, handles numeric...

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTime? ToDateTimeN(this string val)
			=> val.NotNulle() && DateTimeOffset.TryParse(val, out DateTimeOffset v) // see notes above: ToDateTime()
			? (DateTime?)v.DateTime : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DateTimeOffset? ToDateTimeOffsetN(this string val)
			=> val.NotNulle() && DateTimeOffset.TryParse(val, out DateTimeOffset v) ? (DateTimeOffset?)v : null;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Guid? ToGuidN(this string val)
			=> val.NotNulle() && Guid.TryParse(val, out Guid v) ? (Guid?)v : null;
	}
}
