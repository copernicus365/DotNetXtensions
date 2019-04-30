//using System;
//using System.IO;
//using System.IO.Compression;
//using System.Security.Cryptography;

//namespace DotNetXtensions.Cryptography
//{
//	// currently internal. expose public?

//	#region XEncryption

//	/// <summary>
//	/// Encryption extension methods.
//	/// </summary>
//	internal static class XEncryptionOld
//	{
//		#region BYTES

//		public static byte[] Encrypt(this byte[] dataToEncrypt, byte[] privateKey, bool alsoDeflateCompress)
//		{
//			if (dataToEncrypt == null) throw new ArgumentNullException("dataToEncrypt");
//			MemoryStream writeStream = new MemoryStream(dataToEncrypt.Length + 200);
//			Encrypt(new MemoryStream(dataToEncrypt), writeStream, privateKey, alsoDeflateCompress);
//			return writeStream.ToArray();
//		}

//		public static byte[] Decrypt(this byte[] cryptedData, byte[] privateKey, bool alsoDecompress)
//		{
//			if (cryptedData == null) throw new ArgumentNullException("cryptedData");
//			MemoryStream writeStream = new MemoryStream(cryptedData.Length + 200);
//			Decrypt(new MemoryStream(cryptedData), writeStream, privateKey, alsoDecompress);
//			return writeStream.ToArray();
//		}

//		#endregion

//		#region STREAMS

//		public static void Encrypt(this Stream readStream, Stream writeStreamEncryptDest,
//			byte[] privateKey, bool useDeflateCompression)
//		{
//			if (readStream == null) throw new ArgumentNullException("readStreamEncryptSrc");
//			if (writeStreamEncryptDest == null) throw new ArgumentNullException("writeStreamEncryptDest");
//			if (privateKey == null) throw new ArgumentNullException("privateKey");
//			byte[] initVector = GetIVFromEncryptionKey(privateKey);
//			if (initVector == null) throw new ArgumentNullException("initVector");

//			CryptoStream cryptS = null;
//			DeflateStream compressS = null;
//			try
//			{
//				// what, no usings? no Dispose (in finally)?! See notes on Decrypt, STINKING CryptoStream disposes base stream
//				SymmetricAlgorithm aesAlgorithm = Rijndael.Create();
//				ICryptoTransform aesEncryptor = aesAlgorithm.CreateEncryptor(privateKey, initVector);
//				cryptS = new CryptoStream(writeStreamEncryptDest, aesEncryptor, CryptoStreamMode.Write);
//				{
//					if (useDeflateCompression)
//						using (compressS = new DeflateStream(cryptS, CompressionMode.Compress, true))
//							XStream.Write(compressS, readStream);
//					else
//						XStream.Write(cryptS, readStream);
//				}
//			} finally
//			{
//				if (cryptS != null)
//					cryptS.FlushFinalBlock(); // 
//			}
//		}

//		public static void Decrypt(this Stream readStreamEncrypted, Stream writeStream,
//			byte[] privateKey, bool useDeflateCompression)
//		{
//			if (readStreamEncrypted == null) throw new ArgumentNullException("readStreamEncrypted");
//			if (writeStream == null) throw new ArgumentNullException("writeStream");
//			if (privateKey == null) throw new ArgumentNullException("privateKey");
//			byte[] initVector = GetIVFromEncryptionKey(privateKey);
//			if (initVector == null) throw new ArgumentNullException("initVector");

//			CryptoStream cryptS = null;
//			DeflateStream deflS = null;
//			try
//			{
//				SymmetricAlgorithm aesAlgorithm = Rijndael.Create();
//				ICryptoTransform aesDecryptor = aesAlgorithm.CreateDecryptor(privateKey, initVector);

//				if (!useDeflateCompression)
//				{
//					using (cryptS = new CryptoStream(writeStream, aesDecryptor, CryptoStreamMode.Write))
//						XStream.Write(cryptS, readStreamEncrypted);
//				}
//				else
//				{
//					cryptS = new CryptoStream(readStreamEncrypted, aesDecryptor, CryptoStreamMode.Read);
//					using (deflS = new DeflateStream(cryptS, CompressionMode.Decompress, true))
//						XStream.Write(writeStream, deflS);
//				}
//			} finally
//			{
//				// do NOT call flush or flushxyz on CryptoStream, bec here, DeflStrm wraps it
//				// and already calls it. CryptStrm is AMAZINGLY STUPIDLY designed, breaking ALL 
//				// contracts usually expected, like Flush should not throw errors. OH NO, IT DOES.
//				// No wonder so many by pass .NET's version with couple open source options. TOTAL EMBARRASSMENT.
//			}
//		}

//		#endregion

//		#region SHARED

//		/// <summary>
//		/// An enumeration representing the bit-length of the key to be used in AES encryption.
//		/// </summary>
//		public enum AESKeySize
//		{
//			/// <summary>
//			/// A 128 bit length key (equivalent to 16 bytes).
//			/// </summary>
//			_128Bit,
//			/// <summary>
//			/// A 192 bit length key (equivalent to 24 bytes).
//			/// </summary>
//			_192Bit,
//			/// <summary>
//			/// A 256 bit length key (equivalent to 32 bytes).
//			/// </summary>
//			_256Bit
//		}

//		/// <summary>
//		/// Generates a key to be used in AES encryption (either a key or an initialization vector).
//		/// The AESKeySize value dictates how a key is generated (and its length), as follows:<para/>
//		/// AESKeySize._128Bit: a SHA256 hash is generated from key of which the first 
//		/// 16 bytes (= 128 bits) are returned.<para/>
//		/// AESKeySize._256Bit: a SHA256 hash is generated from key; the full 32 byte (256 bit)
//		/// hash is returned;<para/>
//		/// AESKeySize._192Bit: a SHA256 hash is generated from key and the first 
//		/// 24 bytes (= 192 bits) are returned.
//		/// </summary>
//		/// <param name="keyStr">The string to generate the key from.</param>
//		/// <param name="keyLength">The AESKeySize.</param>
//		/// <returns>A key which is 16, 24, or 32 bytes in length.</returns>
//		public static byte[] GetAESKey(string keyStr, AESKeySize keyLength)
//		{
//			switch (keyLength)
//			{
//				case AESKeySize._128Bit:
//					// returns a 128 bit (16 byte) key, composed of the first 16 bytes of a SHA256 hash of strKey
//					return keyStr.GetSHA(SHALevel.SHA256).Copy(0, 16);

//				case AESKeySize._256Bit:
//					// returns a 256 bit (32 byte) key, being a SHA256 hash of strKey
//					return keyStr.GetSHA(SHALevel.SHA256);

//				case AESKeySize._192Bit:
//					// returns a 192 bit (24 byte) key, composed of the first 24 bytes of a SHA256 hash of strKey
//					return keyStr.GetSHA(SHALevel.SHA256).Copy(0, 24);
//			}
//			throw new Exception();
//		}

//		/// <summary>
//		/// Retrieves an initialization vector (or 'public key') from the encryption
//		/// key that is sent in which must be 16, 24, or 32 bytes long.  If it is 
//		/// 16 long, then the same exact <i>key</i> is returned.  Otherwise the
//		/// first 16 bytes of the key are returned to be used as the IV.  Note that the
//		/// name of this method clearly indicates "FromEncryptionKey"; to generate 
//		/// an IV that is not based on the encrytion key, one may want to use 
//		/// GetAESKey and specify AESKeySize._128Bit. 
//		/// </summary>
//		/// <param name="key">The encryption key to use for attaining an IV.</param>
//		/// <returns>An initialization vector to be used in AES encryption.</returns>
//		public static byte[] GetIVFromEncryptionKey(byte[] key)
//		{
//			if (key == null) throw new ArgumentNullException("key");

//			switch (key.Length)
//			{
//				case 16:
//					return key;
//				case 32:
//				case 24:
//					return key.Copy(0, 16);

//				default: throw new ArgumentException("The key is of an invalid AES encryption key length.");
//			}
//		}

//		#endregion
//	}

//	#endregion
//}