using System;
using System.Diagnostics;
using System.Xml.Linq;

namespace DotNetXtensions
{
	public static partial class XML
	{
		[DebuggerStepThrough]
		public static string ValueN(this XElement e)
		{
			if(e == null) return null;
			return (string)e;
		}

		[DebuggerStepThrough]
		public static string ValueN(this XAttribute e)
		{
			if(e == null) return null;
			return (string)e;
		}



		// -------------- ToInt

		[DebuggerStepThrough]
		public static int ToInt(this XElement e, int defaultVal = 0)
		{
			return fix(e).ToInt(defaultVal);
		}

		[DebuggerStepThrough]
		public static int ToInt(this XAttribute e, int defaultVal = 0)
		{
			return fix(e).ToInt(defaultVal);
		}

		[DebuggerStepThrough]
		public static int? ToIntN(this XElement e)
		{
			return fix(e).ToIntN();
		}

		[DebuggerStepThrough]
		public static int? ToIntN(this XAttribute e)
		{
			return fix(e).ToIntN();
		}



		// -------------- ToLong

		[DebuggerStepThrough]
		public static long ToLong(this XElement e, long defaultVal = 0)
		{
			return fix(e).ToLong(defaultVal);
		}

		[DebuggerStepThrough]
		public static long ToLong(this XAttribute e, long defaultVal = 0)
		{
			return fix(e).ToLong(defaultVal);
		}

		[DebuggerStepThrough]
		public static long? ToLongN(this XElement e)
		{
			return fix(e).ToLongN();
		}

		[DebuggerStepThrough]
		public static long? ToLongN(this XAttribute e)
		{
			return fix(e).ToLongN();
		}



		// -------------- ToDecimal

		[DebuggerStepThrough]
		public static decimal ToDecimal(this XElement e, decimal defaultVal = 0)
		{
			return fix(e).ToDecimal(defaultVal);
		}

		[DebuggerStepThrough]
		public static decimal ToDecimal(this XAttribute e, decimal defaultVal = 0)
		{
			return fix(e).ToDecimal(defaultVal);
		}

		[DebuggerStepThrough]
		public static decimal? ToDecimalN(this XElement e)
		{
			return fix(e).ToDecimalN();
		}

		[DebuggerStepThrough]
		public static decimal? ToDecimalN(this XAttribute e)
		{
			return fix(e).ToDecimalN();
		}



		// -------------- ToDouble

		[DebuggerStepThrough]
		public static double ToDouble(this XElement e, double defaultVal = 0)
		{
			return fix(e).ToDouble(defaultVal);
		}

		[DebuggerStepThrough]
		public static double ToDouble(this XAttribute e, double defaultVal = 0)
		{
			return fix(e).ToDouble(defaultVal);
		}

		[DebuggerStepThrough]
		public static double? ToDoubleN(this XElement e)
		{
			return fix(e).ToDoubleN();
		}

		[DebuggerStepThrough]
		public static double? ToDoubleN(this XAttribute e)
		{
			return fix(e).ToDoubleN();
		}



		// -------------- ToDateTime

		private static readonly DateTime _dtDefault = default;
		private static readonly DateTimeOffset _dtoffDefault = default;

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XElement e)
		{
			return fix(e).ToDateTime(_dtDefault);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XElement e, DateTime defaultVal)
		{
			return fix(e).ToDateTime(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XAttribute e)
		{
			return fix(e).ToDateTime(_dtDefault);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XAttribute e, DateTime defaultVal)
		{
			return fix(e).ToDateTime(defaultVal);
		}


		[DebuggerStepThrough]
		public static DateTime? ToDateTimeN(this XElement e)
		{
			return fix(e).ToDateTimeN();
		}

		[DebuggerStepThrough]
		public static DateTime? ToDateTimeN(this XAttribute e)
		{
			return fix(e).ToDateTimeN();
		}



		// -------------- ToDateTimeOffset

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XElement e)
		{
			return fix(e).ToDateTimeOffset(_dtoffDefault);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XElement e, DateTimeOffset defaultVal)
		{
			return fix(e).ToDateTimeOffset(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XAttribute e)
		{
			return fix(e).ToDateTimeOffset(_dtoffDefault);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XAttribute e, DateTimeOffset defaultVal)
		{
			return fix(e).ToDateTimeOffset(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset? ToDateTimeOffsetN(this XElement e)
		{
			return fix(e).ToDateTimeOffsetN();
		}

		[DebuggerStepThrough]
		public static DateTimeOffset? ToDateTimeOffsetN(this XAttribute e)
		{
			return fix(e).ToDateTimeOffsetN();
		}



		// -------------- ToBool

		[DebuggerStepThrough]
		public static bool ToBool(this XElement e, bool defaultVal = false)
		{
			return fix(e).ToBool(defaultVal);
		}

		[DebuggerStepThrough]
		public static bool ToBool(this XAttribute e, bool defaultVal = false)
		{
			return fix(e).ToBool(defaultVal);
		}

		[DebuggerStepThrough]
		public static bool? ToBoolN(this XElement e)
		{
			return fix(e).ToBoolN();
		}

		[DebuggerStepThrough]
		public static bool? ToBoolN(this XAttribute e)
		{
			return fix(e).ToBoolN();
		}



		private static string fix(XElement e)
		{
			if(e == null)
				return null;
			string val = e.Value;
			return string.IsNullOrWhiteSpace(val) ? null : val;
		}

		private static string fix(XAttribute e)
		{
			if(e == null)
				return null;
			string val = e.Value;
			return string.IsNullOrWhiteSpace(val) ? null : val;
		}


	}
}
