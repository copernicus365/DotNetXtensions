// XDictionary_DictionariesAreEqual

using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
		// Remove these string based ones soon...

		public static bool DictionariesAreEqual(this Dictionary<string, string> dict, Dictionary<string, string> expectedMatchDict)
		{
			var d1 = dict;
			var d2 = expectedMatchDict;
			if(d1 == null || d2 == null)
				return d1 == null && d2 == null;

			if(d1.Count != d2.Count)
				return false;

			foreach(string key in d1.Keys) {
				string val = d1[key];

				if(!d2.TryGetValue(key, out string val2))
					return false;

				if(val != val2)
					return false;
			}
			return true;
		}

		public static bool DictionariesAreEqual(this Dictionary<string, string[]> dict, Dictionary<string, string[]> expectedMatchDict)
		{
			var d1 = dict;
			var d2 = expectedMatchDict;
			if(d1 == null || d2 == null)
				return d1 == null && d2 == null;

			if(d1.Count != d2.Count)
				return false;

			foreach(string key in d1.Keys) {
				string[] vals = d1[key];

				if(!d2.TryGetValue(key, out string[] vals2))
					return false;

				if(vals == null || vals2 == null)
					return vals == vals2;

				return vals.SequenceEqual(vals2);
			}

			return true;
		}

		// purpose of this is to make up for current C# shortcoming
		// which doesn't allow easy simple creation of a dictionary
		// using params and tuples we can get super easy calls to make up for this
		// e.g. `dict.DictionaryIsEqual(("a", 1), ("b", 2));` This need is felt
		// mostly in tests 
		public static bool DictionaryIsEqual<TKey, TValue>(
			this IDictionary<TKey, TValue> dict,
			Func<TValue, TValue, bool> comparer,
			params (TKey key, TValue val)[] arr)
		{
			if(dict == null || arr == null)
				return dict == null && arr == null;

			if(dict.Count != arr.Length)
				return false;

			bool hasEqCmpr = comparer != null;

			foreach(var kv in arr) {
				TKey key = kv.key;
				TValue val = kv.val;

				if(!dict.TryGetValue(key, out TValue dval))
					return false;

				if(hasEqCmpr) {
					if(!comparer(val, dval)) // equalityComparer.Equals(val2))
						return false;
				}
				else {
					if(!val.Equals(dval))
						return false;
				}
			}
			return true;
		}

		public static bool DictionaryIsEqual<TKey, TValue>(
			this IDictionary<TKey, TValue> dict,
			params (TKey key, TValue val)[] arr)
		=> DictionaryIsEqual(dict, null, arr);

		public static bool DictionariesAreEqual<TKey, TValue>(
			this IDictionary<TKey, TValue> dict1,
			IDictionary<TKey, TValue> dict2,
			Func<TValue, TValue, bool> comparer = null)
		{
			// IEqualityComparer<TValue> equalityComparer = null)

			if(dict1 == null || dict2 == null)
				return dict1 == null && dict2 == null;

			if(dict1.Count != dict2.Count)
				return false;

			bool hasEqCmpr = comparer != null;

			foreach(var kv in dict1) {
				TKey key1 = kv.Key;
				TValue val1 = kv.Value;

				if(!dict2.TryGetValue(key1, out TValue val2))
					return false;

				if(hasEqCmpr) {
					if(!comparer(val1, val2)) // equalityComparer.Equals(val2))
						return false;
				}
				else {
					if(!val1.Equals(val2))
						return false;
				}
			}
			return true;
		}

		public static bool DictionariesAreEqual<TKey, TValue>(
			this IDictionary<TKey, TValue[]> dict1,
			IDictionary<TKey, TValue[]> dict2,
			Func<TValue[], TValue[], bool> comparer = null)
		{
			if(dict1 == null || dict2 == null)
				return dict1 == null && dict2 == null;

			if(dict1.Count != dict2.Count)
				return false;

			bool hasEqCmpr = comparer != null;

			foreach(var kv in dict1) {
				TKey key1 = kv.Key;
				TValue[] arr1 = kv.Value;

				if(!dict2.TryGetValue(key1, out TValue[] arr2))
					return false;

				if(arr1 == null || arr2 == null)
					if(arr1 != arr2)
						return false;

				if(hasEqCmpr) {
					if(!comparer(arr1, arr2)) // equalityComparer.Equals(val2))
						return false;
				}
				else if(!arr1.SequenceEqual(arr2))
					return false;
			}
			return true;
		}

		public static bool DictionariesAreEqual<TKey, TValue>(
			this IDictionary<TKey, List<TValue>> dict1,
			IDictionary<TKey, List<TValue>> dict2,
			Func<List<TValue>, List<TValue>, bool> comparer = null)
		{
			if(dict1 == null || dict2 == null)
				return dict1 == null && dict2 == null;

			if(dict1.Count != dict2.Count)
				return false;

			bool hasEqCmpr = comparer != null;

			foreach(var kv in dict1) {
				TKey key1 = kv.Key;
				List<TValue> arr1 = kv.Value;

				if(!dict2.TryGetValue(key1, out List<TValue> arr2))
					return false;

				if(arr1 == null || arr2 == null)
					if(arr1 != arr2)
						return false;

				if(hasEqCmpr) {
					if(!comparer(arr1, arr2)) // equalityComparer.Equals(val2))
						return false;
				}
				else if(!arr1.SequenceEqual(arr2))
					return false;
			}
			return true;
		}

	}
}
