using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	public class DnxUnitTestTests : DnxTestBase
	{
		[Fact]
		public void Test1()
		{
			string doc1 = ResDoc1;
			True(doc1.NotNulle());
			True(doc1.StartsWith("Howdy, this is a test of your local broadcasting system!"));
		}

		public override string ResourceBasePath { get; set; } = "src.resources.test";

		public string ResDoc1 => ResourceString("doc1.txt");

	}
}
