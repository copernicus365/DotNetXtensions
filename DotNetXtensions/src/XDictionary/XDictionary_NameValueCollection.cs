// XDictionary_NameValueCollection

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
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

	}
}
