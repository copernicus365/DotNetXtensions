using System;
using System.Collections.Generic;
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
	class HtmlTag
	{
		int pos;
		int endPos;

		/// <summary>
		/// I would like to probably refactor this later, right now, len 
		/// after init is the length of the tag from starter pointy to 1 BEFORE
		/// the end pointy, e.g. input <c><![CDATA[<p>]]></c> will have len of 
		/// 2, because final one is dropped from len: <c><![CDATA[<p]]></c>.
		/// It is a *non-negotiable* that final pointy is dropped, this greatly 
		/// helps our operations. But won't be too much work to refactor, for 
		/// consistency sake, having starter pointy dropped from that as well.
		/// Note also: This value is for internal operations purpose only,
		/// so no need for purity but rather for speed and easier code.
		/// </summary>
		private int len;
		string html;

		public string TagName { get; set; }

		public Dictionary<string, string> Attributes { get; set; }

		public int TagStartIndex { get; private set; }

		public int TagLength { get; private set; }

		public bool IsSelfClosed { get; private set; }

		public int InnerTagStartIndex => TagStartIndex + 1;

		public int InnerTagLength { get; private set; }


		// COMPUTED:

		public bool NoAtts => Attributes.IsNulle();

		bool endReached => pos > endPos;

		//bool countFromPosWouldPassEnd(int countFromPos) => pos + countFromPos > endPos;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputHtml">Input html</param>
		/// <param name="startIndex">Start position</param>
		/// <param name="findTagEnd">True (default) to
		/// have this search for the close of the (opening) tag,
		/// searching of course from <paramref name="startIndex"/>.
		/// False when the caller verifies the end of <paramref name="inputHtml"/> 
		/// is the end of the (open) tag, thereby avoiding that extra search.
		/// </param>
		public bool Parse(
			string inputHtml,
			int startIndex = 0,
			bool findTagEnd = true)
		{
			if (inputHtml == null) throw new ArgumentNullException();
			if (startIndex < 0) throw new ArgumentOutOfRangeException();

			try {
				html = inputHtml;

				if (!init(startIndex, findTagEnd))
					return false;

				if (!setTagName())
					return false;

				if (endReached)
					return true;

				if (!XmlConvert.IsWhitespaceChar(html[pos++]))
					return false;

				while (!endReached) {
					if (!addAttr())
						return false;
				}

				return true;
			}
			finally {
				html = null;
			}
		}


		bool init(int startIndex, bool findTagEnd)
		{
			if (startIndex < 0) throw new ArgumentOutOfRangeException();

			len = html.Length;
			TagStartIndex = pos = startIndex;

			if ((len - pos) < 3)
				return false;

			if (pos >= len)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			if (html[pos] != '<')
				return false;

			int endIdx = findTagEnd
				? html.IndexOf('>', TagStartIndex)
				: len - 1; // remember, we already validated above `len` is AT LEAST 3, so IS OK to step back 1

			// Why +1? Because subtracting two 0-based indexes requires +1 
			// (pos:0, endPos:2, endPos-pos == 2, but there are 3 == 0,1,2)
			TagLength = endIdx - TagStartIndex + 1;

			// Lots of redundancies in the following checks, just chill, it's fine, boosts at a glance confidence
			if (endIdx < 2
				|| endIdx <= TagStartIndex
				|| TagLength < 2
				|| html[endIdx] != '>')
				return false;

			endIdx--; // move back one to disclude in our search closing '>'

			IsSelfClosed = html[endIdx] == '/';
			if (IsSelfClosed)
				endIdx--; // disclude self-close slash as well

			endPos = endIdx; // we've done all this work on a local variable, now we'll final set it to the field
			len = endPos + 1;

			pos++; // Let's move past opening '<', don't require caller to do this, this is perfect for INIT: The start after open '<'

			// See note above when setting `TagLength`
			InnerTagLength = (endPos - pos) + 1;
			if (InnerTagLength < 1)
				return false;

			return true;
		}

		bool setTagName()
		{
			if (len < 2) // == minimum: "<p" (end pointy already removed
				return false;

			int countFromPosToNxtWS = countToNextWSOrEnd();

			int tagLen = (pos + countFromPosToNxtWS) - pos;
			if (tagLen < 1)
				return false;

			string tagNm = html.Substring(pos, tagLen);

			if (!verifyName(tagNm))
				return false;

			TagName = tagNm;

			pos += tagLen;

			return true;
		}

		bool verifyName(string name)
		{
			if (name.IsNulle())
				return false;

			// NOICE! superior validation by winnowing out probably 99% 
			// of actual html tags we'll hit
			if (name.IsAsciiAlphaNumeric(allowDash: true, allowUnderscore: true) 
				&& name[0].IsAsciiLetter()) {
				return true;
			}

			try {
				string nm = XmlConvert.VerifyName(name);
				return true;
			}
			catch {
				return false;
			}
		}

		bool addAttr()
		{
			skipWSpacesOrEnd();
			if (endReached)
				return true;

			int start = pos;

			for (; pos < len; pos++) {

				char c = html[pos];

				// is true a LOT more than "IsWhitespaceChar", so since I think IsAsciiLetterOrDigit
				// is a quicker check probably than "IsWhitespaceChar", let's do it first
				if (!c.IsAsciiLetterOrDigit() && c != '-') {

					// Check for 2 possible attr-name endings:
					if (c == '=') {
						return handleAttrNmThenValue(start, pos - start, false);
					}

					if (XmlConvert.IsWhitespaceChar(c)) {
						int skippedWSs = skipWSpacesOrEnd();
						bool isBoolTag = endReached || html[pos] != '=';

						if (skippedWSs > 0 && isBoolTag)
							pos--;

						return handleAttrNmThenValue(start, pos - start, isBoolTag);
					}

					// We did simpler ascii/number/dash check, now do full allowed name char check
					if (!XmlConvert.IsNCNameChar(c) && c != ':') {
						return false; // ERROR, invalid
					}
				}
			}

			// only get here if did NOT hit '=' NOR a whitespace,
			// so MUST be an empty attribute that ended tag, e.g. "<i is-cool/>"
			return handleAttrNmThenValue(start, pos - start, true); // IS A BOOLEAN TAG
		}

		/// <summary>
		/// Call this when you DO have an attribute name as specified in the 
		/// input <paramref name="start"/> and <paramref name="len"/> parameters.
		/// Also input if you already know this IS a boolean attribute (namely if
		/// you reached the end of the tag before hitting an '=', or if you hit whitespace
		/// and then not a '=' next). This still might be a boolean tag, we will figure that
		/// out from here.
		/// </summary>
		/// <param name="start"></param>
		/// <param name="len"></param>
		/// <param name="isBoolTag">TRUE if this attribute is not followed up by 
		/// an equal '=' sign. It might still be a boolean tag if no quotes follow the
		/// equal sign, but that will be figured out herein.</param>
		/// <returns></returns>
		bool handleAttrNmThenValue(
			int start,
			int len,
			bool isBoolTag)
		{
			int attLen = pos - start;
			if (attLen < 1)
				return false;

			string key = html.Substring(start, len).NullIfEmptyTrimmed();

			if (key.IsNulle())
				return false;

			// === VALIDATE FIRST CHAR OF KEY ===
			// INPUT validated all chars EXCEPT that the first char was restricted
			// to smaller subset of allowed start chars (e.g. can't start with number or dash)

			if (!key[0].IsAsciiLetter() // much faster check, 99.999% of time
				&& !XmlConvert.IsStartNCNameChar(key[0]))
				return false;

			char startQChar = default;

			if (!isBoolTag)
				isBoolTag = onEqualSign_SkipToAttrValue_IsBoolTag(ref startQChar);

			if (isBoolTag) {
				if (!__addAttr(key, ""))
					return false;

				pos++;
				return true;
			}

			// canNOT be a boolean attribute if reach here
			bool isNoQuotesAttr = startQChar == ' ';
			if(!isNoQuotesAttr)
				pos++;

			bool success = isNoQuotesAttr
				? getAttrValWNoQ(startQChar, out string val)
				: getAttrValWQuote(startQChar, out val);

			if(!success)
				return false;

			if (!__addAttr(key, val))
				return false;

			return true;
		}


		bool getAttrValWQuote(char qChar, out string val)
		{
			val = null;

			if (qChar != '"' && qChar != '\'')
				throw new ArgumentException();

			int countToEndC = countToCharFromCurrPos(qChar);
			if (countToEndC < 0)
				return false; // NO end-quote found, ERROR

			int vLen = countToEndC;
			val = vLen == 0 ? "" : html.Substring(pos, vLen);

			pos += vLen + 1; // +1 to get ONE PAST the close quote. Is OK if gets past end, fine

			return true;
		}

		bool getAttrValWNoQ(char qChar, out string val)
		{
			if (qChar != ' ')
				throw new ArgumentException();

			int countToEndC = countToNextWSOrEnd();
			if (countToEndC < 1) {
				// 1) `< 0` (-1) means we found no space, OK, matches: `<div attr=>`
				// 2) `== 0` matches: `<div attr= >`
				// --> they both result in the same: An empty string result
				val = "";
				return true;
			}

			int vLen = countToEndC;
			val = html.Substring(pos, vLen);

			pos += vLen; // +1 to get ONE PAST the close quote. Is OK if gets past end, fine

			return true;
		}

		/// <summary>
		/// Call this ONLY when you HAVE already hit an equals sign
		/// (exception will be thrown if curr pos is not '='). This
		/// will decide if it is still a boolean tag.
		/// </summary>
		/// <param name="startQChar"></param>
		/// <returns></returns>
		bool onEqualSign_SkipToAttrValue_IsBoolTag(ref char startQChar)
		{
			// OK, we have now finished with any not followed by '=', but
			// still have to validate quotes actually follow the =
			if (html[pos] != '=')
				throw new Exception("Invalid code, current pos must be '=' sign");

			pos++;
			if (endReached)
				return true;

			startQChar = html[pos];

			if (startQChar == '"' || startQChar == '\'')
				return false;

			int spacesSkippedAfterEQ = skipWSpacesOrEnd();
			if (endReached)
				return true;

			startQChar = html[pos];

			if (startQChar == '"' || startQChar == '\'')
				return false;

			if (spacesSkippedAfterEQ > 0) {
				// so there was an "=" sign, then WAS space, followed by not a quote,
				// meaning we have a bool-attribute
				return true; // do we need to fix 'pos'?????????????????
			}

			// WAS an '=', followed by NOT a quote NOR a space! --->
			// This looks like a QUOTELESS attribute
			startQChar = ' '; // convention: caller must know this simple space indicates ANY WS terminates
			return false;
		}

		bool __addAttr(string name, string value)
		{
			if (Attributes == null)
				Attributes = new Dictionary<string, string>(4);

			Attributes[name] = value; // OVERWRITE any earlier one
			return true;
		}


		// +++ NOTE: "The "Count" methods below do NOT alter 'pos', while the "Skip" methods actually
		// alter pos till given condition is found (or end)

		/// <summary>
		/// Returns count from current <see cref="pos"/> till we encounter
		/// a whitespace char. If 0 (or less, but should never b), then string
		/// from current pos to end never encounters another ws.
		/// </summary>
		/// <returns></returns>
		int countToNextWSOrEnd()
		{
			int i = pos;
			for (; i < len; i++) {
				if (XmlConvert.IsWhitespaceChar(html[i]))
					break;
			}
			return i - pos;
		}

		int countToCharFromCurrPos(char c)
		{
			int i = pos;
			for (; i < len; i++) {
				if (html[i] == c)
					return i - pos;
			}
			return -1;
		}

		int skipWSpacesOrEnd()
		{
			int wsCount = 0;
			for (; pos < len; pos++, wsCount++) {
				if (!XmlConvert.IsWhitespaceChar(html[pos]))
					break;
			}
			return wsCount;
		}

		int skipTillIsWSpaceOrEnd()
		{
			int wsCount = 0;
			for (; pos < len; pos++, wsCount++) {
				if (XmlConvert.IsWhitespaceChar(html[pos]))
					break;
			}
			return wsCount;
		}

	}
}
