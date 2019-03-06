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
	class HtmlTag
	{
		public string TagName { get; set; }

		public Dictionary<string, string> Attributes { get; set; }

		int pos;
		int endPos;
		int len;
		string tag;


		public bool IsSelfClosed { get; private set; }

		public bool NoAtts => Attributes.IsNulle();

		bool endReached => pos > endPos;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="xmlTag"></param>
		/// <param name="inputTagIsVerifiedFullOpenTag">True if caller verifies
		/// this is a full open tag (not more or less), which CAN also be a self-closing tag.
		/// When true, it avoids a scan to find the first end pointy, which should be the last
		/// char. When FALSE, allows caller to only know the start</param>
		/// <returns></returns>
		public bool Parse(string xmlTag, bool inputTagIsVerifiedFullOpenTag = false)
		{
			try {
				pos = 0;
				tag = xmlTag;
				len = tag?.Length ?? 0;

				if (!initBeginningEnd(inputTagIsVerifiedFullOpenTag))
					return false;

				if (!setTagName())
					return false;

				if (endReached)
					return true;

				if (!XmlConvert.IsWhitespaceChar(tag[pos++]))
					return false;

				while (!endReached) {
					(bool error, bool attrAdded) = addAttr();
					if(error)
						return false;
					if (!attrAdded) {
						// ?
					}
				}

				return true;
			}
			finally {
				tag = null;
			}
		}

		(bool error, bool attrAdded) addAttr()
		{
			int start = pos;
			char c;

			for (; pos < len; pos++) {

				c = tag[pos];

				// is true a LOT more than "IsWhitespaceChar", so since I think IsAsciiLetterOrDigit
				// is a quicker check probably than "IsWhitespaceChar", let's do it first
				if (!c.IsAsciiLetterOrDigit() && c != '-') {

					// Check for 2 possible attr-name endings:
					if (c == '=') {
						return handleAttrNmThenValue(pos, pos - start, false);
					}

					if (XmlConvert.IsWhitespaceChar(c)) {
						int skippedWSs = skipWSpaces();
						bool isOrphanTag = endReached || tag[pos] != '=';
						return handleAttrNmThenValue(pos, pos - start, isOrphanTag);
					}

					// We did simpler ascii/number/dash check, now do full allowed name char check
					if (!XmlConvert.IsNCNameChar(c) && c != ':') {
						return (true, false); // ERROR, invalid
					}
				}
			}

			// only get here if did NOT hit '=' NOR a whitespace,
			// so MUST be an empty attribute that ended tag, e.g. "<i is-cool/>"
			return handleAttrNmThenValue(pos, pos - start, true); // IS AN ORPHAN TAG
		}

		(bool error, bool attrAdded) handleAttrNmThenValue(
			int start, 
			int len, 
			bool isAnOrphanTag)
		{
			int attLen = start - pos;
			if (attLen < 1)
				return (true, false);

			string key = tag.Substring(start, len).NullIfEmptyTrimmed();

			if (key.IsNulle())
				return (true, false);

			// === VALIDATE FIRST CHAR OF KEY ===
			// INPUT validated all chars EXCEPT that the first char was restricted
			// to smaller subset of allowed start chars (e.g. can't start with number of dash)

			if (!key[0].IsAsciiLetter() // much faster check, 99.999% of time
				&& !XmlConvert.IsStartNCNameChar(key[0]))
				return (true, false);

			if (isAnOrphanTag) {
				if (!__addAttr(key, null))
					return (true, false);

				pos++;
				return (false, true);
			}

			if (tag[pos] != '=')
				throw new Exception("Invalid code, current pos must be '=' sign");

			bool gotPassedEQSign = false;

			if (!endReached) {

				gotPassedEQSign = tag[pos] == '=';
				if (gotPassedEQSign)
					pos++;
				else {
					int skippedWSs = skipWSpaces();
					if (!endReached)
						gotPassedEQSign = tag[pos] == '=';
				}

				if (endReached) {
					// does html allow hanging eq with no quotes?: `<tag some-attr= />`?
					// we could return error if it doesn't, OR, we could ignore the error, probably yes

					if (tag[pos] != '"') {
						skipWSpaces();
					}
				}
			}



			return (true, false);
		}

		bool __addAttr(string name, string value)
		{
			if (Attributes == null)
				Attributes = new Dictionary<string, string>(4);

			Attributes[name] = value; // OVERWRITE any earlier one
			return true;
		}

		bool setAttributes()
		{
			for (; pos < len; pos++) {

				// MUST be a ws to start even for first loop; cleaner expectations this way. Otherwise, 
				// previous char could be not ws, making this an illegitmate start...
				if (!XmlConvert.IsWhitespaceChar(tag[pos])) {
					// `endReached` can't be true yet, we just checked it in loop condition
					return false;
				}

				pos++;


			}

			return true;
		}

		bool setNextAttr()
		{
			// FIRST: Let's be super performant, very simply:
			// VAST majority of times: Is simple space, followed by simple ascii-letter
			// If we match that, we're at the attr, ELSE, do slower way with lots of loops and so forth
			if (pos + 2 < len && tag[pos] == ' ' && tag[pos + 1].IsAsciiLetter()) {
				pos += 2;
			}
			else {
				int wsCnt = this.countToNextWS();
				if (wsCnt < 1)
					return false;

				pos += wsCnt;
				if (endReached || !XmlConvert.IsStartNCNameChar(tag[pos]))
					return false;
				pos++;
			}

			int startAttr = pos - 1;

			pos++;

			return false;
		}


		bool initBeginningEnd(bool inputTagIsVerifiedFullOpenTag)
		{
			if (len < 3)
				return false;

			if (tag[0] != '<')
				return false;

			// ok COOL, this takes a perf hit of maybe somewhat needlessly establishing
			// the end of the tag up front, when perf could have just done a 1 forward
			// search that also was always seeing if the end was hit. HOWEVER,
			// that itself has it's own perf hit, of ALWAYS having to test if we 
			// are out of the open tag yet. Especially hard is it had to be aware not
			// just of '>' but also of '/' for "/>". Now we can be unembumbered, we're safely in bounds
			int endIdx = inputTagIsVerifiedFullOpenTag
				? tag.Length - 1
				: tag.IndexOf('>');

			if (endIdx < 2 || tag[endIdx] != '>')
				return false;

			if (tag[endIdx - 1] == '/')
				IsSelfClosed = true;

			len = IsSelfClosed
				? endIdx - 1
				: endIdx;

			// we've now cut off the end ">" or "/>", so if 1 char tag name 
			// (e.g. "<p>"), the MINIMUM length now is 2: "<p"
			if (len < 2)
				return false;

			endPos = len - 1;

			return true;
		}

		bool setTagName()
		{
			pos = 1;

			if (len < 2) // redundant but just check again
				return false;

			int countFromPosToNxtWS = countToNextWS();

			int tagLen = (pos + countFromPosToNxtWS) - pos;

			if (tagLen < 1)
				return false;

			string tagNm = tag.Substring(1, tagLen);

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

			// NOICE! vastly superior validation by winnowing out probably 99.9999% 
			// of actual html tags we'll hit
			if (name.IsAsciiAlphaNumeric() && name[0].IsAsciiLetter()) {
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

		/// <summary>
		/// Returns count from current <see cref="pos"/> till we encounter
		/// a whitespace char. If 0 (or less, but should never b), then string
		/// from current pos to end never encounters another ws.
		/// </summary>
		/// <returns></returns>
		int countToNextWS()
		{
			int i = pos;
			for (; i < len; i++) {
				if (XmlConvert.IsWhitespaceChar(tag[i]))
					break;
			}
			//if (i <= pos)
			//	return -1;

			return i - pos;
		}

		int skipWSpaces()
		{
			int wsCount = 0;
			for (; pos < len; pos++, wsCount++) {
				if (XmlConvert.IsWhitespaceChar(tag[pos]))
					break;
			}
			return wsCount;
		}

		string getNext()
		{
			if (pos >= len)
				return null;

			int start = pos;

			if (XmlConvert.IsStartNCNameChar(tag[pos]))
				;

			while (pos < len) {
				char c = tag[pos];

				if (c.IsAsciiLetter()) // this happens SO often, we want to 
					continue;

				if (c == ' ' // likewise, this happens SO often after above failed
					|| XmlConvert.IsWhitespaceChar(c))
					break;

				//if (XmlConvert
			}
			return null;
		}
	}
}
