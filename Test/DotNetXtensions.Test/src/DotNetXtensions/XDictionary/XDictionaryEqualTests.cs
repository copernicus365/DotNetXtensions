using System;
using Xunit;
using DotNetXtensions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions.Test
{
	public class XDictionaryEqualTests : DnxTestBase
	{
		[Fact]
		public void Pass_Equal()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);

			bool equal = d1.DictionariesAreEqual(d2);
			True(equal);
		}

		[Fact]
		public void CustomComparer_Equal()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);

			bool equal = d1.DictionariesAreEqual(d2, (v1, v2) => v1 == v2);
			True(equal);
		}

		[Fact]
		public void Array_Pass_Equal()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });

			bool equal = d1.DictionariesAreEqual(d2);
			True(equal);

			bool equal2 = d1.DictionariesAreEqual(d2, (a, b) => a.SequenceIsEqual(b));
			True(equal);
		}

		[Fact]
		public void Array_NoPass()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });

			d2["Pears"][1] = 88;

			bool equal = d1.DictionariesAreEqual(d2);
			False(equal);

			bool equal2 = d1.DictionariesAreEqual(d2, (a, b) => a.SequenceIsEqual(b));
			False(equal);
		}

		[Fact]
		public void SameCount_NotEqual()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			d2["Pears"] = 7;

			bool equal = d1.DictionariesAreEqual(d2);
			False(equal);
		}

		[Fact]
		public void SameCount_NotEqual_CustComparer()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			d2["Pears"] = 7;

			bool equal = d1.DictionariesAreEqual(d2, (v1, v2) => v1 == v2);
			False(equal);
		}

		[Fact]
		public void DiffCount_NotEqual()
		{
			var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
			d2.Remove("Pears");

			True(d2.Count == d1.Count - 1);
			bool equal = d1.DictionariesAreEqual(d2);
			False(equal);		
		}

		static Dictionary<string, int> MockVals1 = new Dictionary<string, int>() {
			{ "Apples", 1 },
			{ "Oranges", 1 },
			{ "Peaches", 3 },
			{ "Pears", 3 },
			{ "Pineapples", 5 },
		};

	}
}
