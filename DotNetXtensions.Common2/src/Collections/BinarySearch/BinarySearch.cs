using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetXtensions.Collections
{
	/// <summary>
	/// A class that performs binary chop searches on sorted sequences,
	/// with helpful functions like FindRange, which allows efficient 
	/// binary-search retrieval of values within a certain range of the sorted sequence.
	/// This class also allows searching on virtual sequences, as an actual array or
	/// list never needs to be sent in, which also gets around the framework restriction
	/// whose binary search functions only apply to arrays (see Array class).
	/// </summary>
	/// <typeparam name="T">The given type which either must implement IComparable&lt;T&gt;
	/// or else the Comparer func must be sent in.</typeparam>
	public class BinarySearch<T>
	{
		const int noneFound = IndexRangeWindow.noneFound;

		// --- FIELDS ---
		int _collLength;
		bool _reverse;
		Func<int, T> _getItemAt;
		/// <summary>
		/// This is the main comparer but use compareTo private property for main 
		/// functionality instead, see notes below.</summary>
		Func<T, T, int> _compareTo;


		public int CollectionLength { get { return _collLength; } set { _collLength = value; } }

		public Func<int, T> GetItemAt { get { return _getItemAt; } set { _getItemAt = value; } }


		#region --- COMPARERS ---

		public Func<T, T, int> CompareTo { get { return _compareTo; } set { _compareTo = value; } }

		/// <summary>
		/// If reverse is true returns a reversed result of the _compareTo comparer, 
		/// else returns _compareTo itself. Use this comparer for the main work herein,
		/// as it quite magically handles a reversing of the results if reverse is true
		/// (SO beautiful that this was all that was needed to handle reversed sequences!!! Literally
		/// this is IT, no other changes needed, simply reversing the comparer result 
		/// perfectly handles everything).
		/// </summary>
		Func<T, T, int> compareTo {
			get {
				return _reverse
					? _reverseComparer1
					: _compareTo;
			}
		}

		/// <summary>
		/// A reversed comparer that works by reversing the result of the _compareTo func.
		/// So if _compareTo generates -1, the return here will be 1, etc (0 is still 0).
		/// </summary>
		/// <param name="t1">First value to compare.</param>
		/// <param name="t2">Second value to compare.</param>
		int _reverseComparer1(T t1, T t2)
		{
			int val = _compareTo(t1, t2);
			val = val > 0 ? -1 : (val < 0 ? 1 : 0);
			return val;
		}

		#endregion


		#region --- CONSTRUCTORS / INITS ---

		/// <summary>
		/// Initializes (or reinitializes) the main settings for this BinarySearch instance. 
		/// This allows one to reuse this instance for sequences of the same type T, rather than
		/// having to allocate and set a new instance for every usage.
		/// The constructor is an indirection to this method.
		/// <para/>
		/// If type T does not implement IComparable&lt;T&gt;, you must send in a non-null comparer function.
		/// </summary>
		/// <param name="getItemAtIndex">Function for retrieving 
		/// items from the sorted sequence to search.</param>
		/// <param name="collectionLength">Length of items in the source collection 
		/// (only needed if for methods that need this value set).</param>
		/// <param name="reverse">True if the items source is sorted in reverse order.
		/// Internally, what this does is reverse the integer result of the comparer,
		/// which thankfully gerenates perfect results, without having to reimplement
		/// all the code (!!!).</param>
		/// <param name="comparer">Comparer.</param>
		public BinarySearch<T> Init(Func<int, T> getItemAtIndex, int collectionLength, bool reverse = false, Func<T, T, int> comparer = null)
		{
			if(getItemAtIndex == null) throw new ArgumentNullException(nameof(getItemAtIndex));

			_reverse = reverse;
			_collLength = collectionLength;
			_getItemAt = getItemAtIndex;
			_compareTo = comparer ?? (_compareTo ?? ComparerX<T>.DefaultComparer);

			if(_compareTo == null)
				throw new ArgumentNullException("CompareTo", "The type T does not implement IComparable<T>, so a non-null comparer must be sent in.");

			return this;
		}


		// --- indirects to Init above ---

		/// <summary>
		/// If calling this parameterless constructor, you will have to call Init before any other methods are called.
		/// </summary>
		public BinarySearch() { }

		/// <summary>
		/// See notes on Init which this overloaded constructor calls.
		/// </summary>
		public BinarySearch(Func<int, T> getItemAtIndex, int collectionLength, bool reverse = false, Func<T, T, int> comparer = null)
		{
			Init(getItemAtIndex, collectionLength, reverse, comparer);
		}

		/// <summary>
		/// See notes on Init which this overloaded constructor calls.
		/// </summary>
		public BinarySearch(IList<T> arr, bool reverse = false, Func<T, T, int> comparer = null)
		{
			Init(arr, reverse, comparer);
		}

		public BinarySearch<T> Init(IList<T> arr, bool reverse = false, Func<T, T, int> comparer = null)
		{
			if(arr == null) throw new ArgumentNullException(nameof(arr));
			return Init(i => arr[i], arr.Count, reverse, comparer);
		}

		#endregion


		#region --- Find ---

		/// <summary>
		/// Does a binary search for the given value, returning the found index, or if not found,
		/// returns the negative complement (+ 1 to the positive value).
		/// </summary>
		/// <param name="value">Value to search for.</param>
		/// <param name="index">Index to start the search at, must be less than the collection length.</param>
		/// <param name="count">If not null, is the count after index of range to search within the collection.
		/// Leave null to use CollectionLength (to search through to the collection's end, taking into account index).</param>
		public int Find(T value, int index = 0, int? count = null)
		{
			int low = index;
			int high = _GetHighFromInputLength(index, count);

			if(low < 0) throw new ArgumentOutOfRangeException(nameof(low));
			if(high >= _collLength) throw new ArgumentOutOfRangeException(nameof(high));
			if(high < low) return noneFound; // ALLOW

			int mid = 0;
			int compared = 0;

			while(low <= high) {
				mid = ((high - low) / 2) + low;

				T midItem = _getItemAt(mid);
				compared = compareTo(value, midItem); //_compareTo(value, midItem);

				if(compared < 0) {
					high = mid - 1;
				}
				else if(compared > 0) {
					low = mid + 1;
				}
				else {
					return mid;
				}
			}
			return ~low;

			/* returns neg complement + 1 to the positive. 
			 * ~0 === -1; ~1 === -2; if original array len was 10 [0-9 let's say],
			 * and value to find was higher than last leaving 'low' finally == 9,
			 * then ~low (~9) === -10, thus out of range of length, allowing that to be tested.*/
		}

		/// <summary>
		/// [OBSOLETE] Well probably obsolete, to be replaced, we hope, by the new
		/// FindSingleRange methods and algorithms. However, the difference is this 
		/// performs a linear search once a found index is found, so for testing lets
		/// keep around for now.
		/// [Old notes]
		/// Gets the range of finds (if any). This is performed by calling Find, and then submitting that
		/// result to FindAllFromFound: (<c>FindAllFromFound(value, Find(value), index, count)</c>).
		/// </summary>
		[Obsolete]
		public IndexRangeWindow FindSingleRangeLinearOnceFound(T value, int index = 0, int? count = null) // was named: FindRange
		{
			int foundIndex = Find(value, index, count);
			return FindSingleRangeFromFoundLinear(value, foundIndex, index, count);
		}

		/// <summary>
		/// [OBSOLETE] Well probably obsolete, to be replaced, we hope, by the new
		/// FindSingleRange methods and algorithms. However, the difference is this 
		/// performs a linear search once a found index is found, so for testing lets
		/// keep around for now.
		/// [Old notes]
		/// Takes the result of Find and gets all (any) adjacent matches, forwards and backwards.
		/// At this time this is a primitive linear search, which will have a negative impact
		/// in cases where a sequence has the same (sorted) value occuring many times. In the future this
		/// could be made to search 1, 2, 4, 8, 16, binary multiple jumps forward until a non-match is 
		/// found in each direction, then working back, but ... that's a lot more complex.
		/// </summary>
		/// <param name="value">The original search value.</param>
		/// <param name="foundIdx">The found index from the binary search (returned potentially in this case
		/// from Find).</param>
		/// <param name="index">The index at which the search was started at.</param>
		/// <param name="count">The count after index which the search was limited to (if any).</param>
		[Obsolete]
		public IndexRangeWindow FindSingleRangeFromFoundLinear(T value, int foundIdx, int index = 0, int? count = null)
		{//FindSingleRangeFromFoundLinear
			if(foundIdx < 0)
				return IndexRangeWindow.NoneFound;
			//	BinaryRange r = new BinaryRange(foundIdx, foundIdx);
			//	if(!r.Any)
			//		return r;

			int low = foundIdx;
			int high = foundIdx;

			for(int i = foundIdx - 1; i >= index; i--) {
				T item = _getItemAt(i);
				if(compareTo(value, item) == 0) {
					--low; // --r.Low;
				}
				else
					break;
			}

			int sequenceLength = count ?? _collLength;
			for(int i = foundIdx + 1; i < sequenceLength; i++) {
				T item = _getItemAt(i);
				if(compareTo(value, item) == 0) {
					++high; //++r.High;
				}
				else
					break;
			}
			var result = new IndexRangeWindow(low, high);
			return result;
		}

		#endregion

		#region --- FindRange ---

		/// <summary>
		/// Finds the range of matches that are greater or equal than low value and lesser or equal to
		/// high value within the sorted sequence by means of a binary search.
		/// Example: 
		/// <![CDATA[
		/// int arr = { 1, 1, 2, 5, 5, 5, 8, 8, 10, 12, 12 };
		/// IndexRangeWindow res = FindRange(3, 7); 
		/// //res: [low:3 high:7 count:5]
		/// ]]>
		/// </summary>
		/// <param name="lowValue">The low value to search (the highest in the sequence, 
		/// if sequence is reversed sort as set in constructor, this will be the higher value).</param>
		/// <param name="highValue">A value higher (further on) in the sequence.</param>
		/// <param name="index">The index at which to begin the search (0 by default).</param>
		/// <param name="length">The length of items to search after index (by default is set to the 
		/// sequence length which was set in the Init or constructor).</param>
		public IndexRangeWindow FindRange(T lowValue, T highValue, int index = 0, int? length = null)
		{
			int low = index;
			int high = _GetHighFromInputLength(low, length);

			// --- both methods below do low,high bounds check so don't do here again ---

			// is low value already greater than high?
			if(compareTo(lowValue, highValue) > 0)
				return IndexRangeWindow.NoneFound;

			// GET LOW
			int lowRangeIndex = FindLowRange(lowValue, low, high);

			if(lowRangeIndex > high || lowRangeIndex < 0)
				return IndexRangeWindow.NoneFound;

			// GET HIGH
			int highRangeIndex = FindHighRange(highValue, lowRangeIndex, high);

			var range = new IndexRangeWindow(lowRangeIndex, highRangeIndex);

			return range;
		}

		/// <summary>
		/// Finds the range of matches of a single value within the sorted sequence
		/// by means of a binary search, see overload for further details. 
		/// </summary>
		/// <param name="value">Value to search for. Both a FindLowRange and 
		/// FindHighRange search will be performed on it.</param>
		/// <param name="index">Index</param>
		/// <param name="length">Count from Index to search on</param>
		public IndexRangeWindow FindSingleRange(T value, int index = 0, int? length = null)
		{
			int low = index;
			int high = length == null
				? _collLength - index - 1
				: (index + (int)length) - 1;

			// GET LOW
			int lowRangeIndex = FindLowRange(value, low, high);

			if(lowRangeIndex > high || lowRangeIndex < 0)
				return IndexRangeWindow.NoneFound;

			// GET HIGH
			int highRangeIndex = FindHighRange(value, lowRangeIndex, high);

			var range = new IndexRangeWindow(lowRangeIndex, highRangeIndex);

			return range;
		}

		/// <summary>
		/// Finds the lowest index in this range whose value is
		/// greater or equal to <paramref name="value"/> by means of a binary chop search. 
		/// Two extremity checks are performed first to see if 
		/// the value of <paramref name="low"/> is already >= to <paramref name="value"/> 
		/// (in which case <paramref name="low"/> is returned) or if value is already greater 
		/// than the value of <paramref name="high"/> (thus no match). If no match is found 
		/// returns -1.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <param name="low">The lowest index within sequence to search in.</param>
		/// <param name="high">The highest index within sequence to search in. If less than
		/// low will return -1.</param>
		/// <returns>The lowest index position within range that matched, -1 if no match.</returns>
		public int FindLowRange(T value, int low, int high)
		{
			if(low < 0) throw new ArgumentOutOfRangeException(nameof(low));
			if(high >= _collLength) throw new ArgumentOutOfRangeException(nameof(high));
			if(high < low) return noneFound; // ALLOW

			#region --- check is value already below lowest or above highest ---

			if((high - low) > 4) { // only if sufficient range exists (> 4): 1-2-4-8 = 4 or so checks
							
				T lowestValue = _getItemAt(low); // #1) is value already lower than lowest value (=MATCH)
				if(compareTo(value, lowestValue) <= 0)
					return low;

				T highestValue = _getItemAt(high); // #2) is value already higher than highest value? (=NOT found)
				if(compareTo(value, highestValue) > 0)
					return noneFound;
			}

			#endregion

			int mid;
			int valComparedToMid;
			T midValue;
			int match = noneFound;
#if BinSearchTests
			int loopIdx = 0;
#endif

			// #3) main binary loop that searches for lowest index that is >= to value. 

			while(low <= high) {

				mid = ((high - low) / 2) + low;
				midValue = _getItemAt(mid);
				valComparedToMid = compareTo(value, midValue);

#if BinSearchTests
				_print1(loopIdx++, low, mid, high, midValue);
#endif

				if(valComparedToMid > 0) {
					// value is > than midValue, so no match, we move the lower pane of window UP
					low = mid + 1;
				}
				else { // -- value < midValue
					   // match found (value is <= to mid value), so set match to mid,
					   // then look LOWER if there are any better (lower) matches 
					   // (cut high down 1, this is highest we'll ever search)
					   // note that UP (above) can still be hit above
					match = mid;
					high = mid - 1; // twice decreased now since above linear check also failed
				}
#if BinSearchTests
				_print2(valComparedToMid, match, true);
#endif
			}
			return match;
		}

		/// <summary>
		/// Finds the highest index within the input low/high range
		/// where value is less than or equal to the input value.
		/// A check is made first on the low / high indexes before
		/// entering a the full binary search to see if the value is
		/// already out of range. If out of range returns -1.
		/// </summary>
		/// <param name="value">The value to search for.</param>
		/// <param name="low">The low index value.</param>
		/// <param name="high">The high index value.</param>
		public int FindHighRange(T value, int low, int high)
		{
			if(low < 0) throw new ArgumentOutOfRangeException(nameof(low));
			if(high >= _collLength) throw new ArgumentOutOfRangeException(nameof(high));
			if(high < low) return noneFound; // ALLOW

			#region --- check is value already below lowest or above highest ---

			if((high - low) > 4) {  // only if sufficient range exists (> 4): 1-2-4-8 = 4 or so checks

				T highestValue = _getItemAt(high); // #1) is value already >= to highest value? (=MATCH)
				if(compareTo(value, highestValue) >= 0)
					return high;
			
				T lowestValue = _getItemAt(low); // #2) is value already < than lowest value (=NOT found)
				if(compareTo(value, lowestValue) < 0)
					return noneFound;
			}

			#endregion

			int mid;
			int valComparedToMid;
			T midValue;
			int lastEqualsPos = noneFound;
#if BinSearchTests
			int loopIdx = 0;
#endif
			while(low <= high) {

				mid = ((high - low) / 2) + low;
				midValue = _getItemAt(mid);
				valComparedToMid = compareTo(value, midValue);

#if BinSearchTests
				_print1(loopIdx++, low, mid, high, midValue);
#endif

				if(valComparedToMid >= 0) {
					low = mid + 1; // move LOW window UP to mid + 1, for BOTH > as well as == midValue

					if(valComparedToMid == 0) {
						// see notes. as soon as this is hit even ONCE, it means ALL subsequent >= hits MUST be equality matches
						lastEqualsPos = mid;
					}
				}
				else {
					high = mid - 1;  // move HIGH window DOWN to mid - 1
				}
#if BinSearchTests
				_print2(valComparedToMid, lastEqualsPos < 0 ? high : lastEqualsPos, false);
#endif
			}
			return lastEqualsPos >= 0
				? lastEqualsPos
				: high;

			#region --- explanation ---
			/*  this takes some time to comprehend, but I believe as I have it here
			 *  is the most correct and also the simplest solution to the problem,
			 *  even though it seems odd. The odd thing is the way this FindHighRange
			 *  looks quite a bit different from how the FindLowRange works, and 
			 *  in particular, it seems odd that the two ways of finding a potential 
			 *  match (either valComparedToMid < 0, or valComparedToMid == 0) are not
			 *  in the same if/else block (the top if moving the lower range up, the 
			 *  lower else block moving the high range down). You might expect a match
			 *  to always be in the same block that moves the lower or upper window
			 *  in the same direction, but this is not the case. 
			 *  
			 *  When equal, we can't treat it as the final match yet, neither do we
			 *  move high window pane down, because there could still be one or more 
			 *  duplicates after this position so we have to move the LOW up (to mid+1). 
			 *  The key is that if we EVER have an equals match, how we have it 
			 *  will mean if there are many equals matches, each one will always get closer 
			 *  to the final equals match by moving the low range UP: 
			 *  Ex: { 1, 2, 3, !3, 3, 3, !!3, !!!3, 4, 5, 6 }
			 *  
			 *  Consider the exclamations represent subsequent times we had an equals match, 
			 *  each time it will be forced to move CLOSER to the high end (what we want). 
			 *  However, as soon as we reached the last '3' above (the highest equals)
			 *  we won't get an equals hit again so ALL following hits will
			 *  be valComparedToMid < 0 (which will keep moving the high window DOWN).
			 *  
			 *  So the key is, as soon as there is ever an equals, never again will there
			 *  be a less than, but there *can* be subsequent greater thans followed by
			 *  subsequent equals (though higher up ones, only if there was more than one of value).
			 *  If there ever is an equal, the final one will be the final match, else if
			 *  there is never an equals (value is part of range), then the final hit will be
			 *  the last set high value.
			 *  
			 *  GOT IT?! Really you just have to enable the BinSearchTests which will let you 
			 *  analyze the output, that helps tremendously to understand, with the critical part 
			 *  being the print out on each loop of the binary search.
			 */
			#endregion
		}

#if BinSearchTests
		void _print1(int loopIdx, int low, int mid, int high, T midValue)
		{
			const int pad = -2;
			$"\r\n#{loopIdx++})".Print();
			$@"[{low,pad} — {mid,pad} — {high,pad}]".Print();
			$@"[{_getItemAtIndex(low),pad} — {midValue,pad} — {_getItemAtIndex(high),pad}]".Print();
		}
		void _print2(int valComparedToMid, int match, bool findLowRange)
		{
			string msg1 = null;
			if(findLowRange) {
				msg1 = valComparedToMid > 0
				? "GREATER (Move LOW UP [mid+1])"
				: $"{(valComparedToMid < 0 ? "LESSER" : "EQUAL")} (Move HIGH DOWN [mid-1])";
			}
			else {
				msg1 = valComparedToMid >= 0
					? $"{(valComparedToMid > 0 ? "GREATER" : "EQUAL")} (Move LOW UP [mid+1])"
					: $"LOWER (Move HIGH DOWN [mid-1])";
			}

			string matchValStr = match < 0 ? "-" : _getItemAtIndex(match).ToString();

			$@">>>   ({msg1,-5}) => lastMatch:[i:{match,-2}] !{matchValStr}!".Print();
		}
#endif

		internal int _GetHighFromInputLength(int index, int? length)
		{
			int high = length == null
				? _collLength - index - 1
				: (index + (int)length) - 1;
			return high;
		}

		#endregion

		#region --- GetBinarySearchFailedLowHigh ---

		/// <summary>
		/// Obtains a BinarySearchFailedLowHigh struct to be used when 
		/// the binary search failed (i.e. returning a negative number), useful
		/// for understanding where the queried value would be in the sequence.
		/// This is simply a one-line indirection to the BinarySearchFailedLowHigh 
		/// constructor, put here for discoverability purpose.
		/// </summary>
		/// <param name="failedNegativeReturn">The negative int returned from a failed BinarySearch find.</param>
		/// <param name="sequenceLength">The length of the searched sequence.</param>
		public static BinarySearchFailedLowHigh GetBinarySearchFailedLowHigh(int failedNegativeReturn, int sequenceLength)
		{
			return new BinarySearchFailedLowHigh(failedNegativeReturn, sequenceLength);
		}

		public BinarySearchFailedLowHigh GetFailedLowHigh(int failedNegativeReturn)
		{
			return new BinarySearchFailedLowHigh(failedNegativeReturn, _collLength);
		}

		#endregion

	}
}
