namespace DNX.Test;

public class GenNamesTests1 : DnxTestBase
{
	[Fact]
	public void GetGeoNamesEnumsCode_NotEmptyResult()
	{
		string genCode = GeoNames.GetGeoNamesEnumsCode();
		True(genCode.NotNulle());
	}

	[Fact]
	public void GetCountry_US_NZ_PASS()
	{
		// NOTE: .GetCountry calls (not GetCountryOrNull) will / should throw 
		// if not found
		var arr = tarray(
			"United States",
			"United_States",
			"UnitedStates",
			"uNITed STaTes",
			"US",
			"uS");

		foreach(string v in arr) {
			True(GeoNames.GetCountryOrNull(v) == GeoCountry.United_States);
			True(GeoNames.GetCountry(v) == GeoCountry.United_States);
		}

		arr = tarray(
			"New Zealand",
			"NewZealand",
			"NewZEALand",
			"NZ",
			"nZ");

		foreach(string v in arr) {
			True(GeoNames.GetCountry(v) == GeoCountry.New_Zealand);
			True(GeoNames.GetCountryOrNull(v) == GeoCountry.New_Zealand);
		}
	}

	[Fact]
	public void GetCountryOrNull_None_Fail()
	{
		var arr = tarray(
			"None",
			"none");

		foreach(string v in arr)
			True(GeoNames.GetCountryOrNull(v) == null);
	}

	[Fact]
	public void GetCountryOrNull_BogusNames_Fails()
	{
		var arr = tarray(
			"New Zealand1",
			"nzz",
			"nn",
			"New-Zealand");

		foreach(string v in arr)
			True(GeoNames.GetCountryOrNull(v) == null);
	}

	[Fact]
	public void GetStateOrNull_SpacedStateName_PASS()
	{
		var arr = tarray(
			"New York",
			"New_York",
			"NewYork",
			"nEWYork",
			"NY",
			"nY");

		foreach(string v in arr)
			True(GeoNames.GetStateOrNull(v) == USCanadaState.New_York);
	}

}
