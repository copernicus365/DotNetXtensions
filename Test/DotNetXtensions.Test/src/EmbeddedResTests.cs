using System;
using Xunit;
using DotNetXtensions;

namespace DotNetXtensions.Test
{
	public class EmbeddedResTests // do NOT inherit! need this to be clean for testing `EmbeddedResources`
	{
		static string Txt1StartsWith = "Howdy, this is a test";

		[Fact]
		public void Test_NullBasePath_Pass()
		{
			var er = new EmbeddedResources(typeof(EmbeddedResTests), null);

			string val =  er.ResourceString("src.resources.test.doc1.txt");

			Assert.True(val.NotNulle() && val.StartsWith(Txt1StartsWith));
		}

		[Fact]
		public void Test_WithBasePathSet_Pass()
		{
			var er = new EmbeddedResources(typeof(EmbeddedResTests), "src.resources");

			string val = er.ResourceString("test.doc1.txt");

			Assert.True(val.NotNulle() && val.StartsWith(Txt1StartsWith));
		}

		[Fact]
		public void Test_WithBasePathSet_Fail()
		{
			var er = new EmbeddedResources(typeof(EmbeddedResTests), "resources");

			string val = null;

			try {
				val = er.ResourceString("test.doc1.txt");
			}
			catch {
				Assert.True(false, "A non-find should NOT throw exception but return null");
			}

			Assert.True(val.IsNulle());
		}

		[Fact]
		public void Test_EmptyConstructor_UseInit()
		{
			var er = new EmbeddedResources();

			try {
				string v1 =er.ResourceString("test123");
				Assert.True(false, "Not inited, should have failed");
			}
			catch {
			}

			er.InitEmbeddedResources(typeof(EmbeddedResTests), "src.resources");

			string val = er.ResourceString("test.doc1.txt");

			Assert.True(val.NotNulle() && val.StartsWith(Txt1StartsWith));
		}

	}
}
