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
	}
}
