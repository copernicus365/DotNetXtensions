using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions.Collections
{
	/// <summary>
	/// A dictionary whose items expire after a set amount of time after
	/// being set. This internally wraps a <see cref="ConcurrentDictionary{TKey, TValue}"/>,
	/// allowing this type to be thread-safe, which makes sense for most cases
	/// where a cached dictionary would be needed. The purging of items is handled extremely 
	/// performantly, but passively (no embedded and complicated running timer). See notes on 
	/// that below. But even though at a given time *internally* this dictionary may store
	/// some items that have just expired (or even expired for some time), it is guaranteed
	/// that no such expired items will EVER be returned, their existence will only be reflected
	/// in the fact that they are counted in the normal <see cref="Count"/> of this dictionary.
	/// We intentionally decided to not make that an outrageous calculation, but to simply
	/// return the count of internal items. To get a count of items that haven't expired, 
	/// you can call <see cref="CountPurged"/>, but that does take a full scan (just like 
	/// <see cref="PurgeExpiredItems()"/>), and also note that the moment you get the value,
	/// the count may still change 1 nanosecond later. 
	/// For further details on how the items are actually purged internally, see further below:
	/// <para />
	/// With an eye on the Separation of Concerns as well as KISS principles, 
	/// we've designed this type to NOT embed any kind of running timer that periodically and actively
	/// removes expired items (by "actively" we mean: without any needed interaction with 
	/// an instance of this type). Such would: 1) requires dependencies on timer types,
	/// 2) need to try to be robust if the timer itself failed, leaving items building up in memory 
	/// after expiration, and so forth. For these and other reasons it seemed best to
	/// keep a full polling timer external to this type, but one can freely implement that,
	/// just call <see cref="PurgeExpiredItems()"/> whenever desired.
	/// <para />
	/// On the other hand, we still have implemented passive means to purge expired items. These are:
	/// 1) When an item's value is retrieved which has already expired, that item will be
	/// internally removed, IMMEDIATELY. Then TryGetValue will return false.
	/// 2) In addition, <see cref="RunPurgeTS"/>, which by default is set to 1 minute, is used to set 
	/// a future expiration date added to UtcNow. A number of TRIGGER POINTS will always check first if
	/// that time has expired (i.e. has it been 1 minute or more since a purge was attempted?).
	/// If so then <see cref="PurgeExpiredItems()"/> is fired. Critically though, this
	/// is NOT (!!) fired every time these trigger points are hit. These trigger points are:
	/// a) When a lookup is made (TryGetValue, through the indexer, etc),
	/// b) When an item is Set or Added, or 
	/// c) when items are enumerated (this includes common LINQ calls, like ToArray, 
	/// normal iteration of the collection, and etc). 
	/// All of these triggers will fire a *highly performant* check, whose main cost is
	/// a call to <see cref="DateTime.UtcNow"/> (which is in the nanoseconds range), plus a comparison 
	/// of its value with an already calculated (internal) <see cref="_RunNextPurgeDT"/>.
	/// This means no costly and outrageous extra operations are run for normal Get / Set (etc) operations,
	/// as I saw some online caching dictionaries were architected. 
	/// One must note that this is still a passive means of ensuring expired items get removed, so if 
	/// this instance isn't interacted with for an hour, no purging of expired items will occur. If one critically 
	/// needs such periodic purging without exceptions, then again, simply implement an external timer 
	/// yourself and call <see cref="PurgeExpiredItems()"/> on the desired interval. 
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class CacheDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		readonly ConcurrentDictionary<TKey, (DateTime expires, TValue value)> D;

		public TimeSpan ExpiresAfter { get; private set; }

		/// <summary>
		/// Set this if you want to handle getting of <see cref="DateTime.UtcNow"/>.
		/// Time returned should be a UTC time. This is needed for testing purposes,
		/// but probably won't be needed otherwise. <see cref="DTNow"/> uses this function 
		/// if it is set (not-null).
		/// <para/>
		/// NOTE: Do NOT accidentally call this internally, as it stays null until set. 
		/// Instead call <see cref="DTNow"/>.
		/// </summary>
		public Func<DateTime> GetDateTimeNow { get; set; }

		/// <summary>
		/// Returns the result of <see cref="GetDateTimeNow"/> if it is set (not-null),
		/// else returns <see cref="DateTime.UtcNow"/>.
		/// </summary>
		public DateTime DTNow 
			=> GetDateTimeNow == null ? DateTime.UtcNow : GetDateTimeNow();



		public TimeSpan RunPurgeTS {
			get => _RunPurgeTS;
			// Don't let ts value be < zero. That should just mean a purge is always called
			set => _RunPurgeTS = value.Max(TimeSpan.Zero);
		}
		TimeSpan _RunPurgeTS = TimeSpan.FromMinutes(1);

		DateTime _RunNextPurgeDT;

		/// <summary>
		/// Resets the internal datetime `_RunNextPurgeDT`, which 
		/// is set into the future after DTNow by the amount of <see cref="RunPurgeTS"/>.
		/// We're exposing this only to enable unit-testing scenarios. If this
		/// type had an interface we would not include this member on it.
		/// </summary>
		public DateTime ResetRunNextPurgeDT()
			=> _RunNextPurgeDT = DTNow + _RunPurgeTS;


		// --- CONSTRUCTORS ---

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="expires">Expiration time of items. Minimum value is 1 second.</param>
		/// <param name="equalityComparer"></param>
		public CacheDictionary(TimeSpan expires, IEqualityComparer<TKey> equalityComparer = null)
		{
			if(expires < TimeSpan.FromSeconds(1))
				throw new ArgumentOutOfRangeException(nameof(expires));

			ExpiresAfter = expires;

			// note: It seems that netcore allows null input, but old fx doesn't!
			D = equalityComparer == null
				? new ConcurrentDictionary<TKey, (DateTime expires, TValue value)>()
				: new ConcurrentDictionary<TKey, (DateTime expires, TValue value)>(equalityComparer);
		}



		// --- CONSTRUCTORS ---

		/// <summary>
		/// Keys. Note that this does winnow any already expired internal items
		/// that weren't purged yet (it calls <see cref="GetItems"/> which does this).
		/// </summary>
		public ICollection<TKey> Keys
			=> GetItems().Select(kv => kv.Key).ToList();

		/// <summary>
		/// Values. Note that this does winnow any already expired internal items
		/// that weren't purged yet (it calls <see cref="GetItems"/> which does this).
		/// </summary>
		public ICollection<TValue> Values
			=> GetItems().Select(kv => kv.Value).ToList();

		/// <summary>
		/// Returns the Count of *the internal* ConcurrentDictionary,
		/// this is simply a redirection to that property. To get the count 
		/// after expired items are purged, call <see cref="CountPurged"/>.
		/// </summary>
		public int Count => D.Count;

		/// <summary>
		/// Gets the count of items after any expired items are winnowed.
		/// This requires a full iteration of the inner dictionary.
		/// </summary>
		public int CountPurged() => _GetPurgedItemsCount();

		public bool IsReadOnly => false;

		/// <summary>
		/// Gets or sets the item. For Get, if not found exception is
		/// thrown. For Set, sets (updates) or adds the input value,
		/// this simply calls <see cref="Add(TKey, TValue)"/> (see notes there).
		/// </summary>
		public TValue this[TKey key] {
			get {
				if(TryGetValue(key, out TValue val))
					return val;
				throw new ArgumentException("Key not found.");
			}
			set => Add(key, value);
		}

		/// <summary>
		/// Adds new key/value, OR SETS already existing key to input value.
		/// Note that this acts differently from standard Dictionary which throws
		/// if key already existed, while only the set-indexer can be used to set 
		/// OR add a new OR existing key. Reason for this is, dealing with a caching, 
		/// time-based dictionary should be tolerant of this, even expecting you can 
		/// never know, with items being auto-purged, etc. The set-indexer 
		/// above just calls this function.
		/// </summary>
		public void Add(TKey key, TValue value)
		{
			DateTime now = DTNow;
			_PurgeIfNextPurgeTimeHit(now);

			DateTime vExpires = now + ExpiresAfter;
			D[key] = (vExpires, value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			DateTime now = DTNow;
			_PurgeIfNextPurgeTimeHit(now);

			if(D.TryGetValue(key, out (DateTime expires, TValue value) v)) {
				if(v.expires > now) {
					value = v.value;
					return true;
				}
				// Concious decision to REMOVE when we found it already expired
				// this makes for one PROACTIVE step for removing items even if
				// / when timed purger isn't working
				Remove(key);		
			}
			value = default;
			return false;
		}

		/// <summary>
		/// We have an input time, because 
		/// </summary>
		/// <param name="now"></param>
		void _PurgeIfNextPurgeTimeHit(DateTime now)
		{
			if(now >= _RunNextPurgeDT)
				PurgeExpiredItems(now);
		}

		/// <summary>
		/// Purge all values whose items expiration dates
		/// are less than or equal to <see cref="DTNow"/>.
		/// </summary>
		public void PurgeExpiredItems() 
			=> PurgeExpiredItems(DTNow);

		/// <summary>
		/// Purge all values whose items expiration dates
		/// are less than or equal to the input time (presumably 
		/// equals DateTime.UtcNow).
		/// </summary>
		public void PurgeExpiredItems(DateTime now)
		{
			List<TKey> expKeys = new List<TKey>();

			foreach(var kv in D)
				if(kv.Value.expires <= now)
					expKeys.Add(kv.Key);

			if(expKeys.NotNulle())
				foreach(TKey ky in expKeys)
					Remove(ky);

			ResetRunNextPurgeDT();
		}

		/// <summary>
		/// Indirection to <see cref="Add(TKey, TValue)"/>.
		/// </summary>
		public void Add(KeyValuePair<TKey, TValue> item)
			=> Add(item.Key, item.Value);

		public bool ContainsKey(TKey key)
			=> D.ContainsKey(key);

		public bool Remove(TKey key)
			=> D.TryRemove(key, out _);

		public bool Remove(KeyValuePair<TKey, TValue> item)
			=> D.TryRemove(item.Key, out _);

		public void Clear()
		{
			D.Clear();
			ResetRunNextPurgeDT();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			if(D.TryGetValue(item.Key, out (DateTime expires, TValue value) v))
				if(v.value.Equals(item.Value))
					return true;
			return false;
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			if(array == null)
				throw new ArgumentNullException();

			if(arrayIndex < 0 || arrayIndex >= array.Length)
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			if(Count > (arrayIndex + array.Length))
				throw new ArgumentOutOfRangeException(nameof(arrayIndex));

			int i = 0;
			foreach(var kv in GetItems())
				array[i++ + arrayIndex] = new KeyValuePair<TKey, TValue>(kv.Key, kv.Value);
		}

		public IEnumerable<KeyValuePair<TKey, TValue>> GetItems()
		{
			DateTime now = DTNow;
			_PurgeIfNextPurgeTimeHit(now);

			foreach(var kv in D)
				if(kv.Value.expires > now)
					yield return new KeyValuePair<TKey, TValue>(kv.Key, kv.Value.value);
		}

		/// <summary>
		/// Keep this private, let user-friendly name be <see cref="CountPurged"/>,
		/// which just redirects to this (better grouping here next to GetItems).
		/// Note, we could just iterate <see cref="GetItems"/>, but this avoids
		/// any allocations, and is thus far superior for the job.
		/// </summary>
		int _GetPurgedItemsCount()
		{
			DateTime now = DTNow;
			int count = 0;

			foreach(var kv in D)
				if(kv.Value.expires > now)
					count++;

			return count;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
			=> GetItems().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

	}
}
