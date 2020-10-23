using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DotNetXtensions
{
	public abstract class SimpleEquality<T>
		: IEquatable<T>, IEqualityComparer<T> where T : SimpleEquality<T>
	{
		public abstract IEnumerable<object> GetEqualityItems();

		public bool IsEqual(T x, T y)
		{
			if(x == null || y == null)
				return false;

			var seqX = x.GetEqualityItems();
			var seqY = y.GetEqualityItems();

			if(seqX == null)
				return seqY == null;
			else if(seqY == null)
				return false;

			bool isEqual = seqX.SequenceEqual(seqY);
			return isEqual;
		}

		public abstract bool Equals(T other); // => IsEqual(, other);

		public bool Equals(T x, T y)
			=> IsEqual(x, y);

		public int GetHashCode(T obj)
			=> GetObjectSequenceHashCode(obj?.GetEqualityItems());

		/// <summary>
		/// See : https://stackoverflow.com/a/3404820/264031
		/// </summary>
		public static int GetObjectSequenceHashCode(IEnumerable<object> seq)
		{
			if(seq == null)
				return 0;

			int hc = 7; // array.Length;
			foreach(object o in seq) {
				int objHash = o?.GetHashCode() ?? 0;
				hc = unchecked((hc + 3) * 314159 + objHash);
			}

			return hc;
		}

	}
}
