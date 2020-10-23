// XNumeric_Hex

using System;

namespace DotNetXtensions
{
	public static partial class XNumeric
	{
		// Round

		public static double Round(this double d, int decimalsAfter = 2)
		{
			if(d == 0.0) return d;

			int extraDecimals = 0;
			if(d > .0999)
				extraDecimals = 0;
			else if(d > .00999)
				extraDecimals = 1;
			else if(d > .000999)
				extraDecimals = 2;
			else if(d > .0000999)
				extraDecimals = 3;
			else if(d > .00000999)
				extraDecimals = 4;
			else if(d > .00000099)
				extraDecimals = 5;
			else if(d > .00000009)
				extraDecimals = 6;
			else
				return d;

			return Math.Round(d, decimalsAfter + extraDecimals);
		}

		public static decimal Round(this decimal d, int decimalsAfter = 2)
		{
			if(d == 0.0M) return d;

			int extraDecimals = 0;
			if(d > .0999M)
				extraDecimals = 0;
			else if(d > .00999M)
				extraDecimals = 1;
			else if(d > .000999M)
				extraDecimals = 2;
			else if(d > .0000999M)
				extraDecimals = 3;
			else if(d > .00000999M)
				extraDecimals = 4;
			else if(d > .00000099M)
				extraDecimals = 5;
			else if(d > .00000009M)
				extraDecimals = 6;
			else
				return d;

			return Math.Round(d, decimalsAfter + extraDecimals);
		}

	}
}
