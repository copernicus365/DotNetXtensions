using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DotNetXtensions.Compression
{

	/// <summary>
	/// Compression extension methods.
	/// </summary>
	public static class XCompression
	{
		// NEW, later, use these newer stream zip/unzip versions (perhaps)

		#region === Streams ===

		/// <summary>
		/// Compresses and writes the input stream to the output stream using DeflateStream.
		/// The input stream is only read from and is not altered as such, but rather, it is the 
		/// output stream that is written to with a compressed version of the input
		/// stream.  The input stream will be read from its current position to its end.
		/// </summary>
		/// <param name="readStream">The input stream which must be readable.</param>
		/// <param name="writeStream">The output stream that will contain the 
		/// compressed data of <i>inputStream</i>. This stream must be writable.</param>
		/// <param name="compressionType">The compression type: DeflateStream or GZip.</param>
		public static void Compress(this Stream readStream, Stream writeStream, CompressionType compressionType)
		{
			if (readStream == null) throw new ArgumentNullException("readStream");
			if (writeStream == null) throw new ArgumentNullException("writeStream");

			Stream deflS = null;
			try
			{
				// writeStream NOT be closed when its decorator stream (deflS) is Disposed with 'true'
				if (compressionType == CompressionType.DeflateStream)
					deflS = new DeflateStream(writeStream, CompressionMode.Compress, true);
				else
					deflS = new GZipStream(writeStream, CompressionMode.Compress, true);

				XStream.Write(deflS, readStream);
			}
			finally
			{
				if (deflS != null)
				{
					deflS.Flush();
					deflS.Dispose();  // This flushes the stream!!  Flush() does NOT work.
				}
			}
			/* OLD
		private static void __Zip(this Stream inputStream, Stream outputZippedStream, CompressionType compressionType)
		{
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (outputZippedStream == null) throw new ArgumentNullException("outputZippedStream");

			Stream deflS;

			// we set the output stream to NOT be closed when its decorator stream (deflS) is Disposed with 'true' param
			// note that inputStream is never decorated, so no such action need be taken
			if (compressionType == CompressionType.DeflateStream)
				deflS = new DeflateStream(outputZippedStream, CompressionMode.Compress, true);
			else
				deflS = new GZipStream(outputZippedStream, CompressionMode.Compress, true);

			try
			{
				int bufferLen = SetBufferSize(inputStream.Length);
				byte[] buffer = new byte[bufferLen];
				int bytesRead = 0;

				do
				{
					bytesRead = inputStream.Read(buffer, 0, bufferLen);
					deflS.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);

				deflS.Dispose();  // This flushes the stream!!  Flush() does NOT work.
			}
			finally { if (deflS != null) deflS.Dispose(); }
		}
			 */
		}

		/// <summary>
		/// Decompresses and writes the input stream to the output stream using DeflateStream.
		/// The input stream is only read from and is not altered as such, but rather, it is the 
		/// output stream that is written to with a decompressed version of the input
		/// stream.  The input stream will be read from its current position to its end.
		/// </summary>
		/// <param name="readStream">The input stream which is expected to contain 
		/// data that was compressed with DeflateStream or with a compatible compression type.
		/// This stream must be readable.</param>
		/// <param name="writeStream">The output stream that will contain the 
		/// decompressed data of <i>inputStream</i>.  This stream must be writable.</param>
		/// <param name="compressionType">The compression type to use: DeflateStream or GZip.</param>
		public static void Decompress(this Stream readStream, Stream writeStream, CompressionType compressionType)
		{
			if (readStream == null) throw new ArgumentNullException("readStream");
			if (writeStream == null) throw new ArgumentNullException("writeStream");

			Stream deflS = null;
			try
			{
				if (compressionType == CompressionType.DeflateStream)
					deflS = new DeflateStream(readStream, CompressionMode.Decompress, true);
				else
					deflS = new GZipStream(readStream, CompressionMode.Decompress, true);

				XStream.Write(writeStream, deflS);
			}
			finally
			{
				if (deflS != null)
				{
					deflS.Flush();
					deflS.Dispose();  // This flushes the stream!!  Flush() does NOT work.
				}
			}
			/*
		private static void __Unzip(this Stream inputStream, Stream outputUnzippedStream, CompressionType compressionType)
		{
			if (inputStream == null) throw new ArgumentNullException("inputStream");
			if (outputUnzippedStream == null) throw new ArgumentNullException("outputUnzippedStream");

			Stream deflS = null;

			if (compressionType == CompressionType.DeflateStream)
				deflS = new DeflateStream(inputStream, CompressionMode.Decompress, true);
			else
				deflS = new GZipStream(inputStream, CompressionMode.Decompress, true);

			try
			{
				int bufferSize = SetBufferSize(inputStream.Length);
				byte[] buffer = new byte[bufferSize];
				int bytesRead = 0;

				do
				{
					bytesRead = deflS.Read(buffer, 0, bufferSize);
					outputUnzippedStream.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);

				outputUnzippedStream.Flush();
				deflS.Dispose();
			}
			finally { if (deflS != null) deflS.Dispose(); }
		}
			 */
		}

		#endregion

		#region === Bytes ===

		public static byte[] Compress(this byte[] data, CompressionType compressionType)
		{
			MemoryStream readS = new MemoryStream(data);
			MemoryStream zippedS = new MemoryStream(data.Length + 200);

			Compress(readS, zippedS, compressionType);

			return zippedS.ToArray();
		}

		public static byte[] Decompress(this byte[] data, CompressionType compressionType)
		{
			MemoryStream readSZipped = new MemoryStream(data);
			MemoryStream unzippedS = new MemoryStream(data.Length);

			Decompress(readSZipped, unzippedS, compressionType);

			return unzippedS.ToArray();
		}

		public static byte[] Compress(this byte[] data)
		{
			return Compress(data, CompressionType.DeflateStream);
		}

		public static byte[] Decompress(this byte[] data)
		{
			return Decompress(data, CompressionType.DeflateStream);
		}

		#endregion

		#region Compress

		private static byte[] __Compress(byte[] dataToZip, CompressionType compressionType)
		{
			Stream deflS = null;
			MemoryStream zippedDataMS = new MemoryStream(dataToZip.Length / 2);

			try
			{
				if (compressionType == CompressionType.DeflateStream)
					deflS = new DeflateStream(zippedDataMS, CompressionMode.Compress);
				else
					deflS = new GZipStream(zippedDataMS, CompressionMode.Compress);

				deflS.Write(dataToZip, 0, dataToZip.Length);

				deflS.Dispose();
				// Solves major problems.  deflS.Flush() does NOT work (or do anything for that matter)
				// Calling Dispose DOES flush the last bytes to the underlying zippedDataMS

				return zippedDataMS.ToArray();
			}
			finally { if (deflS != null) deflS.Dispose(); }
		}

		private static byte[] __CompressHub(string textToZip, byte[] dataToZip, Encoding encoding, CompressionType compType)
		{
			if (dataToZip == null)
			{
				if (textToZip == null) throw new ArgumentNullException(); // could be due to dataToZip as well, so put no param

				dataToZip = encoding.GetBytes(textToZip);
			}

			return __Compress(dataToZip, compType);
		}

		private static string __CompressToBase64Hub(string textToZip, byte[] dataToZip, Encoding encoding, CompressionType compType)
		{
			if (dataToZip == null)
			{
				if (textToZip == null) throw new ArgumentNullException(); // could be due to dataToZip as well, so put no param

				dataToZip = encoding.GetBytes(textToZip);
			}

			byte[] zipped = __Compress(dataToZip, compType);

			return Convert.ToBase64String(zipped);
		}

		// =======+++=======

		public static string CompressToBase64(this string textToZip)
		{
			return __CompressToBase64Hub(textToZip, null, Encoding.UTF8, CompressionType.DeflateStream);
		}

		public static string CompressToBase64(this byte[] dataToZip)
		{
			return __CompressToBase64Hub(null, dataToZip, null, CompressionType.DeflateStream);
		}

		public static byte[] Compress(this string textToZip)
		{
			return __CompressHub(textToZip, null, Encoding.UTF8, CompressionType.DeflateStream);
		}

		public static byte[] Compress(this string textToZip, CompressionType compressionType)
		{
			return __CompressHub(textToZip, null, Encoding.UTF8, compressionType);
		}

		#endregion

		#region Decompress

		private static byte[] __Decompress(byte[] bytesToUnzip, CompressionType compressionType)
		{
			Stream deflS = null;
			MemoryStream zippedMS = new MemoryStream(bytesToUnzip);

			try
			{
				if (compressionType == CompressionType.DeflateStream)
					deflS = new DeflateStream(zippedMS, CompressionMode.Decompress);
				else
					deflS = new GZipStream(zippedMS, CompressionMode.Decompress);

				int bufferSize = SetBufferSize(bytesToUnzip.Length);
				byte[] buffer = new byte[bufferSize];
				int bytesRead = 0;
				MemoryStream unzippedBytesMS = new MemoryStream(bytesToUnzip.Length);

				do
				{
					bytesRead = deflS.Read(buffer, 0, bufferSize);
					unzippedBytesMS.Write(buffer, 0, bytesRead);
				} while (bytesRead != 0);

				deflS.Dispose();  // deflS.Flush() does NOT work (or do anything for that matter)
				unzippedBytesMS.Flush();

				return unzippedBytesMS.ToArray();
			}
			finally { if (deflS != null) deflS.Dispose(); }
		}

		private static byte[] __DecompressHub(string base64ZippedString, byte[] bytesToUnzip, CompressionType compType)
		{
			if (bytesToUnzip == null)
			{
				if (base64ZippedString == null) throw new ArgumentNullException();  // could be due to bytesToUnzip as well, so put no param

				bytesToUnzip = Convert.FromBase64String(base64ZippedString);
			}

			return __Decompress(bytesToUnzip, compType);
		}

		private static string __DecompressToStringHub(string base64ZippedString, byte[] bytesToUnzip, Encoding encoding, CompressionType compType)
		{
			if (bytesToUnzip == null)
			{
				if (base64ZippedString == null) throw new ArgumentNullException(); // could be due to bytesToUnzip as well, so put no param

				bytesToUnzip = Convert.FromBase64String(base64ZippedString);
			}

			return encoding.GetString(__Decompress(bytesToUnzip, compType));
		}

		// =======+++=======
		
		public static string DecompressToString(this string base64ZippedString)
		{
			return __DecompressToStringHub(base64ZippedString, null, Encoding.UTF8, CompressionType.DeflateStream);
		}

		public static string DecompressToString(this byte[] zippedBytes)
		{
			return __DecompressToStringHub(null, zippedBytes, Encoding.UTF8, CompressionType.DeflateStream);
		}

		public static string DecompressToString(this byte[] zippedBytes, CompressionType compressionType)
		{
			return __DecompressToStringHub(null, zippedBytes, Encoding.UTF8, compressionType);
		}

		public static byte[] Decompress(this string base64ZippedString)
		{
			return __DecompressHub(base64ZippedString, null, CompressionType.DeflateStream);
		}

		#endregion

		#region STREAM

		public static void Compress(this Stream inputStream, Stream outputZippedStream)
		{
			Compress(inputStream, outputZippedStream, CompressionType.DeflateStream);
		}

		public static bool CompressFile(string fileToZip, string zippedFile)
		{
			//Note that, unlike everywhere else, for FILES our default CompressionType is GZip
			return CompressFile(fileToZip, zippedFile, CompressionType.GZip);
		}

		public static bool CompressFile(string fileToZip, string zippedFile, CompressionType compressionType)
		{
			if (fileToZip == null) throw new ArgumentNullException("fileToZip");
			if (zippedFile == null) throw new ArgumentNullException("zippedFile");

			if (!File.Exists(fileToZip))
				return false;

			using (Stream srcS = File.Open(fileToZip, FileMode.Open, FileAccess.Read))
			using (Stream zipS = File.Open(zippedFile, FileMode.Create, FileAccess.Write))
			{
				Compress(srcS, zipS, compressionType);
			}
			return true;
		}


		/// <summary>
		/// Decompresses and writes the input stream to the output stream using DeflateStream.
		/// The input stream is only read from and is not altered as such, but rather, it is the 
		/// output stream that is written to with a decompressed version of the input
		/// stream.  The input stream will be read from its current position to its end.
		/// <example><code>
		/// <![CDATA[
		/// // see entry under Zip(...)
		/// ]]>
		/// </code></example>
		/// </summary>
		/// <param name="inputStream">The input stream which is expected to contain 
		/// data that was compressed with DeflateStream or with a compatible compression type.
		/// This stream must be readable.</param>
		/// <param name="outputUnzippedStream">The output stream that will contain the 
		/// decompressed data of <i>inputStream</i>.  This stream must be writable.</param>
		public static void Decompress(this Stream inputStream, Stream outputUnzippedStream)
		{
			Decompress(inputStream, outputUnzippedStream, CompressionType.DeflateStream);
		}

		public static bool DecompressFile(string fileToUnzip, string unzippedFile)
		{
			return DecompressFile(fileToUnzip, unzippedFile, CompressionType.GZip);
		}

		/// <summary>
		/// Decompresses the data from <i>fileToUnzip</i> with the specified compression type 
		/// (GZip or DeflateStream) and
		/// writes this to <i>unzippedFile</i>.  If <i>unzippedFile</i> already exists *it will be
		/// overwritten*.  If <i>fileToUnzip</i> does not exist, the method will return
		/// false.
		/// </summary>
		/// <param name="fileToUnzip">The path of the file that contains the data to decompress.</param>
		/// <param name="unzippedFile">The path of the file that will have the decompressed data 
		/// written to it.</param>
		/// <param name="compressionType">The compression type: DeflateStream or GZip.</param>
		/// <returns>False if <i>fileToUnzip</i> does not exist, otherwise true.</returns>
		public static bool DecompressFile(string fileToUnzip, string unzippedFile, CompressionType compressionType)
		{
			if (fileToUnzip == null) throw new ArgumentNullException("fileToUnzip");
			if (unzippedFile == null) throw new ArgumentNullException("unzippedFile");

			if (!File.Exists(fileToUnzip))
				return false;

			using (Stream srcS = File.Open(fileToUnzip, FileMode.Open, FileAccess.Read))
			using (Stream zipS = File.Open(unzippedFile, FileMode.Create, FileAccess.Write))
			{
				srcS.Decompress(zipS, compressionType);
			}
			return true;
		}

		#endregion

		#region Auxiliary

		/// <summary>
		/// Makes for a more intelligent buffer size.
		/// </summary>
		/// <param name="streamLen">Stream length.</param>
		/// <returns>Buffer size.</returns>
		private static int SetBufferSize(long streamLen)
		{
			if (streamLen < 60000)  // 60 kb
				return (int)streamLen;

			if (streamLen < 1000000) // apprx 1 meg
				return (((int)streamLen) / 2);

			return 500000; // == if (streamLen > 1000000) // greater than 1 meg
		}

		#endregion

	}

	#region CompressionType

	/// <summary>
	/// An enumeration of data compression types.
	/// </summary>
	public enum CompressionType
	{
		/// <summary>
		/// Compresses with the Deflate compression algorithm.
		/// </summary>
		DeflateStream,

		/// <summary>
		/// Compresses with the Deflate compression algorithm, but includes 
		/// metadata at the start and end of the compression.  Among other 
		/// things, this allows tools such as 7-Zip or WinRaR to independently 
		/// decompress GZip compressed data when saved to file.
		/// </summary>
		GZip
	}

	#endregion

}