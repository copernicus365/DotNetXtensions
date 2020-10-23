// XDictionary_AddN

using System.Collections.Generic;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
		public static Dictionary<TKey, TValue> AddN<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
		{
			d.Add(key, value);
			return d;
		}

		public static Dictionary<TKey, TValue> AddN<TKey, TValue>(this Dictionary<TKey, TValue> d, KeyValuePair<TKey, TValue> kv)
		{
			d.Add(kv.Key, kv.Value);
			return d;
		}

		public static Dictionary<TKey, List<TValue>> AddN<TKey, TValue>(this Dictionary<TKey, List<TValue>> d, TKey key, params TValue[] values)
		{
			d.Add(key, new List<TValue>(values));
			return d;
		}

		public static Dictionary<TKey, TValue> AddIfNotContainsKey<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
		{
			if(!d.ContainsKey(key))
				d.Add(key, value);
			return d;
		}
	}
}
