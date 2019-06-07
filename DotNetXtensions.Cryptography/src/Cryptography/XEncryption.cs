using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// Extension methods for *ease-of-use* AES (256 by default) encryption and decryption of strings
	/// and byte arrays, for lower-priority based scenarios. In most cases, one should not use 
	/// these for mission-critical scenarios, unless you have 1) analyzed and 2) are satisifed with
	/// the full routine used herein, and importantly, are aware of any and all assumptions that are 
	/// made in that process.
	/// While the encryption used is `CryptoStream` with `System.Security.Cryptography.Rijndael` (AES),
	/// even so, we cannot guarantee that this routine was done perfectly, and,
	/// only a single encryption process is used, as opposed to having the output encrypted and re-encrypted
	/// many times over, which can produce more-secure encryption results. 
	/// <para />
	/// NOTES: 
	/// <para />
	/// 1) Uses AES-256 encryption when a string based key is passed in, though there is always the
	/// option to directly send in a byte array key, in which case it must be one of the valid lengths
	/// (16, 24, or 32 bytes, i.e. 128, 192, or 256 bits).
	/// <para />
	/// 2) Inputed string keys have a SHA-256 hash taken of them to generate the 256-bit key. You can
	/// also generate a key (byte array) youself using the same method by calling 
	/// <see cref="GetAESKey(string, AESKeySize)"/>.
	/// <para />
	/// 3) If no IV is input, the first 128 bytes (16 bytes) of the private key are used for the IV
	/// (which must be 16 bytes / 128 bits long, per AES requirements).
	/// </summary>
	public static class XEncryption
	{
		#region --- Stream based Encrypt / Decrypt algorithms (shift all over to these later) ---

		// NOTE!! In future probably would be best to delete the "RawEncrypt / RawDecrypt" 
		// methods and use these Stream based alogrithms instead.

		public static void Encrypt(this Stream readStream, Stream writeStreamEncryptDest,
			byte[] privateKey, bool useDeflateCompression)
		{
			if (readStream == null) throw new ArgumentNullException(nameof(readStream));
			if (writeStreamEncryptDest == null) throw new ArgumentNullException(nameof(writeStreamEncryptDest));
			if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
			byte[] initVector = GetIVFromEncryptionKey(privateKey);
			if (initVector == null) throw new ArgumentNullException(nameof(initVector));

			CryptoStream cryptS = null;
			DeflateStream compressS = null;
			try {
				// what, no usings? no Dispose (in finally)?! See notes on Decrypt, STINKING CryptoStream disposes base stream
				SymmetricAlgorithm aesAlgorithm = Rijndael.Create();
				ICryptoTransform aesEncryptor = aesAlgorithm.CreateEncryptor(privateKey, initVector);
				cryptS = new CryptoStream(writeStreamEncryptDest, aesEncryptor, CryptoStreamMode.Write);
				{
					if (useDeflateCompression)
						using (compressS = new DeflateStream(cryptS, CompressionMode.Compress, true))
							XStream.Write(compressS, readStream);
					else
						XStream.Write(cryptS, readStream);
				}
			}
			finally {
				if (cryptS != null)
					cryptS.FlushFinalBlock(); // 
			}
		}

		public static void Decrypt(this Stream readStreamEncrypted, Stream writeStream,
			byte[] privateKey, bool useDeflateCompression)
		{
			if (readStreamEncrypted == null) throw new ArgumentNullException(nameof(readStreamEncrypted));
			if (writeStream == null) throw new ArgumentNullException(nameof(writeStream));
			if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
			byte[] initVector = GetIVFromEncryptionKey(privateKey);
			if (initVector == null) throw new ArgumentNullException(nameof(initVector));
			try {
				SymmetricAlgorithm aesAlgorithm = Rijndael.Create();
				ICryptoTransform aesDecryptor = aesAlgorithm.CreateDecryptor(privateKey, initVector);
				CryptoStream cryptS;

				if (!useDeflateCompression) {
					using (cryptS = new CryptoStream(writeStream, aesDecryptor, CryptoStreamMode.Write))
						XStream.Write(cryptS, readStreamEncrypted);
				}
				else {
					cryptS = new CryptoStream(readStreamEncrypted, aesDecryptor, CryptoStreamMode.Read);

					using (DeflateStream deflS = new DeflateStream(cryptS, CompressionMode.Decompress, true))
						XStream.Write(writeStream, deflS);
				}
			}
			finally {
				// do NOT call flush or flushxyz on CryptoStream, bec here, DeflStrm wraps it
				// and already calls it. CryptStrm is AMAZINGLY STUPIDLY designed, breaking ALL 
				// contracts usually expected, like Flush should not throw errors. OH NO, IT DOES.
				// No wonder so many by pass .NET's version with couple open source options. TOTAL EMBARRASSMENT.
			}
		}

		public static byte[] Encrypt(this byte[] dataToEncrypt, byte[] privateKey, bool alsoDeflateCompress)
		{
			if (dataToEncrypt == null) throw new ArgumentNullException(nameof(dataToEncrypt));
			MemoryStream writeStream = new MemoryStream(dataToEncrypt.Length + 200);
			Encrypt(new MemoryStream(dataToEncrypt), writeStream, privateKey, alsoDeflateCompress);
			return writeStream.ToArray();
		}

		public static byte[] Decrypt(this byte[] cryptedData, byte[] privateKey, bool alsoDecompress)
		{
			if (cryptedData == null) throw new ArgumentNullException(nameof(cryptedData));
			MemoryStream writeStream = new MemoryStream(cryptedData.Length + 200);
			Decrypt(new MemoryStream(cryptedData), writeStream, privateKey, alsoDecompress);
			return writeStream.ToArray();
		}

		#endregion



		#region --- Key / IV Prep ---

		internal static void PrepKeys(
			ref byte[] privKeyBytes,
			ref byte[] initVectorBytes,
			string privKeyStr,
			string initVectorStr)
		{
			// PREP THE PRIVATE KEY  // notes are same as those above
			if (privKeyBytes == null) {
				if (privKeyStr == null)
					throw new ArgumentNullException(nameof(privKeyStr));
				else
					privKeyBytes = GetAESKey(privKeyStr, AESKeySize._256Bit);
			}

			// PREP THE PUBLIC KEY
			if (initVectorBytes == null) {
				// See https://crypto.stackexchange.com/a/50786 on IV size 128 only

				if (initVectorStr == null) // no iv was supplied
					initVectorBytes = GetIVFromEncryptionKey(privKeyBytes); // GetIVFromEncryptionKey ensures returns only 128 bytes (16 bytes)
				else // a pubKey string was explicitly specified
					initVectorBytes = GetAESKey(initVectorStr, AESKeySize._128Bit); // again: ***MUST*** be 128 bit for init-vector (128 bytes)
			}

			int privKyLen = privKeyBytes.Length;
			if (privKyLen != 32 && privKyLen != 16 && privKyLen != 24) // final key length check
				throw new ArgumentException("Check that the length of the private key is correct.");

			if (initVectorBytes.Length != 16) //check pubKeyLen
				throw new ArgumentException("Check that the length of the initialization vector is correct.");
		}

		/// <summary>
		/// Generates a key to be used in AES encryption (either a key or an initialization vector).
		/// The AESKeySize value dictates how a key is generated (and its length), as follows:<para/>
		/// AESKeySize._128Bit: a SHA256 hash is generated from key of which the first 
		/// 16 bytes (= 128 bits) are returned.<para/>
		/// AESKeySize._256Bit: a SHA256 hash is generated from key; the full 32 byte (256 bit)
		/// hash is returned;<para/>
		/// AESKeySize._192Bit: a SHA256 hash is generated from key and the first 
		/// 24 bytes (= 192 bits) are returned.
		/// </summary>
		/// <param name="keyStr">The string to generate the key from.</param>
		/// <param name="keySize">The AESKeySize.</param>
		/// <returns>A key which is 16, 24, or 32 bytes in length.</returns>
		public static byte[] GetAESKey(string keyStr, AESKeySize keySize)
		{
			switch (keySize) {
				case AESKeySize._128Bit:
					// returns a 128 bit (16 byte) key, composed of the first 16 bytes of a SHA256 hash of strKey
					return keyStr.GetSHA(SHALevel.SHA256).Copy(0, 16);

				case AESKeySize._256Bit:
					// returns a 256 bit (32 byte) key, being a SHA256 hash of strKey
					return keyStr.GetSHA(SHALevel.SHA256);

				case AESKeySize._192Bit:
					// returns a 192 bit (24 byte) key, composed of the first 24 bytes of a SHA256 hash of strKey
					return keyStr.GetSHA(SHALevel.SHA256).Copy(0, 24);
			}
			throw new Exception();
		}

		/// <summary>
		/// Retrieves an initialization vector (or 'public key') from the encryption
		/// key that is sent in which must be 16, 24, or 32 bytes long.  If it is 
		/// 16 long, then the same exact <i>key</i> is returned.  Otherwise the
		/// first 16 bytes of the key are returned to be used as the IV.  Note that the
		/// name of this method clearly indicates "FromEncryptionKey"; to generate 
		/// an IV that is not based on the encrytion key, one may want to use 
		/// GetAESKey and specify AESKeySize._128Bit. 
		/// </summary>
		/// <param name="key">The encryption key to use for attaining an IV.</param>
		/// <returns>An initialization vector to be used in AES encryption.</returns>
		public static byte[] GetIVFromEncryptionKey(byte[] key)
		{
			if (key == null) throw new ArgumentNullException(nameof(key));

			// note: ALWAYS gets only 16 bytes (128 bit), which is required for init-vector
			switch (key.Length) {
				case 16:
					return key;
				case 32:
				case 24:
					return key.Copy(0, 16);

				default: throw new ArgumentException("The key is of an invalid AES encryption key length.");
			}
		}

		#endregion


		#region CHIEF ENCRYPTION FUNCTIONS

		/// <summary>
		/// This is the main encryption method that is called by the other XEncryption methods
		/// for encrypting the bytes with the specified key and iv. It is publically exposed as a 
		/// static (non-extension) method for cases in which the caller has already prepared 
		/// the aruments (i.e. the in-parameter values).  The only validation performed is against
		/// null parameters.
		/// </summary>
		/// <param name="dataToEncrypt">The data to encrypt.</param>
		/// <param name="privateKey">The 128, 192, or 256-bit key to be used for the AES encryption.</param>
		/// <param name="initVector">The 128-bit initialization vector (cannot be null).</param>
		/// <param name="useDeflateCompression">A nullable parameter specifying whether or not
		/// to additionally compress the data with Deflate compression.  If not null, this setting *overrides*
		/// the UseDeflateCompression static XEncryption property setting.  Otherwise the default UseDeflateCompression
		/// value is used (*false* by default).</param>
		/// <returns>The encrypted data (bytes).</returns>
		public static byte[] RawEncrypt(byte[] dataToEncrypt, byte[] privateKey, byte[] initVector, bool? useDeflateCompression)
		{
			if (dataToEncrypt == null) throw new ArgumentNullException(nameof(dataToEncrypt));
			if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
			if (initVector == null) throw new ArgumentNullException(nameof(initVector));

			bool compress = useDeflateCompression == null
				? false //_UseDeflateCompression 
				: (bool)useDeflateCompression;

			try {
				MemoryStream encryptedDataMS = new MemoryStream(dataToEncrypt.Length / 2);

				using (SymmetricAlgorithm aesAlgorithm = Rijndael.Create())
				using (ICryptoTransform aesEncryptor = aesAlgorithm.CreateEncryptor(privateKey, initVector))
				using (Stream cryptS = new CryptoStream(encryptedDataMS, aesEncryptor, CryptoStreamMode.Write)) {
					if (compress)
						using (Stream compressS = new DeflateStream(cryptS, CompressionMode.Compress))
							compressS.Write(dataToEncrypt, 0, dataToEncrypt.Length);
					else
						cryptS.Write(dataToEncrypt, 0, dataToEncrypt.Length);
				}

				return encryptedDataMS.ToArray();
			}
			catch (CryptographicException) { return null; }
		}

		/// <summary>
		/// This is the main decryption method that is called by the other XEncryption methods. 
		/// It is publically exposed as a static (non-extension) method for cases in which the caller 
		/// has already prepared the aruments (i.e. the in-parameter values).  The only validation 
		/// performed is against null parameters.
		/// </summary>
		/// <remarks>
		/// Since CryptoStreamMode can read and write both ways, we choose the Write route when possible since
		/// in our tests writing is noticably faster and also produces cleaner code (no buffer is needed).
		/// *With* compression however, we do not have this option, because CryptoStreamMode must always Read
		/// with Decompression, which means CryptoStream must also Read as well.  This also means that our 
		/// compressed/encrypted data (byte array) must be wrapped into a stream:
		/// new CryptoStream(new MemoryStream(cryptedData)....  Fortunately however, creating a MemoryStream
		/// that wraps a pre-existing byte array incurrs virtually no memory or performance costs, as confirmed by 
		/// our tests, which verifies that the array is simply wrapped by the new MemoryStream.
		/// </remarks>
		/// <param name="cryptedData">The data to encrypt.</param>
		/// <param name="privateKey">The 128, 192, or 256-bit key to be used for the AES encryption.</param>
		/// <param name="initVector">The 128-bit initialization vector (cannot be null).</param>
		/// <param name="useDeflateCompression">A nullable parameter specifying whether or not
		/// to additionally compress the data with Deflate compression.  If not null, this setting *overrides*
		/// the UseDeflateCompression static XEncryption property setting.  Otherwise the default UseDeflateCompression
		/// value is used (*false* by default).</param>
		/// <returns>The decrypted data (bytes).</returns>
		public static byte[] RawDecrypt(byte[] cryptedData, byte[] privateKey, byte[] initVector, bool? useDeflateCompression)
		{
			if (cryptedData == null) throw new ArgumentNullException(nameof(cryptedData));
			if (privateKey == null) throw new ArgumentNullException(nameof(privateKey));
			if (initVector == null) throw new ArgumentNullException(nameof(initVector));

			// If parameter bool? useDeflateCompression is *not* null, then it overrides the static setting
			bool dataNeedsDecompressed = (useDeflateCompression == null)
				? false //_UseDeflateCompression 
				: (bool)useDeflateCompression;

			MemoryStream decryptedDataMS = new MemoryStream(cryptedData.Length);

			// See the "remarks" sections in the documentation (directly above) for some comments and notes...
			try {
				using (SymmetricAlgorithm aesAlgorithm = Rijndael.Create())
				using (ICryptoTransform aesDecryptor = aesAlgorithm.CreateDecryptor(privateKey, initVector)) {
					if (!dataNeedsDecompressed)
						using (Stream cryptS = new CryptoStream(decryptedDataMS, aesDecryptor, CryptoStreamMode.Write))
							cryptS.Write(cryptedData, 0, cryptedData.Length);
					else {
						using (Stream cryptS = new CryptoStream(new MemoryStream(cryptedData), aesDecryptor, CryptoStreamMode.Read))
						using (Stream deflS = new DeflateStream(cryptS, CompressionMode.Decompress)) {

							int bufferSize = cryptedData.Length.Min(4096);
							byte[] buffer = new byte[bufferSize];
							int bytesRead = 0;

							do {
								bytesRead = deflS.Read(buffer, 0, bufferSize);
								decryptedDataMS.Write(buffer, 0, bytesRead);
							} while (bytesRead != 0);
						}
					}
				}
				return decryptedDataMS.ToArray();
			}
			catch (CryptographicException) { return null; }
		}

		#endregion

		#region ENCRYPTORS

		/// <summary>
		/// The Encryption Hub which handles all of the needed conversions (of data and keys) 
		/// as well as the error checking.
		/// </summary>
		/// <param name="txtToEncrypt">The text to encrypt, or null (see arr).</param>
		/// <param name="privateKeyStr">The encryption key string, or null (see privateKeyBytes).</param>
		/// <param name="initVectorStr">The initialization vector / public key; may be null.</param>
		/// <param name="arr">The data to encrypt, or null (see s).</param>
		/// <param name="privateKeyBytes">The encryption key bytes, or null (see privateKeyStr).</param>
		/// <param name="initVectorBytes">The initialization vector bytes.</param>
		/// <param name="encoding">The encoding to encode text with, or null if no text.</param>
		/// <returns>The encrypted bytes.</returns>
		private static byte[] __EncryptHub(string txtToEncrypt, string privateKeyStr, string initVectorStr,
			byte[] arr, byte[] privateKeyBytes, byte[] initVectorBytes, Encoding encoding)
		{
			//<notes>
			// This may look like a lot of overhead, but it is actually very minimal.  In the end it 
			// boils down to at most 6 checks for null.  The rest
			// is all essential stuff (i.e. not overhead) like converting values 
			// from Base64 first.  If the user wanted the absolute fastest, they could send in bytes 
			// for all three main values (text, privKey, iv); then this boils down to three+ 
			// checks for null and a check that all the keys are of the correct size.  
			// </notes>

			// PREP THE MAIN CONTENT BYTES
			if (arr == null) // if there is no byte-data, a string (s) must contain the data
			{
				if (txtToEncrypt == null)
					throw new ArgumentNullException(nameof(txtToEncrypt));
				else  // convert textual data to bytes
					arr = encoding.GetBytes(txtToEncrypt);
			}

			PrepKeys(ref privateKeyBytes, ref initVectorBytes, privateKeyStr, initVectorStr);

			return RawEncrypt(arr, privateKeyBytes, initVectorBytes, null);
		}

		/// <summary>
		/// Sends all params to Encryption Hub; once the encrypted bytes are returned to this method, they
		/// are converted to a Base64 string.
		/// </summary>
		/// <param name="s">The text to encrypt, or null (see arr).</param>
		/// <param name="privateKeyStr">The encryption key string, or null (see privateKeyBytes).</param>
		/// <param name="initVectorStr">The initialization vector / public key; may be null.</param>
		/// <param name="arr">The data to encrypt, or null (see s).</param>
		/// <param name="privateKeyBytes">The encryption key bytes, or null (see privateKeyStr).</param>
		/// <param name="initVectorBytes">The initialization vector bytes.</param>
		/// <param name="encoding">The encoding to encode text with, or null if no text.</param>
		/// <param name="urlSafeB64">Convert the Base64 result to UrlSafeBase64.</param>
		/// <returns>The encrypted bytes.</returns>
		private static string __ReturnBase64String(string s, string privateKeyStr, string initVectorStr,
			byte[] arr, byte[] privateKeyBytes, byte[] initVectorBytes, Encoding encoding, bool urlSafeB64 = false)
		{
			byte[] encryptedBytes = __EncryptHub(
					s, privateKeyStr, initVectorStr, arr, privateKeyBytes, initVectorBytes, encoding);

			if (encryptedBytes == null)
				return null;

			string b64 = Convert.ToBase64String(encryptedBytes);

			if (urlSafeB64)
				b64 = XBase64.Base64ToUrlSafeBase64(b64);

			return b64;
		}

		// ============+++============

		public static byte[] Encrypt(this string s, string privateKey, string publicKey = null)
		{
			return __EncryptHub(s, privateKey, publicKey, null, null, null, Encoding.UTF8);
		}

		public static byte[] Encrypt(this string s, byte[] privateKey, byte[] publicKey = null)
		{
			return __EncryptHub(s, null, null, null, privateKey, publicKey, Encoding.UTF8);
		}

		public static byte[] Encrypt(this byte[] arr, string privateKey, string publicKey = null)
		{
			return __EncryptHub(null, privateKey, publicKey, arr, null, null, Encoding.UTF8);
		}

		public static byte[] Encrypt(this byte[] arr, byte[] privateKey, byte[] publicKey = null)
		{
			return __EncryptHub(null, null, null, arr, privateKey, publicKey, Encoding.UTF8);
		}

		// =======

		public static string EncryptToBase64(this string s, string privateKey, string publicKey = null)
		{
			return __ReturnBase64String(s, privateKey, publicKey, null, null, null, Encoding.UTF8);
		}

		public static string EncryptToBase64(this string s, byte[] privateKey, byte[] publicKey = null)
		{
			return __ReturnBase64String(s, null, null, null, privateKey, publicKey, Encoding.UTF8);
		}

		public static string EncryptToBase64(this byte[] arr, string privateKey, string publicKey = null)
		{
			return __ReturnBase64String(null, privateKey, publicKey, arr, null, null, Encoding.UTF8);
		}

		public static string EncryptToBase64(this byte[] arr, byte[] privateKey, byte[] publicKey = null)
		{
			return __ReturnBase64String(null, null, null, arr, privateKey, publicKey, Encoding.UTF8);
		}








		public static string EncryptToUrlSafeBase64(this string s, string privateKey, string publicKey = null)
		{
			return __ReturnBase64String(s, privateKey, publicKey, null, null, null, Encoding.UTF8, true);
		}

		public static string EncryptToUrlSafeBase64(this string s, byte[] privateKey, byte[] publicKey = null)
		{
			return __ReturnBase64String(s, null, null, null, privateKey, publicKey, Encoding.UTF8, true);
		}

		public static string EncryptToUrlSafeBase64(this byte[] arr, string privateKey, string publicKey = null)
		{
			return __ReturnBase64String(null, privateKey, publicKey, arr, null, null, Encoding.UTF8, true);
		}

		public static string EncryptToUrlSafeBase64(this byte[] arr, byte[] privateKey, byte[] publicKey = null)
		{
			return __ReturnBase64String(null, null, null, arr, privateKey, publicKey, Encoding.UTF8, true);
		}

		#endregion

		#region DECRYPTORS

		/// <summary>
		/// The Decryption Hub which handles all the needed conversions (of data and keys) 
		/// as well as the error checking.
		/// </summary>
		/// <param name="base64TextToDecrypt">A base-64 string which contains
		/// the encrypted data, or null (see bytesToDecrypt).</param>
		/// <param name="privateKeyStr">The encryption key string, or null (see privateKeyBytes).</param>
		/// <param name="initVectorStr">The initialization vector / public key; may be null.</param>
		/// <param name="bytesToDecrypt">The encrypted data to decrypt, or null 
		/// (see base64TextToDecrypt).</param>
		/// <param name="privateKeyBytes">The encryption key bytes, or null (see privateKeyStr).</param>
		/// <param name="initVectorBytes">The initialization vector bytes.</param>
		/// <param name="inputStrIsUrlSafeBase64"></param>
		/// <returns>The decrypted bytes.</returns>
		private static byte[] __DecryptHub(string base64TextToDecrypt, string privateKeyStr, string initVectorStr,
			byte[] bytesToDecrypt, byte[] privateKeyBytes, byte[] initVectorBytes, bool inputStrIsUrlSafeBase64 = false)
		{
			// PREP THE MAIN CONTENT BYTES
			if (bytesToDecrypt == null)  // if null, a (b64) string must have been passed in with the encrypted data.
			{
				if (base64TextToDecrypt == null)
					throw new ArgumentNullException(nameof(base64TextToDecrypt));
				else { // so convert from the Base64 encoding				
					bytesToDecrypt = inputStrIsUrlSafeBase64
						? base64TextToDecrypt.FromUrlSafeBase64ToBytes()
						: base64TextToDecrypt.FromBase64ToBytes(); // Convert.FromBase64String(base64TextToDecrypt);
				}
			}

			PrepKeys(ref privateKeyBytes, ref initVectorBytes, privateKeyStr, initVectorStr);

			return RawDecrypt(bytesToDecrypt, privateKeyBytes, initVectorBytes, null);
		}

		/// <summary>
		/// Sends all params to Decryption Hub; once the decrypted bytes are returned to this method, they
		/// are converted back to the original string (of the specified encoding).
		/// </summary>
		/// <param name="base64TextToDecrypt">A base-64 string which contains
		/// the encrypted data, or null (see bytesToDecrypt).</param>
		/// <param name="privateKeyStr">The encryption key string, or null (see privateKeyBytes).</param>
		/// <param name="initVectorStr">The initialization vector / public key; may be null.</param>
		/// <param name="bytesToDecrypt">The encrypted data to decrypt, or null 
		/// (see base64TextToDecrypt).</param>
		/// <param name="privateKeyBytes">The encryption key bytes, or null (see privateKeyStr).</param>
		/// <param name="initVectorBytes">The initialization vector bytes.</param>
		/// <param name="encoding">The original encoding type with which to convert the 
		/// decrypted data back to a string.</param>
		/// <param name="inputStrIsUrlSafeBase64"></param>
		/// <returns>The decrypted string.</returns>
		private static string __DecryptHubRetString(string base64TextToDecrypt, string privateKeyStr, string initVectorStr,
			byte[] bytesToDecrypt, byte[] privateKeyBytes, byte[] initVectorBytes, Encoding encoding, bool inputStrIsUrlSafeBase64 = false)
		{
			byte[] decryptedBytes = __DecryptHub(
					base64TextToDecrypt, privateKeyStr, initVectorStr, bytesToDecrypt, privateKeyBytes, initVectorBytes, inputStrIsUrlSafeBase64);

			if (decryptedBytes == null)
				return null;

			return encoding.GetString(decryptedBytes);
		}

		// ============+++============

		public static byte[] Decrypt(this string cryptedBase64Text, string privateKey, string publicKey = null)
		{
			return __DecryptHub(cryptedBase64Text, privateKey, publicKey, null, null, null);
		}

		public static byte[] Decrypt(this string cryptedBase64Text, byte[] privateKey, byte[] publicKey = null)
		{
			return __DecryptHub(cryptedBase64Text, null, null, null, privateKey, publicKey);
		}


		public static byte[] Decrypt(this byte[] cryptedBytes, string privateKey, string publicKey = null)
		{
			return __DecryptHub(null, privateKey, publicKey, cryptedBytes, null, null);
		}

		public static byte[] Decrypt(this byte[] cryptedBytes, byte[] privateKey, byte[] publicKey = null)
		{
			return __DecryptHub(null, null, null, cryptedBytes, privateKey, publicKey);
		}



		public static string DecryptToString(this string cryptedBase64Text, string privateKey, string publicKey = null)
		{
			return __DecryptHubRetString(cryptedBase64Text, privateKey, publicKey, null, null, null, Encoding.UTF8);
		}

		public static string DecryptToString(this string cryptedBase64Text, byte[] privateKey, byte[] publicKey = null)
		{
			return __DecryptHubRetString(cryptedBase64Text, null, null, null, privateKey, publicKey, Encoding.UTF8);
		}



		public static byte[] DecryptUrlSafeBase64(this string cryptedBase64Text, string privateKey, string publicKey = null)
		{
			byte[] decryptedBytes = __DecryptHub(
				cryptedBase64Text, privateKey, publicKey, null, null, null, inputStrIsUrlSafeBase64: true);
			return decryptedBytes;
		}
		public static byte[] DecryptUrlSafeBase64(this string cryptedBase64Text, byte[] privateKey, byte[] publicKey = null)
		{
			byte[] decryptedBytes = __DecryptHub(
				cryptedBase64Text, null, null, null, privateKey, publicKey, inputStrIsUrlSafeBase64: true);
			return decryptedBytes;
		}



		public static string DecryptUrlSafeBase64ToString(this string cryptedBase64Text, string privateKey, string publicKey = null)
		{
			return __DecryptHubRetString(cryptedBase64Text, privateKey, publicKey, null, null, null, Encoding.UTF8, true);
		}

		public static string DecryptUrlSafeBase64ToString(this string cryptedBase64Text, byte[] privateKey, byte[] publicKey = null)
		{
			return __DecryptHubRetString(cryptedBase64Text, null, null, null, privateKey, publicKey, Encoding.UTF8, true);
		}



		public static string DecryptToString(this byte[] cryptedBytes, string privateKey, string publicKey = null)
		{
			return __DecryptHubRetString(null, privateKey, publicKey, cryptedBytes, null, null, Encoding.UTF8);
		}

		public static string DecryptToString(this byte[] cryptedBytes, byte[] privateKey, byte[] publicKey = null)
		{
			return __DecryptHubRetString(null, null, null, cryptedBytes, privateKey, publicKey, Encoding.UTF8);
		}


		#endregion

	}
}
