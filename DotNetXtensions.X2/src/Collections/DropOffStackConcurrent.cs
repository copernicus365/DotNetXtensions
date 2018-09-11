using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetXtensions
{
	/// <summary>
	/// A concurrent version of DropOffStack, see that version for more description,
	/// this one is kept in sync with that code base. 
	/// Any activity that changes or accesses
	/// state that is dynamic acquires a lock first making this thread safe. 
	/// </summary>
	public class DropOffStackConcurrent<T> : ICollection<T>, IList<T>
	{
		private readonly int capacity;
		private readonly T[] items;
		private int top;
		private int count;
		private object lockObj = new object();

		public DropOffStackConcurrent(int capacity)
		{
			if(capacity < 1) throw new ArgumentOutOfRangeException("As a fixed capacity collection the capacity set must be greater than 0.");

			this.capacity = capacity;
			items = new T[this.capacity];
		}


		public int Count { get { return count; } }

		public int Capacity { get { return capacity; } }

		public DropOffStackConcurrent<T> Push(T item)
		{
			lock (lockObj) {
				items[top] = item;
				top = (top + 1) % capacity;
				if(count < capacity) ++count;
			}
			return this;
		}

		public T Pop()
		{
			lock (lockObj) {
				if(count < 1)
					throw new InvalidOperationException();
				--count;
				top = (capacity + top - 1) % capacity;
				T val = items[top];
				items[top] = default(T);
				return val;
			}
		}

		public T Peek()
		{
			lock (lockObj) {
				if(count < 1)
					throw new InvalidOperationException();
				int idx = (capacity + top - 1) % capacity;
				T val = items[idx];
				return val;
			}
		}

		public bool TryPop(out T value)
		{
			lock (lockObj) {
				if(count < 1) {
					value = default(T);
					return false;
				}
				value = Pop();
				return true;
			}
		}

		public bool TryPeek(out T value)
		{
			lock (lockObj) {
				if(count < 1) {
					value = default(T);
					return false;
				}
				value = Peek();
				return true;
			}
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
			foreach(T item in GetItems()) //GetItems aquires lock the whole time, before then we access no state
				array[i++] = item;
		}

		public IEnumerable<T> GetItems()
		{
			lock (lockObj) {
				for(int i = top - 1, taken = 0; taken < count; i--, taken++) {
					if(i < 0) i = capacity - 1;
					yield return items[i];
				}
			}
		}

		public IEnumerable<T> GetItemsAddedOrder()
		{
			lock (lockObj) {
				for(int i = top + 1, taken = 0; taken < count; i++, taken++) {
					if(i >= capacity) i = 0;
					yield return items[i];
				}
			}
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T item)
		{
			int i = 0;
			foreach(T itm in GetItems()) { // GetItems aquires state the whole time
				i++;
				if(itm.Equals(item))
					return i;
			}
			return -1;
		}

		public void Clear()
		{
			lock (lockObj) {
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
			if(index < 0 || index >= count) throw new ArgumentOutOfRangeException();
			int idx = ((capacity + top - 1) % capacity) - index;
			if(idx < 0)
				idx = capacity + idx;
			return idx;
		}

		public T this[int index] {
			get {
				lock (lockObj) {
					int idx = getIndex(index);
					T val = items[idx];
					return val;
				}
			}
			set {
				lock (lockObj) {
					int idx = getIndex(index);
					items[idx] = value;
				}
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