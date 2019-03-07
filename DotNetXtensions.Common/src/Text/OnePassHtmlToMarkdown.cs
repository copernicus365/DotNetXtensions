using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

#if DNXPublic
namespace DotNetXtensions.Text
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
		int tagPointyStartIdx;
		string html;
		string tagNm;
		StringBuilder sb;
		SmallElem currSmlElm;
		BigBlockElem currBigBlk;

		bool currBlkIsBQ => currBigBlk == BigBlockElem.BLOCKQUOTE;


		public void Reset(string newHtml = null, bool doMarkdown = true)
		{
			html = newHtml;
			tagNm = null;
			len = html?.Length ?? 0;
			doMD = doMarkdown;
			i = tagPointyStartIdx = 0;
			currSmlElm = SmallElem.NONE;
			currBigBlk = BigBlockElem.NONE;
			if (sb != null) {
				sb.Clear();
				//sb = null; // do NOT set this yet!! allow Reset to be called without this
			}
		}

		public static string HtmlToMD(
			string htmlText,
			bool onlyCleanHtmlTags = false,
			bool htmlDecode = false,
			bool checkAndReturnIfNoXmlTags = false)
		{
			if (htmlText.IsNulle())
				return null;

			var htmlMd = new OnePassHtmlToMarkdown();
			return htmlMd.ConvertHtmlToMD(
				htmlText,
				onlyCleanHtmlTags,
				htmlDecode,
				checkAndReturnIfNoXmlTags);
		}

		public string ConvertHtmlToMD(
			string htmlText,
			bool onlyCleanHtmlTags = false,
			bool htmlDecode = false,
			bool checkAndReturnIfNoXmlTags = false)
		{
			html = htmlText;

			if (html.IsNulle())
				return html;

			if (htmlDecode)
				html = System.Net.WebUtility.HtmlDecode(html);

			if (checkAndReturnIfNoXmlTags && !XmlTextFuncs.StringContainsAnyXmlTagsQuick(html)) {
				html = html.TrimIfNeeded(); // have to trim now as we're immed returning
				return html;
			}

			// be careful! don't set len field etc in Reset till AFTER htmlDecode above which changes the string potentially 
			Reset(htmlText, !onlyCleanHtmlTags);

			if (sb == null) // otherwise was already `Clear`ed above
				sb = new StringBuilder(len);

			// if we init with a space so we won't have to always check if sb is empty!!!!!!!
			// !!!DANGER!!!: we can't EVER allow cutting sb.Length when it's immedly adding to it
			sb.Append(" ");

			char nChar;

			for (; i < len; i++) {

				nChar = html[i];

				// WHITESPACE handling complexities (culling most), and looking for an open '<'

				if (nChar > ' ') {
					if (nChar != '<') {
						// so is NOT ws unless is nbsp (160), which always gets written anyways

						if (nChar < 97) { 
							// 'a' = 97, so at least it removes the most prevalent: lower case chars

							// at this time, no use making a Dictionary, surely at least while limited to only 3 chars, 
							// a Dict lookup couldn't possibly be more performant
							switch (nChar) {
								case '*': //42
								case '_': //95
								case '`': //96 
									if(doMD) // prioritizing doMD==true perf wise, othewise wld do this check earlier every time
										sb.Append('\\');
									break;
							}
						}

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
				else {
					if (handleCloseTag())
						continue;
				}

				if (whiteSpaceSandwish())
					i++;
			}

			string result = sb.TrimToString();

			if (htmlDecode)
				result = System.Net.WebUtility.HtmlDecode(result);

			Reset();
			return result;
		}

		bool handleOpenTag()
		{
			switch (tagNm) {
				case "div":
					currSmlElm = SmallElem.NONE;
					insertNewLinesIfNeededFacingBack(2, true);
					return true;

				case "p": {
					currSmlElm = SmallElem.P;
					insertNewLinesIfNeededFacingBack(2, true);
					return true;
				}
				case "br": {
					// shouldn't need more than 1 remove. remember, only 1 ws would have been added anyway
					removePrevSpaceIfExists();
					sb.Append("  \r\n");

					AfterNewLineHandleInBigBlockTagNLStuff();

					return true;
				}

				case "hr": {
					currSmlElm = SmallElem.NONE;
					insertNewLinesIfNeededFacingBack(1, true);

					if (doMD) {
						sb.Append(currBlkIsBQ ? "\r\n> " : "\r\n");

						sb.Append("* * *"); // "- - - - - - - - - - - -");

						sb.Append(currBlkIsBQ ? "\r\n> " : "\r\n");
					}

					return true;
				}

				case "ul": {
					setCurrBigBlk(BigBlockElem.UL);
					insertNewLinesIfNeededFacingBack(2, false);
					return true;
				}
				case "ol": {
					setCurrBigBlk(BigBlockElem.OL);
					insertNewLinesIfNeededFacingBack(2, false);
					return true;
				}
				case "li":

					currSmlElm = SmallElem.LI;				
					insertNewLinesIfNeededFacingBack(1, chkBlkQuote: false);

					if (doMD)
						sb.Append("*   ");

					return true;

				case "blockquote":

					setCurrBigBlk(BigBlockElem.BLOCKQUOTE);

					string nextElemAsOpenTagNm = peekNextNonWSIsOpenTag();

					if (nextElemAsOpenTagNm.NotNulle()) {
						var d = _closeTagsDict1.V(nextElemAsOpenTagNm);
						if (d != null && d.BQ) {
							insertNewLinesIfNeededFacingBack(1, false);
							return true;
						}
					}

					insertNewLinesIfNeededFacingBack(2, false);
					sb.Append("> ");

					return true;

				case "a":

					if (doMD) {
						if (__getAnchor(html, tagPointyStartIdx, out string anchorText, out string aHref, out int afterAIdx)) {
							i = afterAIdx - 1; // afterAIdx is 1 char AFTER the closing '>' (of "</a>"), go one back bec next for loop will iterate i
							sb.Append($"[{anchorText}]({aHref})");
							return true;
						}
					}
					return false;

				default: {
					// headers h1, h2, etc
					if (tagNm[0] == 'h') {
						int hCnt = tagNmIsHeader();

						if (hCnt > 0) {
							currSmlElm = SmallElem.H;
							insertNewLinesIfNeededFacingBack(2, true);

							if (doMD) {
								for (int k = 0; k < hCnt; k++)
									sb.Append('#');
								sb.Append(' ');
							}

							return true;
						}
					}
					break;
				}
			}

			if (!sbEmpty() && !sbLast().IsWhitespace())
				sb.Append(" ");

			return false;
		}

		bool handleCloseTag()
		{
			if (_closeTagsDict1.TryGetValue(tagNm, out tagInfo1 obj)) {

				currSmlElm = SmallElem.NONE;

				if (obj.IsBigBlockElem)
					currBigBlk = BigBlockElem.NONE;

				int numLines = obj.NumberLines;

				if (obj.BQ && tagNm == "blockquote") {
					if (previousIsNewLineIgnoreWS(1) < 1)
						sb.Append("\r\n");
				}
				else {
					insertNewLinesIfNeededFacingBack(numLines, obj.BQ, isClose: true);
				}

				return true;
			}

			if (i + 1 < len && !html[i + 1].IsWhitespace())
				sb.Append(" ");

			return false;
		}

		string getTagName(ref char currChar)
		{
			// ok, we're at least a good start-tag char, now let's get the rest of the following name...

			int startTagIdx = i;
			int endTagIdx = i;

			for (; i < len; i++) {
				currChar = html[i];

				if (currChar == '>')
					break;
				else if (XmlConvert.IsNCNameChar(currChar) || currChar == ':') {

					// NOTE! Is only if that does not skip past tag end (
					// `== ':'` -- IsNCNameChar leaves off namespace `:` colon, let's allow it
					endTagIdx++;
				}
				else if (
					currChar == '/' // only if no space after the tag name, e.g. "<br/>" vs "<br />"
					||
					XmlConvert.IsWhitespaceChar(currChar)) {
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

		int tagNmIsHeader()
		{
			if (tagNm[0] == 'h' && tagNm.Length == 2 && tagNm[1].IsAsciiDigit()) {

				int hCnt = tagNm[1].ToInt(); //tagNm[1].ToString().ToInt(-1);

				if (hCnt >= 1 && hCnt <= 6)
					return hCnt;
			}
			return 0;
		}

		/// <summary>Setting this way clears <see cref="currSmlElm"/> at the same time.</summary>
		void setCurrBigBlk(BigBlockElem val)
		{
			currSmlElm = SmallElem.NONE;
			currBigBlk = val;
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

		/// <summary>
		/// We can check without .Length check bec sb was inited with one space char!
		/// </summary>
		char sbLast() => sb[sb.Length - 1];

		bool removePrevSpaceIfExists()
		{
			// IMPORTANT! Do NOT remove the very first space! so have to check 
			// `Length > 1` instead of `Length > 0`
			if (sb.Length > 1 && sbLast() == ' ') {
				sb.Length--;
				return true;
			}
			return false;
		}

		bool prevIsBlockQuoteNewLine()
		{
			// Note: we're not worred about the first line, is never when this is used, 
			// so only testing minimum of: "\r\n> "
			if (sb.Length >= 4) {
				int sbLen = sb.Length;
				bool isMatch = sb[sbLen - 1] == ' ' && sb[sbLen - 2] == '>' && sb[sbLen - 3] == '\n' && sb[sbLen - 4] == '\r';

				return isMatch;
			}
			return false;
		}

		void newSingleLine_RemovePrevSpace()
		{
			removePrevSpaceIfExists();
			sb.Append("\r\n");
		}

		void newDoubleLine_RemovePrevSpace()
		{
			removePrevSpaceIfExists();
			sb.Append("\r\n\r\n");
		}

		void AfterNewLineHandleInBigBlockTagNLStuff()
		{
			if (doMD) {
				switch (currBigBlk) {
					case BigBlockElem.BLOCKQUOTE:
						sb.Append("> ");
						break;
						// ... may have others here in the future as well
				}
			}
		}

		bool sbEmpty() => sb.Length == 0;

		bool readyForExtraWS() => sb.Length > 0 && !sbLast().IsWhitespace();

		bool previousIsNewLine() => sbLast() == '\n';


		int moveForwardTillNotWS()
		{
			int countMoved = 0;
			for (int j = i + 1; j < len; j++) {
				if (!html[j].IsWhitespace())
					break;
				countMoved++;
			}
			if (countMoved > 0)
				i += countMoved;
			return countMoved;
		}

		string peekNextNonWSIsOpenTag()
		{
			int origI = i;

			int cntMvd = moveForwardTillNotWS();

			if (i + 3 >= len // '3' - smallest full tag (<p>), this allows us to remove a bunch of annoying checks below
				|| html[i + 1] != '<'
				|| html[i + 2] == '/') { // exclude closing tags, we're only looking for opening
				return null;
			}

			i += 2;
			char currChar = html[i]; // i+1 == '<'

			// SO: above validated that 
			// 1) there's enough space ahead to check a few
			// 2) then it starts with < , which itself is not followed with / (close tag)
			string nxtTag = getTagName(ref currChar);

			i = origI;
			return nxtTag;
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

		void insertNewLinesIfNeededFacingBack(int count, bool chkBlkQuote, bool isClose = false)
		{
			if (count < 1)
				return;
			if (count > 2) throw new ArgumentOutOfRangeException();

			int prevNLinesCnt = previousIsNewLineIgnoreWS(count);

			if (chkBlkQuote && currBlkIsBQ) {

				bool prevIsBQNewLine = prevIsBlockQuoteNewLine();

				if (prevNLinesCnt > 1) {
					sb.Append("> ");
					return;
				}

				if (prevIsBQNewLine || prevNLinesCnt > 0)
					count--;
				else
					removePrevSpaceIfExists();

				if (isClose) {
					if (!prevIsBQNewLine)
						sb.Append("\r\n> ");
				}
				else {
					if (count == 1)
						sb.Append("\r\n> ");
					else if (count == 2)
						sb.Append("\r\n> \r\n> ");
					else {
						for (int k = 0; k < count; k++)
							sb.Append("\r\n> ");
					}
				}
			}
			else {
				if(!(isClose && currBlkIsBQ))
					removePrevSpaceIfExists();

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
			}
			moveForwardTillNotWS();
		}

		int previousIsNewLineIgnoreWS(int countNewLinesUpTo, int reportStartOfContentCountAs = 2)
		{
			if (sb.Length < 2) // was: `if(sb.Length < 1)`, but now first char is added ' ' space
				return reportStartOfContentCountAs;

			int newLines = 0;

			for (int j = sb.Length - 1; j >= 0; j--) {
				if (!IsWhitespaceMinusNBSP(sb[j]))
					return newLines;

				char c = sb[j];
				if (c == '\n') {
					if (++newLines >= countNewLinesUpTo)
						return newLines;
				}
			}
			return newLines;
		}

		public static bool IsWhitespaceMinusNBSP(char c)
			=> char.IsWhiteSpace(c) && c != NBSPChar;
		
		const char NBSPChar = (char)160; // '\x00a0'

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

		/// <summary>
		/// By "Big" we mean NOT to include paragraph (`p`) and headers (`h1` etc),
		/// but only container block elements that typically *contain* among other things
		/// paragraphs, headers, and so forth. The reason we need to use this would be
		/// ruined if we allowed P, H, etc.
		/// </summary>
		enum BigBlockElem
		{
			NONE = 0,
			UL,
			OL,
			BLOCKQUOTE,
			PRE,
		}

		enum SmallElem
		{
			NONE = 0,
			P,
			H,
			LI,
		}

		class tagInfo1
		{
			public tagInfo1(bool isBig, int numLines)
			{
				IsBigBlockElem = isBig;
				NumberLines = numLines;
			}

			public bool IsBigBlockElem { get; set; }
			public int NumberLines { get; set; }
			public bool BQ { get; set; }
		}

		static tagInfo1 getti(bool isBig, int numLines, bool bq)
			=> new tagInfo1(isBig, numLines) { BQ = bq };

		static Dictionary<string, tagInfo1> _closeTagsDict1 = new Dictionary<string, tagInfo1>() {
			{ "p", getti(false, 2, true) },
			{ "h1", getti(false, 2, true) },
			{ "h2", getti(false, 2, true) },
			{ "h3", getti(false, 2, true) },
			{ "h4", getti(false, 2, true) },
			{ "h5", getti(false, 2, true) },
			{ "h6", getti(false, 2, true) },
			{ "div", getti(false, 2, true) },
			{ "li", getti(false, 1, false) },
			{ "ul", getti(true, 2, false) },
			{ "ol", getti(true, 2, false) },
			{ "blockquote", getti(true, 2, true) },
			{ "pre", getti(true, 2, false) },
		};

	}
}
