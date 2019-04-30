using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Class useful for being able to send in a simple
	/// Func that acts as the comparer for cases when a 
	/// full Comparer (a class!) is requested, as is the 
	/// case for instance with Array and many other CLR types.
	/// </summary>
	/// <typeparam name="T">Type.</typeparam>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	class ComparerX<T> : Comparer<T>, IEqualityComparer<T>
	{
		#region --- static members ---

		static bool _defaultsSet;
		static Func<T, T, int> _defaultComparer;
		static Func<T, T, bool> _defaultEqualityComparer;

		/// <summary>
		/// Gets the default comparer of type T if it implements
		/// IComparable[T], but if not, it does NOT throw an exception 
		/// and simply returns null (before calling Comparer[T].Default.Compare, we first 
		/// check if the Type implements the interface). This value is CACHED on the first 
		/// static call (whether or not it implements the interface) for utmost performance.
		/// </summary>
		public static Func<T, T, int> DefaultComparer {
			get {
				if(!_defaultsSet)
					SetDefaults();
				return _defaultComparer;
			}
		}

		/// <summary>
		/// Gets the default EqualityComparer of type T if it implements
		/// IEquatable[T] OR if it only implements IComparable[T], in which case we use that
		/// to test for equality (if equals 0). But if neither of these is true, it does NOT throw an exception 
		/// and simply returns null (before calling EqualityComparer[T].Default.Equals, we first 
		/// check if the Type implements the interface). This value is CACHED on the first 
		/// static call (whether or not it implements the interfaces) for utmost performance.
		/// </summary>
		public static Func<T, T, bool> DefaultEqualityComparer {
			get {
				if(!_defaultsSet)
					SetDefaults();
				return _defaultEqualityComparer;
			}
		}


		static void SetDefaults()
		{
			Type[] tInterfaces = typeof(T).GetInterfaces();

			Type iComparableType = typeof(IComparable<>);
			if(tInterfaces.Any(
				typ => typ.IsGenericType && typ.GetGenericTypeDefinition() == iComparableType)) {
				_defaultComparer = Comparer<T>.Default.Compare;
			}

			Type tEqIComparableG = typeof(IEquatable<>);
			if(tInterfaces.Any(
				typ => typ.IsGenericType && typ.GetGenericTypeDefinition() == tEqIComparableG)) {
				_defaultEqualityComparer = EqualityComparer<T>.Default.Equals;
			}
			else if(_defaultComparer != null) {
				_defaultEqualityComparer = (t1, t2) => _defaultComparer(t1, t2) == 0;
			}

			_defaultsSet = true;
		}


		#endregion

		Func<T, T, int> _comparer;
		Func<T, int> _getHash;

		/// <summary>
		/// Constructor. This is what is helpful, just 
		/// say: 
		/// <code>new ComparerX(/*your func here, e.g.: s => s == "cool" */)</code>.
		/// </summary>
		/// <param name="comparer">Comparer Func.</param>
		/// <param name="getHash"></param>
		public ComparerX(Func<T, T, int> comparer, Func<T, int> getHash = null)
		{
			_comparer = comparer ?? Comparer<T>.Default.Compare;
			_getHash = getHash;
		}

		/// <summary>
		/// Compare function that simply calls the function 
		/// sent in to the constructor.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">B.</param>
		/// <returns>Standard.</returns>
		public override int Compare(T a, T b)
		{
			return _comparer(a, b);
		}

		public bool Equals(T x, T y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(T obj)
		{
			if(_getHash != null)
				return _getHash(obj);
			return obj.GetHashCode();
		}
	}

	public class ComparerX<T, TKey> : Comparer<T>, IEqualityComparer<T>
	{
		Func<TKey, TKey, int> _comparer;
		Func<TKey, int> _getHash;
		Func<T, TKey> _keySelector;
		bool _reverse;

		/// <summary>
		/// Constructor. This is what is helpful, just 
		/// say: 
		/// <code>new ComparerX(/*your func here, e.g.: s => s == "cool" */)</code>.
		/// </summary>
		/// <param name="keySelector"></param>
		/// <param name="comparer">Comparer Func.</param>
		/// <param name="getHash"></param>
		/// <param name="reverse"></param>
		public ComparerX(
			Func<T, TKey> keySelector,
			Func<TKey, TKey, int> comparer,
			Func<TKey, int> getHash = null,
			bool reverse = false)
		{
			if(keySelector == null) throw new ArgumentNullException("keySelector");

			this._keySelector = keySelector;

			_comparer = comparer == null
				? Comparer<TKey>.Default.Compare
				: comparer;

			if(getHash != null)
				_getHash = getHash;

			_reverse = reverse;
		}

		/// <summary>
		/// Compare function that simply calls the function 
		/// sent in to the constructor.
		/// </summary>
		/// <param name="a">A.</param>
		/// <param name="b">B.</param>
		/// <returns>Standard.</returns>
		public override int Compare(T a, T b)
		{
			int val = _comparer(_keySelector(a), _keySelector(b));
			return _reverse ? -val : val;
		}

		public bool Equals(T x, T y)
		{
			return Compare(x, y) == 0;
		}

		public int GetHashCode(T t)
		{
			if(_getHash != null)
				return _getHash(_keySelector(t));
			return -1;
		}
	}

}
