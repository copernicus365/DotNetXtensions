using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Globalization;
using System.Security.Cryptography;
using Fnv1a;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// Extension methods for generating hashes.
	/// </summary>
	public static class XHash
	{
		/// <summary>
		/// Gets a SHA hash on the input data of the selected SHA encryption level.
		/// </summary>
		/// <param name="data">The byte array to get the hash on, or null if 
		/// <i>text</i> contains the data.</param>
		/// <param name="text">The text to get the hash on, or null if 
		/// <i>data</i> contains the data.</param>
		/// <param name="shaLevel">The SHA encryption level.</param>
		public static byte[] GetSHAHash(byte[] data, string text, SHALevel shaLevel)
		{
			if (text.NotNulle())
				data = Encoding.UTF8.GetBytes(text);
			else
				if (data == null)
					throw new ArgumentNullException(); // this could mean text param was wrongly null

			// data is now properly set (whether it started as text or not)
			switch (shaLevel)
			{
				case SHALevel.SHA256:
					return SHA256.Create().ComputeHash(data);
				case SHALevel.SHA1:
					return SHA1.Create().ComputeHash(data);
				case SHALevel.SHA384:
					return SHA384.Create().ComputeHash(data);
				case SHALevel.SHA512:
					return SHA512.Create().ComputeHash(data);
				default:
					return SHA256.Create().ComputeHash(data);
			}
		}

		/// <summary>
		/// Gets a SHA hash on the input data of the selected SHA encryption level
		/// and returns the resultant hash as a hexstring.
		/// </summary>
		/// <param name="data">The input data byte array, or null if sending in text.</param>
		/// <param name="text">The input text, or null if sending in data byte array.</param>
		/// <param name="shaLevel">The SHA encryption level.</param>
		/// <param name="lowerCase">Case of hex.</param>
		public static string GetSHAHashString(byte[] data, string text, SHALevel shaLevel, bool lowerCase = false)
		{
			byte[] hash = GetSHAHash(data, text, shaLevel);
			return hash.ToHexString(lowerCase);
			//return ByteArrayToHexString();
		}
	}
}
