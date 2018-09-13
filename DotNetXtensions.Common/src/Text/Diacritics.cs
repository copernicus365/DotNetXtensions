using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DotNetXtensions.Globalization
{
	public static class Diacritics
	{
		public const char LowestDiacritic = 'À';
		public const char HighestDiacritic = 'ỷ';
		public const int BelowDiacritic = LowestDiacritic - 1; // 192 - 1

		public static char[] DTable;
		public static Dictionary<char, char> DiacriticsToAsciiDictionary;

		// --- static constructor ---
		static Diacritics()
		{
			Dictionary<char, char> dAll = new Dictionary<char, char>(600); ;

			foreach (var kv in SourceDictionary) {
				char asciiChar = kv.Key;
				string letters = kv.Value;
				char[] chars = letters.Where(c => c != ',' && c != ' ').ToArray();

				foreach (var c in chars) {
					dAll.Add(c, asciiChar);

					char upperD = char.ToUpper(c);
					if (upperD > BelowDiacritic && !dAll.ContainsKey(upperD)) { 
						// make sure a simple asciii isn't generated, which happens for 1 char in testing
						dAll.Add(upperD, char.ToUpper(asciiChar));
					}
				}
			}
			DiacriticsToAsciiDictionary = dAll;

			// --- INIT_DTable ---

			DTable = new char[8000];
			foreach (var kv in DiacriticsToAsciiDictionary)
				DTable[kv.Key] = kv.Value;

#if Diacritics_DTableShort

			DTableShort = new char[1000];
			foreach (var kv in DiacriticsToAsciiDictionary) {
				
				int i = kv.Key < 512 
					? kv.Key 
					: kv.Key - conDTableShortHigherSetOffset;

				DTableShort[i] = kv.Value;
			}

#endif
		}



		/// <summary>
		/// If this char is a diacritic value, it is converted to a simple ascii value,
		/// else the same char is returned. MAX performance guaranteed! Including that
		/// simple ascii letter/numbers will immediately return on a single lesser than check.
		/// </summary>
		/// <param name="c">The char.</param>
		public static char ToAscii(this char c)
		{
			// #1 check) for best performance for non diacritics, a single lesser-than comparison and BOOM we return
			// #2 check) no choice, have to make sure char is in range of our 8000 long array
			
			if (c < LowestDiacritic || c > HighestDiacritic)
				return c;

			char r = DTable[c];
			return r > char.MinValue
				? r
				: c;
		}

		/// <summary>
		/// Use DiacriticToAscii instead. This is for Testing purposes only (it uses a dictionary 
		/// lookup instead of the ~ 7 X faster array lookup).
		/// </summary>
		/// <param name="c">The char.</param>
		public static char _ToAscii_DictLookup(char c)
		{
			if (!c.IsInBasicDiacriticsRange())
				return c;

			char result = char.MinValue;
			if (Diacritics.DiacriticsToAsciiDictionary.TryGetValue(c, out result))
				return result;
			return c;

			//return Diacritics.DiacriticsToAsciiDictionary.ValueOrDefault(c, c);
		}

		public static bool IsInBasicDiacriticsRange(this char c)
		{
			if(c < LowestDiacritic)
				return false;
			
			// val > 191 // not needed now
			// there are many gaps in the < 512 range (why we call this *Basic*DiacriticsRange), but long stretches of matches as well
			return (c < 512) || (c < 7928 && c > 7691); // check order here is important!
		}



		/// <summary>
		/// Converts any European type diacritics (see the 438 in the accompanying dictionary)
		/// in this string to regular ascii type chars. Heavy duty performance! Heavily geared towards
		/// cases where no conversion is needed. In those cases, there is nothing but a one 
		/// time zip through the string (checking each char if it's numerical value is greater
		/// than a constant value - higher than all ascii letter values). See accompanying times
		/// in this class to see what diacritics are handled.
		/// </summary>
		public static string ToAsciiString(this string str)
		{
			if (str.IsNulle())
				return str;

			int len = str.Length;
			
			for (int i = 0; i < len; i++) {
				if (str[i] > BelowDiacritic) {
					char _c = str[i].ToAscii();
					if (_c > char.MinValue) {
						char[] chars = str.ToCharArray();
						chars[i] = _c;
						return new string(i + 1 < len ? chars.SetToAscii(i + 1) : chars); 
					}
				}
			}
			return str;
		}

		public static char[] SetToAscii(this char[] arr, int startIdx = 0)
		{
			if (arr.IsNulle() || startIdx >= arr.Length)
				return arr;

			int len = arr.Length;

			for (int i = startIdx; i < len; i++) {
				if (arr[i] > BelowDiacritic) {
					char _c = arr[i].ToAscii();
					if (_c > char.MinValue)
						arr[i] = _c;
				}
			}
			return arr;
		}


		/// <summary>
		/// Convert accented characters to a non-accentered form using the .NET "ISO-8859-8" encoding.
		/// For test purposes (particularly for performance).
		/// 
		/// http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
		/// http://stackoverflow.com/a/2086575/264031
		/// </summary>
		public static string AlternativeDeaccentString(this string str)
		{
			if (str.IsNulle())
				return str;

			byte[] data;
			data = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(str);
			string asciiStr = System.Text.Encoding.UTF8.GetString(data);
			return asciiStr;
		}


		#region -- main values dict --

		/// <summary>
		/// Started with information from here: http://personal-pages.lvc.edu/sayers/diacritic_table.html
		/// Would gladly have started from a more offical page, like on wikipedia, but this worked best.
		/// Grunt work sorting this was done. There's nothing stopping us from adding other values,
		/// though this list is quite nicely done. 
		/// <para/>
		/// Compromises were made for certain values that 
		/// don't have a clear ascii value, like the 'thorn' þ. Another big issue are letters that
		/// represent two chars (œ, ǽ ... ß to 's' instead of 'ss', etc). We made a critically important 
		/// decision to NOT make these into two. It is far better performance; much of what we have programmed
		/// here would be almost useless to adopt that model, and doesn't matter in scenarios
		/// where the question is more of a programmatic one where two sides of an equation are handled
		/// in house. For instance, a list of company names, if a user searches for a company and inserts
		/// a given diacritic, we convert that search query first, as well as all company names in place
		/// that they are compared. Surely, in other cases we would rather have it the other way, but 
		/// you would greatly loose in performance and simplicity for those scenarios, so may need
		/// to handle those few cases separately.
		/// </summary>
		public static Dictionary<char, string> SourceDictionary = new Dictionary<char, string>() 
		{
			{ 'a', "à,á,â,ã,ä,å,æ,ā,ă,ą,ǎ,ǻ,ǽ,ạ,ả,ấ,ầ,ẩ,ẫ,ậ,ắ,ằ,ẳ,ẵ,ặ" },
			{ 'c', "ç,ć,ĉ,ċ,č" },
			{ 'd', "ð,ď,đ,ḍ,ḏ,ḑ,ḓ" },
			{ 'e', "è,é,ê,ë,ē,ĕ,ė,ę,ě,ḕ,ḗ,ḙ,ḛ,ḝ,ẹ,ẻ,ẽ,ế,ề,ể,ễ,ệ" },
			{ 'f', "ḟ" },
			{ 'g', "ĝ,ğ,ġ,ģ,ḡ" },
			{ 'h', "ĥ,ħ,ḣ,ḥ,ḧ,ḩ,ḫ" },
			{ 'i', "ì,í,î,ï,ĩ,ī,ĭ,į,ı,ĳ,ǐ,ḭ,ḯ,ỉ,ị" },
			{ 'j', "ĵ" },
			{ 'k', "ķ,ḱ,ḳ,ḵ" },
			{ 'l', "ĺ,ļ,ľ,ŀ,ł,ḷ,ḹ,ḻ,ḽ" },
			{ 'm', "ḿ,ṁ,ṃ" },
			{ 'n', "ñ,ń,ņ,ň,ŋ,ṅ,ṇ,ṉ,ṋ" },
			{ 'o', "ò,ó,ô,õ,ö,ø,ō,ŏ,ő,œ,ơ,ǒ,ǿ,ṍ,ṏ,ṑ,ṓ,ọ,ỏ,ố,ồ,ổ,ỗ,ộ,ớ,ờ,ở,ỡ,ợ" },
			{ 'p', "ṕ,ṗ" },
			{ 'r', "ŕ,ŗ,ř,ṙ,ṛ,ṝ,ṟ" },
			{ 's', "ś,ŝ,ş,š,ṡ,ṣ,ṥ,ṧ,ṩ,ß" },
			{ 't', "þ,ţ,ť,ŧ,ṫ,ṭ,ṯ,ṱ" },
			{ 'u', "ù,ú,û,ü,ũ,ū,ŭ,ů,ű,ų,ư,ǔ,ǖ,ǘ,ǚ,ǜ,ṳ,ṵ,ṷ,ṹ,ṻ,ụ,ủ,ứ,ừ,ử,ữ,ự" },
			{ 'w', "ŵ,ẁ,ẃ,ẅ,ẇ,ẉ" },
			{ 'v', "ṽ,ṿ" },
			{ 'x', "ẋ,ẍ" },
			{ 'y', "ý,ÿ,ŷ,ẏ,ỳ,ỵ,ỷ" },
			{ 'z', "ź,ż,ž,ẑ,ẓ,ẕ" },
		};

		#endregion 

		#region --- TESTS and META ---

		public static string _CodeGen_Sort_AsciiDiacriticsDictionary_Values()
		{
			StringBuilder sb = new StringBuilder();

			foreach (var kv in SourceDictionary) {
				char letter = kv.Key;
				string letters = kv.Value;
				char[] chars = letters.Where(c => c != ',' && c != ' ').ToArray();
				chars.Sort();
				string lettersSorted = chars.JoinToString(",");

				sb.AppendFormat("\t\t\t{{ '{0}', \"{1}\" }},\r\n", letter, lettersSorted);
			}

			string result = sb.ToString();
			result.Print();
			return result; ;
		}

		/// <summary>
		/// Prints out our AsciiDiacriticsDictionary values in a sorted way that shows
		/// the decimal numeric values, etc.
		/// </summary>
		public static string _PrintoutDiacritics()
		{
			var sbres = new StringBuilder();

			int i = 1;
			foreach (char ck in Diacritics.DiacriticsToAsciiDictionary.Keys.OrderBy(k => k)) {
				char upCk = char.ToUpper(ck);
				char asciiLt = Diacritics.DiacriticsToAsciiDictionary[ck];
				sbres.AppendLine(string.Format("{0} - {1} - {2} - {3}", (i++).ToString("000"), ck, asciiLt, ((int)ck).ToString("0000")));
			}
			string result = sbres.ToString();
			return result;

			/* --- result ---
				001 - À - A - 0192
				002 - Á - A - 0193
				003 - Â - A - 0194
				004 - Ã - A - 0195
				005 - Ä - A - 0196
				006 - Å - A - 0197
				007 - Æ - A - 0198 ...
			 */
		}

		/// <summary>
		/// Great. After all this, found out there is already a native string comparison available that
		/// handles most of our diacritics. Here are the results (we've excepted the uppercase equivalents):
		/// <para />
		/// æ ǽ ĳ ŋ œ ß þ ŧ
		/// <para />
		/// Regardless, without having tested this, it is no doubt a much slower comparison to make
		/// as opposed to converting values beforehand.
		/// </summary>
		/// <returns></returns>
		public static string _ViewAllDiacriticsNotHandledByCompareOptionsIgnoreNonSpace()
		{
			int i = 0;
			var sb = new StringBuilder();

			foreach (var vals in Diacritics.DiacriticsToAsciiDictionary) {
				i++;
				int val = string.Compare(vals.Key.ToString(), vals.Value.ToString(), CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace);
				if (val != 0)
					sb.Append(vals.Key);
			}
			string result = sb.ToString();
			return result;
		}

		/// <summary>
		/// RESULTS:
		/// --- All following were for 100 million lookups total:
		/// DiacriticToAsciiDict:		~ 2220ms (2.2 seconds)
		/// DiacriticToAsciiSlower:	~ 578ms! (nearly 4 times faster [3.8] than dict lookup!!!)
		/// DiacriticToAscii:			~ 375ms! (6X faster than dict, and a whopping 35% faster than other array method
		///												But not magnitudes faster than smaller array, so not necc. 
		///												worth the extra memory needed . OTOHand, code is simpler)
		///												Equal 1 million lookups every 3 ms
		/// </summary>
		public static void PerfTest_DiacriticsMain()
		{
			int trials = 100000000; // 10 mill
			double result = 0;
			int i = 0;
			int repeats = 7;

			"=== DICT Lookup ===".Print();

			while (i++ < repeats)
				result = PerfTest_Loop(0, trials).TotalMilliseconds.Print();

			i = 0;

			"=== Array Slower Lookup (less memory: 1000 chars) ===".Print();

			while (i++ < repeats)
				result = PerfTest_Loop(1, trials).TotalMilliseconds.Print();

			i = 0;

			"=== Array Faster Lookup (but more memory: 8000 chars) ===".Print();

			while (i++ < repeats)
				result = PerfTest_Loop(2, trials).TotalMilliseconds.Print();


			"-------done-------".Print();
			Console.ReadLine();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="version">0 for dictionary, 1 for slower array if available, 
		/// 2 for fast array lookup.</param>
		/// <param name="numhits">How many total hits tests to run.</param>
		public static TimeSpan PerfTest_Loop(int version, int numhits)
		{
			char[] dchars = Diacritics.DiacriticsToAsciiDictionary.Keys.ToArray();

			if (numhits < dchars.Length) throw new ArgumentOutOfRangeException();

			char chr = char.MinValue;

			int len = dchars.Length;
			int trialsCount = 0;

			if (version == 1) {
				"This version is not currently available (include processor instruction Diacritics_DTableShort)".Print();
				return TimeSpan.MinValue;
			}

			DateTime now = DateTime.UtcNow;

			while (trialsCount < numhits) {
				for (int i = 0; i < len && trialsCount < numhits; i++, trialsCount++) {

					switch (version) {
						case 0: chr = _ToAscii_DictLookup(dchars[i]); break;
						//case 1: chr = dchars[i].DiacriticToAsciiSlower(); break;
						case 2: chr = dchars[i].ToAscii(); break;

						default: throw new ArgumentOutOfRangeException();
					}
				}
			}

			TimeSpan elap = DateTime.UtcNow - now;
			chr.Print();
			return elap;
		}

		#endregion

		#region --- DTableShort (not used now, but important to keep for diagnostics still) ---

#if Diacritics_DTableShort

		public const int conDTableShortHigherSetOffset = 7000;
		public static char[] DTableShort;
		public static char DiacriticToAsciiSlower(this char c)
		{
			if (c < LowestDiacritic)
				return c;

			char result = char.MinValue;

			if (c < 512)
				result = DTableShort[c];
			else if (c > 7691 && c < 7928)
				result = DTableShort[c - conDTableShortHigherSetOffset];
			else
				return c;

			return result > char.MinValue ? result : c;
		}

#endif

		#endregion

	}
}
