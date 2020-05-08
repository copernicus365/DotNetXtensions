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

			string val = er.ResourceString("src.resources.test.doc1.txt");

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
		public void Test_AllResPathTypes_Same_Pass()
		{
			EmbeddedResources getER(string resPath)
			{
				return new EmbeddedResources(typeof(EmbeddedResTests), resPath);
			}

			var er1_None = getER(null);
			var er2_NoBaseProj = getER("src.resources");
			var er3_WFullBaseProj = getER("DotNetXtensions.Test.src.resources");

			string path = "test.doc1.txt";

			string val1 = er1_None.ResourceString("DotNetXtensions.Test.src.resources." + path);
			string val1_2 = er1_None.ResourceString("src.resources." + path);
			string val2 = er2_NoBaseProj.ResourceString(path);
			string val3 = er3_WFullBaseProj.ResourceString(path);

			bool pass =
				val1.NotNulle()
				&& val1 == val1_2
				&& val1.StartsWith(Txt1StartsWith)
				&& val1 == val2
				&& val2 == val3;

			Assert.True(pass);
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
				string v1 = er.ResourceString("test123");
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
