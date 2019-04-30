﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XStopwatch
	{
		public static TimeSpan ElapsedAndRestart(this Stopwatch sw)
		{
			TimeSpan elapsed = sw.Elapsed;
			sw.Restart();
			return elapsed;
		}

		public static TimeSpan ElapsedAndReset(this Stopwatch sw)
		{
			TimeSpan elapsed = sw.Elapsed;
			sw.Reset();
			return elapsed;
		}

		public static string ElapsedStringBest(this Stopwatch sw)
		{
			string elapsed = sw.Elapsed.ToStringBest();
			return elapsed;
		}

		public static string ElapsedStringAndRestart(this Stopwatch sw)
		{
			string elapsed = sw.Elapsed.ToStringBest();
			sw.Restart();
			return elapsed;
		}

		public static TimeSpan ElapsedAndStop(this Stopwatch sw)
		{
			TimeSpan elapsed = sw.Elapsed;
			sw.Restart();
			return elapsed;
		}

	}
}