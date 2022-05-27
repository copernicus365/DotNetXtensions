namespace DNX.Test;

public class TZ_GenTests : DnxTestBase
{
	[Fact]
	public void TestNewIndianaTZ()
	{
		string tzId = "America/Indiana/Indianapolis"; // google is returning this in some geo calls under the property: "timeZoneId"

		TimeZoneInfo tzi = TimeZones.GetTimeZoneInfoFromTZId(tzId);

		True(tzi != null);
		True(tzi.Id == "Eastern Standard Time");
	}

	[Fact]
	public void TZI_GetTZId_1()
	{
		string tzId = "America/Indiana/Indianapolis"; // google is returning this in some geo calls under the property: "timeZoneId"

		TimeZoneInfo tzi = TimeZones.GetTimeZoneInfoFromTZId(tzId);

		True(tzi != null);
		True(tzi.Id == "Eastern Standard Time");
		string tzid2 = tzi.TZId();
		True(tzid2 == "America/New_York");
	}

}
