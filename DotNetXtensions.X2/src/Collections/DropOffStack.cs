using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetXtensions
{
	/// <summary>
	/// A Stack that is of a static capacity, which does the pushing and popping
	/// by internally cycling through an array with a 'top' indexer. So what this
	/// means is if you set the capacity to 100 and reach 100, upon pushing 
	/// the next one, the item added 100 ago will be dropped off.
	/// <para />
	/// The capacity one must set in the constructor is the size that will be 
	/// set at the start internally, it does not dynamically increase in size up to that value,
	/// so if the capacity is 1 million, be aware that that instantiates an array
	/// internally of size 1 million. The whole implementation depends on this.
	/// However, if one was wanting items to drop off at a certain point,
	/// they already were willing to have a data structure that large in memory
	/// (of course the array until filled will simply be null items, i.e. simply empty 
	/// pointers).
	/// <para />
	/// Inspired from the following sources: 
	/// http://stackoverflow.com/a/384097/264031 and 
	/// http://courses.cs.vt.edu/~cs2704/spring04/projects/DropOutStack.pdf.
	/// </summary>
	/// <typeparam name="T">Type.</typeparam>
	public class DropOffStack<T> : ICollection<T>, IList<T>
	{
		private readonly int capacity;
		private readonly T[] items;
		private int top;
		private int count;


		public DropOffStack(int capacity)
		{
			if(capacity < 1) throw new ArgumentOutOfRangeException("As a fixed capacity collection the capacity set must be greater than 0.");

			this.capacity = capacity;
			items = new T[this.capacity];
		}


		public int Count { get { return count; } }

		public int Capacity { get { return capacity; } }

		public DropOffStack<T> Push(T item)
		{
			items[top] = item;
			top = (top + 1) % capacity;
			if(count < capacity) ++count;
			return this;
		}

		public T Pop()
		{
			if(count < 1)
				throw new InvalidOperationException();
			--count;
			top = (capacity + top - 1) % capacity;
			T val = items[top];
			items[top] = default(T);
			return val;
		}

		public T Peek()
		{
			if(count < 1)
				throw new InvalidOperationException();
			int idx = (capacity + top - 1) % capacity;
			T val = items[idx];
			return val;
		}

		public bool TryPop(out T value)
		{
			if(count < 1) {
				value = default(T);
				return false;
			}
			value = Pop();
			return true;
		}

		public bool TryPeek(out T value)
		{
			if(count < 1) {
				value = default(T);
				return false;
			}
			value = Peek();
			return true;
		}

		public T PopN()
		{
			T value;
			TryPop(out value);
			return value;
		}

		// used by ToArray so must implement
		public void CopyTo(T[] array, int arrayIndex)
		{
			int i = arrayIndex;
			foreach(T item in GetItems())
				array[i++] = item;
		}

		public IEnumerable<T> GetItems()
		{
			for(int i = top - 1, taken = 0; taken < count; i--, taken++) {
				if(i < 0) i = capacity - 1;
				yield return items[i];
			}
		}

		public IEnumerable<T> GetItemsAddedOrder()
		{
			for(int i = top + 1, taken = 0; taken < count; i++, taken++) {
				if(i >= capacity) i = 0;
				yield return items[i];
			}
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T item)
		{
			int i = 0;
			foreach(T itm in GetItems()) {
				i++;
				if(itm.Equals(item))
					return i;
			}
			return -1;
		}

		public void Clear()
		{
			// was:
			//items = new T[capacity];
			//top = count = 0;

			T defT = default(T);
			for(int i = top - 1, taken = 0; taken < count; i--, taken++) {
				if(i < 0) i = capacity - 1;
				items[i] = defT;
			}
			top = count = 0;
		}

		#region --- ICollection ---

		public bool IsReadOnly { get { return false; } }

		public void Add(T item) { Push(item); }

		public IEnumerator<T> GetEnumerator()
		{
			return GetItems().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion


		// --- IList ---

		int getIndex(int index)
		{
			if(index < 0 || index >= count)  throw new ArgumentOutOfRangeException();
			int idx = ((capacity + top - 1) % capacity) - index;
			if(idx < 0)
				idx = capacity + idx;
			return idx;
		}

		public T this[int index] {
			get {
				int idx = getIndex(index);
				T val = items[idx];
				return val;
			}
			set {
				int idx = getIndex(index);
				items[idx] = value;
			}
		}




		// --- NotImplemented ---

		bool ICollection<T>.Remove(T item)
		{
			throw new NotImplementedException();
		}

		void IList<T>.RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		void IList<T>.Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

	}
}