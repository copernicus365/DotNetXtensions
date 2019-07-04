using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Globalization;

namespace DotNetXtensions.Test
{
	public class GenNamesTests1 : DnxTestBase
	{
		[Fact]
		public void Test_GetGeoNamesEnumsCode_NotEmptyResult()
		{
			string genCode = GeoNames.GetGeoNamesEnumsCode();
			True(genCode.NotNulle());
		}

		[Fact]
		public void GetCountryOrNull_US_PASS()
		{
			gcNull_PASS(GeoCountry.United_States, 
				"United States",
				"United_States",
				"UnitedStates",
				"uNITed STaTes",
				"US",
				"uS");
		}

		[Fact]
		public void GetCountry_US_PASS()
		{
			gc_PASS(GeoCountry.United_States,
				"United States",
				"United_States",
				"UnitedStates",
				"uNITed STaTes",
				"US",
				"uS");
		}

		[Fact]
		public void GetCountryOrNull_NZ_PASS()
		{
			gcNull_PASS(GeoCountry.New_Zealand,
				"New Zealand",
				"NewZealand",
				"NewZEALand",
				"NZ",
				"nZ");
		}

		[Fact]
		public void GetCountryOrNull_None_Fail()
		{
			gcNull_FAIL("None");
			gcNull_FAIL("none");
		}

		[Fact]
		public void GetCountryOrNull_BogusNames_Fails()
		{
			gcNull_FAIL("New Zealand1");
			gcNull_FAIL("nzz");
			gcNull_FAIL("nn");
			gcNull_FAIL("New-Zealand");
		}





		[Fact]
		public void GetStateOrNull_NewYork_PASS()
		{
			gstateNull_PASS(USCanadaState.New_York,
				"New York",
				"New_York",
				"NewYork",
				"nEWYork",
				"NY",
				"nY");
		}






		void gstateNull_PASS(USCanadaState val, params string[] names)
		{
			for(int i = 0; i < names.Length; i++)
				True(GeoNames.GetStateOrNull(names[i]) == val);
		}




		void gcNull_PASS(GeoCountry val, params string[] names)
		{
			for(int i = 0; i < names.Length; i++)
				True(GeoNames.GetCountryOrNull(names[i]) == val);
		}

		void gc_PASS(GeoCountry val, params string[] names)
		{
			for(int i = 0; i < names.Length; i++)
				True(GeoNames.GetCountry(names[i]) == val);
		}

		void gc_FAIL(string nm)
		{
			try {
				var val = GeoNames.GetCountry(nm);
			} catch(Exception ex) {
				return; // we WANT an exception, PASSES (a fail test "pass")
			}
			False(true);
		}


		void gcNull_PASS(string nm)
			=> True(GeoNames.GetCountryOrNull(nm) == GeoCountry.United_States);

		void gcNull_FAIL(string nm)
			=> True(GeoNames.GetCountryOrNull(nm) == null);

	}
}
