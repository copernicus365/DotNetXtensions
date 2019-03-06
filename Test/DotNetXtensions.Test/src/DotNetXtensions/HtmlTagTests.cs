using System;
using System.Text;
using System.Linq;
using DotNetXtensions.Globalization;
using Xunit;

namespace DotNetXtensions.Test
{
	public class HtmlTagTests
	{
		[Fact]
		public void NoAtts_Img_Space()
			=> RunIt1("<img />", t => t.TagName == "img" && t.NoAtts);
		[Fact]
		public void NoAtts_Img_NoSpace()
			=> RunIt1("<img/>", t => t.TagName == "img" && t.NoAtts);

		[Fact]
		public void NoAtts_1Long()
			=> RunIt1("<p>", t => t.TagName == "p");

		[Fact]
		public void NoAtts_1Long_XtraWS()
			=> RunIt1("<p  \t>", t => t.TagName == "p" && t.NoAtts);

		[Fact]
		public void NoAtts_1Long_SelfCloseWSpace()
			=> RunIt1("<i />", t => t.TagName == "i" && t.IsSelfClosed && t.NoAtts);

		[Fact]
		public void NoAtts_1Long_SelfCloseWOutSpace()
			=> RunIt1("<i/>", t => t.TagName == "i" && t.IsSelfClosed && t.NoAtts);

		[Fact]
		public void AttrWithNoVal()
			=> RunIt2("<img is-cool />", "img", 
				("is-cool", null));

		bool RunTest(HtmlTag t, string tagName, params (string key, string val)[] args)
		{
			if (t.TagName != tagName)
				return false;

			Assert.True(t.Attributes.CountN() == args.CountN());

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
			HtmlTag hTag = new HtmlTag();

			bool isValid = hTag.Parse(tag, true);
			Assert.True(isValid);

			bool isFinalValid = RunTest(hTag, tagName, args);
			Assert.True(isFinalValid);
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
