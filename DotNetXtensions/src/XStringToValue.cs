using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XStringToValue
	{
		public static int ToInt(this string val, int dflt = 0)
		{
			if(val.NotNulle()) {
				int i;
				if(int.TryParse(val, out i))
					return i;
			}
			return dflt;
		}

		public static long ToLong(this string val, long dflt = 0)
		{
			if(val.NotNulle()) {
				long i;
				if(long.TryParse(val, out i))
					return i;
			}
			return dflt;
		}

		public static decimal ToDecimal(this string val, decimal dflt = 0)
		{
			if(val.NotNulle()) {
				decimal i;
				if(decimal.TryParse(val, out i))
					return i;
			}
			return dflt;
		}

		public static double ToDouble(this string val, double dflt = 0)
		{
			if(val.NotNulle()) {
				double i;
				if(double.TryParse(val, out i))
					return i;
			}
			return dflt;
		}

		public static bool ToBool(this string val, bool dflt = false)
		{
			if(val.NotNulle()) {
				bool i;
				if(bool.TryParse(val, out i))
					return i;

				int num;
				if(int.TryParse(val, out num) && num < 2) {
					if(num == 0) return false;
					if(num == 1) return true;
				}
			}
			return dflt;
		}

		public static DateTime ToDateTime(this string val, DateTime? defaultVal = null)
		{
			if(val.NotNulle()) {
				DateTimeOffset dt;
				if(DateTimeOffset.TryParse(val, out dt))
					return dt.DateTime;
				//long ticks;
				//if(long.TryParse(val, out ticks) 
				//	&& ticks > 100000000000) {
				//	return new DateTime(ticks);
				//}
			}
			return defaultVal ?? DateTime.MinValue;
		}

		public static DateTimeOffset ToDateTimeOffset(this string val, DateTimeOffset? dflt = null)
		{
			if(val.NotNulle()) {
				DateTimeOffset dt;
				if(DateTimeOffset.TryParse(val, out dt))
					return dt;
			}
			return dflt ?? DateTimeOffset.MinValue;
		}





		public static int? ToIntN(this string val)
		{
			if(val.NotNulle()) {
				int i;
				if(int.TryParse(val, out i))
					return i;
			}
			return null;
		}

		public static long? ToLongN(this string val)
		{
			if(val.NotNulle()) {
				long i;
				if(long.TryParse(val, out i))
					return i;
			}
			return null;
		}

		public static decimal? ToDecimalN(this string val)
		{
			if(val.NotNulle()) {
				decimal i;
				if(decimal.TryParse(val, out i))
					return i;
			}
			return null;
		}

		public static double? ToDoubleN(this string val)
		{
			if(val.NotNulle()) {
				double i;
				if(double.TryParse(val, out i))
					return i;
			}
			return null;
		}

		public static bool? ToBoolN(this string val)
		{
			if(val.NotNulle()) {
				bool i;
				if(bool.TryParse(val, out i))
					return i;
			}
			return null;
		}

		public static DateTime? ToDateTimeN(this string val)
		{
			if(val.NotNulle()) {
				DateTimeOffset dt;
				if(DateTimeOffset.TryParse(val, out dt))
					return dt.DateTime;
			}
			return null;
		}

		public static DateTimeOffset? ToDateTimeOffsetN(this string val)
		{
			if(val.NotNulle()) {
				DateTimeOffset dt;
				if(DateTimeOffset.TryParse(val, out dt))
					return dt;
			}
			return null;
		}


	}
}
