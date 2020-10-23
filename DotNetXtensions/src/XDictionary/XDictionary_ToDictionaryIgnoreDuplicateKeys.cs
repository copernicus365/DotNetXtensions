// XDictionary_ToDictionaryIgnoreDuplicateKeys

using System;
using System.Collections.Generic;

namespace DotNetXtensions
{
	public static partial class XDictionary
	{
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
			Func<TKey, TElement, TElement, TElement> handleDuplicate = null)
		//Func<Tuple<TKey, TElement, TElement>, TElement> handleDuplicate = null
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
	}
}
