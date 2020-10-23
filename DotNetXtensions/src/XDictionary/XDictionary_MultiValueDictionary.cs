// XDictionary_MultiValueDictionary

using System;
using System.Collections.Generic;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
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

		public static Dictionary<TKey, List<TSource>> ToGroupedDictionary<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			if(source == null) return null;
			if(keySelector == null) throw new ArgumentNullException();

			var dict = new Dictionary<TKey, List<TSource>>();

			foreach(var item in source) {
				TKey key = keySelector(item);

				if(dict.TryGetValue(key, out List<TSource> grp) == false) {
					var list = new List<TSource>();
					list.Add(item);
					dict[key] = list;
				}
				else
					grp.Add(item);
			}
			return dict;
		}

	}
}
