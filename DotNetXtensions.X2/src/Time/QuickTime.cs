using System;

namespace DotNetXtensions
{
	/// <summary>        
	/// Gets the current time in an optimized fashion,
	/// retrieving new values only when Environment.TickCount
	/// increments.
	/// </summary>
	public static class QTime
	{
		private static int lastTicks = -1;
		private static DateTime lastDateTime = DateTime.MinValue;
		private static DateTime lastUtcDateTime = DateTime.MinValue;

		public static DateTime UtcNow
		{
			get
			{
				int tickCount = Environment.TickCount;
				if (tickCount == lastTicks)
					return lastUtcDateTime;

				DateTime dt = DateTime.UtcNow;
				lastTicks = tickCount;
				lastUtcDateTime = dt;
				return dt;
			}
		}

		public static DateTime Now
		{
			get
			{
				int tickCount = Environment.TickCount;
				if (tickCount == lastTicks)
					return lastDateTime;

				DateTime dt = DateTime.Now;
				lastTicks = tickCount;
				lastDateTime = dt;
				return dt;
			}
		}

	}
}
