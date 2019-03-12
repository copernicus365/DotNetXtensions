using System;
using System.Linq;
using Xunit;
using DotNetXtensions.Text;

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
		string RUN_HtmlToMDTest(HtmlToMDTestArgs testArgs)
		{
			var test = testArgs;
			bool pass = false;
			string oldResult;
			string mdResult = null;
			string result;
			bool decode = testArgs.HtmlDecode;

			oldResult = XmlTextFuncs.ClearXmlTags(test.Text, trim: true);

			if (test.ExpectedResult != null) {

				var onePassHtmlMd = new OnePassHtmlToMarkdown();
				if (testArgs.IgnoreTagsCS.NotNulle()) {
					onePassHtmlMd.TagsToIgnore = testArgs
						.IgnoreTagsCS
						.SplitAndRemoveWhiteSpaceEntries(',').
						ToDictionary(v => v, v => false);
				}

				mdResult = result = onePassHtmlMd.ConvertHtmlToMD(test.Text, htmlDecode: decode);

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

			return mdResult;

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

		string RUN_HtmlToMDTest(string testName, string text, string expectedResult, string expectedResultNoMD = null, string ignoreTagsCS = null)
			=> RUN_HtmlToMDTest(new HtmlToMDTestArgs(testName, text, expectedResult, expectedResultNoMD) { IgnoreTagsCS = ignoreTagsCS });


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

		[Fact]
		public void ClearCommentsAndMSJunk()
		{
			string mdResult = RUN_HtmlToMDTest(
				nameof(ClearCommentsAndMSJunk),
				MS_Html_Ex1,
				@"Lorem ipsum dolor sit amet

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.  

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.&nbsp; Lorem ipsum dolor sit.  

Lorem ipsum dolor sit.

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

Lorem ipsum dolor sit amet

&nbsp;");
		}

		[Fact]
		public void IgnoreTags1()
		{
			string mdResult = RUN_HtmlToMDTest(
				nameof(IgnoreTags1),
				ExStr_TagsToClear_1,
@"# Howdy, I'm h1 text within a header tag

Howdy I'm just in a normal p

Howdy I'm plain-text in a footer

I'm plain-text in a p in a footer");
		}

		[Fact]
		public void IgnoreTags_KeepHeadAndTitle_RemoveFooter()
		{
			string mdResult = RUN_HtmlToMDTest(
				nameof(IgnoreTags1),
				ExStr_TagsToClear_1,
@"Title Test!

# Howdy, I'm h1 text within a header tag

Howdy I'm just in a normal p",
				ignoreTagsCS: "script,style,form,nav,footer");
		}

		[Fact]
		public void IgnoreTags_KeepScriptTag()
		{
			// This illustrates how ugly it is if such tags are NOT escaped

			string mdResult = RUN_HtmlToMDTest(
				nameof(IgnoreTags1),
				ExStr_TagsToClear_1,
@"# Howdy, I'm h1 text within a header tag

Howdy I'm just in a normal p

;(function () { var input = document.getElementById('form') // blah blah function doDa () { output.something = 13; } })()",
				ignoreTagsCS: "head,style,form,nav,footer"); // 
		}


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
			public string IgnoreTagsCS { get; set; }
		}


		static string sampleImgUrl1 = "https://www.gannett-cdn.com/presto/2018/08/23/PPHX/c6bf90a6-362d-4e61-ac2e-a21bbc4318ab-Lion2.jpg?crop=959,540,x0,y295&width=3200&height=1680&fit=bounds";

		static string ExStr_TagsToClear_1 =
@"<!DOCTYPE html>
<html lang=[Q]en[Q]>
<head>
<meta charset=[Q]utf-8[Q]>
<title>Title Test!</title>
<meta name=[Q]viewport[Q] content=[Q]width=device-width[Q]>
<style>
  * {
    -webkit-box-sizing: border-box;
    -moz-box-sizing: border-box;
    box-sizing: border-box;
  }
	html {
	  font-size: 100%;
	  -webkit-text-size-adjust: 100%;
	  -ms-text-size-adjust: 100%;
	}

	a:focus {
	  outline: thin dotted #333;
	  outline: 5px auto -webkit-focus-ring-color;
	  outline-offset: -2px;
	}

  @media (min-width: 20em) {
    .col {
      float: left;
      width: 50%;
    }
  }
</style>
<script src=[Q]https://cool.com/example.js[Q]></script>
</head>
<body>
<header>
  <h1>Howdy, I'm h1 text within a header tag</h1>
</header>

<nav>I'm plain text within a nav bar</nav>

<p>Howdy I'm just in a normal p</p>

<div class=[Q]row[Q]>
  <form method=[Q]GET[Q] action=[Q]/example[Q] id=[Q]options[Q]>

	<p>I'm text within a p within a form</p>  
  </form>
</div>

<footer>Howdy I'm plain-text in a footer
<p>I'm plain-text in a p in a footer</p></footer>
<script>
  ;(function () {
    var input = document.getElementById('form')
	
   // blah blah

    function doDa () {
      output.something = 13;
    }

  })()
</script>
</body>
</html>
".Replace("[Q]", "\"");

		static string MS_Html_Ex1 =
@"<p>Lorem ipsum dolor sit amet</p><p><!--[if gte mso 9]><xml>
 <o:OfficeDocumentSettings>
  <o:AllowPNG></o:AllowPNG>
 </o:OfficeDocumentSettings>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <w:WordDocument>
  <w:View>Normal</w:View>
  <w:Zoom>0</w:Zoom>
  <w:IgnoreMixedContent>false</w:IgnoreMixedContent>
  <w:LidThemeOther>EN-US</w:LidThemeOther>
  <w:LidThemeAsian>X-NONE</w:LidThemeAsian>
  <w:Compatibility>
   <w:BreakWrappedTables></w:BreakWrappedTables>
   <w:SnapToGridInCell></w:SnapToGridInCell>
  </w:Compatibility>
  <m:mathPr>
   <m:mathFont m:val=[Q]Cambria Math[Q]></m:mathFont>
   <m:brkBin m:val=[Q]before[Q]></m:brkBin>
  </m:mathPr></w:WordDocument>
</xml><![endif]--><!--[if gte mso 9]><xml>
 <w:LatentStyles DefLockedState=[Q]false[Q] DefUnhideWhenUsed=[Q]false[Q]
  DefSemiHidden=[Q]false[Q] DefQFormat=[Q]false[Q] DefPriority=[Q]99[Q]
  LatentStyleCount=[Q]371[Q]>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]0[Q] QFormat=[Q]true[Q] Name=[Q]Normal[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]9[Q] QFormat=[Q]true[Q] Name=[Q]heading 1[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]9[Q] SemiHidden=[Q]true[Q] UnhideWhenUsed=[Q]true[Q] QFormat=[Q]true[Q] Name=[Q]heading 2[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] SemiHidden=[Q]true[Q] UnhideWhenUsed=[Q]true[Q] Name=[Q]header[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]73[Q] Name=[Q]Colorful Grid Accent 6[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]19[Q] QFormat=[Q]true[Q] 
  Name=[Q]Subtle Emphasis[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]21[Q] QFormat=[Q]true[Q] 
  Name=[Q]Intense Emphasis[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]31[Q] QFormat=[Q]true[Q] Name=[Q]Subtle Reference[Q]></w:LsdException>
  <w:LsdException Locked=[Q]false[Q] Priority=[Q]52[Q] Name=[Q]List Table 7 Colorful Accent 6[Q]></w:LsdException>
 </w:LatentStyles>
</xml><![endif]--><!--[if gte mso 10]>
<style>
 /* Style Definitions */
 table.MsoNormalTable
	{mso-style-name:[Q]Table Normal[Q];
	mso-tstyle-rowband-size:0;
	mso-tstyle-colband-size:0;
	mso-style-noshow:yes;
	mso-style-priority:99;
	mso-style-parent:[Q][Q];
	font-size:11.0pt;
	font-family:[Q]Calibri[Q],sans-serif;
	mso-ascii-font-family:Calibri;
	mso-ascii-theme-font:minor-latin;
	mso-hansi-font-family:Calibri;
	mso-hansi-theme-font:minor-latin;
	mso-bidi-font-family:[Q]Times New Roman[Q];
	mso-bidi-theme-font:minor-bidi;}
</style>
<![endif]-->

</p><p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. <br></span></p><p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.<span style=[Q]mso-spacerun:yes[Q]>&nbsp; </span>Lorem ipsum dolor sit. <br></span></p><p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>Lorem 

ipsum dolor 

sit.</span></p>

<p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>Lorem ipsum dolor sit amet, 
consectetur adipiscing elit, sed do eiusmod tempor 
incididunt ut labore et dolore 
magna aliqua.</span></p>

<p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]><span style=[Q]mso-spacerun:yes[Q]></span>Lorem 
ipsum dolor sit amet, consectetur 
adipiscing elit, sed do eiusmod tempor 
incididunt ut labore et dolore magna 
aliqua. </span></p>

<p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>Lorem 
ipsum dolor sit amet</span></p>

<p class=[Q]MsoNormal[Q]><span style=[Q]font-size:12.0pt[Q]>&nbsp;</span></p>
".Replace("[Q]", "\"");
	}
}
