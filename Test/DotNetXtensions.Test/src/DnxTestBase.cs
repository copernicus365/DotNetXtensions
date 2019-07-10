using System;
using Xunit;
using DotNetXtensions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DotNetXtensions.Test
{
	public class DnxTestBase : XUnitTestBase
	{
		public DnxTestBase() : base(true)
		{
		}

		public override Type UnitTestType { get; set; } = typeof(DnxTestBase);

		public override string ResourceBasePath { get; set; } = "src.resources";

	}
}
