using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace DotNetXtensions.Cryptography
{
	public static class RandomNumbers
	{
		public static uint[] GetRandomUIntArray(int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException();

			uint[] intArr = new uint[length];

			if (length > 0) {
				byte[] rndByteArr = new byte[length * sizeof(uint)];
				var rnd = new RNGCryptoServiceProvider();
				rnd.GetBytes(rndByteArr);
				Buffer.BlockCopy(rndByteArr, 0, intArr, 0, rndByteArr.Length);
			}

			return intArr;
		}

		public static int[] GetRandomIntArray(int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException();

			int[] intArr = new int[length];

			if (length > 0) {
				byte[] rndByteArr = new byte[length * sizeof(int)];
				var rnd = new RNGCryptoServiceProvider();
				rnd.GetBytes(rndByteArr);
				Buffer.BlockCopy(rndByteArr, 0, intArr, 0, rndByteArr.Length);
			}

			return intArr;
		}


		public static ulong[] GetRandomLongArray(int length)
		{
			if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
			ulong[] arr = new ulong[length];
			if (length > 0) { // if they want 0, why 'throw' a fit, just give it to them ;)
				byte[] rndByteArr = new byte[length * sizeof(ulong)];
				var rnd = new RNGCryptoServiceProvider();
				rnd.GetBytes(rndByteArr);
				Buffer.BlockCopy(rndByteArr, 0, arr, 0, rndByteArr.Length);
			}
			return arr;
		}

		//public static void TestSomeRandoms1()
		//{
		//	int totalN = 100;
		//	ulong[] randoms = RandomNumbers.GetRandomLongArray(totalN);

		//	int[] vals = new int[totalN];

		//	for (int i = 0; i < randoms.Length; i++) {
		//		ulong rand = randoms[i];
		//		int val = vals[i] = RandomNumbers.GetRandomNumber(0, totalN, rand);
		//	}

		//	vals.JoinToString().Print();

		//	int[] valsOrdered = vals.OrderBy(v => v).ToArray();
		//	valsOrdered.JoinToString().Print();

		//	int lastNum = 0;
		//	int consec = 0;
		//	List<int> consecs = new List<int>();
		//	List<int> gaps = new List<int>();

		//	for (int i = 0; i < valsOrdered.Length; i++) {
		//		int val = valsOrdered[i];
		//		if (val == lastNum)
		//			consec++;
		//		else {
		//			if (consec > 0) {
		//				consecs.Add(consec);
		//				consec = 0;
		//			}
		//			int gap = val - lastNum;
		//			gaps.Add(gap);
		//			lastNum = val;
		//		}
		//	}
		//	$"\r\nGAPS: \r\n{gaps.JoinToString()}".Print();
		//	$"\r\nGAPS > 2: \r\n{gaps.Where(g => g > 2).JoinToString()}".Print();
		//	$"\r\nCONSECS: \r\n{consecs.JoinToString()}".Print();
		//	$"\r\nCONSECS > 1: \r\n{consecs.Where(g => g > 1).JoinToString()}".Print();
		//}

	}
}
