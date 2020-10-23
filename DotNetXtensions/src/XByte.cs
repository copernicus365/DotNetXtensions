using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for byte arrays.
	/// </summary>
	public static class XByte
	{
		#region CopyBytes

		public static byte[] CopyBytes(this byte[] bytes)
		{
			if(bytes == null) return null;

			return CopyBytes(bytes, 0, bytes.Length);
		}

		public static byte[] CopyBytes(this byte[] bytes, int index)
		{
			if(bytes == null) return null;

			return CopyBytes(bytes, index, (bytes.Length - index));
		}

		public static byte[] CopyBytes(this byte[] bytes, int index, int length)
		{
			if(bytes == null) return null;

			// as elsewhere, with simple indirection calls like this, the bounds checking is left to the called method  
			byte[] range = new byte[length];
			Buffer.BlockCopy(bytes, index, range, 0, length);
			return range;
		}

		#endregion

		#region GetString

		public static string GetString(this byte[] bytes)
		{
			if(bytes == null) return null;

			return Encoding.UTF8.GetString(bytes);
		}

		public static string GetString(this byte[] bytes, int index, int length)
		{
			if(bytes == null) return null;

			// as elsewhere, with simple indirection calls like this, the bounds checking is left to the called method  
			return Encoding.UTF8.GetString(bytes, index, length);
		}

		#endregion

		public static void Save(this byte[] bytesToSave, string filePath)
		{
			if(bytesToSave == null) throw new ArgumentNullException("bytesToSave");
			if(filePath == null) throw new ArgumentNullException("filePath");

			File.WriteAllBytes(filePath, bytesToSave);
		}

		/// <summary>
		/// Converts the byte array to a hex string very fast. Excellent job
		/// with code lightly adapted from 'community wiki' here: http://stackoverflow.com/a/14333437/264031
		/// (the function was originally named: ByteToHexBitFiddle). Now allows a native lowerCase option
		/// to be input and allows null or empty inputs (null returns null, empty returns empty).
		/// </summary>
		public static string ToHexString(this byte[] bytes, bool lowerCase = false)
		{
			if(bytes == null)
				return null;
			else if(bytes.Length == 0)
				return "";

			char[] c = new char[bytes.Length * 2];

			int b;
			int xAddToAlpha = lowerCase ? 87 : 55;
			int xAddToDigit = lowerCase ? -39 : -7;

			for(int i = 0; i < bytes.Length; i++) {

				b = bytes[i] >> 4;
				c[i * 2] = (char)(xAddToAlpha + b + (((b - 10) >> 31) & xAddToDigit));

				b = bytes[i] & 0xF;
				c[i * 2 + 1] = (char)(xAddToAlpha + b + (((b - 10) >> 31) & xAddToDigit));
			}

			string val = new string(c);
			return val;

			/* Notes taken directly from the SO source above:

An explanation of the weird bit fiddling:

1.) bytes[i] >> 4 extracts the high nibble of a byte
bytes[i] & 0xF extracts the low nibble of a byte
2.) b - 10
is < 0 for values b < 10, which will become a decimal digit
is >= 0 for values b > 10, which will become a letter from A to F.
3.) Using i >> 31 on a signed 32 bit integer extracts the sign, thanks to sign extension. It will be -1 for i < 0 and 0 for i >= 0.
4.) Combining 2) and 3), shows that (b-10)>>31 will be 0 for letters and -1 for digits.
5.) Looking at the case for letters, the last summand becomes 0, and b is in the range 10 to 15. We want to map it to A(65) to F(70), which implies adding 55 ('A'-10).
6.) Looking at the case for digits, we want to adapt the last summand so it maps b from the range 0 to 9 to the range 0(48) to 9(57). This means it needs to become -7 ('0' - 55). Now we could just multiply with 7. But since -1 is represented by all bits being 1, we can instead use & -7 since (0 & -7) == 0 and (-1 & -7) == -7.

Some further considerations:

*) I didn't use a second loop variable to index into c, since measurement shows that calculating it from i is cheaper.
*) Using exactly i < bytes.Length as upper bound of the loop allows the JITter to eliminate bounds checks on bytes[i], so I chose that variant.
*) Making b an int allows unnecessary conversions from and to byte.

*/
		}

		public static string ToHexString(this IEnumerable<byte> bytes, bool lowerCase = false)
		{
			if(bytes == null)
				return null;
			byte[] arr = bytes.ToArray();
			return arr.ToHexString(lowerCase);
		}

	}
}
