using System;
using Xunit;
using DotNetXtensions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions.Test
{
	public class XDictionary_NameValueCollection_ToDictionary : BaseUnitTest
	{
		public XDictionary_NameValueCollection_ToDictionary()
		{
			MockSetup();
		}

		public List<(string key, string val)> mockValues = new List<(string key, string val)>() {
			("", ""),
			("name", ""),
			("age", "22"),
			("name", "Tony"),
			("name", "Joey"),
			("nAMe", "Yishai"),
			("height", "6.1"),
			("name", "Tiger"),
			("", ""),
		}.Where(kv => kv.key.NotNulle()).ToList();

		NameValueCollection nvColl_CaseSens;
		NameValueCollection nvColl_IgnCase;

		public void TestAll()
		{
			FirstValue_IgnoreCase();
			FirstValue_CaseSens();
			AllValuesString_IgnCase();
			AllValuesString_CaseSens();
			MultiValues_IgnCase();
			MultiValues_CaseSens();
		}

		[Fact]
		public void FirstValue_IgnoreCase()
		{
			bool IGNORE_CASE = true;
			var expected = getMockExpectedDict(ignoreCase: IGNORE_CASE);

			var result = nvColl_IgnCase.ToDictionary(
				forMultiValuesOnlyGetFirst: true,
				ignoreCase: IGNORE_CASE);

			printDict(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}

		[Fact]
		public void FirstValue_CaseSens()
		{
			bool IGNORE_CASE = false;
			var expected = getMockExpectedDict(ignoreCase: IGNORE_CASE);

			var result = nvColl_CaseSens.ToDictionary(
				forMultiValuesOnlyGetFirst: true,
				ignoreCase: IGNORE_CASE);

			printDict(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}

		[Fact]
		public void AllValuesString_IgnCase()
		{
			bool IGNORE_CASE = true;
			var expected = getMockExpectedDict(ignoreCase: IGNORE_CASE, firstNonNullValueOnly: false);

			var result = nvColl_IgnCase.ToDictionary(
				forMultiValuesOnlyGetFirst: false,
				ignoreCase: IGNORE_CASE);

			printDict(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}

		[Fact]
		public void AllValuesString_CaseSens()
		{
			bool IGNORE_CASE = false;
			var expected = getMockExpectedDict(ignoreCase: IGNORE_CASE, firstNonNullValueOnly: false);

			var result = nvColl_CaseSens.ToDictionary(
				forMultiValuesOnlyGetFirst: false,
				ignoreCase: IGNORE_CASE);

			printDict(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}


		[Fact]
		public void MultiValues_IgnCase()
		{
			bool IGNORE_CASE = true;
			var expected = getMockExpectedDictMultiVals(ignoreCase: IGNORE_CASE);

			var result = nvColl_IgnCase.ToDictionaryMultiValues(
				ignoreCase: IGNORE_CASE);

			printDict2(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}

		[Fact]
		public void MultiValues_CaseSens()
		{
			bool IGNORE_CASE = false;
			var expected = getMockExpectedDictMultiVals(ignoreCase: IGNORE_CASE);

			var result = nvColl_CaseSens.ToDictionaryMultiValues(
				ignoreCase: IGNORE_CASE);

			printDict2(result);

			Assert.True(DictionariesAreEqual(result, expected));
		}


		public void MockSetup()
		{
			// nvColl = HttpUtility.ParseQueryString(queryStr);

			nvColl_IgnCase = new NameValueCollection(mockValues.Count,
				StringComparer.OrdinalIgnoreCase);

			nvColl_CaseSens = new NameValueCollection(mockValues.Count,
				StringComparer.Ordinal);

			void AddNVKeyValues(NameValueCollection nvColl)
			{
				foreach ((string key, string val) in mockValues)
					nvColl.Add(key, val ?? "");
			}

			AddNVKeyValues(nvColl_IgnCase);
			AddNVKeyValues(nvColl_CaseSens);
		}

		Dictionary<string, string> getMockExpectedDict(bool ignoreCase, bool firstNonNullValueOnly = true)
		{
			var expected = mockValues.ToDictionaryIgnoreDuplicateKeys(
				kv => kv.key, kv => kv.val,
				ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal,
				handleDuplicate: (key, currentVal, newVal) =>
					firstNonNullValueOnly
						? currentVal.FirstNotNulle(newVal)
						: $"{currentVal},{newVal}");
			return expected;
		}

		Dictionary<string, string[]> getMockExpectedDictMultiVals(bool ignoreCase)
		{
			var expected = mockValues.ToDictionaryIgnoreDuplicateKeys(
				kv => kv.key, 
				kv => new string[] { kv.val },
				ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal,
				handleDuplicate: (key, currentVals, newVals) =>
					currentVals.E().ConcatToArray(newVals));
			return expected;
		}

		int printNum = 0;
		string printDict(Dictionary<string, string> d)
		{
			return $" --- {printNum++} --- \r\n" + d
				.JoinToString(kv => $"{kv.Key}, {kv.Value}", "\r\n")
				.Print();
		}
		string printDict2(Dictionary<string, string[]> d)
		{
			return $" --- {printNum++} --- \r\n" + d
				.JoinToString(kv => $"{kv.Key}, {kv.Value?.JoinToString("\r\n")}")
				.Print();
		}

		//string kvInputStr = @"
		//name:
		//age:22
		//name:Tony
		//name:Joey
		//nAMe: Yishai
		//height:6.1
		//name:Tiger
		//";
		// var kvs = GetKVsPerLine(kvInputStr);
		// string queryStr = kvs.JoinToString(kv => $"{kv.Key}={kv.Value}", "&");
	}
}
