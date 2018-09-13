using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions
{
	/// <summary>
	/// This nice solution was altered from http://stackoverflow.com/a/8105057/264031.
	/// </summary>
	public class BiDirectionalDictionary<TLeft, TRight> : IDictionary<TLeft, TRight> 
	{
		Dictionary<TLeft, TRight> leftD = new Dictionary<TLeft, TRight>();
		Dictionary<TRight, TLeft> rightD = new Dictionary<TRight, TLeft>();

		//public enum BiDirectionalDictionaryAddType
		//{
		//	DuplicateThrowsException,
		//	Replace,
		//	KeepFirstIn
		//}

		// -- Indexers --
		public TLeft this[TRight rightKey] {
			get { return rightD[rightKey]; }
			set { Add(value, rightKey); }
		}

		public TRight this[TLeft leftKey] {
			get { return leftD[leftKey]; }
			set { Add(leftKey, value); }
		}


		// -- ContainsKey --
		public bool ContainsKey(TLeft leftKey) { return leftD.ContainsKey(leftKey); }
		public bool ContainsKey(TRight rightKey) { return rightD.ContainsKey(rightKey); }


		public bool Remove(TLeft key)
		{
			TRight r;
			if(leftD.TryGetValue(key, out r)) {
				leftD.Remove(key);
				rightD.Remove(r);
				return true;
			}
			return false;
		}

		public bool Remove(TRight v)
		{
			TLeft r;
			if(rightD.TryGetValue(v, out r)) {
				rightD.Remove(v);
				leftD.Remove(r);
				return true;
			}
			return false;
		}


		public TLeft V(TRight rightKey, TLeft defaultV = default(TLeft))
		{
			TLeft val;
			if(rightD.TryGetValue(rightKey, out val)) {
				return val;
			}
			return defaultV;
		}

		public TRight V(TLeft leftKey, TRight defaultV = default(TRight))
		{
			TRight val;
			if(leftD.TryGetValue(leftKey, out val)) {
				return val;
			}
			return defaultV;
		}


		public bool TryGetValue(TLeft key, out TRight value)
		{
			if(leftD.TryGetValue(key, out value)) {
				return true;
			}
			return false;
		}

		public bool TryGetValue(TRight key, out TLeft value)
		{
			if(rightD.TryGetValue(key, out value)) {
				return true;
			}
			return false;
		}


		// Add
		public BiDirectionalDictionary<TLeft, TRight> Add(TLeft left, TRight right)
		{
			//if(leftD.ContainsKey(left) || rightD.ContainsKey(right))
			//	throw new ArgumentException("Dictionary already contains one of the keys.");

			leftD.Add(left, right);
			rightD.Add(right, left);
			return this;
		}

		void IDictionary<TLeft, TRight>.Add(TLeft left, TRight right)
		{
			Add(left, right);
		}


		public void Add(KeyValuePair<TLeft, TRight> item)
		{
			Add(item.Key, item.Value);
		}


		public bool Contains(KeyValuePair<TLeft, TRight> item)
		{
			TRight r;
			if(leftD.TryGetValue(item.Key, out r) && r.Equals(item.Value))
				return true;
			return false;
		}

		public void CopyTo(KeyValuePair<TLeft, TRight>[] array, int arrayIndex)
		{
			int i = arrayIndex;
			foreach(var kv in leftD) {
				array[i++] = kv;
			}
		}

		public void Clear()
		{
			leftD.Clear();
			rightD.Clear();
		}

		public bool Remove(KeyValuePair<TLeft, TRight> item)
		{
			if(leftD.ContainsKey(item.Key)) {
				leftD.Remove(item.Key);
				rightD.Remove(item.Value);
			}
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<TLeft, TRight>> GetEnumerator()
		{
			return leftD.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public ICollection<TLeft> Keys { get { return leftD.Keys; } }

		public ICollection<TRight> Values { get { return rightD.Keys; } }

		public int Count { get { return leftD.Count; } }

		public bool IsReadOnly { get { return false; } }

	}
}