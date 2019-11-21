using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XmlTextFuncs
	{
		/* NOTE! Since System.Net.WebUtility.HtmlDecode is included in vanilla netcore package (Microsoft.NETCore.App), 
         we can simplify and just allow this... maybe ... for now... so removing for now this conditional

		#if SYSTEMNET // htmlDecode in methods below rely on System.Net.WebUtility.HtmlDecode
        */

		public static string ClearXmlTagsAndHtmlDecode(string value, bool trim)
		{
			const bool htmlDecode = true;

			value = trimHtmlEscape(value, trim, htmlDecode); // decodes first, then any 

			if(value.IsNulle())
				return value;

			string result = ClearXmlTags(value, trim: false); // already trimmed

			if(htmlDecode) {
				result = System.Net.WebUtility.HtmlDecode(result);
				if(trim)
					result = result.TrimIfNeeded();
			}
			return result;
		}

		public static string ClearEscapedXmlTags(string value, bool trim, bool htmlDecode)
		{
			value = trimHtmlEscape(value, trim, htmlDecode);
			if(value.IsNulle())
				return value;
			return ClearEscapedXmlTags(value, trim: false);
		}

		static string trimHtmlEscape(string value, bool trim = false, bool htmlDecode = false)
		{
			if(value.IsNulle())
				return value;
			if(trim && value.IsTrimmable()) {
				value = value.TrimN();
				if(value.IsNulle())
					return value;
			}

			if(htmlDecode) {
				value = System.Net.WebUtility.HtmlDecode(value);
				if(trim && value.IsTrimmable())
					value = value.TrimN();
				if(value.IsNulle())
					return null;
			}
			return value;
		}

		//#endif

		public static string ClearXmlTags(string value, bool trim = false)
		{
			if(trim)
				value = value.TrimIfNeeded();
			if(value.IsNulle())
				return value;

			int start = 0;
			int end = 0;
			int len = value.Length;
			int prevSegLen = 0;

			StringBuilder sb = null;
			char currLook = '<';

			for(int i = 0; i < len; i++) {
				if(value[i] == currLook) {
					if(currLook == '<') {

						int nextI = i + 1;
						if(nextI < len) {
							// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
							char n = value[nextI];
							if(!(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
								continue;
						}

						//char n = value[i + 1];
						//// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
						//if (i + 1 < len && !(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
						//	continue;

						start = i;
						currLook = '>';
					}
					else if(currLook == '>') {
						//value[i + 1].IsAsciiLetter() || value[i + 1] == '/'

						if(i > 0) {
							char c = value[i - 1];

							if(c == '/') { // this could be a self-closing open tag ('<br />')
							}

							// I'm confused what this was about, but I *think* these following conditions 
							// represent things that COULDN'T be in a CLOSING tag, which we're expecting this is ... ??
							else if(c != '"' && c != '\'' && !c.IsAsciiLetter())
								continue;

							// -- old, replaced with above
							//if (value.PreviousIndexIsMatch(i - 1, c => c != '"' && c != '\'' && !c.IsAsciiLetter() 
							//	? true 
							//	: (char.IsWhiteSpace(c) ? (bool?)null : false))) // what in the WORLD was this whitespace check? which still returned null/false?!
							//	continue;
						}

						prevSegLen = start - end;
						if(prevSegLen > 0) {
							if(sb == null)
								sb = new StringBuilder(value.Length);

							int _start = end;
							int _len = prevSegLen;
							if(sb.NotNulle() && sb[sb.Length - 1].IsWhitespace() && value[_start].IsWhitespace()) {
								_start++;
								_len--;
							}
							sb.Append(value, _start, _len);
						}
						end = i + 1;
						currLook = '<';
					}
				}
			}

			#region -- old loop --
			//for (int i = 0; i < len; i++) {
			//	if (value[i] == currLook) {

			//		if (currLook == '<') {
			//			start = i;
			//			currLook = '>';
			//		}
			//		else if (currLook == '>') {
			//			prevSegLen = start - end;
			//			if (prevSegLen > 0 || start == 0) {
			//				if (sb == null)
			//					sb = new StringBuilder(value.Length);
			//				sb.Append(value, end, prevSegLen);
			//			}
			//			end = i + 1;
			//			currLook = '<';
			//		}
			//	}
			//}
			#endregion

			//if (sb == null) // CHANGED 2019/02/20, this was not allowing cases where only text occurred at end, after all tags
			//	return value;

			prevSegLen = len - end;
			if(prevSegLen > 0) {
				if(sb == null)
					sb = new StringBuilder(prevSegLen);

				sb.Append(value, end, prevSegLen);
			}

			string result = trim ? sb.TrimToString() : sb.ToString();
			return result;
		}

		// NOT used currently
		public static string ClearEscapedXmlTags(string value, bool trim = false)
		{
			if(trim)
				value = value.TrimIfNeeded();
			if(value.IsNulle())
				return value;

			int start = 0;
			int end = 0;
			int pos = 0;
			int len = value.Length;
			int cnt = 0;
			StringBuilder sb = new StringBuilder();

			while(pos < len) {

				start = value.IndexOf("&lt;", pos);
				if(start < 0)
					break;

				end = value.IndexOf("&gt;", start);
				if(end < 0)
					break;

				cnt = start - pos;
				if(cnt > 0)
					sb.Append(value, pos, cnt);

				pos = end + 4;
			}

			cnt = len - pos;
			if(cnt > 0)
				sb.Append(value, pos, cnt);

			string result = null;
			if(trim && sb.IsTrimmable()) {
				sb.TrimEnd();
				if(sb.IsTrimmable()) {
				}
				result = sb.Length > 0 && char.IsWhiteSpace(sb[0])
					? sb.ToString()
					: sb.ToString().TrimN();
			}
			else
				result = sb.ToString();

			return result;
		}

		/// <summary>
		/// Looks for an HTML or XML opening tag. Is very simple but highly performant, 
		/// returning true if found. This is NOT an XML validator! It's just 
		/// looking for the very basic apparent signature like this:
		/// <![CDATA[<tag> or <tag....>]]> (where `....` can be ANYTHING except for another opening
		/// pointy tag). So at its root it's a one pass through the string
		/// looking simply for one char: <![CDATA[<]]>, which MUST be followed by a
		/// valid XML tag char (NOT whitespace). When this much is found, it expects and looks
		/// then for a closing pointy: <![CDATA[>]]>, which if it finds it returns true 
		/// (if it finds another opening LT pointy, it continues the search afresh from that point).
		/// In this way, we can filter out the vast majority of plain text which simply had 
		/// lesser or greater than pointy characters (which usually will have whitespace
		/// on either side and especially on the right side). This function is particularly useful
		/// in cases such as an RSS feed reader in deciding if text in one of the fields needs
		/// to be treated as plain-text, or as HTML (whether or not of some mixed form).
		/// </summary>
		public static bool StringContainsAnyXmlTagsQuick(string val)
		{
			if(val.IsNulle())
				return false;

			int len = val.Length;
			int lastI = len - 1;

			for(int i = 0; i < len; i++) {
				if(val[i] == '<') {
					if(i != lastI &&
						(XmlConvert.IsStartNCNameChar(val[i + 1])
						|| val[i + 1] == '!')) {
						// on '!': allows comments (<!--hi-->) or DOCTYPEs (<!DOCTYPE...>) to signal a match

						i += 1; // we already checked 1 ahead is ascii letter

						while(++i < len) {
							if(val[i] == '>')
								return true;
							else if(val[i] == '<') {
								// So, we have two '<' in a row, method IS allowing this,
								// so we now walk i back one (it had just been skipped ff 2),
								// so next outer loop will start afresh with a test on is
								// this new point an opening tag
								i--;
								break;
							}
						}
					}
				}
			}

			return false;
		}

	}
}
