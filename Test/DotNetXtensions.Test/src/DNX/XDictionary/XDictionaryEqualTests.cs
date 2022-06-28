namespace DotNetXtensions.Test;

public class XDictionaryEqualTests : DnxTestBase
{
	[Fact]
	public void Pass_Equal()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var arr2 = d2.Select(kv => (kv.Key, kv.Value)).ToArray();

		True(d1.DictionariesAreEqual(d2));
		True(d1.DictionaryIsEqual(arr2));
	}

	[Fact]
	public void CustomComparer_Equal()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var arr2 = d2.Select(kv => (kv.Key, kv.Value)).ToArray();

		True(d1.DictionariesAreEqual(d2, (v1, v2) => v1 == v2));
		True(d1.DictionaryIsEqual((v1, v2) => v1 == v2, arr2));
	}

	[Fact]
	public void Array_Pass_Equal()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });

		True(d1.DictionariesAreEqual(d2));
		True(d1.DictionariesAreEqual(d2, (a, b) => a.SequenceIsEqual(b)));
	}

	[Fact]
	public void Array_Pass_Equal_List()
	{
		Dictionary<string, List<int>> d1 = MockVals1.ToDictionary(kv => kv.Key, kv => new List<int>() { kv.Value, kv.Value + 1, kv.Value + 2 });
		Dictionary<string, List<int>> d2 = MockVals1.ToDictionary(kv => kv.Key, kv => new List<int>() { kv.Value, kv.Value + 1, kv.Value + 2 });

		True(d1.DictionariesAreEqual(d2));
		True(d1.DictionariesAreEqual(d2, (a, b) => a.SequenceEqual(b)));

		// TEST FAIL
		// ALTER first dict (alter first item's value's first List value) so they should NOT be equal
		d1.Values.First()[0] = 10001;

		False(d1.DictionariesAreEqual(d2));
	}


	[Fact]
	public void Array_NoPass()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => new int[] { kv.Value, kv.Value + 1, kv.Value + 2 });

		d2["Pears"][1] = 88;

		False(d1.DictionariesAreEqual(d2));
		False(d1.DictionariesAreEqual(d2, (a, b) => a.SequenceIsEqual(b)));
	}

	[Fact]
	public void SameCount_NotEqual()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		d2["Pears"] = 7;
		var arr2 = d2.Select(kv => (kv.Key, kv.Value)).ToArray();

		False(d1.DictionariesAreEqual(d2));
		False(d1.DictionaryIsEqual(arr2));
	}

	[Fact]
	public void SameCount_NotEqual_CustComparer()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		d2["Pears"] = 7;
		var arr2 = d2.Select(kv => (kv.Key, kv.Value)).ToArray();

		False(d1.DictionariesAreEqual(d2, (v1, v2) => v1 == v2));
		False(d1.DictionaryIsEqual((v1, v2) => v1 == v2, arr2));
	}

	[Fact]
	public void DiffCount_NotEqual()
	{
		var d1 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		var d2 = MockVals1.ToDictionary(kv => kv.Key, kv => kv.Value);
		d2.Remove("Pears");
		var arr2 = d2.Select(kv => (kv.Key, kv.Value)).ToArray();

		True(d2.Count == d1.Count - 1);
		True(arr2.Length == d1.Count - 1);
		False(d1.DictionariesAreEqual(d2));
		False(d1.DictionaryIsEqual(arr2));
	}

	static Dictionary<string, int> MockVals1 = new Dictionary<string, int>() {
		{ "Apples", 1 },
		{ "Oranges", 1 },
		{ "Peaches", 3 },
		{ "Pears", 3 },
		{ "Pineapples", 5 },
	};

}
