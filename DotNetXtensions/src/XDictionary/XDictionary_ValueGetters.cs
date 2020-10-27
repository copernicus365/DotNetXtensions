// XDictionary_ValueGetters

using System.Collections.Generic;
using System.Diagnostics;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
		/// <summary>
		/// Allows lookup from a Dictionary of the specified value while returning the defaultValue 
		/// if either the key is null or if the key does not exist in the dictionary.
		/// </summary>
		[DebuggerStepThrough]
		public static TValue V<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
		{
			if(key != null && dictionary.TryGetValue(key, out TValue value))
				return value;
			return defaultValue;
		}

		/// <summary>
		/// Indirection to V (left as this is a more semantic description still).
		/// </summary>
		[DebuggerStepThrough]
		public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default)
			=> V(dictionary, key, defaultValue);

		/// <summary>
		/// Allows lookup of a key value from the input dictionary where the key is of type struct,
		/// returning NULL if value is not found.
		/// </summary>
		[DebuggerStepThrough]
		public static TValue? ValueN<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
		{
			// `key != null` IS needed bec string comes thru as struct (believe it or not)
			if(key != null && dictionary.TryGetValue(key, out TValue value))
				return value;
			return null;
		}

		[DebuggerStepThrough]
		public static bool TryGetValueAny<TKey, TVal>(this IDictionary<TKey, TVal> dict, out TVal val, params TKey[] values)
		{
			if(dict.NotNulle() && values.NotNulle()) {
				for(int i = 0; i < values.Length; i++)
					if(dict.TryGetValue(values[i], out val))
						return true;
			}
			val = default;
			return false;
		}

		public static KeyValuePair<TKey, TValue> GetKeyValue<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key)
		{
			return new KeyValuePair<TKey, TValue>(key, d[key]);
		}
	}
}
