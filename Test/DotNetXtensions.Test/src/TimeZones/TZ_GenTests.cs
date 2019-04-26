using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	public class TZ_GenTests : BaseUnitTest
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
