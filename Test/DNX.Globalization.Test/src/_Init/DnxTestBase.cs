using DotNetXtensions;
using DotNetXtensions.Test;

namespace DNX.Globalization.Test
{
	public class DnxTestBase : XUnitTestBase
	{
		public DnxTestBase() : this("src.resources") { }

		public DnxTestBase(string resourceBasePath)
			: base(resourceBasePath.IsNulle() ? null : typeof(DnxTestBase), resourceBasePath)
		{
			CacheResourceGetsDefault = true;
		}
	}
}
