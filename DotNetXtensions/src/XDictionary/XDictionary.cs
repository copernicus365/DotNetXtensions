using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for Dictionary.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static partial class XDictionary
	{

		#region --- Nulle ---

		public static bool Nulle(this DateTime? dt)
		{
			return dt == null || dt.Value == DateTime.MinValue
				? true
				: false;
		}

		public static bool IsNullOrEmpty(this DateTime? dt)
		{
			return dt == null || dt.Value == DateTime.MinValue
				? true
				: false;
		}

		public static bool IsEmpty(this DateTime dt)
		{
			return dt == DateTime.MinValue
				? true
				: false;
		}

		#endregion


		#region --- AddN ---

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

		public static KeyValuePair<TKey, TValue> GetKeyValue<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key)
		{
			return new KeyValuePair<TKey, TValue>(key, d[key]);
		}

		#endregion


		#region --- V ---

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
		public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
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

		#endregion

		public static bool NoKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
		{
			if(dict == null) return false;
			bool noExist = !dict.ContainsKey(key);
			return noExist;
		}

		#region --- NameValueCollection ---

		/// <summary>
		/// Converts the <see cref="NameValueCollection"/> into a `Dictionary(string, string)`.
		/// For keys that have more than one value, see <paramref name="forMultiValuesOnlyGetFirst"/>, 
		/// which as it says, will retrieve only the first value of any multi-valued keys (preference is 
		/// still given to any NotNullOrEmpty values). Otherwise, any multi-values will be returned as 
		/// a comma-separated list. Any null values are converted into an empty string.
		/// </summary>
		/// <param name="nvcoll">NameValueCollection</param>
		/// <param name="ignoreCase">
		/// Set to true to ignore the case of the keys / to make the dictionary case-INsensitive
		/// (default case-sensitive: false). Note, when this is set to false it will have
		/// no impact if the <see cref="NameValueCollection"/>'s comparer already was
		/// case-insensitive (such as HttpUtility.ParseQueryString returns), because any 
		/// such keys would already have been combined into one key-values pair.
		/// (FYI:'https://stackoverflow.com/a/24700171/264031')</param>
		/// <param name="forMultiValuesOnlyGetFirst">True to only get the first value when many
		/// values existed per key; preference is still given to any not null or empty values.
		/// If false, return value will be a comma-separated (no-space) string of all values,
		/// which standard comes from <see cref="NameValueCollection"/> </param>
		public static Dictionary<string, string> ToDictionary(
			this NameValueCollection nvcoll,
			bool ignoreCase = false,
			bool forMultiValuesOnlyGetFirst = false)
		{
			if(nvcoll == null)
				return null;

			IEqualityComparer<string> equalityComparer = ignoreCase
				? StringComparer.OrdinalIgnoreCase
				: StringComparer.Ordinal;

			var d = new Dictionary<string, string>(nvcoll.Count, equalityComparer);

			if(nvcoll.Count < 1)
				return d;

			foreach(var kv in nvcoll.ToEnumerable(forMultiValuesOnlyGetFirst)) {

				string val = kv.Value.TrimIfNeeded() ?? "";

				if(d.TryGetValue(kv.Key, out string currVal)) {

					if(currVal == null) currVal = "";

					if(forMultiValuesOnlyGetFirst) {
						// only add if original val was empty string and new val is notNulle
						if(val.NotNulle() && currVal.IsNulle())
							d[kv.Key] = val;
					}
					else {
						val = currVal + "," + val;
						d[kv.Key] = val;
					}
				}
				else {
					d[kv.Key] = val;
				}
			}

			return d;
		}

		/// <summary>
		/// Converts the <see cref="NameValueCollection"/> into a `Dictionary(string, string[])`,
		/// whose values are string arrays. For some other documentation see o
		/// </summary>
		/// <param name="nvcoll">Name value collection</param>
		/// <param name="ignoreCase">True to ignore the case of the keys
		/// (false by default). See notes on related `ToDictionary` method.</param>
		public static Dictionary<string, string[]> ToDictionaryMultiValues(
			this NameValueCollection nvcoll,
			bool ignoreCase = false)
		{
			if(nvcoll == null)
				return null;

			IEqualityComparer<string> equalityComparer = ignoreCase
				? StringComparer.OrdinalIgnoreCase
				: StringComparer.Ordinal;

			var d = new Dictionary<string, string[]>(nvcoll.Count, equalityComparer);

			if(nvcoll.Count < 1)
				return d;

			foreach(var kv in nvcoll.ToEnumerableMultiValues()) {

				string[] values = kv.Value;

				if(d.TryGetValue(kv.Key, out string[] currValues)) {
					values = values.ConcatToArray(currValues);
				}

				d[kv.Key] = values;
			}

			return d;
		}

		/// <summary>
		/// Returns an enumerable <see cref="KeyValuePair{TKey, TValue}"/> from the <see cref="NameValueCollection"/>.
		/// If the key has multiple values, by default, true to how the NVM works,
		/// a comma-separated string of all of the values will be set as the value. 
		/// Or set <paramref name="forMultiValuesOnlyGetFirst"/> to true to only retrieve the 
		/// first of the values.
		/// </summary>
		public static IEnumerable<KeyValuePair<string, string>> ToEnumerable(
			this NameValueCollection nvcoll,
			bool forMultiValuesOnlyGetFirst = false)
		{
			if(nvcoll != null) {
				foreach(string key in nvcoll.AllKeys) {

					if(forMultiValuesOnlyGetFirst) {

						// IMPORTANT! From brief internal inspection I believe the NameValueCollection
						// ALREADY stores the multi-values as string arrays, so I don't think 
						// calling GetValues hurts. Nonetheless, do this if they ask for it
						string[] values = nvcoll.GetValues(key);
						string val = values
							?.FirstOrDefault() // -- note: this is not a collection iterator, there's no predicate
							?.NullIfEmptyTrimmed();

						if(val == null && values.CountN() > 1) {
							// Perf! This AVOIDS these LINQ operations if first already existed and not nulle and if Length was already == 1
							val = values?
								.Select(v => v.NullIfEmptyTrimmed())
								.FirstOrDefault(v => v.NotNulle());
						}

						yield return new KeyValuePair<string, string>(key, val);
					}
					else {
						string value = nvcoll[key];
						yield return new KeyValuePair<string, string>(key, value);
					}
				}
			}
		}

		public static IEnumerable<KeyValuePair<string, string[]>> ToEnumerableMultiValues(this NameValueCollection nvcoll)
		{
			if(nvcoll != null) {
				foreach(string key in nvcoll.AllKeys) {
					string[] values = nvcoll.GetValues(key);
					yield return new KeyValuePair<string, string[]>(key, values);
				}
			}
		}

		#endregion

		#region --- ToDictionaryIgnoreDuplicateKeys ---

		/// <summary>
		/// Converts source into a Dictionary, but in the process IGNORES any duplicate key items
		/// (subsequent items that already match the key are not added).
		/// After creating the dictionary from source, it is just a regular dictionary, the ignoring
		/// of the same keys applies only to geration time within this function.
		/// </summary>
		public static Dictionary<TSource, TSource> ToDictionaryIgnoreDuplicateKeys<TSource>(
			this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer = null)
		{
			return source.ToDictionaryIgnoreDuplicateKeys(s => s, s => s, comparer);
		}

		public static Dictionary<TKey, TSource> ToDictionaryIgnoreDuplicateKeys<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			IEqualityComparer<TKey> comparer = null)
		{
			return source.ToDictionaryIgnoreDuplicateKeys(keySelector, s => s, comparer);
		}

		/// <summary>
		/// Creates a new Dictionary from source which ignores duplicate keys if they
		/// already existed (first in wins). To handle the duplicates yourself set 
		/// <paramref name="handleDuplicate"/>. In that case, 
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TElement"></typeparam>
		/// <param name="source"></param>
		/// <param name="keySelector"></param>
		/// <param name="elementSelector"></param>
		/// <param name="comparer"></param>
		/// <param name="handleDuplicate">
		/// If not null, this Func will be called when there is a duplicate,
		/// the return value will be set as the new value. The 3 input parameters
		/// align with the following example call: 
		/// `handleDuplicate: (key, currentVal, newVal) => $"{currentVal},{newVal}"`</param>
		public static Dictionary<TKey, TElement> ToDictionaryIgnoreDuplicateKeys<TSource, TKey, TElement>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TSource, TElement> elementSelector,
			IEqualityComparer<TKey> comparer = null,
			Func<TKey, TElement, TElement, TElement> handleDuplicate = null) //Func<Tuple<TKey, TElement, TElement>, TElement> handleDuplicate = null
		{
			if(keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if(elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

			var d = comparer == null
				? new Dictionary<TKey, TElement>()
				: new Dictionary<TKey, TElement>(comparer);

			if(source == null)
				return d;

			bool handleDups = handleDuplicate != null;

			foreach(TSource item in source) {
				TKey key = keySelector(item);
				TElement val = elementSelector(item);
				if(!d.TryGetValue(key, out TElement currVal)) {
					d.Add(key, val);
				}
				else if(handleDups) { // ELSE: Ignore, first value in wins ignore next
									  //TElement newVal = handleDuplicate(new Tuple<TKey, TElement, TElement>(key, currVal, val));
					TElement newVal = handleDuplicate(key, currVal, val);
					d[key] = newVal;
				}
			}
			return d;
		}

		#endregion

		/// <summary>
		/// Creates a new Dictionary whose values are a List of TElement, making it a 'MultiValueDictionary'.
		/// </summary>
		public static Dictionary<TKey, List<TSource>> ToMultiValueDictionary<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			IEqualityComparer<TKey> comparer = null)
		{
			return source.ToMultiValueDictionary(keySelector, e => e, comparer);
		}

		/// <summary>
		/// Creates a new Dictionary whose values are a List of TElement, making it a 'MultiValueDictionary'.
		/// </summary>
		public static Dictionary<TKey, List<TElement>> ToMultiValueDictionary<TSource, TKey, TElement>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TSource, TElement> elementSelector,
			IEqualityComparer<TKey> comparer = null)
		{
			if(keySelector == null) throw new ArgumentNullException(nameof(keySelector));
			if(elementSelector == null) throw new ArgumentNullException(nameof(elementSelector));

			var d = comparer == null
				? new Dictionary<TKey, List<TElement>>()
				: new Dictionary<TKey, List<TElement>>(comparer);

			if(source == null)
				return d;

			foreach(TSource item in source) {

				TKey key = keySelector(item);
				TElement val = elementSelector(item);

				List<TElement> list; // currVal;
				if(!d.TryGetValue(key, out list)) {
					list = new List<TElement>(source.CountIfCollection(defaultVal: 16));
					d.Add(key, list);
				}
				list.Add(val);
			}
			return d;
		}



		#region --- ToGroupedDictionary / GroupByQuick ---

		//public static Dictionary<TKey, GroupKV<TSource, TKey>> ToGroupedDictionary<TSource, TKey>(
		//	this IEnumerable<TSource> source,
		//	Func<TSource, TKey> keySelector)
		//{
		//	if (source == null) return null;
		//	if (keySelector == null) throw new ArgumentNullException();

		//	var dict = new Dictionary<TKey, GroupKV<TSource, TKey>>();

		//	foreach (var item in source) {
		//		TKey key = keySelector(item);

		//		GroupKV<TSource, TKey> grp;
		//		if (dict.TryGetValue(key, out grp) == false)
		//			dict[key] = new GroupKV<TSource, TKey>(item) { Key = key };
		//		else
		//			grp.Add(item);
		//	}
		//	return dict;
		//}

		public static Dictionary<TKey, List<TSource>> ToGroupedDictionary<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			if(source == null) return null;
			if(keySelector == null) throw new ArgumentNullException();

			var dict = new Dictionary<TKey, List<TSource>>();

			foreach(var item in source) {
				TKey key = keySelector(item);

				List<TSource> grp;
				if(dict.TryGetValue(key, out grp) == false) {
					var list = new List<TSource>();
					list.Add(item);
					dict[key] = list;
				}
				else
					grp.Add(item);
			}
			return dict;
		}

		//public static List<GroupKV<TSource, TKey>> GroupByQuick<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		//{
		//	var dict = ToGroupedDictionary(source, keySelector);
		//	var list = dict.Select(kv => kv.Value).ToList();
		//	return list;
		//}

		#endregion



		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="dict">The source dictionary.</param>
		/// <param name="ignore">Values to ignore.</param>
		/// <returns></returns>
		public static Dictionary<TValue, TKey> ReverseDictionary<TKey, TValue>(
			this Dictionary<TKey, TValue> dict,
			IEnumerable<TKey> ignore = null)
		//bool firstInWins = false)
		{
			if(dict == null)
				return null;

			Dictionary<TKey, TKey> ignoreD = ignore != null ? ignore.ToDictionary(k => k) : null;
			if(ignoreD.IsNulle())
				ignoreD = null;

			bool chkIgnoreD = ignoreD != null;

			Dictionary<TValue, TKey> rdict = new Dictionary<TValue, TKey>(dict.Count);

			foreach(var kv in dict) {
				if(chkIgnoreD && ignoreD.ContainsKey(kv.Key))
					continue;

				//if (firstInWins && rdict.ContainsKey(kv.Value))
				//	continue;

				rdict.Add(kv.Value, kv.Key);
			}
			return rdict;
		}

		#region IDictionary (non-generic)

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

		#endregion

	}

}
