using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetXtensions
{
	/// <summary>
	/// Wraps a generic System.Comparison[T] delegate in an IComparer to make it easy 
	/// to use a lambda expression for methods that take an IComparer or IComparer T.
	/// Source: http://www.velir.com/blog/index.php/2011/02/17/ilistt-sorting-a-better-way/
	/// </summary>
	public class ComparisonComparer<T> : IComparer<T>, IComparer
	{
		private readonly Comparison<T> _comparison;

		public ComparisonComparer(Comparison<T> comparison)
		{
			_comparison = comparison;
		}

		public int Compare(T x, T y)
		{
			return _comparison(x, y);
		}

		public int Compare(object o1, object o2)
		{
			return _comparison((T)o1, (T)o2);
		}
	}

#if !DNXPrivate
	public
#endif
		static class ComparisonComparerZ
	{
		/// <summary>
		/// Converts (wraps) the input Comparison[T] into a ComparisonComparer which 
		/// implements IComparer[T] and IComparer, interfaces which are often required
		/// when a Comparison[T] is not accepted.
		/// </summary>
		public static ComparisonComparer<T> ToIComparer<T>(this Comparison<T> comparison)
		{
			return new ComparisonComparer<T>(comparison);
		}
	}
}
