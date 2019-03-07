using System;
using System.Linq;
using Xunit;
using DotNetXtensions.Text;

namespace DotNetXtensions.Test
{
	public class HtmlTagTests
	{
		[Fact]
		public void BadTag_SpaceBeforeTagName_FAIL()
			=> RunIt2(name: "img", tag: "                <img>         ", selfClose: false, startPos: 16);

		[Fact]
		public void With1AttrNormal()
			=> RunIt2(name: "img", startPos: 4, tag: @"    <img src=""hi"">", selfClose: false, args: ("src", "hi"));


		[Fact]
		public void NoAtts_1Long()
			=> RunIt2(name: "p", tag: "<p>");

		[Fact]
		public void NoAtts_1Long_XtraWS()
			=> RunIt2(name: "p", tag: "<p  \t>");

		[Fact]
		public void NoAtts_1Long_SelfCloseWSpace()
			=> RunIt2(name: "i", tag: "<i />", selfClose: true);

		[Fact]
		public void NoAtts_1Long_SelfCloseWOutSpace()
			=> RunIt2(name: "i", tag: "<i/>", selfClose: true);

		[Fact]
		public void BoolAttrNoSpace()
			=> RunIt2(name: "img", tag: "<img is-cool/>", args: ("is-cool", ""));

		[Fact]
		public void BoolAttrWEquals()
			=> RunIt2(name: "img", tag: "<img is-cool=/>", args: ("is-cool", ""));

		[Fact]
		public void BoolAttrWEqualsNoSpace()
			=> RunIt2(name: "img", tag: "<img is-cool= >", args: ("is-cool", ""));

		[Fact]
		public void MultAtts1()
		{
			RunIt2(
				name: "div",
				tag: @"<div class=""red=, !~"" data-good data-happy=""a"" >",
				args: gargs(
				("class", "red=, !~"),
				("data-good", ""),
				("data-happy", "a")));
		}

		[Fact]
		public void NoAtts_Img_NoSpace()
			=> RunIt2(name: "img", tag: "<img/>", selfClose: true);

		[Fact]
		public void With1AttrNoQuotes()
			=> RunIt2(name: "img", tag: @"<img src=hi >", args: ("src", "hi"));

		[Fact]
		public void With1AttrNoQuotes_NoSpaceOnEnd()
			=> RunIt2(name: "img", tag: @"<img src=hi>", args: ("src", "hi"));

		const string dummyHtmlContent = "\t\t <p>howdy</p> \r\n\t";

		[Fact]
		public void MixedAttributes_FinalFails_FAIL()
			=> RunIt2(
				name: "img",
				tag: $"<img is-cool hello='yes>  ",
				success: false,
				args: gargs(("is-cool", ""), ("hello", "yes")));


		[Fact]
		public void NotAtStart_Test1()
		{
			string tagStr = $"<img is-cool hello='yes'>";
			string tagStrFull = $"{dummyHtmlContent}{tagStr}  {dummyHtmlContent}";

			RunIt2(
				  name: "img",
				  tag: tagStrFull,
				  //success: false,
				  startPos: dummyHtmlContent.Length - 0,
				  validate: ht => {
					  if (ht.TagStartIndex != dummyHtmlContent.Length)
						  return false;
					  string fullTag = tagStrFull.Substring(ht.TagStartIndex, ht.TagLength);
					  if (fullTag != tagStr)
						  return false;
					  return true;
				  },
				  args: gargs(("is-cool", ""), ("hello", "yes")));
		}

		[Fact]
		public void BigExample1_ComplicatedUrlValue()
		{
			const string imgUrl = "https://static1.example.com/static/abc/t/abcedfg123/ge/Some+Cool+Idea+1K.png";
			RunIt2(
				name: "img",
				selfClose: true,
				tag:
	$@"<img  class='thumb-image' data-image=""{imgUrl}"" data-image-dimensions=1920x1080 data-image-focal-point=""0.5,0.5"" alt='Some Cool Pic 1K.png'  data-image-id=""abcedfg123"" data-type=""image"" src=""{imgUrl}?format=1200w"" data-load=false/>
		",
				args: gargs(
		  ("class", "thumb-image"),
		  ("data-image", imgUrl),
		  ("data-image-dimensions", "1920x1080"),
		  ("data-image-focal-point", "0.5,0.5"),
		  ("alt", "Some Cool Pic 1K.png"),
		  ("data-load", "false"),
		  ("data-image-id", "abcedfg123"),
		  ("data-type", "image"),
		  ("src", $"{imgUrl}?format=1200w")
		  ));
		}





		bool RunTest(HtmlTag t, string tagName, bool? isSelfClose, params (string key, string val)[] args)
		{
			if (t.TagName != tagName)
				return false;

			if (t.Attributes.CountN() != args.CountN())
				Assert.True(false);

			if (isSelfClose != null)
				Assert.True(t.IsSelfClosed == isSelfClose);

			if (!t.NoAtts) {
				// as for duplicates, the input kvs must CHOOSE which duplicated key wins, 
				// only send in that one
				var d = args.ToDictionary(kv => kv.key, kv => kv.val);

				foreach (var kv in d) {

					string key = kv.Key;
					string val = kv.Value;

					if (!t.Attributes.ContainsKey(key))
						return false;

					string tVal = t.Attributes[key];
					if (tVal != val)
						return false;
				}
			}

			return true;
		}

		//void RunIt2(string tagName, int startPos, string tag, params (string key, string val)[] args)
		//{
		//	RunIt2(tag, tagName, startPos, null, true, args);
		//}

		//void RunIt2( params (string key, string val)[] args)
		//{
		//	RunIt2(tag, tagName, startPos, null, true, args);
		//}

		void RunIt2(
			bool success = true,
			string name = null,
			int startPos = 0,
			bool? selfClose = null,
			string tag = null,
			Func<HtmlTag, bool> validate = null,
			params (string key, string val)[] args)
		{
			HtmlTag hTag = new HtmlTag();

			bool inputTagIsVerifiedFullOpenTag = !tag.IsTrimmable();


			bool passedParse = hTag.Parse(tag, startPos, inputTagIsVerifiedFullOpenTag);
			bool testPass1 = passedParse == success;

			Assert.True(testPass1);

			if (success) {

				bool isFinalValid = RunTest(hTag, name, selfClose, args);

				Assert.True(isFinalValid);

				if (validate != null) {
					if (!validate(hTag))
						Assert.True(false);
				}
			}
		}

		static (string key, string val)[] gargs(params (string key, string val)[] args) => args;

	}
}
