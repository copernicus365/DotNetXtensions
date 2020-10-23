using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetXtensions
{
	/// <summary>
	/// Extra LINQ type extension methods, particularly for collections.
	/// </summary>
	public static class XLinq
	{

		#region --- Nulle (etc) ---

		[DebuggerStepThrough]
		public static bool IsNulle<TSource>(this TSource[] source)
		{
			return (source == null || source.Length == 0)
				? true
				: false;
		}
		//test2
		[DebuggerStepThrough]
		public static bool IsNulle<TSource>(this ICollection<TSource> source)
		{
			return (source == null || source.Count == 0)
				? true
				: false;
		}

		/// <summary>
		/// Returns null if source is null or empty, else returns source.
		/// </summary>
		[DebuggerStepThrough]
		public static T[] NullIfEmpty<T>(this T[] arr)
		{
			if (arr == null || arr.Length == 0)
				return null;
			return arr;
		}

		/// <summary>
		/// Returns null if source is null or empty, else returns source.
		/// </summary>
		[DebuggerStepThrough]
		public static List<T> NullIfEmpty<T>(this List<T> list)
		{
			if (list == null || list.Count == 0)
				return null;
			return list;
		}

		[DebuggerStepThrough]
		public static bool NotNulle<TSource>(this TSource[] source)
		{
			return !(source == null || source.Length == 0)
				? true
				: false;
		}

		[DebuggerStepThrough]
		public static bool NotNulle<TSource>(this ICollection<TSource> source)
		{
			return !(source == null || source.Count == 0)
				? true
				: false;
		}

		/// <summary>
		/// Returns if source is null or has no items, checked as efficiently as 
		/// by getting an enumerator and performing a single MoveNext call. This is exactly what Linq.Any 
		/// (without a parameter) does, but of course it throws an exception for null.
		/// NOTE: We would have liked to name this Nulle, or IsNulle, but that conflicts with 
		/// other interfaces and types (List, IList, ICollection, etc) which also implement IEnumerable. 
		/// </summary>
		/// <typeparam name="TSource"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		[DebuggerStepThrough]
		public static bool IsNullOrHasNone<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
				return true;
			// https://github.com/dotnet/corefx/blob/master/src/System.Linq/src/System/Linq/AnyAll.cs

			using (IEnumerator<TSource> e = source.GetEnumerator())
				return !e.MoveNext();
		}

		[DebuggerStepThrough]
		public static TSource[] ToArrayN<TSource>(this IEnumerable<TSource> source)
		{
			if (source == null)
				return null;
			return source.ToArray();
		}




		public static bool IsNulle<TValue>(this Nullable<TValue> value) where TValue : struct
		{
			return (value != null && !value.Value.Equals(default(TValue)))
				? false
				: true;
		}

		public static bool NotNulle<TValue>(this Nullable<TValue> value) where TValue : struct
		{
			return !value.IsNulle();
		}

		#endregion

		#region --- LengthN / CountN ---

		[DebuggerStepThrough]
		public static int LengthN<T>(this T[] arr, int defaultVal = 0)
		{
			return arr == null ? defaultVal : arr.Length;
		}



		[DebuggerStepThrough]
		public static int LengthN(this string s, int defaultVal = 0)
		{
			return s == null ? defaultVal : s.Length;
		}

		[DebuggerStepThrough]
		public static int CountN(this string s, int defaultVal = 0)
		{
			return s == null ? defaultVal : s.Length;
		}



		[DebuggerStepThrough]
		public static int CountN<T>(this IList<T> list, int defaultVal = 0)
		{
			return list == null ? defaultVal : list.Count;
		}

		[DebuggerStepThrough]
		public static int CountN<T>(this ICollection<T> coll, int defaultVal = 0)
		{
			return coll == null ? defaultVal : coll.Count;
		}

		//[DebuggerStepThrough]
		//public static int CountN<TKey, TValue>(this Dictionary<TKey, TValue> d, int defaultVal = 0)
		//{
		//	return d == null ? defaultVal : d.Count;
		//}


		[DebuggerStepThrough]
		public static int CountIfCollection<T>(this IEnumerable<T> source, int defaultVal = -1)
		{
			ICollection<T> collection = source as ICollection<T>;
			return collection != null ? collection.Count : defaultVal;
		}


		#endregion

		#region E

		/// <summary>
		/// If the object is null, this function will return a new instance of the object 
		/// (which is possible because this type is constrained by the new() constraint),
		/// else returns the non-null object immediately.
		/// </summary>
		/// <typeparam name="T">Type that has a parameterless constructor.</typeparam>
		/// <param name="t">Object.</param>
		[DebuggerStepThrough]
		public static T E<T>(this T t) where T : class, new()
		{
			if (t != null)
				return t;
			return new T();
		}

		[DebuggerStepThrough]
		public static T V<T>(this Nullable<T> t) where T : struct
		{
			return t.GetValueOrDefault();
		}

		[DebuggerStepThrough]
		public static T V<T>(this Nullable<T> t, T defaultValue) where T : struct
		{
			return t.GetValueOrDefault(defaultValue);
		}

		/// <summary>
		/// Returns an empty string if the string is empty.
		/// </summary>
		/// <param name="s">This string.</param>
		[DebuggerStepThrough]
		public static string E(this string s)
		{
			return s == null ? "" : s;
		}

		/// <summary>
		/// If this array is not-null, this function immediately returns it,
		/// else if the array is NULL it returns an empty T array.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="array">Array.</param>
		[DebuggerStepThrough]
		public static T[] E<T>(this T[] array)
		{
			return array != null ? array : new T[0];
		}

		/// <summary>
		/// If this enumerable is not-null, this function immediately returns it,
		/// else if the enumerable is NULL it returns an empty T array.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="enumerable"></param>
		[DebuggerStepThrough]
		public static IEnumerable<T> E<T>(this IEnumerable<T> enumerable)
		{
			return enumerable != null ? enumerable : new T[0];
		}



		[DebuggerStepThrough]
		public static T? NullIfD<T>(this T t) where T : struct
		{
			if (t.Equals(default(T)))
				return null;
			return t;
		}

		#endregion

		#region ToStringN

		/// <summary>
		/// Returns ToString on the input object if it is not null, else returns the 
		/// specified string value.
		/// value
		/// </summary>
		/// <param name="obj">Object.</param>
		/// <param name="defaultIfNull">Default value to return if obj is null.</param>
		[DebuggerStepThrough]
		public static string ToStringN(this object obj, string defaultIfNull = null)
		{
			return obj == null ? defaultIfNull : obj.ToString();
		}

		[DebuggerStepThrough]
		public static string ToStringN<T>(this Nullable<T> val, string defaultIfNull = null) where T : struct
		{
			return val == null
				? (defaultIfNull ?? default(T).ToString())
				: val.ToString();
		}

		[DebuggerStepThrough]
		public static string ToStringN<T>(this Nullable<T> val, string defaultIfNull, string formatStr, IFormatProvider provider = null) where T : struct, IFormattable
		{
			if (val == null)
				return defaultIfNull ?? default(T).ToString();

			return formatStr == null
					? val.Value.ToString()
					: val.Value.ToString(formatStr, provider);
		}

		#endregion

		#region FirstN

		//[DebuggerStepThrough]
		//public static T FirstOrDefaultN<T>(this IEnumerable<T> seq)
		//{
		//	return seq == null ? default(T) : seq.FirstOrDefault();
		//}

		//[DebuggerStepThrough]
		//public static T FirstOrDefaultN<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
		//{
		//	return seq == null ? default(T) : seq.FirstOrDefault(predicate);
		//}

		[DebuggerStepThrough]
		public static T FirstN<T>(this IEnumerable<T> seq)
		{
			return seq == null ? default(T) : seq.FirstOrDefault();
		}

		[DebuggerStepThrough]
		public static T FirstN<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
		{
			return seq == null ? default(T) : seq.FirstOrDefault(predicate);
		}

		[DebuggerStepThrough]
		public static T LastN<T>(this IEnumerable<T> seq)
		{
			return seq == null ? default(T) : seq.LastOrDefault();
		}

		[DebuggerStepThrough]
		public static T LastN<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
		{
			return seq == null ? default(T) : seq.LastOrDefault(predicate);
		}

		#endregion

		/// <summary>
		/// Determines whether a sequence contains no elements or is null
		/// (this just returns the opposite of Any, but allows a null sequence).
		/// </summary>
		public static bool None<T>(this IEnumerable<T> seq)
		{
			if (seq == null)
				return false;
			return !seq.Any();
		}

		/// <summary>
		/// Determines whether a sequence contains no elements that match the condition
		/// (this just returns the opposite of Any, but allows a null sequence).
		/// </summary>
		public static bool None<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
		{
			if (seq == null)
				return false;
			if (predicate == null)
				throw new ArgumentNullException();
			return !seq.Any(predicate);
		}

		#region --- Except ---

		public static IEnumerable<T> ExceptIf<T, TOther>(this IEnumerable<T> source,
			bool condition, IDictionary<T, TOther> exceptDict)
		{
			return !condition ? source : Except(source, exceptDict);
		}

		public static IEnumerable<T> Except<T, TOther>(this IEnumerable<T> source,
			IDictionary<T, TOther> exceptDict) //where T : IEquatable<T>
		{
			if (source == null || exceptDict.IsNulle()) return source;
			return source.Where(itm => !exceptDict.ContainsKey(itm));
		}

		public static IEnumerable<T> ExceptIf<T>(this IEnumerable<T> source, bool condition, params T[] except)
		{
			return !condition ? source : Except(source, except);
		}

		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, params T[] except)
		{
			if (source == null || except.IsNulle()) return source;

			var d = new Dictionary<T, T>();

			foreach (var itm in except)
				if (!d.ContainsKey(itm))
					d[itm] = itm;

			return source.Except(d);
		}

		#endregion

		#region --- ReplaceMatch ---

		public static IEnumerable<T> ReplaceMatch<T>(this IEnumerable<T> source, T match, T replace, bool canMatchMany = true) where T : IEquatable<T>
		{
			return ReplaceMatchIf(source, true, match, replace, canMatchMany);
		}

		public static IEnumerable<T> ReplaceMatchIf<T>(this IEnumerable<T> source, bool condition, T match, T replace, bool canMatchMany = true) where T : IEquatable<T>
		{
			if (source != null && condition) {
				bool matchNotNull = match != null;
				bool cont = true;
				foreach (var item in source) {
					if (cont && matchNotNull && item != null && item.Equals(match)) {
						yield return replace;
						if (!canMatchMany)
							cont = false;
					}
					else
						yield return item;
				}
			}
		}

		#endregion

		#region --- OrderBy ---

		public static IEnumerable<T> OrderByIf<T, TKey>(this IEnumerable<T> source, bool doOrder, Func<T, TKey> keySelector)
		{
			if (source == null || !doOrder)
				return source;
			return source.OrderBy(keySelector);
		}

		public static IEnumerable<T> OrderByIf<T, TKey>(this IEnumerable<T> source, bool doOrder, bool ascending, Func<T, TKey> keySelector)
		{
			if (source == null || !doOrder)
				return source;
			return ascending
				? source.OrderBy(keySelector)
				: source.OrderByDescending(keySelector);
		}


		public static IEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> source, bool ascending, Func<T, TKey> keySelector)
		{
			if (source == null)
				return source;
			return ascending
				? source.OrderBy(keySelector)
				: source.OrderByDescending(keySelector);
		}



		public static IEnumerable<string> OrderByNatural(this IEnumerable<string> items, StringComparer stringComparer = null)
		{
			return items.OrderByNatural(s => s, stringComparer);
		}

		public static IEnumerable<T> OrderByNatural<T>(this IEnumerable<T> items, Func<T, string> selector, StringComparer stringComparer = null)
		{
			var regex = new Regex(@"\d+", RegexOptions.Compiled);

			int maxDigits = items
				.SelectMany(i => regex
					.Matches(selector(i))
					.Cast<Match>()
					.Select(digitChunk => (int?)digitChunk.Value.Length))
				.Max() ?? 0;

			return items.OrderBy(
						i => regex.Replace(selector(i),
						match => match.Value.PadLeft(maxDigits, '0')),
						stringComparer ?? StringComparer.Ordinal);
		}

		#endregion

		#region --- IsSorted / AnySortedDuplicates  ---

		/// <summary>
		/// Iterates through the sorted sequence testing each item 
		/// with the previously gotten item. If <paramref name="predicateCompareLastToCurr"/>
		/// returns true immediately returns true, else false. This allows one
		/// to test any condition that can be tested by comparing the last item to current
		/// in a sorted sequence. For instance, if the sequence was ints, you could verify 
		/// if any item is ever more than 10 greater than the previous item like follows: 
		/// <c><paramref name="predicateCompareLastToCurr"/>: (tLast, tCurr) =&gt; (tCurr - tLast) &gt; 10</c>
		/// One can also test if the sequence has any duplicates for instance if: <c>(tLast, tCurr) =&gt; tCurr == tLast</c>
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <param name="seq">Sequence</param>
		/// <param name="predicateCompareLastToCurr">Compares the last gotten item with the current
		/// item.</param>
		public static bool AnySortedCondition<T>(this IEnumerable<T> seq, Func<T, T, bool> predicateCompareLastToCurr)
		{
			if (seq == null) return false;

			T last = seq.First();

			foreach (T item in seq.Skip(1)) {
				if (predicateCompareLastToCurr(last, item))
					return true;
				last = item;
			}
			return false;
		}

		/// <summary>
		/// Determines if there are any duplicates in this sorted sequence or if the sequence 
		/// is not actually sorted, either case of which will immediately return true.
		/// If it returns false it means every item in the sequence is sequential.
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <param name="seq">Sequence</param>
		/// <param name="descending">True if the sequence is reverse sorted.</param>
		public static bool AnySortedDuplicates<T>(this IEnumerable<T> seq, bool descending = false) where T : IComparable<T>
		{
			return seq.AnySortedDuplicates((tLast, tCurr) => tLast.CompareTo(tCurr), descending);
		}

		public static bool AnySortedDuplicates<T, TKey>(this IEnumerable<T> seq, Func<T, TKey> getKey, bool descending = false) where TKey : IComparable<TKey>
		{
			return seq?.Select(t => getKey(t)).AnySortedDuplicates(descending) ?? false;
		}

		public static bool AnySortedDuplicates<T>(this IEnumerable<T> seq, Func<T, T, int> comparerLastToCurrent, bool descending = false)
		{
			if (descending) comparerLastToCurrent = reverseComparer(comparerLastToCurrent);
			return seq.AnySortedCondition((tLast, tCurr) => comparerLastToCurrent(tLast, tCurr) >= 0);
		}

		static Func<T, T, int> reverseComparer<T>(Func<T, T, int> comparer)
		{
			Func<T, T, int> f = (t1, t2) => {
				int compared = comparer(t1, t2);
				if (compared > 0)
					return -1;
				if (compared < 0)
					return 1;
				return 0;
				// since we're only dealing with ints (where we can trust the great than 
				// or lesser than operators), it is IMPOSSIBLE that this is not equal
			};
			return f;
		}



		public static bool IsSorted<T>(this IEnumerable<T> seq, Func<T, T, int> predicateCompareLastToCurr, out bool anyDuplicates, bool descending = false)
		{
			anyDuplicates = false;
			if (seq == null) return false;

			if (descending)
				predicateCompareLastToCurr = reverseComparer(predicateCompareLastToCurr);

			T last = seq.First();

			foreach (T item in seq.Skip(1)) {
				int compare = predicateCompareLastToCurr(last, item);
				if (compare >= 0) {
					if (compare == 0) {
						anyDuplicates = true;
					}
					else if (compare > 0) {
						anyDuplicates = true;
						return false;
					}
				}
				last = item;
			}
			return true;
		}

		public static bool IsSorted<T>(this IEnumerable<T> seq, bool descending = false) where T : IComparable<T>
		{
			bool anyDuplicates;
			return seq.IsSorted(out anyDuplicates, descending);
		}

		/// <summary>
		/// Determines if the sequence is sorted by a single iteration through
		/// comparing the last item to current item. At the same time it is 
		/// determined if the sequence had any duplicate items (this value could
		/// only be correct with our single sequential test if the sequence actually is sorted,
		/// so if !isSorted, this value will also always be set to true).
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <param name="seq">Sequence</param>
		/// <param name="anyDuplicates">Any duplicates will be indicated in this
		/// value IF the sequence was actually sorted.</param>
		/// <param name="descending">True if checking for descending sort</param>
		public static bool IsSorted<T>(this IEnumerable<T> seq, out bool anyDuplicates, bool descending = false) where T : IComparable<T>
		{
			return seq.IsSorted((tLast, tCurr) => tLast.CompareTo(tCurr), out anyDuplicates, descending);
		}

		public static bool IsSorted<T, TKey>(this IEnumerable<T> seq, Func<T, TKey> getKey, out bool anyDuplicates, bool descending = false) where TKey : IComparable<TKey>
		{
			anyDuplicates = false;
			return seq?.Select(t => getKey(t)).IsSorted(out anyDuplicates, descending) ?? false;
		}

		public static bool IsSorted<T, TKey>(this IEnumerable<T> seq, Func<T, TKey> getKey, bool descending = false) where TKey : IComparable<TKey>
		{
			return seq?.Select(t => getKey(t)).IsSorted(descending) ?? false;
		}

		#endregion

		public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(
			this IEnumerable<TSource> source,
			Func<TSource, TResult> selector,
			Func<TResult, bool> predicate = null)
		{
			foreach (var item in source) {
				if (item != null) {
					TResult r = selector(item);
					if (r != null) {
						if (predicate == null || predicate(r))
							yield return r;
					}
				}
			}
		}

		/// <summary>
		/// Allows one to conditionally alter the sequence while staying within a LINQ chain.
		/// If doIf is false, source is immediately returned with no consequence, else source items
		/// are passed to alterItems which the caller implements. It's possible that code could do nothing
		/// but immediately return the same source items, but it's there that they would be able to perform
		/// some LINQ transformation now that the condition was met.
		/// </summary>
		/// <typeparam name="TSource">Source type.</typeparam>
		/// <param name="source">Source items.</param>
		/// <param name="doIf">True to have the source items transformed via alterItems. False will immediately return source items.</param>
		/// <param name="alterItems">Gives to caller the source items at this point in the LINQ chain.</param>
		public static IEnumerable<TSource> DoIf<TSource>(this IEnumerable<TSource> source, bool doIf, Func<IEnumerable<TSource>, IEnumerable<TSource>> alterItems)
		{
			if (!doIf)
				return source;
			return alterItems(source);
		}

		#region --- Distinct ---

		/// <summary>
		/// Returns distinct elements from a sequence, using the default equality comparer
		/// *for the given TKey*. What sets DistinctBy apart from LINQ's Distinct is the ability
		/// to get the key from the object, rather than having to test equality on the object itself.
		/// Author: Jon Skeet: http://stackoverflow.com/a/489421/264031
		/// </summary>
		/// <typeparam name="TSource">Sequence type.</typeparam>
		/// <typeparam name="TKey">Key type</typeparam>
		/// <param name="source">Sequence</param>
		/// <param name="keySelector">Key</param>
		public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source) {
				if (seenKeys.Add(keySelector(element))) {
					yield return element;
				}
			}
		}

		/// <summary>
		/// Returns a distinct collection from the input sequence while allowing 
		/// any duplicate items to first be altered (or filtered still) with <paramref name="handleNonUniqueItem"/>.
		/// Note that if the return result of <paramref name="handleNonUniqueItem"/> is non-null it will
		/// be added to the result *even if* the key from the returned item is still a duplicate
		/// (this is why we pass into <paramref name="handleNonUniqueItem"/> the dictionary of keys already
		/// found, to allow the consumer to see if the new key they may have generated is itself unique).
		/// <para/>
		/// A typical scenario is that <paramref name="handleNonUniqueItem"/> 
		/// will be used to alter the id or key of the source item. 
		/// Other useful things can be done with this, such as using  <paramref name="handleNonUniqueItem"/> as a
		/// means to get ahold of all the items that were duplicates. Even if one still needs 
		/// to filter those from the return sequence, it could come in handy, such as for
		/// getting a list of ids that are being (perhaps erroneously) duplicated.
		/// </summary>
		/// <typeparam name="TSource">Source type.</typeparam>
		/// <typeparam name="TKey">Key type.</typeparam>
		/// <param name="source">Source sequence.</param>
		/// <param name="keySelector">Key selector.</param>
		/// <param name="handleNonUniqueItem">
		/// For every item in the source sequence whose key is null or which is a duplicate of a key already encountered,
		/// this func will be called with input parameters of that item and of the dictionary of key-items that have been
		/// built up to that point in time of enumerating the source sequence. If the *TSource returned item* 
		/// is null, it will be filtered from the return sequence, else it will be added to it, even if 
		/// the new item's key is still a duplicate or null. The returned item's key
		/// will be added to the dictionary if it is non-null and a duplicate key. 
		/// </param>
		public static IEnumerable<TSource> Distinct<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			Func<TSource, Dictionary<TKey, TSource>, TSource> handleNonUniqueItem)
		{
			if (source == null) return new TSource[0];

			var icoll = (source as ICollection<TSource>);
			int initCount = icoll != null ? icoll.Count : 100;
			bool hasHandler = handleNonUniqueItem != null;
			//TSource dflt = default(TSource);
			//bool isNullable = TSource is Nullable;

			var result = new List<TSource>(initCount);
			Dictionary<TKey, TSource> dict = null;
			if (dict == null)
				dict = new Dictionary<TKey, TSource>(initCount);

			foreach (TSource item in source) {

				if (item == null) // || item.Equals(dflt)) // could do this instead later, ... cogitating
					continue;

				TKey key = keySelector(item);

				if (key != null && !dict.ContainsKey(key)) {
					dict.Add(key, item);
					result.Add(item);
				}
				else if (hasHandler) {
					TSource newItem = handleNonUniqueItem(item, dict);
					if (newItem != null) {
						TKey newKey = keySelector(newItem);
						if (newKey != null && !dict.ContainsKey(newKey))
							dict.Add(newKey, newItem);
						result.Add(newItem);
					}
				}
			}
			return result;
		}

		public static bool AnyDuplicates<T>(this IEnumerable<T> seq, IEqualityComparer<T> comparer = null)
		{
			if (seq == null) return false;

			Dictionary<T, bool> dict = comparer == null
				? new Dictionary<T, bool>()
				: new Dictionary<T, bool>(comparer);

			foreach (T item in seq) {
				if (dict.ContainsKey(item))
					return true;
				dict.Add(item, false);
			}
			return false;
		}

		#endregion

		#region --- JoinToString ---

		// See XString for the string type equivalent (using the more efficient String.Join for that type)

		static string _jsr(string result, string separator = ",")
		{
			return (result.IsNulle() || result != separator)
				? result
				: "";
		}

		//[DebuggerStepThrough]
		public static string JoinToString<T>(this IEnumerable<T> seq, Func<T, string> tToString, string separator = ",", bool winnowEmptyResults = false)
		{
			if (seq == null) return null;
			if (separator == null) separator = "";

			if (tToString == null) {
				T defT = default(T);
				string defTStr = defT == null ? null : defT.ToString();
				tToString = t => t.ToStringN(defTStr);
			}

			StringBuilder sb = new StringBuilder("");

			foreach (T item in seq) {
				string tStr = tToString(item);
				if (!winnowEmptyResults || tStr.NotNulle())
					sb.Append(tStr + separator);
			}

			if (sb.Length == 0)
				return "";
			if (separator.Length > 0 && sb.Length > separator.Length)
				sb.Length = sb.Length - separator.Length; // get rid of the last separator append

			return sb.ToString();
			//return _jsr(sb.ToString(), separator);
		}

		[DebuggerStepThrough]
		public static string JoinToString<T>(this IEnumerable<T> thisEnumerable, string separator = ",")
		{
			if (thisEnumerable == null) return null;
			if (separator == null) throw new ArgumentNullException("separator");

			StringBuilder sb = new StringBuilder("");

			foreach (T item in thisEnumerable)
				sb.Append(item.ToString() + separator);

			if (sb.Length == 0)
				return "";
			if (sb.Length > separator.Length)
				sb.Length = sb.Length - separator.Length; // get rid of the last separator append

			return _jsr(sb.ToString(), separator);
		}

		/* Functions with type string below call String.Join, which seems to be quicker.  
		 * In our tests, a couple times faster after JIT. */

		[DebuggerStepThrough]
		public static string JoinToString(this string[] arr, string separator = ",")
		{
			return _jsr(String.Join(separator, arr));
		}

		[DebuggerStepThrough]
		public static string JoinToString(this string[] arr, string separator, int index, int count)
		{
			return _jsr(String.Join(separator, arr, index, count), separator);
		}

		#endregion

		[DebuggerStepThrough]
		public static T[] TakeRange<T>(this IList<T> items, int index, int count)
		{
			if (items == null)
				return null;
			int len = items.Count;
			if (index >= len)
				return null;
			int cnt = Math.Min(count, len - index);
			if (cnt < 1)
				return null;

			T[] arr = new T[cnt];
			for (int i = 0; i < cnt; i++)
				arr[i] = items[i + index];

			return arr;
		}


		/// <summary>
		/// Splits the sequence up into arrays of the specified size.
		/// Efficiently handles if source is already less than or equal to 
		/// the split size if sequence is ICollection, in which case 
		/// <remarks>
		/// Code influenced by 'SLaks': http://stackoverflow.com/a/5215506/264031
		/// </remarks>
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <param name="sequence">Input sequence.</param>
		/// <param name="size">Split size.</param>
		public static IEnumerable<T[]> Split<T>(this IEnumerable<T> sequence, int size)
		{
			var icoll = sequence as ICollection;
			if (icoll != null && icoll.Count <= size) {
				yield return sequence.ToArray();
			}
			else {
				T[] arr = new T[size];
				int i = 0;
				foreach (var item in sequence) {
					arr[i++] = item;
					if (i == size) {
						yield return arr;
						arr = new T[size];
						i = 0;
					}
				}
				if (i > 0)
					yield return arr.Take(i).ToArray();
			}
		}


		#region --- Count, Average ... (Aggregates) ---

		/// <summary>
		/// Returns the count of this IEnumerable if it is in fact an ICollection 
		/// (generic or regular, which includes Array). Else returns null.  
		/// </summary>
		/// <typeparam name="TSource">TSource</typeparam>
		/// <param name="source">Source</param>
		public static int? CountIfCollection<TSource>(IEnumerable<TSource> source)
		{
			if (source == null) return null;
			ICollection coll = (ICollection)(source as ICollection<TSource>) ?? source as ICollection;
			if (coll != null)
				return coll.Count;
			return null;
		}

		/// <summary>
		/// Returns the count of this IEnumerable if it is in fact an ICollection 
		/// (generic or regular, which includes Array). Else returns null.  
		/// </summary>
		/// <param name="source">Source</param>
		public static int? CountIfCollection(IEnumerable source)
		{
			if (source == null) return null;
			ICollection coll = source as ICollection;
			if (coll != null)
				return coll.Count;
			return null;
		}


		/// <summary>Computes the median of a sequence of items that have an int gotten from the selector.</summary>
		public static int Median<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null) return 0;
			return source.Select(i => selector(i)).Median();
		}

		/// <summary>
		/// Computes the median of a sequence of ints.
		/// <para/>
		/// Code transformed from: Richard Carr, http://www.blackwasp.co.uk/LinqMedian_2.aspx
		/// </summary>
		public static int Median(this IEnumerable<int> source)
		{
			if (source == null) return 0;

			var arr = source.ToArray();               // Cache the sequence
			int len = arr.Length;             // Use the cached version
			if (len != 0) {
				var midpoint = (len - 1) / 2;
				var sorted = arr.OrderBy(n => n).ToArray();    // Use the cached version
				var median = sorted.ElementAt(midpoint);

				if (len % 2 == 0)
					median = (median + sorted.ElementAt(midpoint + 1)) / 2;

				return median;
			}
			return 0;
		}

		/// <summary>Computes the average of a sequence of items that have an int gotten from the selector,
		/// and does NOT throw an exception if the sequence is (null or) empty! </summary>
		public static int AverageOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
		{
			if (source == null) return 0;
			return source.Select(i => selector(i)).AverageOrDefault();
		}

		/// <summary>Computes the average of the items,
		/// and does NOT throw an exception if the sequence is (null or) empty!</summary>
		public static int AverageOrDefault(this IEnumerable<int> source)
		{
			if (source == null) return 0;

			int cnt = 0;
			int sum = source.Sum(i => { cnt++; return i; });

			return cnt == 0 ? 0 : sum / cnt;
		}

		#endregion


		#region --- Where (WhereIf, etc) ---


		public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool filter, Func<T, bool> predicate)
		{
			if (!filter)
				return source;
			return source.Where(predicate);
		}

		public static IEnumerable<T> WhereIfElse<T>(this IEnumerable<T> source, bool usePredicate1IFTrue, Func<T, bool> predicateIf, Func<T, bool> predicateElse)
		{
			return source.Where(usePredicate1IFTrue ? predicateIf : predicateElse);
		}

		public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool filter, Func<T, int, bool> predicate)
		{
			if (!filter)
				return source;
			return source.Where(predicate);
		}


		public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
		{
			return !condition ? source : source.Where(predicate);
		}

		public static IQueryable<T> SkipIf<T>(this IQueryable<T> source, bool condition, int count)
		{
			return !condition
				? source
				: source.Skip(count);
		}

		public static IQueryable<T> TakeIf<T>(this IQueryable<T> source, bool condition, int count)
		{
			return !condition
				? source
				: source.Take(count);
		}

		public static IQueryable<T> SkipTakeIf<T>(this IQueryable<T> source, bool condition, int skip, int take)
		{
			return !condition
				? source
				: source.Skip(skip).Take(take);
		}

		#endregion

		public static IEnumerable<T> TakeIf<T>(this IEnumerable<T> source, bool doTake, int count)
		{
			if (!doTake)
				return source;
			return source.Take(count);
		}

		#region --- Concat / ConcatToArray ---

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> seq1, T item, bool itemFirst = false)
		{
			if(itemFirst)
				yield return item;

			if (seq1 != null) {
				foreach (var itm in seq1)
					yield return itm;
			}
			if (!itemFirst)
				yield return item;
		}

		public static IEnumerable<T> ConcatN<T>(this IEnumerable<T> seq1, IEnumerable<T> seq2)
		{
			if (seq1 == null)
				return seq2 ?? Enumerable.Empty<T>();
			else if (seq2 == null)
				return seq1;
			else
				return seq1.Concat(seq2);
		}

		//public static IEnumerable<T> AddToSequence<T>(this IEnumerable<T> seq1, params T[] items)
		//{
		//	if (seq1 == null)
		//		return items == null ? Enumerable.Empty<T>() : items;
		//	else if (items == null)
		//		return seq1;
		//	else
		//		return seq1.Concat(items);
		//}

		/// <summary>
		/// Joins the two or three sequences into one array, with <paramref name="seq1"/>
		/// items occuring first and <paramref name="seq2"/> items second, etc.
		/// </summary>
		/// <typeparam name="T">The type.</typeparam>
		/// <param name="seq1">The first sequence.</param>
		/// <param name="seq2">The second sequence.</param>
		/// <param name="seq3">A third sequence.</param>
		public static T[] ConcatToArray<T>(this IList<T> seq1, IList<T> seq2, IList<T> seq3 = null)
		{
			int seq1Cnt = seq1?.Count ?? 0;
			int seq2Cnt = seq2?.Count ?? 0;
			int seq3Cnt = seq3?.Count ?? 0;

			T[] combined = new T[seq1Cnt + seq2Cnt + seq3Cnt];

			int i = 0;
			int j = 0;

			for (j = 0; j < seq1Cnt; j++, i++)
				combined[i] = seq1[j];
			for (j = 0; j < seq2Cnt; j++, i++)
				combined[i] = seq2[j];
			for (j = 0; j < seq3Cnt; j++, i++)
				combined[i] = seq3[j];

			return combined;
		}

		#endregion


		#region --- FindIndex, FindLastIndex, Find, FindLast, Contains ---

		public static bool Contains<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			if (items == null)
				return false;
			foreach (T item in items)
				if (predicate(item))
					return true;
			return false;
		}

		public static bool Contains<T>(this IList<T> list, int start, Func<T, bool> predicate)
		{
			return list.FindIndex(start, predicate) >= 0;
		}

		public static bool Contains<T>(this IList<T> list, int start, int length, Func<T, bool> predicate)
		{
			return list.FindIndex(start, length, predicate) >= 0;
		}

		public static bool In<T>(this T item, IEnumerable<T> items) //where T : IEquatable<T> // IEqualityComparer<T> //, Func<T, bool> predicate = null)
		{
			if (item == null || items == null)
				return false;

			Func<T, T, bool> comparer = ComparerX<T>.DefaultEqualityComparer;
			//	comparer = Comparer<T>.Default;

			if (comparer == null) {
				foreach (var itm in items)
					if (itm != null && item.Equals(itm))
						return true;
			}
			else {
				foreach (var itm in items)
					if (comparer.Invoke(item, itm))
						return true;
			}
			return false;
		}

		//

		// ... nah...
		//public static KeyValuePair<int, T> FirstWithIndex<T>(this IList<T> list, Func<T, bool> match)
		//{
		//	int idx = list.FindIndex(match);
		//	return new KeyValuePair<int, T>(idx, idx >= 0 ? list[idx] : default(T));
		//}

		public static T Find<T>(this IList<T> list, Func<T, bool> match)
		{
			int idx = list.FindIndex(match);
			return idx > 0 ? list[idx] : default(T);
		}

		public static T FindLast<T>(this IList<T> list, Func<T, bool> match)
		{
			int idx = list.FindLastIndex(match);
			return idx > 0 ? list[idx] : default(T);
		}

		public static int FindIndex<T>(this IList<T> list, Func<T, bool> match)
		{
			return list.FindIndex(0, list.CountN(), match);
		}

		public static int FindIndex<T>(this IList<T> list, int start, Func<T, bool> match)
		{
			return list.FindIndex(start, list.CountN(), match);
		}

		public static int FindIndex<T>(this IList<T> list, int start, int length, Func<T, bool> match)
		{
			if (list != null) {
				int len = Math.Min(list.Count, length);
				for (int i = start; i < len; i++)
					if (match(list[i]))
						return i;
			}
			return -1;
		}

		public static int FindIndex(this string str, Func<char, bool> match)
		{
			return FindIndex(str, 0, str?.Length ?? 0, match);
		}

		public static int FindIndex(this string str, int start, int length, Func<char, bool> match)
		{
			if (str != null) {
				int len = Math.Min(str.Length, length);
				for (int i = start; i < len; i++)
					if (match(str[i]))
						return i;
			}
			return -1;
		}

		//

		public static int FindLastIndex<T>(this IList<T> list, Func<T, bool> match)
		{
			return FindLastIndex(list, 0, list.CountN(), match);
		}

		public static int FindLastIndex<T>(this IList<T> list, int start, Func<T, bool> match)
		{
			return FindLastIndex(list, start, list.CountN() - start, match);
		}

		/// <summary>
		/// Have not tested this code yet... needs a good test put to it
		/// </summary>
		public static int FindLastIndex<T>(this IList<T> list, int start, int length, Func<T, bool> match)
		{
			if (list != null) {
				int len = Math.Min(length, list.Count);
				int min = start - length;
				if (min < 0) min = 0;

				for (int i = start; i >= min; i--)
					if (match(list[i]))
						return i;
			}
			return -1;
		}

		#endregion

		
		/// <summary>
		/// Gets the minimum item. Source must not be null or empty.
		/// Inspired by: http://stackoverflow.com/a/914198/264031
		/// </summary>
		public static TSource MinBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> selector,
			IComparer<TKey> comparer = null)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (selector == null) throw new ArgumentNullException(nameof(selector));
			comparer = comparer ?? Comparer<TKey>.Default;

			// this will guarantee Count is 1 or more, let exception throw here if not, the right exception will throw
			TSource min = source.First();
			TKey minKey = selector(min);
			int i = 0;

			foreach (TSource item in source) {
				if (i == 0) { i++; continue; }
				TKey key = selector(item);
				if (comparer.Compare(key, minKey) < 0) {
					min = item;
					minKey = key;
				}
			}
			return min;
		}

		/// <summary>
		/// Gets the maximum item. Source must not be null or empty.
		/// </summary>
		public static TSource MaxBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> selector,
			IComparer<TKey> comparer = null)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (selector == null) throw new ArgumentNullException(nameof(selector));
			comparer = comparer ?? Comparer<TKey>.Default;

			// this will guarantee Count is 1 or more, let exception throw here if not, the right exception will throw
			TSource max = source.First();
			TKey maxKey = selector(max);
			int i = 0;

			foreach (TSource item in source) {
				if (i == 0) { i++; continue; }
				TKey key = selector(item);
				if (comparer.Compare(key, maxKey) > 0) {
					max = item;
					maxKey = key;
				}
			}
			return max;
		}

	}
}
