using System;
using System.Text;
using System.Linq;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	/// <summary>
	/// In the following tests, note that by necessity, they 
	/// are inflexibly demanding, requiring an exact match of whitespace, which in 
	/// many cases we don't need it exactly that close. But, no other simple way 
	/// exists to test these things.
	/// </summary>
	public class OnePassHtmlToMarkdownTests : BaseUnitTest
	{


		public void RUN_HtmlToMDTest(HtmlToMDTestArgs testArgs)
		{
			var test = testArgs;
			bool pass = false;
			string oldResult;
			string result;

			oldResult = XmlTextFuncs.ClearXmlTags(test.Text, trim: true);

			if (test.ExpectedResult != null) {

				result = OnePassHtmlToMarkdown.HtmlToMD(test.Text);

				pass = result == test.ExpectedResult;

				printResult(true);
			}

			if (test.ExpectedResultNoMD != null) {
				result = OnePassHtmlToMarkdown.HtmlToMD(test.Text, justCleanHtmlTags: true);

				pass = result == test.ExpectedResultNoMD;

				printResult(false);
			}

			if (!pass)
				Assert.True(pass, $"Failed: {test.TestName}");

			void printResult(bool md)
			{
				$@"--- HtmlToMD: {test.TestName} ({(md ? "MD" : "No-MD")}):
 - {(pass ? "PASS" : "FAIL")}

 - ORIGINAL : 
`{test.Text}`

 - CONVERTED: 
`{result}`
".Print();
				// - OLDRESULT: 
				//`{oldResult}`

				if (!pass) {
					$@" - EXPECTED (md):
`{test.ExpectedResult}`
".Print();
				}
			}
		}



		[Fact]
		public void BlockQuotes1()
		{
			string htmlText =
@"<p>Hi, Mr. Dude says:</p><blockquote cite=""http://www.howdy.com/"">Do da day <b>awesome</b> possom.</blockquote><p>Th' th' th' that's all folks!</p>";

			string exResult =
@"Hi, Mr. Dude says: 
> Do da day **awesome** possom. 

Th' th' th' that's all folks!";

			RUN_HtmlToMDTest(new HtmlToMDTestArgs(
				nameof(BlockQuotes1), htmlText, exResult));
		}

		[Fact]
		public void BlockQuotes2()
		{
			string htmlText =
@"<p>Hi</p>
<blockquote cite=""http://www.howdy.com/""> 
Cool P0. 
<p>Cool P1.</p>
<p>Cool P2.</p>
<h3>Heading dude</h3>

</blockquote>
";

			string exResult =
@"Hi
> Cool P0.
Cool P2.
";

			RUN_HtmlToMDTest(new HtmlToMDTestArgs(
				nameof(BlockQuotes2), htmlText, exResult));
		}

		[Fact]
		public void Links1()
		{
			RUN_HtmlToMDTest(new HtmlToMDTestArgs(
				nameof(Links1),

				$@"<p><strong class=""some-strong"">Hello</strong> there.</p>
	<p>Check out <a class=""some-img"" href=""{sampleImgUrl1}"" style=""color: red;"">this lion</a>! 
</p>",

					$"**Hello** there.\r\n\t\r\nCheck out [this lion]({sampleImgUrl1})!",

					"Hello there.\r\n\t\r\nCheck out this lion!")
				);
		}

		[Fact]
		public void NoXmlToClear() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(NoXmlToClear),
			"hello world",
			"hello world"));

		[Fact]
		public void NoXmlToClear2_RtBracketsOnly() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(NoXmlToClear2_RtBracketsOnly),
			"he>llo >world>",
			"he>llo >world>"));

		[Fact]
		public void LotOfWSLineBreaksInAttributes1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(LotOfWSLineBreaksInAttributes1),
				"ok <p \r\nclass=\"cool-red\">hi</p><p>world</p><div>do da</div><font /> hi",
				"ok \r\n\r\nhi \r\n\r\nworld do da hi"));

		const string wsMess = "    \t\t \r\n\r\n\t\t\t ";

		[Fact]
		public void WSFun1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(WSFun1),
				$"ok {wsMess}<p>hi{wsMess}</p>{wsMess}<p {wsMess}class=\"cool-red \"{wsMess}>{wsMess}every<b>one</b> in{wsMess}the <i>world</i><strong>!</strong></p>test!", //<div  >test</div> ",
@"ok 

hi 

 every**one** in the *world***!** test!"));

		[Fact]
		public void WSFun2() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(WSFun2),
$"ok {wsMess}<p>hi{wsMess}</p>G", //<div  >test</div> ",
@"ok 

hi G"));

		[Fact]
		public void Attributes2() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(Attributes2),
				"ok <p \r\nclass=\"cool-red\">hi</p><p>world</p>",
				"ok \r\n\r\nhi \r\n\r\nworld"));

		[Fact]
		public void WSBetweenTags1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(WSBetweenTags1),
				"ok <p>hi</p>there <p>world</p>",
				"ok \r\n\r\nhi there \r\n\r\nworld"));

		[Fact]
		public void Doctype() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(Doctype),
				"<!DOCTYPE html>\r\n<p>hi</p>",
				"hi"));

		[Fact]
		public void OpensWComment() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(OpensWComment),
				"<!--yo--><p>hi <!--howdy comment--> there</p>",
				"hi there"));

		[Fact]
		public void LIList1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(LIList1),
				"Ok <ul><li  >apples</li>\r\n<li>Oranges</li> <li>Peaches</li></ul>",
				"Ok \r\n* apples\r\n* Oranges \r\n* Peaches"));

		[Fact]
		public void LineBreaks1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(LineBreaks1),
				"Hi<br />Friends<br/>Hello and hi",
				"Hi\r\nFriends\r\nHello and hi"));

		[Fact]
		public void BoldItalics() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(BoldItalics),
				"Ex<b>press</b>ing one<em>se</em>lf!",
				"Ex**press**ing one*se*lf!",
				"Expressing oneself!"));

		[Fact]
		public void Misc1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(Misc1),
				"<p><i>He<strong class=\"some-strong\">llo</b></i> there.</p><p>Co<em class=\"y23\">ol beans</em>.</p>",
				"*He**llo*** there. \r\n\r\nCo*ol beans*.",
				"Hello there. \r\n\r\nCool beans."));



		public class HtmlToMDTestArgs
		{
			public HtmlToMDTestArgs(string testName, string text, string expectedResult, string expectedResultNoMD = null)
			{
				TestName = testName;
				Text = text;
				ExpectedResult = expectedResult;
				ExpectedResultNoMD = expectedResultNoMD;
			}

			public string TestName { get; set; }
			public string Text { get; set; }
			public string ExpectedResult { get; set; }
			public string ExpectedResultNoMD { get; set; }
			public bool EnableMarkdown { get; set; } = true;
		}


		static string sampleImgUrl1 = "https://www.gannett-cdn.com/presto/2018/08/23/PPHX/c6bf90a6-362d-4e61-ac2e-a21bbc4318ab-Lion2.jpg?crop=959,540,x0,y295&width=3200&height=1680&fit=bounds";

	}
}
