using System;
using System.Text;
using System.Linq;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	public class TextFuncs_ClearXmlTagsTests : BaseUnitTest
	{
		/// <summary>
		/// Note: The following runs a huge number of tests. Note that by necessity, they 
		/// are inflexibly demanding, requiring an exact match whitespaces and all, which in 
		/// many cases we don't need it exactly that close, but, no other simple way to test 
		/// for a general success.
		/// </summary>
		[Fact]
		public void ClearHtmlTagsVariousTests()
		{
			string sampleImgUrl1 = "https://www.gannett-cdn.com/presto/2018/08/23/PPHX/c6bf90a6-362d-4e61-ac2e-a21bbc4318ab-Lion2.jpg?crop=959,540,x0,y295&width=3200&height=1680&fit=bounds";

			var arr = new WhitespaceBetweenTags[] {

				new WhitespaceBetweenTags("Links",
					$@"<p><strong class=""some-strong"">Hello</strong> there.</p>
	<p>Check out <a class=""some-img"" href=""{sampleImgUrl1}"" style=""color: red;"">this lion</a>! 
</p>",
					$"**Hello** there.\r\n\t\r\nCheck out [this lion]({sampleImgUrl1})!",

					"Hello there.\r\n\t\r\nCheck out this lion!"),

				new WhitespaceBetweenTags("Blockquote", @"<p>Hi, Mr. Dude says:</p><blockquote cite=""http://www.howdy.com/"">Do da day <b>awesome</b> possom.</blockquote><p>Th' th' th' that's all folks!</p>
", @"Hi, Mr. Dude says: 
> Do da day **awesome** possom. 

Th' th' th' that's all folks!"),

				new WhitespaceBetweenTags("Do nothing (no brackets at all)", "hello world", "hello world"),
				new WhitespaceBetweenTags("Do nothing (only some end brackets, no convert)", "he>llo >world>", "he>llo >world>"),

				new WhitespaceBetweenTags("Much whitespace and returns in attributes, divs, etc",
					"ok <p \r\nclass=\"cool-red\">hi</p><p>world</p><div>do da</div><font /> hi",
					"ok \r\n\r\nhi \r\n\r\nworld do da hi"),

				new WhitespaceBetweenTags("Example1",
					"ok <p>hi</p><p class=\"cool-red \">every<b>one</b> in the <i>world</i><strong>!</strong></p><div  >yoddle</div> ",
					"ok \r\n\r\nhi \r\n\r\nevery**one** in the *world***!** yoddle"),

				new WhitespaceBetweenTags("With attributes 1", "ok <p \r\nclass=\"cool-red\">hi</p><p>world</p>", "ok \r\n\r\nhi \r\n\r\nworld"),
				new WhitespaceBetweenTags("Open text between p tags", "ok <p>hi</p>there <p>world</p>", "ok \r\n\r\nhi there \r\n\r\nworld"),

				new WhitespaceBetweenTags("Test DOCTYPE", "<!DOCTYPE html>\r\n<p>hi</p>", "hi"),

				new WhitespaceBetweenTags("Test Comments", "<!--yo--><p>hi <!--howdy comment--> there</p>", "hi there"),

				new WhitespaceBetweenTags("LIs",
					"Ok <ul><li  >apples</li>\r\n<li>Oranges</li> <li>Peaches</li></ul>",
					"Ok \r\n* apples\r\n* Oranges \r\n* Peaches"),

				new WhitespaceBetweenTags("Test breaks", 
					"Hi<br />Friends<br/>Hello and hi", 
					"Hi\r\nFriends\r\nHello and hi"),

				new WhitespaceBetweenTags("Test bold tag, with and w/out MD",
					"Ex<b>press</b>ing one<em>se</em>lf!", 
					"Ex**press**ing one*se*lf!", 
					"Expressing oneself!"),

				new WhitespaceBetweenTags("Mix", 
					"<p><i>He<strong class=\"some-strong\">llo</b></i> there.</p><p>Co<em class=\"y23\">ol beans</em>.</p>",
					"*He**llo*** there. \r\n\r\nCo*ol beans*.",
					"Hello there. \r\n\r\nCool beans."),
			};

			for (int i = 0; i < arr.Length; i++) {

				var test = arr[i];

				bool pass = false;
				string result = null;
				string oldResult = null;
				
				void printHeader(bool md)
				{
					$@"--- ClearHtmlTags {i}: {test.TestName} (No-MD):
 - {(pass ? "PASS" : "FAIL")}

 - ORIGINAL : 
`{test.Text.SubstringMax(120)}`

 - CONVERTED: 
`{result}`

 - OLDRESULT: 
`{oldResult}`

".Print();
				}
	
				oldResult = XmlTextFuncs.ClearXmlTags(test.Text, trim: true);

				if (test.ExpectedResult != null) {
					result = XmlTextFuncs.ClearHtmlTags(test.Text, convertWithMinimalMarkdown: true, trim: true);

					pass = result == test.ExpectedResult;

					printHeader(true);
				}

				if (test.ExpectedResultNoMD != null) {
					result = XmlTextFuncs.ClearHtmlTags(test.Text, convertWithMinimalMarkdown: false, trim: true);

					pass = result == test.ExpectedResultNoMD;

					printHeader(false);
				}

				if(!pass)
					Assert.True(pass, $"Failed: {test.TestName}");
			}
		}

		public class WhitespaceBetweenTags
		{
			public WhitespaceBetweenTags(string testName, string text, string expectedResult, string expectedResultNoMD = null)
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
	}
}
