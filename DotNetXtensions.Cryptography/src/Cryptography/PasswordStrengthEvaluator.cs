using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// Type that helps determine the strength of a password.
	/// Inspiration and initial code base (strongly modified) from Tom Gullen on Stack Overflow
	/// (https://stackoverflow.com/a/18108837/264031).
	/// </summary>
	public class PasswordStrengthEvaluator
	{
		public string Password { get; }
		public int Length { get; set; }

		public int LowercaseCount { get; set; }
		public int UppercaseCount { get; set; }
		public int NumberCount { get; set; }
		public int SpecialCharCount { get; set; }

		public int ConsecutiveLowercaseSpansCount { get; set; }
		public int ConsecutiveUppercaseSpansCount { get; set; }
		public int ConsecutiveNumberSpansCount { get; set; }
		public int ConsecutiveSpecialCharSpansCount { get; set; }

		/// <summary>
		/// For strength validation, setting this property may not be needed if you already
		/// are setting the main counts (<see cref="LowercaseCount"/> etc), as those will guarantee
		/// a certain number of consecutive char type spans.
		/// </summary>
		public int TotalConsecutiveSpansCount { get; set; }

		public int TotalDistinctCharsCount { get; set; }
		public double DistinctCharsRatioToLength { get; set; }
		public double DistinctCharsRatioToLengthPercent => (DistinctCharsRatioToLength * 100).Round(3);


		public bool ContainsNumber => NumberCount > 0;
		public bool ContainsUppercase => UppercaseCount > 0;
		public bool ContainsLowercase => LowercaseCount > 0;
		public bool ContainsSpecialChar => SpecialCharCount > 0;


		public Dictionary<char, int> CharFrequencyDict;

		public static string ConsecutiveSpanExampleMessage = "What is a consecutive span of characters of a given type? In the text: 'helloThere91', there are 2 sets of consecutive lowercase letters ('hello' and 'here'), 1 set of consecutive numbers ('91'), and 1 set of consecutive uppercase letters (just one letter in this case but that is still a consecutive set: 'T'). That totals 4 consecutive spans of unique characters types.";

		public PasswordStrengthEvaluator() { }

		public PasswordStrengthEvaluator(
			string password, 
			bool persistPassword = false, 
			bool keepCharFrequencyDict = false)
		{
			string p = password;
			if (string.IsNullOrEmpty(p))
				throw new ArgumentNullException();

			if (persistPassword)
				Password = p;
			
			int len = Length = p.Length;
			CharType lastTyp = default(CharType);

			CharFrequencyDict = new Dictionary<char, int>(len);

			for (int i = 0; i < len; i++) {

				char c = password[i];
				int n = (int)c;
				CharType typ = default(CharType);

				if (char.IsLower(c)) { //(n >= 97 && n <= 122)
					typ = CharType.LetterLowercase;
					LowercaseCount++;
				}
				else if (char.IsUpper(c)) { //(n >= 65 && n <= 90)
					typ = CharType.LetterUppercase;
					UppercaseCount++;
				}
				else if (char.IsDigit(c)) { // (n >= 48 && n <= 57)
					typ = CharType.Digit;
					NumberCount++;
				}
				else {
					typ = CharType.SpecialChar;
					SpecialCharCount++;
				}

				bool changedType = typ != lastTyp || i == 0;
				lastTyp = typ;

				if (changedType) {
					switch (typ) {
						case CharType.LetterLowercase: ConsecutiveLowercaseSpansCount++; break;
						case CharType.LetterUppercase: ConsecutiveUppercaseSpansCount++; break;
						case CharType.Digit: ConsecutiveNumberSpansCount++; break;
						case CharType.SpecialChar: ConsecutiveSpecialCharSpansCount++; break;
						default: throw new ArgumentOutOfRangeException();
					}
				}

				CharFrequencyDict[c] = CharFrequencyDict.TryGetValue(c, out int cnt)
					? ++cnt
					: 1;
			}

			TotalDistinctCharsCount = CharFrequencyDict.Count;

			DistinctCharsRatioToLength = TotalDistinctCharsCount / (double)len;

			TotalConsecutiveSpansCount = ConsecutiveLowercaseSpansCount + ConsecutiveUppercaseSpansCount + ConsecutiveNumberSpansCount + ConsecutiveSpecialCharSpansCount;

			if (!keepCharFrequencyDict)
				CharFrequencyDict = null;
		}

		enum CharType
		{
			LetterLowercase,
			LetterUppercase,
			Digit,
			SpecialChar
		}

		public (bool success, string errorMsg) StrongEnough(
			PasswordStrengthEvaluator minimumRequirements,
			int maxPasswordLength = 0,
			bool appendConsSpanMsg = false)
		{
			var m = minimumRequirements;
			string msg = null;

			if (m.DistinctCharsRatioToLength > 1.0)
				throw new ArgumentOutOfRangeException();

			bool _outOfRangeHigh(int value, int minValue) 
				=> minValue > 0 && value < minValue;

			bool _outOfRangeHighD(double value, double minValue)
				=> minValue > 0 && value < minValue;

			bool _outOfRangeHighMsg(int value, int minValue, string notEnoughTitle, ref string errmsg)
			{
				if (_outOfRangeHigh(value, minValue)) {
					errmsg = $"There are not enough {notEnoughTitle} ({minValue - value} more needed).";
					return true;
				}
				return false;
			}

			string consSpanMsg = ConsecutiveSpanExampleMessage;
			if (!appendConsSpanMsg)
				consSpanMsg = null;

			bool _outOfRangeHighConsSpans(int value, int minValue, string notEnoughTitle, ref string errmsg)
			{
				if (_outOfRangeHighMsg(value, minValue, notEnoughTitle, ref errmsg)) {
					errmsg = $"There are not enough consective spans of {notEnoughTitle} characters ({minValue - value} more needed). " + consSpanMsg;
					return true;
				}
				return false;
			}

			if (maxPasswordLength > 0 && Length > maxPasswordLength)
				return (false, $"Password length is too long (max: {maxPasswordLength}).");

			if (_outOfRangeHigh(Length, m.Length))
				return (false, $"Password length is too short (min: {m.Length}).");

			if (_outOfRangeHighMsg(LowercaseCount, m.LowercaseCount, "lowercase letters", ref msg))
				return (false, msg);

			if (_outOfRangeHighMsg(UppercaseCount, m.UppercaseCount, "uppercase letters", ref msg))
				return (false, msg);

			if (_outOfRangeHighMsg(NumberCount, m.NumberCount, "numbers", ref msg))
				return (false, msg);

			if (_outOfRangeHighMsg(SpecialCharCount, m.SpecialCharCount, "special characters (* ! @ $ etc)", ref msg))
				return (false, msg);

			if (_outOfRangeHighConsSpans(ConsecutiveLowercaseSpansCount, m.ConsecutiveLowercaseSpansCount, "lowercase", ref msg))
				return (false, msg);

			if (_outOfRangeHighConsSpans(ConsecutiveUppercaseSpansCount, m.ConsecutiveUppercaseSpansCount, "uppercase", ref msg))
				return (false, msg);

			if (_outOfRangeHighConsSpans(ConsecutiveNumberSpansCount, m.ConsecutiveNumberSpansCount, "number", ref msg))
				return (false, msg);

			if (_outOfRangeHighConsSpans(ConsecutiveSpecialCharSpansCount, m.ConsecutiveSpecialCharSpansCount, "special", ref msg))
				return (false, msg);

			// More often than not, if someone uses this consecutive set notion, it will be this one:
			if (_outOfRangeHigh(TotalConsecutiveSpansCount, m.TotalConsecutiveSpansCount)) {
				msg = $"There are only *{TotalConsecutiveSpansCount}* consecutive spans of characters of a given type (lowercase, uppercase, numbers, etc), but there needs to be {m.TotalConsecutiveSpansCount}. " + consSpanMsg;
				return (false, msg);
			}

			if (_outOfRangeHigh(TotalDistinctCharsCount, m.TotalDistinctCharsCount))
				return (false, $"There are only *{TotalDistinctCharsCount}* unique characters, but there needs to be a minimum of {m.TotalDistinctCharsCount}.");

			if (_outOfRangeHighD(DistinctCharsRatioToLength, m.DistinctCharsRatioToLength))
				return (false, $"Only {DistinctCharsRatioToLengthPercent}% of the characters are unique (min: minimum of {(m.DistinctCharsRatioToLength * 100).Round(3)}%). There are too many duplicate characters.");

			return (true, null);
		}

		public override string ToString()
			=> $@"---Password Strenth Info ---
Distinct chars: `{TotalDistinctCharsCount}` out of {Length}, Ratio: {DistinctCharsRatioToLengthPercent}%
Counts (count / consecutive spans of type):
a:({LowercaseCount} / {ConsecutiveLowercaseSpansCount})
A:({UppercaseCount} / {ConsecutiveUppercaseSpansCount})
1:({NumberCount} / {ConsecutiveNumberSpansCount})
!:({SpecialCharCount} / {ConsecutiveSpecialCharSpansCount})";
	}
}
