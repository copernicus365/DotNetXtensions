using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	public class StringContainsAnyXmlTagsTests : DnxTestBase
	{
		[Fact]
		public void Null_Fail() =>
			RunTest(false, null);

		[Fact]
		public void EmptyString_Fail() =>
			RunTest(false, "");

		[Fact]
		public void PlainTextNoPointies_Fail() =>
			RunTest(false, "hello");

		[Fact]
		public void PlainTextMultipleLinesNoPointies2_Fail() =>
			RunTest(false, @"  test\r\n\r\n ");

		[Fact]
		public void HasBothPointiesButNeverProperTags1_Fail() =>
			RunTest(false, @"Test 

 > test
  < test >
> ignore
");

		[Fact]
		public void TagWithAttr_NoEndTag_Pass() =>
			RunTest(true, "test <b class=\"\">howdy ... ");

		[Fact]
		public void PlainTextOpenLT_ThenRealXmlTag_Pass() =>
			RunTest(true, "If 3 < 4, then <b>hello</b>!");

		/// <summary>
		/// Need this one too, logic is important at this secondary stage within 
		/// a tag and moving i one ff
		/// </summary>
		[Fact]
		public void PlainTextOpenLT_ThenRealXmlTagWithAttr_Pass() =>
			RunTest(true, "If 3 < 4, then <b class=\"cool\">hello</b>!");

		[Fact]
		public void BadlyFormedOpenTagWNoEnd_FollowedByGoodTag_Pass() =>
			RunTest(true, "some <bracket hi <b>hi!");

		[Fact]
		public void OnlyAXmlComment_Pass() =>
			RunTest(true, "<!-- comment -->");

		[Fact]
		public void SingleFullStartEndTag_Pass() =>
			RunTest(true, "<b>test</b>");

		[Fact]
		public void EndsWithRealTag_NoCloseTag_Pass() =>
			RunTest(true, "test     \r\n<b>");

		[Fact]
		public void EndsWithRealTag_ButNameStartsWithNum_Fail() =>
			RunTest(false, "test     \r\n<1b>");

		[Fact]
		public void EndsWithOpenP_Fail() =>
			RunTest(false, "test test <");

		[Fact]
		public void StartsWithNeverClosedOpenPointy() =>
			RunTest(false, "<Cheers maties \r\nhowdy.");

		void RunTest(bool isXml, string text)
		{
			bool testHadXml = XmlTextFuncs.StringContainsAnyXmlTagsQuick(text);

			bool passed = testHadXml == isXml;
			if(!passed)
				Assert.True(passed);
		}
	}
}
