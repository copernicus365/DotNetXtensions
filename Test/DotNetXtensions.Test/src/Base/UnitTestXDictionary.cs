using System;
using Xunit;
using DotNetXtensions;
using System.Web;
using System.Collections.Specialized;

namespace DotNetXtensions.Test
{
	public class UnitTestXDictionary
	{
		[Fact]
		public void NameValueCollectionToDictionary1()
		{
			string qs = "name=&age=22&name=Tony&name=Joey&height=6.1";

			NameValueCollection nvColl1 = HttpUtility.ParseQueryString(qs);

			var dict = nvColl1.ToDictionary(forMultiValuesOnlyGetFirst: true);

			var dictMulti = nvColl1.ToDictionaryMultiValues();


		}
	}
}
