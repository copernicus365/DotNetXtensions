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
	class OnePassHtmlToMarkdown
	{
		bool doMD;
		int i;
		int len;
		string html;
		StringBuilder sb;
		int tagPointyStartIdx;
		int openedBlockQuoteSbIdx;
		bool inBlockquote = false;
		string tagNm;

		public static string HtmlToMD(string value, bool justCleanHtmlTags = false, bool htmlDecode = false)
		{
			if (value.IsNulle())
				return null;

			var htmlMd = new OnePassHtmlToMarkdown();
			return htmlMd.ConvertHtmlToMD(
				value,
				justCleanHtmlTags,
				htmlDecode);
		}

		public string ConvertHtmlToMD(
			string htmlText,
			bool onlyCleanHtmlTags = false,
			bool htmlDecode = false)
		{
			doMD = !onlyCleanHtmlTags;
			html = htmlText;

			if (html.IsNulle())
				return html;

			if (htmlDecode)
				html = System.Net.WebUtility.HtmlDecode(html);

			if (!XmlTextFuncs.StringContainsAnyXmlTagsQuick(html)) {
				html = html.TrimIfNeeded(); // have to trim now as we're immed returning
				return html;
			}

			i = 0;
			len = html.Length;
			char nChar;
			sb = new StringBuilder(len);
			openedBlockQuoteSbIdx = -1;
			//const char nbsp = (char)160; // '\x00a0'

			for (; i < len; i++) {

				nChar = html[i];

				if (nChar > ' ') {
					if (nChar != '<') {
						// so is NOT ws unless is nbsp (160), which always gets written anyways
						sb.Append(nChar);
						continue;
					}
					// Thus nChar is '<': FALL THROUGH
				}
				else {
					// The WS logic below taken directly from framework's (https://github.com/dotnet/coreclr/blob/master/src/System.Private.CoreLib/shared/System/Char.cs)
					// but with some less checks or ones not needed
					if (nChar == ' '
						|| (nChar >= '\x0009'
						&& nChar <= '\x000d')) { //(uint)(nChar - '\x0009') <= ('\x000d' - '\x0009')) { // || nChar == nbsp) {

						if (readyForExtraWS())
							sb.Append(' '); // do NOT append anything but space! new lines etc, only get a space! html...
					}
					else {
						// Hmmm, are there any <32 characters non-ws checked in the if above that would even be valid here?
						// for now just still append it
						sb.Append(nChar);
					}
					continue;
				}

				if (i + 2 >= len) // ??????
					break;

				tagPointyStartIdx = ++i;
				nChar = html[i];
				bool isEndTag = nChar == '/'; // save that this IS an end tag, to be used in a bit 

				if (isEndTag) {
					++i; // <-- let's get i past so we can get tag name with same logic below
					nChar = html[i]; // get nxChar also on next, past close slash
				}

				bool isStartTagName = XmlConvert.IsStartNCNameChar(nChar);
				bool isDoctypeOrComment = nChar == '!'; //<!--howdy comment--> or <!DOCTYPE html>

				if (isDoctypeOrComment || !isStartTagName) {
					skipPastTagEnd();

					if (whiteSpaceSandwish())
						i++;
					continue;
				}

				tagNm = getTagName(ref nChar);

				if (tagNm.IsNulle())
					continue;

				if (handleNoWSTags())
					continue;

				if (!isEndTag) {
					if (handleOpenTag())
						continue;
				}
				else { // IS END-tag
					if (tagNm == "blockquote" && sb.Length > openedBlockQuoteSbIdx) {

						string cut = sb.ToString(openedBlockQuoteSbIdx);
						sb.Length = openedBlockQuoteSbIdx;
						int _len1 = cut.Length;

						if (cut[0] == '\r')
							sb.Append("> ");

						for (int k = 2; k < _len1; k++) {
							if (cut[k] == '\r') {
								if (cut[k - 2] != '>') {
									sb.Append("> ");
								}
								k += 4; // "\r\n> "
								continue;
							}
							sb.Append(cut[k]);
						}

						openedBlockQuoteSbIdx = -1;
					}

					if (i + 1 < len && !html[i + 1].IsWhitespace())
						sb.Append(" ");
				}

				if (whiteSpaceSandwish())
					i++;
			}

			string result = sb.TrimToString();

			if (htmlDecode)
				result = System.Net.WebUtility.HtmlDecode(result);

			return result;
		}

		bool handleOpenTag()
		{
			// headers h1, h2, etc
			if (tagNm[0] == 'h' && tagNm.Length == 2 && tagNm[1].IsAsciiDigit()) {
				int hCnt = tagNm[1].ToInt(); //tagNm[1].ToString().ToInt(-1);
				if (hCnt < 1 || hCnt > 6)
					return true;

				sb.Append("\r\n\r\n");

				//insertNewLinesIfNeededFacingBack(hCnt < 3 ? 2 : 1);

				for (int k = 0; k < hCnt; k++)
					sb.Append('#');

				return true;
			}

			switch (tagNm) {
				case "p": {
					sb.Append("\r\n\r\n");

					if (inBlockquote)
						sb.Append("> ");

					//insertNewLinesIfNeededFacingBack(2);
					return true;
				}
				case "br": {
				
					sb.Append("\r\n");

					if (inBlockquote)
						sb.Append("> ");

					//insertNewLinesIfNeededFacingBack(1);
					return true;
				}
				case "li":
					if (doMD)
						sb.Append("\r\n");
					else
						sb.Append("\r\n* ");

					return true;
					//moveForwardTillNotWS();

					//if (!previousIsNewLine())
					//	sb.Append("\r\n");

					//if (doMD)
					//	sb.Append("* ");

					//moveForwardTillNotWS();

					//return true;

				case "blockquote":

					inBlockquote = true;

					// HMMMMMMMMM! Going to have to look ahead here.
					if (!previousIsNewLine())
						sb.Append("\r\n");

					if (doMD)
						sb.Append("> ");

					moveForwardTillNotWS();
					openedBlockQuoteSbIdx = sb.Length;

					return true;

				case "a":
					// Should NOT have reached here if `convertWithMinimalMarkdown` wasn't already true
					if (doMD) {
						if (__getAnchor(html, tagPointyStartIdx, out string anchorText, out string aHref, out int afterAIdx)) {
							i = afterAIdx - 1; // afterAIdx is 1 char AFTER the closing '>' (of "</a>"), go one back bec next for loop will iterate i
							sb.Append($"[{anchorText}]({aHref})");
							return true;
						}
					}
					// Now THIS would be a lot of work, but it wld pay big dividend!
					return false; //break;
			}

			if (!sbEmpty() && !sbLast().IsWhitespace())
				sb.Append(" ");

			return false;
		}

		string getTagName(ref char nChar)
		{
			// ok, we're at least a good start-tag char, now let's get the rest of the following name...

			int startTagIdx = i;
			int endTagIdx = i;

			for (; i < len; i++) {
				nChar = html[i];

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
					skipPastTagEnd();
					break;
				}
				else {
					// hmmm, did we hit an invalid name char???
					skipPastTagEnd();

					return null; // goto CONTINUE_MAIN_LOOP;
				}
			}

			if (endTagIdx <= startTagIdx)
				return null; // continue;

			string tagNm = html
				.Substring(startTagIdx, endTagIdx - startTagIdx)
				.ToLower();

			return tagNm;
		}

		/// <summary>
		/// Returns true if current tag is a no-whitespace adding type,
		/// and handles adding replacement md value if any. True return
		/// means to immediately continue the loop, else continue falling through.
		/// </summary>
		bool handleNoWSTags()
		{
			string noWSTagReplaceVal = TagNamesToNotAddWhitespaceBetweenTags.V(tagNm);

			if (noWSTagReplaceVal != null) {

				//if (noW != "" && convertWithMinimalMarkdown)
				//	sb.Append(noW);
				//continue;

				if (tagNm == "a" && doMD)
					return false; // just let it fall through, anchor is only handled here if NOT doing MD
				else {
					if (doMD && noWSTagReplaceVal != "")
						sb.Append(noWSTagReplaceVal);
					return true; //continue;
				}
			}
			return false;
		}

		char sbLast() => sb.Length == 0 ? default(char) : sb[sb.Length - 1];
		char sbLastX() => sb[sb.Length - 1];

		bool sbEmpty() => sb.Length == 0;

		bool readyForExtraWS() => sb.Length > 0 && !sbLastX().IsWhitespace();

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

		int moveForwardTillNotWS()
		{
			int countMoved = 0;
			for (int j = i; j < len; j++) {
				if (!html[j].IsWhitespace())
					break;
				countMoved++;
			}
			if (countMoved > 0)
				i += countMoved;
			return countMoved;
		}

		bool whiteSpaceSandwish() =>
			!sbEmpty()
			&& (i + 1 < len)
			&& sbLast().IsWhitespace()
			&& sbLast() == html[i + 1];

		void skipPastTagEnd()
		{
			while (html[i] != '>' && i < len)
				i++;
		}

		void insertNewLinesIfNeededFacingBack(int count)
		{
			if (count < 1)
				return;

			int prevNLinesCnt = previousIsNewLineIgnoreWS(count);
			int add = count - prevNLinesCnt;

			if (add > 0) {
				if (add == 1)
					sb.Append("\r\n");
				else if (add == 2)
					sb.Append("\r\n\r\n");
				else {
					for (int k = 0; k < add; k++)
						sb.Append("\r\n");
				}
			}
			moveForwardTillNotWS();
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



		static Dictionary<string, string> TagNamesToNotAddWhitespaceBetweenTags = new Dictionary<string, string>() {
			{ "b", "**" },
			{ "strong", "**" },
			{ "em", "*" },
			{ "i", "*" },
			{ "u", "_" },
			{ "span", "" },
			{ "a", "" },
		};

		//public static string HtmlToMD(string value, bool justCleanHtmlTags = false)
		//{
		//	var htmlMd = new OnePassHtmlToMarkdown();
		//	return htmlMd.ConvertHtmlToMD(
		//		value,
		//		justCleanHtmlTags);
		//}

	}
}
