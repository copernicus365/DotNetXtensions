// SOURCE: https://referencesource.microsoft.com/#PresentationCore/Shared/MS/Internal/PriorityQueue.cs, 
// StackOverflow notes and discussion, with contributions to bug fixes here:
// https://stackoverflow.com/questions/44221454/bug-in-microsofts-internal-priorityqueuet 
// 

//-----------------------------------------------------------------------
//
//  Microsoft Windows Client Platform
//  Copyright (C) Microsoft Corporation. All rights reserved.
//
//  File:      PriorityQueue.cs
//
//  Contents:  Implementation of PriorityQueue class.
//
//  Created:   2-14-2005 Niklas Borson (niklasb)
//
//------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace DotNetXtensions // MS.Internal
{
	/// <summary>
	/// PriorityQueue provides a stack-like interface, except that objects
	/// "pushed" in arbitrary order are "popped" in order of priority, i.e., 
	/// from least to greatest as defined by the specified comparer.
	/// </summary>
	/// <remarks>
	/// Push and Pop are each O(log N). Pushing N objects and them popping
	/// them all is equivalent to performing a heap sort and is O(N log N).
	/// </remarks>
	public class PriorityQueue<T>
	{
		//
		// The _heap array represents a binary tree with the "shape" property.
		// If we number the nodes of a binary tree from left-to-right and top-
		// to-bottom as shown,
		//
		//             0
		//           /   \
		//          /     \
		//         1       2
		//       /  \     / \
		//      3    4   5   6
		//     /\    /
		//    7  8  9
		//
		// The shape property means that there are no gaps in the sequence of
		// numbered nodes, i.e., for all N > 0, if node N exists then node N-1
		// also exists. For example, the next node added to the above tree would
		// be node 10, the right child of node 4.
		//
		// Because of this constraint, we can easily represent the "tree" as an
		// array, where node number == array index, and parent/child relationships
		// can be calculated instead of maintained explicitly. For example, for
		// any node N > 0, the parent of N is at array index (N - 1) / 2.
		//
		// In addition to the above, the first _count members of the _heap array
		// compose a "heap", meaning each child node is greater than or equal to
		// its parent node; thus, the root node is always the minimum (i.e., the
		// best match for the specified style, weight, and stretch) of the nodes 
		// in the heap.
		//
		// Initially _count < 0, which means we have not yet constructed the heap.
		// On the first call to MoveNext, we construct the heap by "pushing" all
		// the nodes into it. Each successive call "pops" a node off the heap
		// until the heap is empty (_count == 0), at which time we've reached the
		// end of the sequence.
		//

		private T[] _heap;
		private int _count;
		private IComparer<T> _comparer;
		private const int DefaultCapacity = 6;


		public PriorityQueue(int capacity, IComparer<T> comparer = null)
		{
			_heap = new T[capacity > 0 ? capacity : DefaultCapacity];
			_count = 0;

			if (comparer == null) {
				comparer = ComparerX<T>.Default;
				if (comparer == null)
					throw new ArgumentNullException(nameof(comparer));
			}

			_comparer = comparer;
		}




		/// <summary>
		/// Gets the number of items in the priority queue.
		/// </summary>
		public int Count => _count;

		/// <summary>
		/// Gets the first or topmost object in the priority queue, which is the
		/// object with the minimum value.
		/// </summary>
		public T Top {
			get {
				Debug.Assert(_count > 0);
				return _heap[0];
			}
		}

		/// <summary>
		/// Adds an object to the priority queue.
		/// </summary>
		public PriorityQueue<T> Push(T value)
		{
			// Increase the size of the array if necessary.
			if (_count == _heap.Length) {
				T[] temp = new T[_count * 2];
				for (int i = 0; i < _count; ++i) {
					temp[i] = _heap[i];
				}
				_heap = temp;
			}

			// Loop invariant:
			//
			//  1.  index is a gap where we might insert the new node; initially
			//      it's the end of the array (bottom-right of the logical tree).
			//
			int index = _count;
			while (index > 0) {
				int parentIndex = HeapParent(index);
				if (_comparer.Compare(value, _heap[parentIndex]) < 0) {
					// value is a better match than the parent node so exchange
					// places to preserve the "heap" property.
					_heap[index] = _heap[parentIndex];
					index = parentIndex;
				}
				else {
					// we can insert here.
					break;
				}
			}

			_heap[index] = value;
			_count++;
			return this;
		}

		/// <summary>
		/// Removes the first node (i.e., the logical root) from the heap.
		/// </summary>
		/// <remarks>
		/// Code contributed by Kevin Gosse, https://stackoverflow.com/a/44222169/264031
		/// </remarks>
		public void Pop()
		{
			Debug.Assert(_count != 0);

			if (_count > 1) {
				// Loop invariants:
				//
				//  1.  parent is the index of a gap in the logical tree
				//  2.  leftChild is
				//      (a) the index of parent's left child if it has one, or
				//      (b) a value >= _count if parent is a leaf node
				//
				int parent = 0;
				int leftChild = HeapLeftChild(parent);

				while (leftChild < _count) {
					int rightChild = HeapRightFromLeft(leftChild);
					int bestChild =
						(rightChild < _count && _comparer.Compare(_heap[rightChild], _heap[leftChild]) < 0) ?
							rightChild : leftChild;

					// Promote bestChild to fill the gap left by parent.
					_heap[parent] = _heap[bestChild];

					// Restore invariants, i.e., let parent point to the gap.
					parent = bestChild;
					leftChild = HeapLeftChild(parent);
				}

				// Fill the last gap by moving the last (i.e., bottom-rightmost) node.
				_heap[parent] = _heap[_count - 1];

				// FIX: Rebalance the heap
				int index = parent;
				var value = _heap[parent];

				while (index > 0) {
					int parentIndex = HeapParent(index);
					if (_comparer.Compare(value, _heap[parentIndex]) < 0) {
						// value is a better match than the parent node so exchange
						// places to preserve the "heap" property.
						var pivot = _heap[index];
						_heap[index] = _heap[parentIndex];
						_heap[parentIndex] = pivot;
						index = parentIndex;
					}
					else {
						// Heap is balanced
						break;
					}
				}
			}

			_count--;
		}

		//// --- Code from Jim Mischel on SO (https://stackoverflow.com/a/44266945/264031), but it has it's own bugs...
		///// <summary>
		///// Removes the first node (i.e., the logical root) from the heap.
		///// </summary>
		//public void Pop1()
		//{
		//	Debug.Assert(_count != 0);

		//	if (_count > 0) {
		//		--_count;
		//		// Logically, we're moving the last item (lowest, right-most) to the root and then sifting it down.
		//		int idx = 0;
		//		while (idx < _count / 2) {
		//			// find the smallest child
		//			int smallestChild = HeapLeftChild(idx);
		//			int rightChild = HeapRightFromLeft(smallestChild);
		//			if (rightChild < _count - 1 && _comparer.Compare(_heap[rightChild], _heap[smallestChild]) < 0) {
		//				smallestChild = rightChild;
		//			}

		//			// If the item is less than or equal to the smallest child item, then we're done.
		//			if (_comparer.Compare(_heap[_count], _heap[smallestChild]) <= 0) {
		//				break;
		//			}

		//			// Otherwise, move the child up
		//			_heap[idx] = _heap[smallestChild];

		//			// and adjust the index
		//			idx = smallestChild;
		//		}
		//		// Place the item where it belongs
		//		_heap[idx] = _heap[_count];
		//		// and clear the position it used to occupy
		//		_heap[_count] = default(T);
		//	}
		//}

		// -- original Pop method with bug(s)
		///// <summary>
		///// Removes the first node (i.e., the logical root) from the heap.
		///// </summary>
		//public void Pop()
		//{
		//	Debug.Assert(_count != 0);

		//	if (_count > 1) {
		//		// Loop invariants:
		//		//
		//		//  1.  parent is the index of a gap in the logical tree
		//		//  2.  leftChild is
		//		//      (a) the index of parent's left child if it has one, or
		//		//      (b) a value >= _count if parent is a leaf node
		//		//
		//		int parent = 0;
		//		int leftChild = HeapLeftChild(parent);

		//		while (leftChild < _count) {
		//			int rightChild = HeapRightFromLeft(leftChild);
		//			int bestChild =
		//				(rightChild < _count && _comparer.Compare(_heap[rightChild], _heap[leftChild]) < 0) ?
		//				rightChild : leftChild;

		//			// Promote bestChild to fill the gap left by parent.
		//			_heap[parent] = _heap[bestChild];

		//			// Restore invariants, i.e., let parent point to the gap.
		//			parent = bestChild;
		//			leftChild = HeapLeftChild(parent);
		//		}

		//		// Fill the last gap by moving the last (i.e., bottom-rightmost) node.
		//		_heap[parent] = _heap[_count - 1];
		//	}

		//	_count--;
		//}

		public T Dequeue()
		{
			T val = Top;
			Pop();
			return val;
		}

		public T DequeueOrDefault()
		{
			if (_count <= 0)
				return default(T);
			T val = Top;
			Pop();
			return val;
		}


		/// <summary>
		/// Calculate the parent node index given a child node's index, taking advantage
		/// of the "shape" property.
		/// </summary>
		private static int HeapParent(int i) => (i - 1) / 2;

		/// <summary>
		/// Calculate the left child's index given the parent's index, taking advantage of
		/// the "shape" property. If there is no left child, the return value is >= _count.
		/// </summary>
		private static int HeapLeftChild(int i) => (i * 2) + 1;

		/// <summary>
		/// Calculate the right child's index from the left child's index, taking advantage
		/// of the "shape" property (i.e., sibling nodes are always adjacent). If there is
		/// no right child, the return value >= _count.
		/// </summary>
		private static int HeapRightFromLeft(int i) => i + 1;

	}
}
