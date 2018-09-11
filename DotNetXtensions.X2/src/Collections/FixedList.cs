using System;
using System.Collections.Generic;

namespace DotNetXtensions.Collections
{
	/// <summary>
	/// Unlike Lists, arrays (Array, T[]), which are a fixed size, can only be constructed
	/// by indexing into them. They cannot be built up in a sequential
	/// manner with convenient and clean methods like "array.Add(T)" or "array.AddRange(T[])"
	/// which would require Array to internally track the 'position' at which 
	/// items have been added. Also, Arrays of course do not have convenient
	/// AddRange methods. You would have to copy all elements from one array to the 
	/// other, while also always specifying the index position for each. 
	/// <para/>
	/// Sometimes, however, it would be greatly advantageous to have the convenience
	/// and greater cleanness in code of being able to build up an array of a 
	/// fixed size (not a dynamically changing size as with List) 
	/// with Add and AddRange methods, all while expecting it to remain of a fixed-size
	/// (with an OutOfRangeException thrown if you try to add beyond its size).
	/// <para/>
	/// Instead of multiplying many words, the following demonstration demonstrates 
	/// the usefulness of FixedList and what it offers:
	/// <example><code><![CDATA[
	/// static void DemonstrateFixedList(Stream stream)
	/// { 
	///		int bufferSize = 17;
	///		FixedList<byte> buffer = new FixedList<byte>(bufferSize);
	///	
	///		// the values to add here (a byte, two ints, and a long) are phony
	///		// for this demo, as they are hardcoded. Expectation in real scenario
	///		// is these are being gotten somehow in a real example
	///	
	///		for (int i = 0; i < 100; i++) // in real scenario, loop iterations determined otherwise of course
	///		{
	///			buffer.Clear(true); // FixedList.Clear(bool justResetPositionToZero) 
	///			// means only ONE internal byte[] ever made, no repeated byte[] instantiations
	///		
	///			byte b = 33;
	///			buffer.Add(b);
	///			// many lines later...
	///	
	///			// AddRange! regular byte[] would require iteration to copy
	///			// one to the other, or a call to Array.Copy every time, FixedList
	///			// takes care of all of that, and just as performantly
	///			int i1 = 3898823;
	///			buffer.AddRange(BitConverter.GetBytes(i1));
	///			// many lines later...
	///		
	///			int i2 = 89719;
	///			buffer.AddRange(BitConverter.GetBytes(i2));
	///			// many lines later...
	///		
	///			long l1 = 897521487454;
	///			buffer.AddRange(BitConverter.GetBytes(l1));
	///		
	///			// buffer has now had exactly 17 bytes added to it, write directly to stream
	///			stream.Write(buffer.ToArray(true), 0, bufferSize);
	///		}
	///		
	///		// On FixedList.ToArray(bool returnInternalArrayIfFull):
	///		// This returns the internal array (when true is specified) and when 
	///		// the FixedList is full, so that NO COPY needs be made, 
	///		// giving us the exact performance if we had built up a regular byte[] buffer
	///		// but this way, the code is easier and cleaner, as FixedList
	///		// worries about the current index add position, and as AddRange handles
	///		// (just as performantly as you could have) adding of ranges
	///	}
	/// ]]></code></example>
	/// </summary>
	/// <typeparam name="T">Type, not constraints.</typeparam>
	public class FixedList<T> : IList<T>
	{
		#region FIELDS

		T[] arr;
		int pos;
		int len;

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="arraySize">Size of FixedList.</param>
		public FixedList(int arraySize)
		{
			if (arraySize < 0) throw new ArgumentOutOfRangeException("arraySize");
			arr = new T[arraySize];
			len = arr.Length;
		}

		#region Used Methods / Indexers

		/// <summary>
		/// Index of item.
		/// </summary>
		/// <param name="item">Item to find.</param>
		/// <returns>Found index or -1.</returns>
		public int IndexOf(T item)
		{
			return Array.IndexOf<T>(arr, item);
		}

		/// <summary>
		/// Gets item at index.
		/// </summary>
		/// <param name="index">Index.</param>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= len)
					throw new ArgumentOutOfRangeException("index");
				return arr[index];
			}
			set
			{
				if (index < 0 || index >= len)
					throw new ArgumentOutOfRangeException("index");
				arr[index] = value;
			}
		}

		/// <summary>
		/// Returns all items in the FixedList.
		/// If returnInternalArrayIfFull is true and the internal
		/// array is full (with pos == FixedList.Length), 
		/// the actual internal array is returned,
		/// not a new copy of it. In LINQ and functional programming
		/// principles, a new copy would be expected, but the whole
		/// point of FixedList is to act highly performant
		/// as a buffer that is called repeatedly without having
		/// to instantiate new array objects every iteration. The
		/// key point is that we want FixedSizeArrayList to act 
		/// exactly like a regular array, but with the convenience
		/// and code simplified relief of being able to use 
		/// Add and AddRange (which for us is done by tracking a 
		/// current add position, equivalent to List's Count).
		/// </summary>
		/// <returns>Returns the internal array whole sale, not
		/// copying it for high efficiency.</returns>
		public T[] ToArray(bool returnInternalArrayIfFull)
		{
			if (returnInternalArrayIfFull && pos == len)
				return arr;
			else
				return ToArray();
		}

		/// <summary>
		/// Returns a copy of all items in the FixedList.
		/// </summary>
		public T[] ToArray()
		{
			if (pos > len) throw new Exception();
			T[] copyWhatsWritten = new T[pos];

			Array.Copy(arr, 0, copyWhatsWritten, 0, copyWhatsWritten.Length);

			return copyWhatsWritten;
		}

		/// <summary>
		/// Adds item to List. If Count is already full, exception
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			AssertCanAdd(1);
			arr[pos++] = item;
		}

		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="items">Items to add.</param>
		public void AddRange(T[] items)
		{
			int itemsLen = items.Length;

			AssertCanAdd(itemsLen);

			if (itemsLen < 20)
			{
				for (int i = 0; i < itemsLen; i++)
					arr[pos++] = items[i];
			}
			else
			{
				items.CopyTo(arr, pos);
				pos += itemsLen;
			}
		}

		/// <summary>
		/// True if FixedList is filled capacity.
		/// </summary>
		public bool IsFull
		{
			get { return pos == len; }
		}

		void AssertCanAdd(int numToAdd)
		{
			if ((pos + numToAdd) > len)
			{
				throw new ArgumentOutOfRangeException(
					"Cannot add attempted count of items, capacity reached.");
			}
		}

		#endregion

		#region ICollection<T> Members

		/// <summary>
		/// Clears the items. Actually replaces
		/// internal array.
		/// </summary>
		public void Clear()
		{
			arr = new T[len];
			pos = 0; 
		}

		/// <summary>
		/// For our unique purposes, this was largely the point of 
		/// having FixedSizeArrayList: having <paramref name="justRestPositionToZero"/>
		/// allows us to not allocate a new array EVERY time, thus
		/// making for much more efficiency when the array is being 
		/// used as a repetively used buffer filled up to len thousands
		/// to millions of times repetitively.
		/// </summary>
		/// <param name="justRestPositionToZero">True to simply
		/// reset the position to zero, meaning future Adds
		/// will be added starting at position 0 in the internal
		/// array. This means that all values currently in array
		/// *will remain* after calling Clear(true), so the expectation
		/// is that the whole array will be filled first thus 
		/// overwriting previously written values. But 
		/// just as with a regular array, which is always
		/// fixed size, *the expectation is always that it will
		/// be entirely written to the last position*, so this is 
		/// a reasonable expectation.</param>
		public void Clear(bool justRestPositionToZero)
		{
			if (!justRestPositionToZero)
				Clear();
			else
				pos = 0;
		}

		/// <summary>
		/// Indicates if item is contained in List.
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Contains(T item)
		{
			return Array.IndexOf<T>(arr, item) >= 0;
		}

		/// <summary>
		/// Copies items in List (starting at 0) into array
		/// at arrayIndex.
		/// </summary>
		/// <param name="array">Array to copy into.</param>
		/// <param name="arrayIndex">Index in array to start copying
		/// into it at.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			if (array == null) throw new ArgumentNullException("array");
			if (arrayIndex < 0 || (arrayIndex + array.Length) > arr.Length)
				throw new ArgumentOutOfRangeException();

			array.CopyTo(arr, arrayIndex);
		}

		/// <summary>
		/// Count of items in List.
		/// </summary>
		public int Count
		{
			get { return arr.Length; }
		}

		/// <summary>
		/// False.
		/// </summary>
		public bool IsReadOnly
		{
			get { return false; }
		}

		#endregion

		#region NotImplemented

		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IEnumerable<T> Members

		/// <summary>
		/// Gets list enumerator.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return arr.GetEnumerator() as IEnumerator<T>;
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return arr.GetEnumerator();
		}

		#endregion
	}
}
 