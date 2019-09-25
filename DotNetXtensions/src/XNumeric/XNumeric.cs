using System;
using System.Text;
using System.Globalization;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for numbers, math, and related functions.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XNumeric
	{
		// --- Positive ---

		public static double Pos(this double d)
			=> d < 0 ? -d : d;
		
		public static int Pos(this int d)
			=> d < 0 ? -d : d;
		
		public static decimal Pos(this decimal d)
			=> d < 0 ? -d : d;
		
		public static long Pos(this long d)
			=> d < 0 ? -d : d;
		

		// --- DivideUp ---

		public static int DivideUp(this int dividend, int divisor)
		{
			int r = (dividend + (divisor - 1)) / divisor;
			return r;
			// note also: int result; int remainder = Math.DivRem(dividend, divisor, out result);
		}

	}
}
