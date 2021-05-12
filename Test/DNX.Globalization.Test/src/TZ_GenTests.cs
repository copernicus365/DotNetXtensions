using System;

using DNX.Globalization.Test;

using DotNetXtensions.Globalization;

using Xunit;

namespace DNX.Test
{
	public class TZ_GenTests : DnxTestBase
	{
		[Fact]
		public void TestNewIndianaTZ()
		{
			string tzId = "America/Indiana/Indianapolis"; // google is returning this in some geo calls under the property: "timeZoneId"

			TimeZoneInfo tzi = TimeZones.GetTimeZoneInfoFromTZId(tzId);

			True(tzi != null && tzi.Id == "Eastern Standard Time");
		}

	}
}
