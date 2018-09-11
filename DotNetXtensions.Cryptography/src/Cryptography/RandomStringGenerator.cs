using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// A random string or password generator, which uses <see cref="CryptoRandom"/> for the core
	/// random generation, in a very high performance manner (despite using a crytographic randomizer, but speed
	/// is gained greatly through <see cref="CryptoRandom"/>'s cache), 
	/// and which gives many options to conveniently customize
	/// the char set used, whether certain characters are required, and so forth. To increase randomization, 
	/// the charset (which is readonly set in the constructor) is itself randomized in the constructor, giving
	/// a doubly randomized result (though note that the same randomized char set is reused per instance).
	/// The chararacter set you input
	/// can be used to either remove certain characters from use (like 'l' and '1'), or also to increase 
	/// their frequency probability, by simply increasing the number of times you input them in the char
	/// set (so to increase the frequency probabilty if 'z' or 'Z', input those chars more than once). 
	/// This is not thread safe, but you may take a lock of your own on a given instance to obtain thread
	/// safety.
	/// </summary>
	public class RandomStringGenerator
	{
		public const string ConUpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		public const string ConLowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
		public const string ConNumberChars = "0123456789";

		/// <summary>
		/// Unless changed, the default value is  A-Z, a-z, 0-9.
		/// </summary>
		public static string CharSetDef = ConUpperCaseChars + ConLowerCaseChars + ConNumberChars;
		public static string SymbolCharsDef = "!@#$*.?|_-";
		public static int DefaultLengthStatic = 10;

		char[] _charSet;

		public int DefaultLength { get; set; }

		public string CharacterSet { get; }

		/// <summary>
		/// This is set from all chars in the final char set which 
		/// are not a letter or digit (<see cref="char.IsLetterOrDigit(char)"/>).
		/// </summary>
		public string SymbolSet { get; }

		public HashSet<char> SymbolsHashset { get; }

		/// <summary>
		/// True to require at least one upper case letter. If the random 
		/// generated string 
		/// </summary>
		public bool RequireUpper { get; set; }
		public bool RequireLower { get; set; }
		public bool RequireNumber { get; set; }
		public bool RequireSymbol { get; set; }

		CryptoRandom _cryptoRandom;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="defaultLength">Default length for generated values.
		/// This is only used when <see cref="GetRandomString"/> has an input length 
		/// of 0.</param>
		/// <param name="characterSet">The character set to base the random generation off of.
		/// You MAY have duplicate chars, which is useful to increase the frequency of those characters.</param>
		/// <param name="symbolsToAdd">We've made this a separate parameter, because in most users
		/// cases, they want the default ascii alpha-numeric chars (A-Z, a-z, 0-9), but usually
		/// would want their own customized set of which symbols to use. This way, you don't have to 
		/// send in a custom char set every time just to set which symbols you want to set. Note:
		/// if this is set, <paramref name="addDefaultSymbolSet"/> CANNOT be set to true.</param>
		/// <param name="addDefaultSymbolSet">True to add the default symbol set (see static 
		/// property: <see cref="SymbolCharsDef"/>, which is not readonly, so you may set your own
		/// default for this globally if desired). Do NOT set this to true if you send in your own
		/// <paramref name="symbolsToAdd"/>.</param>
		/// <param name="enforceDistinctCharSet">This will filter the input char set with a Distinct
		/// operation, which will (without error) remove any (presumably accidental) duplicates.
		/// This is NOT run by default, because you may *want* to increase a chars probability by
		/// repeating it n number of times in the input char set.</param>
		/// <param name="bufferLength">Sets the buffer size in bytes of the internal <see cref="CryptoRandom"/>
		/// instance. Increase this value to a kilobyte or greater in order to get extremely good performance.
		/// This makes most sense when reusing this instance more than once.</param>
		public RandomStringGenerator(
			int defaultLength = 0,
			string characterSet = null,
			string symbolsToAdd = null,
			bool addDefaultSymbolSet = false,
			bool enforceDistinctCharSet = false,
			int bufferLength = 512)
		{
			if (defaultLength > 0)
				DefaultLength = defaultLength.Max(0);

			CharacterSet = characterSet ?? CharSetDef;

			if (addDefaultSymbolSet) {
				if (symbolsToAdd.NotNulle())
					throw new ArgumentException($"Argument '{nameof(addDefaultSymbolSet)}' cannot be true while '{nameof(symbolsToAdd)}' is set (not null).");
				symbolsToAdd = SymbolCharsDef;
			}

			if (symbolsToAdd.NotNulle())
				CharacterSet += symbolsToAdd;

			// need this HashSet because its hard to detect if a char is actually a symbol later
			// (char.IsSymbol is deficient, so just counting as 'symbol' any non letterOrDigit
			char[] symbols = CharacterSet.Where(c => !char.IsLetterOrDigit(c)).ToArray();
			if (symbols.NotNulle()) {
				SymbolsHashset = new HashSet<char>(symbols.Distinct());
				SymbolSet = new string(symbols);
			}

			_charSet = enforceDistinctCharSet
				? CharacterSet.Distinct().ToArray()
				: CharacterSet.ToArray();

			if (_charSet.IsNulle())
				throw new ArgumentOutOfRangeException("No characters were set.", nameof(CharacterSet));

			bufferLength = (CharacterSet.Length * 2).Max(bufferLength); // gets bigger of two, doesn't let input be smaller than charSet*2
			bufferLength = bufferLength.MinMax(256, 1024 * 4); // 4 kb max, 1/2 a byte min, for this max, no need to generate huge buffer value, perf is regardless once your getting > 4 kb

			_cryptoRandom = new CryptoRandom(bufferLength);

			// If true, before generating the random string, we will radomize the source char set
			// as well, making for a doubly randomized result. TRUE by default.
			bool randomizeCharSet = true;
			if (randomizeCharSet) {
				_charSet = _cryptoRandom.RandomShuffle(_charSet);

				bool viewDiagnostic = false;
				if (viewDiagnostic) {
					//// just to see none of the original chars were lost in that sort:
					string shuffledStr = new string(_charSet);
					string sortedStr = _charSet.OrderBy(c => c).JoinToString(""); // view to witness no chars were dropped (or added)
				}
			}
		}

		/// <summary>
		/// Gets a random string based on the settings of this class.
		/// <para/>
		/// NOTE: If you set any of the Require... properties to true
		/// (e.g. <see cref="RequireUpper"/>), any missing chars will 
		/// be *added to* the final length (this will still be properly randomized in the end however,
		/// in fact whenever an extra or more chars are added, we fully shuffle the result string,
		/// after randomly getting the missing char types). 
		/// This was done for practical purposes.
		/// (Otherwise it would require *deleting* some previous randomly generated 
		/// values, but that would mess up potentially some char types already thought
		/// one had, it all gets too complicated, so we simplified by just allowing the final
		/// length to grow).
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public string GetRandomString(int length = 0)
		{
			if (length < 1) {
				length = DefaultLength;
				if (length < 1) {
					length = DefaultLengthStatic;
					if (length < 1)
						throw new ArgumentOutOfRangeException(nameof(length));
				}
			}

			int maxLen = int.MaxValue / 6;
			if (length > maxLen)
				throw new ArgumentException($"length must be less than {maxLen}", nameof(length));

			char[] randArr =
				_cryptoRandom.GetRandomValues(_charSet, length)
				.ToArray();

			string val = new string(randArr);

			// now add any required missing extras
			string extras = GetExtrasIfMissing(val);
			if (extras.NotNulle())
				val = _cryptoRandom.RandomShuffle(val + extras);

			return val;
		}

		public string GetExtrasIfMissing(string val)
		{
			List<char> extras = new List<char>();

			if (RequireUpper && !val.Any(c => char.IsUpper(c)))
				extras.Add(_cryptoRandom.GetRandomCharFromString(ConUpperCaseChars));

			if (RequireLower && !val.Any(c => char.IsLower(c)))
				extras.Add(_cryptoRandom.GetRandomCharFromString(ConLowerCaseChars));

			if (RequireNumber && !val.Any(c => char.IsNumber(c)))
				extras.Add(_cryptoRandom.GetRandomCharFromString(ConNumberChars));

			if (RequireSymbol && !val.Any(c => SymbolsHashset.Contains(c))) {
				extras.Add(_cryptoRandom.GetRandomCharFromString(SymbolSet));
			}

			return extras.IsNulle()
				? null
				: new string(extras.ToArray());
		}
	}
}
