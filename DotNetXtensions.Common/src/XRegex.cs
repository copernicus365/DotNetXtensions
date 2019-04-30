using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetXtensions
{
	/// <summary>
	/// Regular Expression extension methods.
	/// </summary>
#if !DNXPrivate
	public
#endif
	static class XRegex
	{

		#region FindAllRx

		public static string[] FindAllRx(this string s, string pattern)
		{
			return FindAllRx(s, pattern, RegexOptions.None);
		}

		public static string[] FindAllRx(this string s, string pattern, RegexOptions rxOptions)
		{
			if(s == null) throw new ArgumentNullException("s");
			if(pattern == null) throw new ArgumentNullException("pattern");

			MatchCollection mc = Regex.Matches(s, pattern, rxOptions);
			int len = mc.Count;
			string[] values = new string[len];

			for(int i = 0; i < len; i++)
				values[i] = mc[i].Value;

			return values;
		}

		#endregion

		public static IEnumerable<Match> ToEnumerable(this MatchCollection mc)
		{
			if (mc != null) {
				foreach (Match m in mc)
					yield return m;
			}
		}

		#region GetRxPattern

		static public string GetRxPattern(RxPattern rxPattern)
		{
			return GetRxPattern(rxPattern, null);
		}

		static public string GetRxPattern(RxPattern rxPattern, bool? soloMatch)
		{
			switch(rxPattern) {
				case RxPattern.Email:
					string email = @"\w+([-.+']\w+)*@\w+([.-]\w+)*\.\w{2,6}";
					if(soloMatch == null) return email;
					if((bool)soloMatch) return string.Format(@"^{0}$", email);
					else return string.Format(@"\b{0}\b", email);

				case RxPattern.PhoneUS:
					string phone = @"((\(\d{3}\)[\s]?)|(\d{3}[\s-]))\d{3}[\s-]\d{4}";
					if(soloMatch == null) return phone;
					if((bool)soloMatch) return string.Format(@"^{0}$", phone);
					else return string.Format(@"(?<=[^0-9a-zA-Z]|^){0}\b", phone);  // notes

				case RxPattern.SocSecNumUS:
					string socsecnum = @"\d{3}-\d{2}-\d{4}";
					if(soloMatch == null) return socsecnum;
					if((bool)soloMatch) return string.Format(@"^{0}$", socsecnum);
					else return string.Format(@"\b{0}\b", socsecnum);

				case RxPattern.WebURL:
					string url = @"(http(s)?://)([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
					if(soloMatch == null) return url;
					if((bool)soloMatch) return string.Format(@"^{0}$", url);
					else return string.Format(@"\b{0}\b", url);

				case RxPattern.ZIPCodeUS:
					string zip = @"\d{5}(-\d{4})?";
					if(soloMatch == null) return zip;
					if((bool)soloMatch) return string.Format(@"^{0}$", zip);
					else return string.Format(@"\b{0}\b", zip);

				default:
					throw new ArgumentException();

					// for phone, we need this beast: (?<=[^0-9a-zA-Z]|^), a look ahead, because if there is a 
					// parentheses with the area code, the \b seems to go awol and the first paren 
					// does not become part of the match.  This *may* be an error in the .NET Regex
					// engine's handling of \b.  That's a highly complex area though (e.g. note that specified
					// word breaks (\b) do *not* become part of matches, opposite almost everything else)
			}
		}

		#endregion

		#region IndexOfRx / LastIndexOfRx

		private static int __IndexOfRx(string s, string regexPattern, RegexOptions rxOptions)
		{
			if(s == null) throw new ArgumentNullException("s");
			if(regexPattern == null) throw new ArgumentNullException("regexPattern");

			Match m = Regex.Match(s, regexPattern, rxOptions);

			if(m.Success)
				return m.Index;
			else
				return -1;
		}

		//  ===+++===+++===

		public static int IndexOfRx(this string s, string regexPattern)
		{
			return __IndexOfRx(s, regexPattern, RegexOptions.None);
		}

		public static int IndexOfRx(this string s, string regexPattern, RegexOptions rxOptions)
		{
			return __IndexOfRx(s, regexPattern, rxOptions);
		}

		public static int LastIndexOfRx(this string s, string regexPattern)
		{
			return __IndexOfRx(s, regexPattern, RegexOptions.RightToLeft);
		}

		public static int LastIndexOfRx(this string s, string regexPattern, RegexOptions rxOptions)
		{
			return __IndexOfRx(s, regexPattern, rxOptions | RegexOptions.RightToLeft);
		}

		#endregion

		#region IndexOfAllRx

		private static int[] __IndexOfAllRx(string s, string regexPattern, RegexOptions rxOptions)
		{
			if(s == null) throw new ArgumentNullException("s");
			if(regexPattern == null) throw new ArgumentNullException("regexPattern");

			MatchCollection mc = Regex.Matches(s, regexPattern, rxOptions);

			int count = mc.Count;
			int[] finds = new int[count];

			for(int i = 0; i < count; i++)
				finds[i] = mc[i].Index;

			return finds;
		}

		// ===+++===+++===

		public static int[] IndexOfAllRx(this string s, string regexPattern)
		{
			return __IndexOfAllRx(s, regexPattern, RegexOptions.None);
		}

		public static int[] IndexOfAllRx(this string s, string regexPattern, RegexOptions rxOptions)
		{
			return __IndexOfAllRx(s, regexPattern, rxOptions);
		}

		#endregion

		#region ReplaceRx

		public static string ReplaceRx(this string inputText, string findPattern, string replacement)
		{
			return Regex.Replace(inputText, findPattern, replacement);
		}

		public static string ReplaceRx(this string inputText, string findPattern, string replacement, RegexOptions rxOptions)
		{
			return Regex.Replace(inputText, findPattern, replacement, rxOptions);
		}

		public static string ReplaceRx(this string inputText, string findPattern, MatchEvaluator matchEvaluator)
		{
			return Regex.Replace(inputText, findPattern, matchEvaluator);
		}

		public static string ReplaceRx(this string inputText, string findPattern, MatchEvaluator matchEvaluator, RegexOptions rxOptions)
		{
			return Regex.Replace(inputText, findPattern, matchEvaluator, rxOptions);
		}

		#endregion

		#region RxEscape / RxUnescape

		public static string RxEscape(this string s)
		{
			return Regex.Escape(s);
		}

		public static string RxUnescape(this string s)
		{
			return Regex.Unescape(s);
		}

		public static bool PresentCharIsRegexEscaped(string text, int posInText)
		{
			int numBefore = NumEscapeCharsBefore(text, posInText, '\\');
			return (numBefore == 0 || numBefore % 2 == 0) ? false : true;
		}

		public static bool PresentCharIsEscaped(string text, int posInText, char escapeChar)
		{
			int numBefore = NumEscapeCharsBefore(text, posInText, escapeChar);
			return (numBefore == 0 || numBefore % 2 == 0) ? false : true;
		}

		public static int NumEscapeCharsBefore(string text, int posInText, char escapeChar)
		{
			if(posInText >= text.Length || posInText < 0) throw new ArgumentOutOfRangeException();
			int numBefore = 0;
			for(int i = posInText - 1; i >= 0; i--, numBefore++)
				if(text[i] != escapeChar)
					return numBefore;
			return numBefore;
		}

		#endregion

		#region RxIsMatch

		public static bool RxIsMatch(this string s, string pattern)
		{
			return Regex.IsMatch(s, pattern);
		}

		public static bool RxIsMatch(this string s, RxPattern rxPatternType)
		{
			return Regex.IsMatch(s, XRegex.GetRxPattern(rxPatternType, true));
		}

		public static bool RxIsMatch(this string s, string pattern, RegexOptions rxOptions)
		{
			return Regex.IsMatch(s, pattern, rxOptions);
		}

		#endregion

		#region RxMatch

		public static Match RxMatch(this string s, string pattern)
		{
			return Regex.Match(s, pattern);
		}

		public static Match RxMatch(this string s, RxPattern rxPatternType)
		{
			return Regex.Match(s, XRegex.GetRxPattern(rxPatternType, false));
		}

		public static Match RxMatch(this string s, string pattern, RegexOptions rxOptions)
		{
			return Regex.Match(s, pattern, rxOptions);
		}

		#endregion

		#region RxMatches

		public static MatchCollection RxMatches(this string s, string pattern)
		{
			return Regex.Matches(s, pattern);
		}

		public static MatchCollection RxMatches(this string s, RxPattern rxPatternType)
		{
			return Regex.Matches(s, XRegex.GetRxPattern(rxPatternType, false));
		}

		public static MatchCollection RxMatches(this string s, string pattern, RegexOptions rxOptions)
		{
			return Regex.Matches(s, pattern, rxOptions);
		}

		#endregion

		#region RxSplit

		public static string[] RxSplit(this string inputText)
		{
			if(inputText == null) throw new ArgumentNullException("inputText");

			MatchCollection mc = Regex.Matches(inputText, @"\b\w+\b");
			string[] matches = new string[mc.Count];
			int len = mc.Count;

			for(int i = 0; i < len; i++)
				matches[i] = mc[i].Value;

			return matches.ToArray();
		}

		public static string[] RxSplit(this string inputText, string findPattern)
		{
			return Regex.Split(inputText, findPattern);
		}

		public static string[] RxSplit(this string inputText, string findPattern, RegexOptions rxOptions)
		{
			return Regex.Split(inputText, findPattern, rxOptions);
		}

		#endregion


		public static string Group1(this Match m)
		{
			return m.Group(1);
		}
		public static string Group2(this Match m)
		{
			return m.Group(2);
		}
		public static string Group3(this Match m)
		{
			return m.Group(3);
		}

		public static string Group(this Match m, int groupIndex)
		{
			if(m != null && m.Success) {
				var groups = m.Groups;
				if(groups.Count > 0) {
					Group g = groups[groupIndex];
					if(g.Success)
						return g.Value;
				}
			}
			return null;
		}

		public static IEnumerable<string> GroupValues(this Match m, bool includeWhole = false)
		{
			if(m != null && m.Success) {
				var groups = m.Groups;
				if(groups.Count > 0) {
					for(int i = includeWhole ? 0 : 1; i < groups.Count; i++) {
						var g = groups[i];
						if(g != null && g.Success)
							yield return g.Value;
					}
				}
			}
		}


		public static string Group1(this Regex rx, string input)
		{
			return rx.Group(input, 1);
		}
		public static string Group2(this Regex rx, string input)
		{
			return rx.Group(input, 2);
		}
		public static string Group3(this Regex rx, string input)
		{
			return rx.Group(input, 3);
		}

		public static string Group(this Regex rx, string input, int groupIndex)
		{
			if(rx != null) {
				Match m = rx.Match(input);
				if(m != null && m.Success) {
					var groups = m.Groups;
					if(groups.Count > 0) {
						Group g = groups[groupIndex];
						if(g.Success)
							return g.Value;
					}
				}
			}
			return null;
		}

		public static IEnumerable<string> Groups(this Regex rx, string input, bool includeWhole = false)
		{
			if(rx != null) {
				var m = rx.Match(input);
				if(m != null && m.Success) {
					var groups = m.Groups;
					if(groups.Count > 0) {
						for(int i = includeWhole ? 0 : 1; i < groups.Count; i++) {
							var g = groups[i];
							if(g != null && g.Success)
								yield return g.Value;
						}
					}
				}
			}
		}


	}

	/// <summary>
	/// An enumeration of regular expression patterns.
	/// </summary>
	public enum RxPattern
	{
		/// <summary>
		/// An email address.
		/// </summary>
		Email,

		/// <summary>
		/// A US phone number.
		/// </summary>
		PhoneUS,

		/// <summary>
		/// A US Social Security Number.
		/// </summary>
		SocSecNumUS,

		/// <summary>
		/// A web URL.
		/// </summary>
		WebURL,

		/// <summary>
		/// A US Postal ZIP code.
		/// </summary>
		ZIPCodeUS
	}

}
