using Xunit;

namespace DotNetXtensions.Test
{
	public class DnxUnitTestTests : DnxTestBase
	{
		public DnxUnitTestTests() : base("src.resources.test") { }

		[Fact]
		public void ResTypeNOTNull()
		{
			True(this.TypeForResources != null);
		}

		[Fact]
		public void Test1()
		{
			string doc1 = ResDoc1;
			True(doc1.NotNulle());
			True(doc1.StartsWith("Howdy, this is a test of your local broadcasting system!"));
		}

		public string ResDoc1 => ResourceString("doc1.txt");

	}

	public class DnxUnitTestTests_NoInit : DnxTestBase
	{
		public DnxUnitTestTests_NoInit()
			: base(resourceBasePath: null) // this SHOULD make it so Resource Assm stuff does NOT init
		{

		}

		[Fact]
		public void ResTypeISNull()
		{
			True(this.TypeForResources == null);
		}
	}
}
