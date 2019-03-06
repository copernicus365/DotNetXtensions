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
		void RUN_HtmlToMDTest(HtmlToMDTestArgs testArgs)
		{
			var test = testArgs;
			bool pass = false;
			string oldResult;
			string result;
			bool decode = testArgs.HtmlDecode;

			oldResult = XmlTextFuncs.ClearXmlTags(test.Text, trim: true);

			if (test.ExpectedResult != null) {

				result = OnePassHtmlToMarkdown.HtmlToMD(test.Text, htmlDecode: decode);

				pass = result == test.ExpectedResult;

				printResult(true);
			}

			if (test.ExpectedResultNoMD != null) {
				result = OnePassHtmlToMarkdown.HtmlToMD(
					test.Text, onlyCleanHtmlTags: true, htmlDecode: decode);

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

				if (!pass) {
					$@" - EXPECTED (md):
`{test.ExpectedResult}`
".Print();
				}
			}
		}

		void RUN_HtmlToMDTest(string testName, string text, string expectedResult, string expectedResultNoMD = null)
			=> RUN_HtmlToMDTest(new HtmlToMDTestArgs(testName, text, expectedResult, expectedResultNoMD));


		[Fact]
		public void BlockQuotes1()
		{
			string htmlText =
@"<p>Hi, Mr. Dude says:</p><blockquote cite=""http://www.howdy.com/"">Do da day <b>awesome</b> possom.</blockquote><p>Th' th' th' that's all folks!</p>";

			string exResult =
@"Hi, Mr. Dude says:

> Do da day **awesome** possom.

Th' th' th' that's all folks!";

			RUN_HtmlToMDTest(
				nameof(BlockQuotes1), htmlText, exResult);
		}

		[Fact]
		public void BlockQuotes2()
		{
			string htmlText =
@"<p>Hi</p>
<blockquote cite=""http://www.howdy.com/""> 
<p>Cool P0.</p>
<p>Cool P1.</p>
<p>Cool P2.</p>
<h3>Heading dude</h3>

</blockquote>
<p>That's all folks</p>
";

			string exResult =
@"Hi

> Cool P0.
> 
> Cool P1.
> 
> Cool P2.
> 
> ### Heading dude
> 

That's all folks";

			RUN_HtmlToMDTest(
				nameof(BlockQuotes2), htmlText, exResult);
		}

		[Fact]
		public void Sample1()
		{
			string htmlText =
@"<h1>Test</h1>

<p>Sample <a href=""https://test.com/test123"">link!</a> – test.</p>

<h2>Lorem Ipsum</h2>

cool
<hr />

<p>Lorem ipsum dolor sit amet, vim quis stet detraxit ne, posse eleifend periculis cu nam. 
Dico oporteat salutatus ei per, te quo platonem voluptatum: <a href=""http://howdy.org/"">Howdy</a>. 
Test some escapes:</p>			        This gibberish is (this or that), and this `code` is so relative ~ it's not *neither*, **naither** nor __that__?
<ul>
  <li>Apples</li>
  <li>Oranges</li>
  <li>Peaches</li>
</ul>
";

			string exResult =
@"# Test

Sample [link!](https://test.com/test123) – test.

## Lorem Ipsum

cool

* * *

Lorem ipsum dolor sit amet, vim quis stet detraxit ne, posse eleifend periculis cu nam. Dico oporteat salutatus ei per, te quo platonem voluptatum: [Howdy](http://howdy.org/). Test some escapes:

This gibberish is (this or that), and this \`code\` is so relative ~ it's not \*neither\*, \*\*naither\*\* nor \_\_that\_\_?

*   Apples
*   Oranges
*   Peaches";

			string exResultNoMd =
@"Test

Sample link! – test.

Lorem Ipsum

cool

Lorem ipsum dolor sit amet, vim quis stet detraxit ne, posse eleifend periculis cu nam. Dico oporteat salutatus ei per, te quo platonem voluptatum: Howdy. Test some escapes:

This gibberish is (this or that), and this `code` is so relative ~ it's not *neither*, **naither** nor __that__?

Apples
Oranges
Peaches";

			RUN_HtmlToMDTest(
				nameof(BlockQuotes2), htmlText, exResult, exResultNoMd);
		}

		[Fact]
		public void Links1()
		{
			RUN_HtmlToMDTest(
				nameof(Links1),

				$@"<p><strong class=""some-strong"">Hello</strong> there.</p>
	<p>Check out <a class=""some-img"" href=""{sampleImgUrl1}"" style=""color: red;"">this lion</a>! 
</p>",

					$"**Hello** there.\r\n\r\nCheck out [this lion]({sampleImgUrl1})!",

					"Hello there.\r\n\r\nCheck out this lion!");
		}

		[Fact]
		public void NoXmlToClear() => RUN_HtmlToMDTest(
			nameof(NoXmlToClear),
			"hello world",
			"hello world");

		[Fact]
		public void NoXmlToClear2_RtBracketsOnly() => RUN_HtmlToMDTest(
			nameof(NoXmlToClear2_RtBracketsOnly),
			"he>llo >world>",
			"he>llo >world>");

		const string dbln = "\r\n\r\n";

		[Fact]
		public void LotOfWSLineBreaksInAttributes1() => RUN_HtmlToMDTest(
			nameof(LotOfWSLineBreaksInAttributes1),
				"ok <p \r\nclass=\"cool-red\">hi</p><p>world</p><div>do da</div><font /> hi",
				$"ok{dbln}hi{dbln}world{dbln}do da{dbln}hi");

		const string wsMess = "    \t\t \r\n\r\n\t\t\t ";

		[Fact]
		public void WSFun1() => RUN_HtmlToMDTest(
			nameof(WSFun1),
				$"ok {wsMess}<p>hi{wsMess}</p>{wsMess}<p {wsMess}class=\"cool-red \"{wsMess}>{wsMess}every<b>one</b> in{wsMess}the <i>world</i><strong>!</strong></p>test!", //<div  >test</div> ",
				$"ok{dbln}hi{dbln}every**one** in the *world***!**{dbln}test!");

		[Fact]
		public void WSFun2() => RUN_HtmlToMDTest(
			nameof(WSFun2),
			$"ok {wsMess}<p>hi{wsMess}</p>G", //<div  >test</div> ",
			$"ok{dbln}hi{dbln}G");

		[Fact]
		public void Attributes2() => RUN_HtmlToMDTest(
			nameof(Attributes2),
				"ok <p \r\nclass=\"cool-red\">hi</p><p>world</p>",
				$"ok{dbln}hi{dbln}world");

		[Fact]
		public void DecodingHtml1() => RUN_HtmlToMDTest(new HtmlToMDTestArgs(
			nameof(Attributes2),
				"ok <p \r\nclass=\"cool-red\">hi &nbsp; there &mdash; world!</p>",
				$"ok{dbln}hi   there — world!") { HtmlDecode = true });

		[Fact]
		public void WSBetweenTags1() => RUN_HtmlToMDTest(
			nameof(WSBetweenTags1),
				$"ok <p>hi</p>there <p>world</p>",
				$"ok{dbln}hi{dbln}there{dbln}world");

		[Fact]
		public void Doctype() => RUN_HtmlToMDTest(
			nameof(Doctype),
				"<!DOCTYPE html>\r\n<p>hi</p>",
				"hi");

		[Fact]
		public void OpensWComment() => RUN_HtmlToMDTest(
			nameof(OpensWComment),
				"<!--yo--><p>hi <!--howdy comment--> there</p>",
				"hi there");

		[Fact]
		public void LIList1() => RUN_HtmlToMDTest(
			nameof(LIList1),
				"Ok <ul><li  >apples</li><li>Oranges</li> <li>Peaches</li></ul>",
				"Ok\r\n\r\n*   apples\r\n*   Oranges\r\n*   Peaches");
		[Fact]
		public void LIList2() => RUN_HtmlToMDTest(
			nameof(LIList1),
				"Ok <ul><li class=\"howdy!\" >apples</li><li class=\"howdy2!\" > Oranges</li> <li>Peaches</li>\t\r\n</ul> d",
				"Ok\r\n\r\n*   apples\r\n*   Oranges\r\n*   Peaches\r\n\r\nd");

		[Fact]
		public void LineBreaks1() => RUN_HtmlToMDTest(
			nameof(LineBreaks1),
				$"Hi \t\r\n {strX("<br />", 7)}Friends<br/>Hello and hi", //
				$"Hi{strX("  \r\n", 7)}Friends  \r\nHello and hi");

		static string dblnX(int times)
			=> string.Concat(Enumerable.Repeat(dbln, times));

		static string strX(string val, int times)
			=> string.Concat(Enumerable.Repeat(val, times));

		[Fact]
		public void BoldItalics() => RUN_HtmlToMDTest(
			nameof(BoldItalics),
				"Ex<b>press</b>ing one<em>se</em>lf!",
				"Ex**press**ing one*se*lf!",
				"Expressing oneself!");

		[Fact]
		public void Misc1() => RUN_HtmlToMDTest(
			nameof(Misc1),
				"<p><i>He<strong class=\"some-strong\">llo</b></i> there.</p><p>Co<em class=\"y23\">ol beans</em>.</p>",
				$"*He**llo*** there.{dbln}Co*ol beans*.",
				$"Hello there.{dbln}Cool beans.");



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
			public bool HtmlDecode { get; set; }
		}


		static string sampleImgUrl1 = "https://www.gannett-cdn.com/presto/2018/08/23/PPHX/c6bf90a6-362d-4e61-ac2e-a21bbc4318ab-Lion2.jpg?crop=959,540,x0,y295&width=3200&height=1680&fit=bounds";

	}
}
