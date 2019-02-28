using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
#if DNXPublic
	public
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

			if (value.IsNulle())
				return value;

			string result = ClearXmlTags(value, trim: false); // already trimmed

			if (htmlDecode) {
				result = System.Net.WebUtility.HtmlDecode(result);
				if (trim)
					result = result.TrimIfNeeded();
			}
			return result;
		}

		public static string ClearHtmlTagsAndHtmlDecode(string value, bool convertWithMinimalMarkdown = true, bool trim = false)
		{
			const bool htmlDecode = true;

			value = trimHtmlEscape(value, trim, htmlDecode); // decodes first, then any 

			if (value.IsNulle())
				return value;

			string result = ClearHtmlTags(value,
				convertWithMinimalMarkdown: convertWithMinimalMarkdown,
				trim: false); // already trimmed

			if (htmlDecode) {
				result = System.Net.WebUtility.HtmlDecode(result);
				if (trim)
					result = result.TrimIfNeeded();
			}
			return result;
		}

		public static string ClearEscapedXmlTags(string value, bool trim, bool htmlDecode)
		{
			value = trimHtmlEscape(value, trim, htmlDecode);
			if (value.IsNulle())
				return value;
			return ClearEscapedXmlTags(value, trim: false);
		}

		static string trimHtmlEscape(string value, bool trim = false, bool htmlDecode = false)
		{
			if (value.IsNulle())
				return value;
			if (trim && value.IsTrimmable()) {
				value = value.TrimN();
				if (value.IsNulle())
					return value;
			}

			if (htmlDecode) {
				value = System.Net.WebUtility.HtmlDecode(value);
				if (trim && value.IsTrimmable())
					value = value.TrimN();
				if (value.IsNulle())
					return null;
			}
			return value;
		}

		//#endif

		static Dictionary<string, string> TagNamesToNotAddWhitespaceBetweenTags = new Dictionary<string, string>() {
			{ "b", "**" },
			{ "strong", "**" },
			{ "em", "*" },
			{ "i", "*" },
			{ "u", "_" },
			{ "span", "" },
			{ "a", "" },
		};

		public static string ClearHtmlTags(string value, bool convertWithMinimalMarkdown = true, bool trim = true)
		{
			if (value.IsNulle())
				return value;

			if (value.IndexOf('<') < 0) {

				if (trim)
					value = value.TrimIfNeeded();

				return value;
			}

			int len = value.Length;
			var sb = new StringBuilder(len);

			for (int i = 0; i < len; i++) {

				if (value[i] != '<') {
					sb.Append(value[i]);
					continue;
				}

				if (i + 2 >= len)
					break;

				int tagPointyStartIdx = i;
				i++;
				char nChar = value[i];

				// --- LOCAL-FUNCTIONS ---

				char sbLast() => sb.Length == 0 ? default(char) : sb[sb.Length - 1];
				char sbLastX() => sb[sb.Length - 1];

				bool sbEmpty() => sb.Length == 0;

				bool previousIsNewLine() => sbLast() == '\n';

				int previousIsNewLineIgnoreWS(int countNewLinesUpTo, int reportStartOfContentCountAs = 2)
				{
					if (sb.Length < 1)
						return reportStartOfContentCountAs;

					int newLines = 0;

					for (int j = sb.Length - 1; j >= 0; j--) {
						if (!sb[j].IsWhitespace())
							return newLines;

						char c = sb[j];
						if (c == '\n') {
							if (++newLines >= countNewLinesUpTo)
								return newLines;
						}
					}
					return newLines;
				}

				bool whiteSpaceSandwish() =>
					!sbEmpty()
					&& (i + 1 < len)
					&& sbLast().IsWhitespace()
					&& sbLast() == value[i + 1];

				void SkipPastTagEnd()
				{
					while (value[i] != '>' && i < len)
						i++;
				}

				// --- END ---

				bool isEndTag = nChar == '/'; // save that this IS an end tag, to be used in a bit 

				if (isEndTag) {
					++i; // <-- let's get i past so we can get tag name with same logic below
					nChar = value[i]; // get nxChar also on next, past close slash
				}

				bool isStartTagName = XmlConvert.IsStartNCNameChar(nChar);
				bool isDoctypeOrComment = nChar == '!'; //<!--howdy comment--> or <!DOCTYPE html>

				if (isDoctypeOrComment || !isStartTagName) {
					SkipPastTagEnd();

					if (whiteSpaceSandwish())
						i++;
					continue;
				}

				// ok, we're at least a good start-tag char, now let's get the rest of the following name...

				int startTagIdx = i;
				int endTagIdx = i;

				for (; i < len; i++) {
					nChar = value[i];

					if (nChar == '>')
						break;
					else if (XmlConvert.IsNCNameChar(nChar) || nChar == ':') {

						// NOTE! Is only if that does not skip past tag end (
						// `== ':'` -- IsNCNameChar leaves off namespace `:` colon, let's allow it
						endTagIdx++;
					}
					else if (
						nChar == '/' // only if no space after the tag name, e.g. "<br/>" vs "<br />"
						||
						XmlConvert.IsWhitespaceChar(nChar)) {
						SkipPastTagEnd();
						break;
					}
					else {
						// hmmm, did we hit an invalid name char???
						SkipPastTagEnd();
						goto CONTINUE_MAIN_LOOP;
					}
				}

				if (endTagIdx <= startTagIdx) {
					continue;
				}

				string tagNm = value
					.Substring(startTagIdx, endTagIdx - startTagIdx)
					.ToLower();

				string noW = TagNamesToNotAddWhitespaceBetweenTags.V(tagNm);

				if (noW != null) {

					//if (noW != "" && convertWithMinimalMarkdown)
					//	sb.Append(noW);
					//continue;

					if (tagNm == "a" && convertWithMinimalMarkdown) {
						// just let it fall through, anchor is only handled here if NOT doing MD
					}
					else {
						if (convertWithMinimalMarkdown && noW != "")
							sb.Append(noW);
						continue;
					}
				}

				if (!isEndTag) {

					switch (tagNm) {
						case "p": {
							int prevNLinesCnt = previousIsNewLineIgnoreWS(2);
							if (prevNLinesCnt < 2) {

								if (prevNLinesCnt == 1)
									sb.Append("\r\n");
								else // else is 0, add 2
									sb.Append("\r\n\r\n");
							}
							continue;
						}
						case "br": {
							int prevNLinesCnt = previousIsNewLineIgnoreWS(1);
							if (prevNLinesCnt < 1) {
								sb.Append("\r\n");
							}
							continue;
						}
						case "li":
							if (!previousIsNewLine())
								sb.Append("\r\n");

							if (convertWithMinimalMarkdown)
								sb.Append("* ");

							continue;

						case "blockquote":
							if (!previousIsNewLine())
								sb.Append("\r\n");

							if (convertWithMinimalMarkdown)
								sb.Append("> ");

							continue;

						case "a":
							// Should NOT have reached here if `convertWithMinimalMarkdown` wasn't already true
							if (convertWithMinimalMarkdown) {
								if (__getAnchor(value, tagPointyStartIdx, out string anchorText, out string aHref, out int afterAIdx)) {
									i = afterAIdx - 1; // afterAIdx is 1 char AFTER the closing '>' (of "</a>"), go one back bec next for loop will iterate i
									sb.Append($"[{anchorText}]({aHref})");
									continue;
								}
							}
							// Now THIS would be a lot of work, but it wld pay big dividend!
							break;
					}
				}

				if (!isEndTag) {
					if (!sbEmpty() && !sbLast().IsWhitespace())
						sb.Append(" ");
				}
				else {
					if (i + 1 < len && !value[i + 1].IsWhitespace())
						sb.Append(" ");
				}

				if (whiteSpaceSandwish())
					i++;

				CONTINUE_MAIN_LOOP:;
			}

			string result = trim ? sb.TrimToString() : sb.ToString();
			return result;
		}

		static bool __getAnchor(string value, int aIdx, out string anchorText, out string href, out int afterAnchorIdx)
		{
			afterAnchorIdx = -1;
			anchorText = href = null;

			if (value == null || aIdx >= value.Length)
				return false;

			int startTextIdx = value.IndexOf('>', aIdx) + 1;
			if (startTextIdx < 3)
				return false;

			int startEndATagIdx = value.IndexOf('<', startTextIdx); // "</a>"
			int textLen = startEndATagIdx - startTextIdx;

			if (textLen < 1
				|| startEndATagIdx + 4 >= value.Length
				|| !value.Substring(startEndATagIdx, 4).EqualsIgnoreCase("</a>"))
				return false;

			afterAnchorIdx = startEndATagIdx + 4;

			anchorText = value.Substring(startTextIdx, textLen).NullIfEmptyTrimmed();

			if (anchorText.NotNulle()) {
				int startHrefIdx = value.IndexOf("href=\"", aIdx, startTextIdx - aIdx);
				if (startHrefIdx > 0) {
					startHrefIdx += 6;
					int aEndQIdx = value.IndexOf('\"', startHrefIdx, startTextIdx - startHrefIdx);
					if (aEndQIdx > 0)
						href = value.Substring(startHrefIdx, aEndQIdx - startHrefIdx).NullIfEmptyTrimmed();
				}
			}

			if (href == null)
				return false;

			return true;
		}

		public static string ClearXmlTags(string value, bool trim = false)
		{
			if (trim)
				value = value.TrimIfNeeded();
			if (value.IsNulle())
				return value;

			int start = 0;
			int end = 0;
			int len = value.Length;
			int prevSegLen = 0;

			StringBuilder sb = null;
			char currLook = '<';

			for (int i = 0; i < len; i++) {
				if (value[i] == currLook) {
					if (currLook == '<') {

						int nextI = i + 1;
						if (nextI < len) {
							// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
							char n = value[nextI];
							if (!(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
								continue;
						}

						//char n = value[i + 1];
						//// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
						//if (i + 1 < len && !(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
						//	continue;

						start = i;
						currLook = '>';
					}
					else if (currLook == '>') {
						//value[i + 1].IsAsciiLetter() || value[i + 1] == '/'

						if (i > 0) {
							char c = value[i - 1];

							if (c == '/') { // this could be a self-closing open tag ('<br />')
							}

							// I'm confused what this was about, but I *think* these following conditions 
							// represent things that COULDN'T be in a CLOSING tag, which we're expecting this is ... ??
							else if (c != '"' && c != '\'' && !c.IsAsciiLetter())
								continue;

							// -- old, replaced with above
							//if (value.PreviousIndexIsMatch(i - 1, c => c != '"' && c != '\'' && !c.IsAsciiLetter() 
							//	? true 
							//	: (char.IsWhiteSpace(c) ? (bool?)null : false))) // what in the WORLD was this whitespace check? which still returned null/false?!
							//	continue;
						}

						prevSegLen = start - end;
						if (prevSegLen > 0) {
							if (sb == null)
								sb = new StringBuilder(value.Length);

							int _start = end;
							int _len = prevSegLen;
							if (sb.NotNulle() && sb[sb.Length - 1].IsWhitespace() && value[_start].IsWhitespace()) {
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
			if (prevSegLen > 0) {
				if (sb == null)
					sb = new StringBuilder(prevSegLen);

				sb.Append(value, end, prevSegLen);
			}

			string result = trim ? sb.TrimToString() : sb.ToString();
			return result;
		}

		// NOT used currently
		public static string ClearEscapedXmlTags(string value, bool trim = false)
		{
			if (trim)
				value = value.TrimIfNeeded();
			if (value.IsNulle())
				return value;

			int start = 0;
			int end = 0;
			int pos = 0;
			int len = value.Length;
			int cnt = 0;
			StringBuilder sb = new StringBuilder();

			while (pos < len) {

				start = value.IndexOf("&lt;", pos);
				if (start < 0)
					break;

				end = value.IndexOf("&gt;", start);
				if (end < 0)
					break;

				cnt = start - pos;
				if (cnt > 0)
					sb.Append(value, pos, cnt);

				pos = end + 4;
			}

			cnt = len - pos;
			if (cnt > 0)
				sb.Append(value, pos, cnt);

			string result = null;
			if (trim && sb.IsTrimmable()) {
				sb.TrimEnd();
				if (sb.IsTrimmable()) {
				}
				result = sb.Length > 0 && char.IsWhiteSpace(sb[0])
					? sb.ToString()
					: sb.ToString().TrimN();
			}
			else
				result = sb.ToString();

			return result;
		}

	}
}

//public static string ClearXmlTags11(string value, bool trim = false)
//{
//	if (trim)
//		value = value.TrimIfNeeded();

//	if (value.IsNulle())
//		return value;

//	if (value.IndexOf('<') < 0)
//		return value;

//	int start = 0;
//	int end = 0;
//	int len = value.Length;
//	var sb = new StringBuilder(len);
//	char currLook = '<';

//	string tagName = null;
//	bool isCloseTag = false;
//	bool priorToTagIsWhitesp = false;
//	int prevSegLen;
//	int lenMinus2 = len - 2;

//	for (int i = 0; i < lenMinus2; i++) {
//		if (value[i] != '<') {
//			sb.Append(value[i]);
//			continue;
//		}

//		if (value[i] == '<') {

//			i++;

//			void SkipPastTagEnd()
//			{
//				while (value[i] != '>' && i < len) {
//					i++;
//				}
//			}

//			char nChar = value[i];

//			bool isEndTag = nChar == '/';
//			if (isEndTag) {
//				// 1) save that this IS isEndTag to be used in a bit 

//				++i; // <-- let's get i past so we can get tag name with same logic below
//				nChar = value[i]; // get nxChar also on next, past close slash
//			}

//			bool isStartTagName = XmlConvert.IsStartNCNameChar(nChar);
//			bool isDoctypeOrComment = nChar == '!'; //<!--howdy comment--> or <!DOCTYPE html>

//			if (isDoctypeOrComment || !isStartTagName) {
//				SkipPastTagEnd();
//				continue;
//			}

//			// if here, means we had first letter of a tag name already, skip i to there;

//			////////if (i + 1 < len) {
//			////////	// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
//			////////	char n = value[i + 1];
//			////////	if (!(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
//			////////		continue;
//			////////}

//			//char n = value[i + 1];
//			//// to allow < and > symbols, ignore if next char is not a valid start tag char (simply formulated) 
//			//if (i + 1 < len && !(n.IsAsciiLetter() || n == '/') && n != '!') // '!' because of: "<!DOCTYPE..."
//			//	continue;

//			priorToTagIsWhitesp = i < 1 || value[i - 1].IsWhitespace();

//			int maxISrchForTag = i + 8;
//			int tagStartIdx = i + 1;
//			int j = tagStartIdx;
//			isCloseTag = false;

//			for (; j < maxISrchForTag && j < len; j++) {
//				char nxtChar = value[j];
//				if (!nxtChar.IsAsciiLetter()) {

//					if (nxtChar == '>')
//						break;

//					if (nxtChar == '/' && j == tagStartIdx) {
//						isCloseTag = true;
//						tagStartIdx++;
//					}
//				}
//			}

//			tagName = j > tagStartIdx
//				? value.Substring(tagStartIdx, j - tagStartIdx)
//				: null;

//			start = i;
//			currLook = '>';
//		}
//		else if (currLook == '>') {
//			//value[i + 1].IsAsciiLetter() || value[i + 1] == '/'

//			if (i > 0) {
//				char c = value[i - 1];

//				if (c == '/') { // this could be a self-closing open tag ('<br />')
//				}

//				// I'm confused what this was about, but I *think* these following conditions 
//				// represent things that COULDN'T be in a CLOSING tag, which we're expecting this is ... ??
//				else if (c != '"' && c != '\'' && !c.IsAsciiLetter())
//					continue;

//				// -- old, replaced with above
//				//if (value.PreviousIndexIsMatch(i - 1, c => c != '"' && c != '\'' && !c.IsAsciiLetter() 
//				//	? true 
//				//	: (char.IsWhiteSpace(c) ? (bool?)null : false))) // what in the WORLD was this whitespace check? which still returned null/false?!
//				//	continue;
//			}

//			prevSegLen = start - end;
//			if (prevSegLen > 0) {
//				//if (sb == null)
//				//	sb = new StringBuilder(value.Length);

//				int _start = end;
//				int _len = prevSegLen;
//				if (sb.NotNulle() && sb[sb.Length - 1].IsWhitespace() && value[_start].IsWhitespace()) {
//					_start++;
//					_len--;
//				}

//				bool padValueWithWhitespace = false;

//				if (tagName.NotNulle()) {
//					bool? noWhitespace = TagNamesToNotAddWhitespaceBetweenTags.ValueN(tagName);
//					padValueWithWhitespace = noWhitespace != true;
//				}

//				if (!isCloseTag && padValueWithWhitespace && _len > 0) { // && sb.Length > 0) {
//					if (tagName.EqualsIgnoreCase("p") || tagName.EqualsIgnoreCase("div")) {
//						sb.Append("\r\n");
//					}
//					else {
//						if (!priorToTagIsWhitesp)
//							sb.Append(' ');
//					}
//				}

//				sb.Append(value, _start, _len);

//				if (isCloseTag && padValueWithWhitespace) {
//					int nextCharIdx = i + 1;

//					if (nextCharIdx < len) {
//						char nxtChar = value[nextCharIdx];
//						if (!nxtChar.IsWhitespace() &&
//							nxtChar != '<') // IF it is an open bracket, then that will be handled later as the next tag
//							{
//							sb.Append(' ');
//						}
//					}
//				}

//				isCloseTag = false;
//			}
//			end = i + 1;
//			currLook = '<';
//		}
//	}

//	//if (sb == null) // CHANGED 2019/02/20, this was not allowing cases where only text occurred at end, after all tags
//	//	return value;

//	//if (sb == null) {
//	//	if (end == 0 && prevSegLen == value.Length)
//	//		return value;
//	//	else
//	//		sb = new StringBuilder(len);
//	//}

//	prevSegLen = len - end;

//	sb.Append(value, end, prevSegLen);

//	string result = trim ? sb.TrimToString() : sb.ToString();
//	return result;
//}
