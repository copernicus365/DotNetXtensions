using System;
using DotNetXtensions.Test;

namespace DotNetXtensions.Test.ConsoleApp
{
	public class XDatesProg
	{
		public static void Run()
		{
			DateTimeOffset dt = DateTimeOffset.Now;
			string tziId = OperatingSystem.IsWindows()
				? "Eastern Standard Time"
				: "America/New_York";

			TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tziId);
			var dt2 = XDateTimes.ToDateTimeOffset(dt, tzi, keepUtcTime: false);
		}

	}
}
