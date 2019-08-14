// XDictionary_LinqForNonGenericIDictionary

using System;
using System.Collections;
using System.Collections.Generic;

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XDictionary
	{
		public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(
			this IDictionary dict,
			Func<object, TKey> keyConverter,
			Func<object, TValue> valueConverter)
		{
			if(dict == null) return null;
			var d = new Dictionary<TKey, TValue>(dict.Count);

			foreach(var kv in dict.AsEnumerable(keyConverter, valueConverter))
				d.Add(kv.Key, kv.Value);

			return d;
		}

		public static Dictionary<object, object> ToDictionary(this IDictionary dict)
		{
			if(dict == null) return null;
			var d = new Dictionary<object, object>(dict.Count);

			foreach(var kv in dict.AsEnumerable())
				d.Add(kv.Key, kv.Value);

			return d;
		}

		public static IEnumerable<KeyValuePair<object, object>> AsEnumerable(this IDictionary dict)
		{
			if(dict == null)
				yield break;

			var enm = dict.GetEnumerator();
			while(enm.MoveNext())
				yield return new KeyValuePair<object, object>(enm.Key, enm.Value);
		}

		public static IEnumerable<KeyValuePair<TKey, TValue>> AsEnumerable<TKey, TValue>(
			this IDictionary dict,
			Func<object, TKey> keyConverter,
			Func<object, TValue> valueConverter)
		{
			if(dict == null)
				yield break;

			var enm = dict.GetEnumerator();
			while(enm.MoveNext()) {
				TKey key = keyConverter(enm.Key);
				TValue value = valueConverter(enm.Value);
				yield return new KeyValuePair<TKey, TValue>(key, value);
			}
		}

	}
}
