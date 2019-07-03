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
		public void Test_ToCountryOrNull()
		{
			True(GeoNames.ToCountryOrNull("United States") == GeoCountry.United_States);

			True(GeoNames.ToCountryOrNull("None") == null);

			True(GeoNames.ToCountryOrNull("None", includeNone: true) == GeoCountry.None);
		}
	}
}
