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
			=> RunIt2("< img>", "img", false, success: false);

		[Fact]
		public void NoAtts_Img_NoSpace()
			=> RunIt2("<img/>", "img", true);

		[Fact]
		public void NoAtts_1Long()
			=> RunIt2("<p>", "p", false);

		[Fact]
		public void NoAtts_1Long_XtraWS()
			=> RunIt2("<p  \t>", "p", false);

		[Fact]
		public void NoAtts_1Long_SelfCloseWSpace()
			=> RunIt2("<i />", "i", true);

		[Fact]
		public void NoAtts_1Long_SelfCloseWOutSpace()
			=> RunIt2("<i/>", "i", true);

		[Fact]
		public void With1AttrNormal()
			=> RunIt2(@"<img src=""hi"">", "img", ("src", "hi"));

		[Fact]
		public void With1AttrNoQuotes()
			=> RunIt2(@"<img src=hi >", "img", ("src", "hi"));

		[Fact]
		public void With1AttrNoQuotes_NoSpaceOnEnd()
			=> RunIt2(@"<img src=hi>", "img", ("src", "hi"));

		[Fact]
		public void BoolAttr()
			=> RunIt2("<img is-cool />", "img", ("is-cool", ""));

		[Fact]
		public void BoolAttrNoSpace()
			=> RunIt2("<img is-cool/>", "img", ("is-cool", ""));

		[Fact]
		public void BoolAttrWEquals()
			=> RunIt2("<img is-cool=/>", "img", ("is-cool", ""));

		[Fact]
		public void BoolAttrWEqualsNoSpace()
			=> RunIt2("<img is-cool= >", "img", ("is-cool", ""));

		[Fact]
		public void MultAtts1()
			=> RunIt2(@"<div class=""red=, !~"" data-good data-happy=""a"" >", "div",
				("class", "red=, !~"),
				("data-good", ""),
				("data-happy", "a"));

		bool RunTest(HtmlTag t, string tagName, bool? isSelfClose, params (string key, string val)[] args)
		{
			if (t.TagName != tagName)
				return false;

			Assert.True(t.Attributes.CountN() == args.CountN());

			if(isSelfClose != null)
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
					if(tVal != val)
						return false;
				}
			}

			return true;
		}

		void RunIt2(string tag, string tagName, params (string key, string val)[] args)
		{
			RunIt2(tag, tagName, null, true, args);
		}

		void RunIt2(string tag, string tagName, bool? isSelfClose, params (string key, string val)[] args)
		{
			RunIt2(tag, tagName, null, true, args);
		}

		void RunIt2(string tag, string tagName, bool? isSelfClose, bool success, params (string key, string val)[] args)
		{
			HtmlTag hTag = new HtmlTag();

			bool inputTagIsVerifiedFullOpenTag = !tag.IsTrimmable();


			bool passedParse = hTag.Parse(tag, inputTagIsVerifiedFullOpenTag);
			bool testPass1 = passedParse == success;

			Assert.True(testPass1);

			if (success) {
				bool isFinalValid = RunTest(hTag, tagName, isSelfClose, args);

				Assert.True(isFinalValid);
			}
		}

		void RunIt1(string tag, Predicate<HtmlTag> isCorrect, bool shouldParse = true)
		{
			HtmlTag hTag = new HtmlTag();

			bool isValid = hTag.Parse(tag, true);
			Assert.True(isValid);

			bool isFinalValid = isCorrect(hTag);
			Assert.True(isFinalValid);
		}
	}
}
