using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Fnv1a;
using DotNetXtensions.Cryptography;

namespace DotNetXtensions
{

	/// <summary>
	/// Extension methods for generating hashes.
	/// </summary>
	public static class XHashCommon
	{

		#region HexString Conversions

		/// <summary>
		/// Converts a string representing a byte array represented as a concatenated string of 
		/// hexadecimal values (where each byte was represented by two chars, so that e.g. byte 15
		/// is represented by "0F") back to a byte array.  
		/// </summary>
		/// <param name="concatenatedHexValues">The string representing a byte array.
		/// Each byte should have been represented with two chars, so that the length of this string 
		/// must be even.</param>
		/// <returns>The original byte array.</returns>
		public static byte[] ByteArrayHexStringToBytes(string concatenatedHexValues)
		{
			if (concatenatedHexValues == null)
				throw new ArgumentNullException("concatenatedHexValues");
			if (concatenatedHexValues.Length % 2 != 0)
				throw new ArgumentException("concatenatedHexValues.Length should be an even number " +
			"since every hex value in this string should represent a byte value, each of which should be represented " +
			"by two chars (e.g. integer 15 should be represented by \"0F\", not by \"F\").");

			byte[] bytes = new byte[concatenatedHexValues.Length / 2];

			for (int i = 0, j = 0; i < concatenatedHexValues.Length; i += 2, j++)
				bytes[j] = Byte.Parse(concatenatedHexValues.Substring(i, 2), NumberStyles.HexNumber);

			return bytes;
		}

		#endregion

		#region SHA HASH

		/// <summary>
		/// Gets a SHA256 hash on this string after encoding it into UTF-8 encoding.
		/// The hash (byte array) is returned.
		/// </summary>
		/// <param name="text">The string to get the SHA256 hash on.</param>
		/// <param name="shaLevel">SHALevel</param>
		public static byte[] GetSHA(this string text, SHALevel shaLevel = SHALevel.SHA256)
		{
			return XHash.GetSHAHash(null, text, shaLevel);
		}

		/// <summary>
		/// Gets a SHA256 hash on this string after encoding it into UTF-8 encoding.
		/// A hexadecimal string representing the hash (byte array) is returned.
		/// </summary>
		/// <param name="text">The string to get the SHA256 hash on.</param>
		/// <param name="shaLevel">SHALevel</param>
		/// <param name="lowerCase">Case of hex.</param>
		public static string GetSHAString(this string text, SHALevel shaLevel = SHALevel.SHA256, bool lowerCase = false)
		{
			return XHash.GetSHAHashString(null, text, shaLevel, lowerCase);
		}

		public static byte[] GetSHA(this byte[] data, SHALevel shaLevel = SHALevel.SHA256)
		{
			return XHash.GetSHAHash(data, null, shaLevel);
		}

		public static string GetSHAString(this byte[] data, SHALevel shaLevel = SHALevel.SHA256, bool lowerCase = false)
		{
			return XHash.GetSHAHashString(data, null, shaLevel, lowerCase);
		}

		#endregion

		public static ulong FastHashFNV1a64(this byte[] data, int offset = 0, int count = 0)
		{
			FNV1a64Hash fff = new FNV1a64Hash();

			byte[] hash = offset > 0 || count > 0
				? fff.ComputeHash(data, offset, count)
				: fff.ComputeHash(data);

			return fff.Hash64;
		}

		public static string FastHashFNV1a64ToString(this byte[] data, int offset = 0, int count = 0, bool lowercaseHex = false)
		{
			FNV1a64Hash fff = new FNV1a64Hash();

			byte[] hash = offset > 0 || count > 0
				? fff.ComputeHash(data, offset, count)
				: fff.ComputeHash(data);

			return hash.ToHexString(lowercaseHex);
		}

	}
}