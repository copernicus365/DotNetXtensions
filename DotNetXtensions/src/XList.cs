using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extra LINQ type extension methods for Lists.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XList
	{
		[DebuggerStepThrough]
		public static List<TSource> ToListN<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
				return null;
			return source.ToList();
		}

		/// <summary>
		/// If this list is not-null, this function immediately returns it,
		/// else if the list is NULL it returns a new (empty) List T.
		/// </summary>
		[DebuggerStepThrough]
		public static List<T> E<T>(this List<T> list)
		{
			if (list == null) {
				var l = new List<T>();
				return l as List<T>;
			}
			return list;
		}






		public static List<T> AddN<T>(this List<T> list, T item)
		{
			if (list == null) throw new ArgumentNullException();
			list.Add(item);
			return list;
		}

		/// <summary>
		/// Adds KeyValuePair items, without having to generate stinking wordy 'new KeyValuePair TKey, TValue ()'
		/// </summary>
		/// <param name="list">List</param>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		public static List<KeyValuePair<TKey, TValue>> AddN<TKey, TValue>(this List<KeyValuePair<TKey, TValue>> list, TKey key, TValue value)
		{
			if (list != null)
				list.Add(new KeyValuePair<TKey, TValue>(key, value));
			return list;
		}


		/// <summary>
		/// Adds KeyValuePair items, without having to generate stinking wordy 'new KeyValuePair TKey, TValue ()'
		/// </summary>
		/// <param name="list">List</param>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		public static List<KeyValuePair<TKey, string>> AddN<TKey>(this List<KeyValuePair<TKey, string>> list, TKey key, object value)
		{
			if (list != null) {
				string strVal = value?.ToString();
				list.Add(new KeyValuePair<TKey, string>(key, strVal));
			}
			return list;
		}


		public static List<T> AddIf<T>(this List<T> list, bool add, T item)
		{
			if (add)
				list.Add(item);
			return list;
		}

		public static List<T> AddIfNotNull<T>(this List<T> list, T item)
		{
			if (item != null)
				list.Add(item);
			return list;
		}

		public static List<string> AddIfNotNulle(this List<string> list, string item)
		{
			if (item.NotNulle())
				list.Add(item);
			return list;
		}




		/// <summary>
		/// AddRange only exists on List (not IList).
		/// </summary>
		public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> items)
		{
			if (items != null) {
				foreach (T item in items)
					list.Add(item);
			}
			return list;
		}

		public static List<T> AddRangeN<T>(this List<T> list, IEnumerable<T> range)
		{
			if (range != null)
				list.AddRange(range);
			return list;
		}

		public static List<T> AddMany<T>(this List<T> list, params T[] items)
		{
			if (items.NotNulle())
				list.AddRange(items);
			return list;
		}

		public static List<T> AddRangeIf<T>(this List<T> list, bool add, IEnumerable<T> range)
		{
			if (add)
				list.AddRange(range);
			return list;
		}

		public static List<T> AddWhereIf<T>(this List<T> list, bool add, IEnumerable<T> range, Func<T, bool> pred)
		{
			if (add)
				return AddWhere(list, range, pred);
			return list;
		}


		public static List<T> AddWhere<T>(this List<T> list, IEnumerable<T> range, Func<T, bool> pred)
		{
			if (list != null && range != null && pred != null)
				foreach (var item in range)
					if (pred(item))
						list.Add(item);
			return list;
		}

		/// <summary>
		/// No null exception if list is null, and fixes if index is too high
		/// out of range (in that case simply adds item, which is fastest anyways).
		/// </summary>
		public static List<T> InsertN<T>(this List<T> list, int index, T item)
		{
			if (list != null) {
				if (index < 0) index = 0;
				if (index > list.Count)
					list.Add(item);
				else
					list.Insert(index, item);
			}
			return list;
		}

	}
}
