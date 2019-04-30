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
	static class TextFuncs
	{
		public static bool IsWebLink(string s, bool checkForWww = true)
		{
			if (s != null && s.Length > 9) { //http://bit.ly/ = 14 chars, www.msn.ly
				if (checkForWww &&
					s[0] == 'w' &&
					s[1] == 'w' &&
					s[2] == 'w' &&
					s[3] == '.') {
					return true;
				}
				else if (
					s[0] == 'h' &&
					s[1] == 't' &&
					s[2] == 't' &&
					s[3] == 'p') {

					if (
						(s[4] == ':' && s[5] == '/' && s[6] == '/') ||
						(s[4] == 's' && s[5] == ':' && s[6] == '/' && s[7] == '/')) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Removes brackets of type [] and (), and trims the result afterwards
		/// if they existed. In future more options need available here, like what
		/// brackets to remove. Make sure to trim value before sending in here.
		/// The goal of this is to be very performant for cases where no bracket 
		/// exists.
		/// </summary>
		public static string RemoveBrackets(string value)
		{
			if (value.CountN() > 2) {
				if (value[0] == '(' && value[value.Length - 1] == ')') {
				}
				else if (value[0] == '[' && value[value.Length - 1] == ']') {
				}
				else
					return value;

				value = value.Substring(1, value.Length - 2).TrimIfNeeded();
			}
			return value;
		}

		static char[] _splitChar = { ',' };

		public static KeyValuePair<string, string>[] ParseQueryString(string query)
		{
			if (query.NotNulle()) {

				var queries = new List<KeyValuePair<string, string>>();

				int lastStartIndex = query[0] == '?' ? 1 : 0;

				for (int i = lastStartIndex; i < query.Length; i++) {
					if (query[i] == '&') {
						if (!query.FollowsWith("amp;", i + 1)) {
							int len = i - lastStartIndex;
							if (len > 0) {

								string q = query.Substring(lastStartIndex, len);

								// --- NOW get KV pair split on equals ---
								int eqIndex = q.IndexOf('=');

								if (eqIndex < 1) {
									// either of these means badly formed...
									if (eqIndex < 0) // NO equals, just set whole as key
										queries.Add(new KeyValuePair<string, string>(q, ""));
									else if (q.Length > 1) // no key (begins with equals); this check also just skips if q == "="
										queries.Add(new KeyValuePair<string, string>("", q.Substring(1)));
								}
								else {
									queries.Add(new KeyValuePair<string, string>(
										q.Substring(0, eqIndex),
										eqIndex == q.Length - 1 ? "" : q.Substring(eqIndex + 1)));
								}
								// --- end KV parsing ---
							}
							lastStartIndex = i + 1;
						}
					}
				}
				return queries.ToArray();
			}
			return null;
		}

		public static KeyValuePair<string, string>[] GetQueryStrings(this Uri uri)
		{
			if (uri == null)
				return null;
			string query = uri.Query;
			if (query.IsNulle())
				return null;

			return ParseQueryString(query);
		}

		/// <summary>
		/// Alters a pascal string (e.g. 'doSomethingCool') to an underscore string ('do_something_cool').
		/// </summary>
		/// <param name="val"></param>
		/// <param name="toLower">True to lowercase the final result, false to uppercase, null to return as is.</param>
		public static string PascalToUnderscoreString(string val, bool? toLower = true)
		{
			if (val.IsNulle())
				return val;

			if (val.Length < 2) {
				if (toLower == true)
					return val.ToLower();
				if (toLower == false)
					return val.ToUpper();
				return val;
			}

			StringBuilder sb = new StringBuilder(val.Length + 5);

			sb.Append(val[0]);

			char prevC = 'a';
			for (int i = 1; i < val.Length; i++) {
				char c = val[i];
				if (c.IsUpper()) {
					prevC = val[i - 1];
					if (char.IsLower(prevC) || !char.IsLetter(prevC))
						sb.Append('_');
				}
				sb.Append(c);
			}
			string v = sb.ToString();

			if (toLower == true)
				return v.ToLower();
			if (toLower == false)
				return v.ToUpper();
			return v;
		}

		/// <summary>
		/// Converts any loner "\n" carriage returns to the full "\r\n" version,
		/// but with a focus on best possible performance. NO allocations are made
		/// at all and the original string is returned if it has no loner \n's. 
		/// Otherwise is still very high performance
		/// </summary>
		/// <param name="val"></param>
		public static string ConvertNCarriageReturnsToRN(string val)
		{
			if (val == null || val.Length == 0)
				return val;

			List<int> hits = null; // *NOT* allocated until first hit (!)

			void addHit(int idx)
			{
				if (hits == null)
					hits = new List<int>();
				hits.Add(idx);
			}

			if (val[0] == '\n') // by checking this first, the loop is free to do 1 char lookbacks without bounds check :0)
				addHit(0);

			int len = val.Length;
			for (int i = 1; i < len; i++) {
				if (val[i] == '\n' && val[i - 1] != '\r')
					addHit(i);
			}

			// If hits is null, it means there were NO allocations or ANYTHING 
			// other than the single for loop, which itself checked only 1 char 
			// (and one char lookback on finding '\n's)
			if (hits == null)
				return val;

			// *** Nothing below is hit if there were no loner \n's ***

			int sbLen = val.Length + hits.Count; // perf critical: set the exact length, so internally will never grow
			StringBuilder sb = new StringBuilder(sbLen);

			int lastHitIdx = 0;

			for (int i = 0; i < hits.Count; i++) {
				int nIdx = hits[i];
				int count = nIdx - lastHitIdx; // -1 because not including the last char '\n'

				if (count > 0)
					sb.Append(val, lastHitIdx, count);

				sb.Append('\r').Append('\n'); // I'm guessing appending 2 chars is faster than 1 "\r\n" string

				lastHitIdx = nIdx + 1;
			}

			if (lastHitIdx < val.Length)
				sb.Append(val, lastHitIdx, val.Length - lastHitIdx); // append the remaining, if any

			val = sb.ToString();
			return val;
		}

		public static string ConvertNCarriageReturnsToRN_Traditional(string val)
		{
			val = val.Replace("\n", "\r\n").Replace("\r\r\n", "\r\n");
			return val;
		}

		public static bool ValidateEmail(string email, int maxLength = 254)
		{
			if (email == null || email.Length < 7 || email.Length > maxLength)
				return false;
			return _rxEmail.IsMatch(email);
		}

		/// <summary>
		/// Sources:
		/// https://stackoverflow.com/a/45177249/264031
		/// https://github.com/Microsoft/referencesource/blob/master/System.ComponentModel.DataAnnotations/DataAnnotations/EmailAddressAttribute.cs
		/// </summary>
		public static Regex GetEmailRegex(TimeSpan? matchTimeout = null)
		{
			const string emailRxPattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";

			TimeSpan _tm = matchTimeout ?? TimeSpan.FromSeconds(1);

			var rx = new Regex(
				emailRxPattern,
				RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
				matchTimeout: _tm);

			return rx;
		}

		static Regex _rxEmail = GetEmailRegex(TimeSpan.FromSeconds(1));

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
