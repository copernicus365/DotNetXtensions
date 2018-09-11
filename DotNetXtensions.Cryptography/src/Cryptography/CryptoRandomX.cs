using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// Helper extensions to <see cref="CryptoRandom"/> class, containing many beautiful 
	/// randomizing functions for collections and strings.
	/// </summary>
	public static class CryptoRandomX
	{
		/// <summary>
		/// Gets a random string limited to characters that appear in the input string.
		/// This method simply calls <see cref="CryptoRandomX.GetRandomValues{T}(CryptoRandom, IList{T}, int)"/>,
		/// see further notes there.
		/// </summary>
		/// <param name="cr">CR instance.</param>
		/// <param name="str">The source string which dictates which characters to choose from.</param>
		/// <param name="count">Count of random values to get.</param>
		public static string GetRandomString(this CryptoRandom cr, string str, int count)
		{
			if (str == null || str.Length == 0)
				return str;

			char[] randomCharArr = cr.GetRandomValues(str.ToCharArray(), count);

			string randomStr = new string(randomCharArr);
			return randomStr;
		}

		/// <summary>
		/// Gets <paramref name="count"/> number of random values from the source collection.
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="cr">CR instance.</param>
		/// <param name="coll">Source collection to get random values from.</param>
		/// <param name="count">Count of random values to retrieve.</param>
		public static T[] GetRandomValues<T>(this CryptoRandom cr, IList<T> coll, int count)
		{
			if (coll == null)
				return null;

			int collCount = coll.Count;
			if (collCount == 0 || count < 1)
				return new T[0];

			int[] randomIndexesIntoColl = new int[count];
			cr.Next(randomIndexesIntoColl, max: collCount); // get the min: 0, maxIdx: count of the collection

			T[] result = randomIndexesIntoColl.Select(i => coll[i]).ToArray();

			return result;
		}

		/// <summary>
		/// Randomly shuffles a copy of the source string and returns the value.
		/// Simply calls <see cref="RandomShuffle{T}(CryptoRandom, IList{T})"/> after turning string
		/// to a char array.
		/// </summary>
		/// <param name="cr">CR instance.</param>
		/// <param name="str">Source string to shuffle</param>
		public static string RandomShuffle(this CryptoRandom cr, string str)
		{
			if (str == null || str.Length == 0)
				return str;

			char[] randomCharArr = cr.RandomShuffle(str.ToCharArray());

			string randomStr = new string(randomCharArr);
			return randomStr;
		}

		/// <summary>
		/// Randomly shuffles a copy of the source collection and returns the new, shuffled array.
		/// This depends on a call to <see cref="CryptoRandom.Next(int[], int, int)"/> to get random 
		/// integers to sort by.
		/// </summary>
		/// <typeparam name="T">T</typeparam>
		/// <param name="cr">CR instance.</param>
		/// <param name="coll">Source collection</param>
		public static T[] RandomShuffle<T>(this CryptoRandom cr, IList<T> coll)
		{
			if (coll == null)
				return null;

			int collCount = coll.Count;
			if (collCount == 0)
				return new T[0];

			int[] randomIndexesIntoColl = new int[collCount];
			cr.Next(randomIndexesIntoColl, 0, max: collCount);

			T[] randomColl = coll.ToArray();

			Array.Sort(randomIndexesIntoColl, randomColl);

			return randomColl;
		}

		/// <summary>
		/// Gets a random char from the input string. Highly performant, requires
		/// a single call to <see cref="CryptoRandom.Next(int, int)"/> and then simply indexes into
		/// the string. String CANNOT be null or empty (is treated as equivalent 
		/// to indexing into an empty or null string).
		/// </summary>
		/// <param name="cr">CR instance.</param>
		/// <param name="str">Input string.</param>
		public static char GetRandomCharFromString(this Random cr, string str)
		{
			if (str.IsNulle())
				throw new ArgumentOutOfRangeException();
			int idx = cr.Next(0, str.Length);
			char c = str[idx];

			//for (int i = 0; i < 100; i++) // something WRONG about this, diagnostic....
			//	str[NextInt(0, str.Length)].Print();

			return c;
		}

		/// <summary>
		/// Gets a random value from the input collection. Highly performant, requires
		/// a single call to <see cref="CryptoRandom.Next(int, int)"/> and then simply indexes into
		/// collection. Collection CANNOT be null or empty (is treated as equivalent 
		/// to indexing into an empty or null array).
		/// </summary>
		/// <typeparam name="T">Type</typeparam>
		/// <param name="cr">CR instance.</param>
		/// <param name="coll">Collection</param>
		public static T GetRandomValue<T>(this Random cr, IList<T> coll)
		{
			if (coll.IsNulle())
				throw new ArgumentOutOfRangeException();
			int idx = cr.Next(0, coll.Count);
			T val = coll[idx];
			return val;
		}

	}
}
