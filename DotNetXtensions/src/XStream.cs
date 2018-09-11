using System;
using System.IO;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Extension methods for Streams.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XStream
	{
		#region Buffer Size

		public static int m_DefaultBufferSize = 8192; // 8kb

		public static int DefaultBufferSize
		{
			get { return m_DefaultBufferSize; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (value < 256)
					m_DefaultBufferSize = 256;
				else if (value > 1048576)
					m_DefaultBufferSize = 1048576;
				else
					m_DefaultBufferSize = value;
			}
		}

		public static int GetBufferSize(Stream readStream, int? readLength)
		{
			// readLength: when we know will not be asking to read to stream end, but 
			// read length num of bytes, send that in.

			int bufferSize;

			// for this first batch, bufferSize sets are NOT completed, will be altered if too big or small next
			if (readLength != null)
				bufferSize = (int)readLength;
			else if (readStream.CanSeek)
				bufferSize = checked((int)readStream.Length) - checked((int)readStream.Position);
			else
				return m_DefaultBufferSize;

			if (bufferSize > m_DefaultBufferSize)
				return m_DefaultBufferSize;

			if (bufferSize < 256)
			{
				if (bufferSize < 0)
					throw new ArgumentOutOfRangeException();
				else
					return 256;
			}
			return bufferSize;
		}

		#endregion

		#region CopyTo

		public static int CopyTo(this Stream stream, byte[] destination)
		{
			if (stream == null) throw new ArgumentNullException("stream");
			if (destination == null) throw new ArgumentNullException("destination");
			//if (!stream.CanRead) throw new ArgumentException("Stream must be readable.");

			int destLen = destination.Length;

			int numRead = stream.Read(destination, 0, destLen);

			if (numRead == destLen)
				return destLen;

			else if (numRead < 1)
				return 0;

			else
			{
				int totalRead = numRead;
				while ((numRead = stream.Read(destination, totalRead, destLen - totalRead)) > 0)
					totalRead += numRead;

				return totalRead;
			}
		}

		#endregion

		#region ReadBytes

		public static byte[] ReadBytes(this Stream stream)
		{
			return __ReadBytes(stream, null);
		}

		/// <summary>
		/// Reads the contents of this stream into a byte array reading from the stream's 
		/// present position by count number of bytes, or less if stream end is reached first.
		/// This stream does not have to be seekable.
		/// </summary>
		/// <param name="stream">This stream.</param>
		/// <param name="count">The number of bytes to read from stream
		/// (the number of read bytes will be less if the stream end is reached first).</param>
		public static byte[] ReadBytes(this Stream stream, int count)
		{
			return __ReadBytes(stream, count);
		}

		/// <summary>
		/// Reads the contents of this stream into a byte array reading from the stream's 
		/// present position by count number of bytes if count is non null (or less if stream 
		/// end is reached first), or to the stream's end if count is null.
		/// This stream does not have to be seekable.
		/// </summary>
		/// <param name="stream">This stream.</param>
		/// <param name="count">The number of bytes to read from stream
		/// (the number of read bytes will be less if the stream end is reached first).
		/// Enter NULL to specify to read to stream end.</param>
		private static byte[] __ReadBytes(this Stream stream, int? count)
		{
			#region Validate Args

			if (stream == null) throw new ArgumentNullException("stream");
			if (count != null && (int)count < 0) throw new ArgumentOutOfRangeException("maxCount");
			//if (!stream.CanRead) throw new ArgumentException("Stream must be readable.");

			#endregion

			#region IF CANSEEK || COUNT != NULL

			if (stream.CanSeek || count != null)
			{
				int len = count != null
					? (int)count
					: checked((int)stream.Length) - checked((int)stream.Position);

				if (len < 1)
					return new byte[0];

				byte[] _data = new byte[len];

				int numRd = stream.CopyTo(_data);

				if (numRd == len)
					return _data;
				else
					return _data.Copy(0, numRd);
			}

			#endregion

			#region ELSE

			else
			{
				int numRead = 0;
				int buffLen = m_DefaultBufferSize;
				byte[] buff = new byte[buffLen];
				MemoryStream ms = new MemoryStream(buffLen);

				while ((numRead = stream.Read(buff, 0, buffLen)) > 0)
					ms.Write(buff, 0, numRead);

				return ms.ToArray();
			}

			#endregion
		}

		#endregion

		#region Read | Write (Stream to Stream)

		/// <summary>
		/// Writes the specified number of bytes from readStream into writeStream, 
		/// reading and writing (respectively) from each stream's present Position.
		/// </summary>
		/// <param name="writeStream">The stream to write to.</param>
		/// <param name="readStream">The stream to read from.</param>
		/// <param name="length">The count of bytes to read from readStream
		/// into writeStream (or less if readStream ends first),
		/// or NULL to read the entirety of bytes from read stream.</param>
		private static void __WRITE_STREAM_TO_STREAM(this Stream writeStream, Stream readStream, int? length)
		{
			#region Param checks
			if (readStream == null) throw new ArgumentNullException("readStream");
			if (writeStream == null) throw new ArgumentNullException("writeStream");
			if (!readStream.CanRead) throw new IOException("readStream could not be read from (CanRead = false).");
			if (!writeStream.CanWrite) throw new IOException("writeStream could not be written to (CanWrite = false).");
			#endregion

			// === Establish Buffer + buffer size ===

			int buffLen = GetBufferSize(readStream, length);
			byte[] buff = new byte[buffLen];
			int numRead = 0;

			// === Handle length != null === (meaning length is max read from stream, else read to end)
			// Critically important, when it is not null, to never read further than length... of course

			if (length == null) // means read to readStream end
			{
				while ((numRead = readStream.Read(buff, 0, buffLen)) > 0)
					writeStream.Write(buff, 0, numRead);
			}
			else // means stop reading *exactly* at length, or under if stream ended
			{
				int lenLeft = (int)length;
				while ((numRead = readStream.Read(buff, 0, ((lenLeft -= numRead) < buffLen ? lenLeft : buffLen))) > 0)
					writeStream.Write(buff, 0, numRead);
			}
			writeStream.Flush();
		}

		// Following are all complete indirections to __WRITE_STREAM_TO_STREAM

		/// <summary>
		/// Reads the entirety of readStream bytes into writeStream, reading and
		/// writing (respectively) from each stream's present Position.
		/// </summary>
		/// <param name="readStream">The stream to read from.</param>
		/// <param name="writeStream">The stream to write to.</param>
		public static void Read(this Stream readStream, Stream writeStream)
		{
			__WRITE_STREAM_TO_STREAM(writeStream, readStream, null);//Read(readStream, writeStream, null);
		}

		/// <summary>
		/// Reads the readStream by <paramref name="length"/> number of bytes 
		/// into writeStream, reading and writing (respectively) from each stream's 
		/// present Position.
		/// </summary>
		/// <param name="readStream">The stream to read from.</param>
		/// <param name="writeStream">The stream to write to.</param>
		/// <param name="length">The count of bytes to read from readStream
		/// into writeStream (or less if readStream ends first),
		/// or NULL to read the entirety of bytes from read stream.</param>
		public static void Read(this Stream readStream, Stream writeStream, int? length)
		{
			__WRITE_STREAM_TO_STREAM(writeStream, readStream, length);
		}

		/// <summary>
		/// Writes the entirety of readStream into writeStream, reading and
		/// writing (respectively) from each stream's present Position.
		/// </summary>
		/// <param name="writeStream">The stream to write to.</param>
		/// <param name="readStream">The stream to read from.</param>
		public static void Write(this Stream writeStream, Stream readStream)
		{
			__WRITE_STREAM_TO_STREAM(writeStream, readStream, null);
		}

		/// <summary>
		/// Writes the specified number of bytes from readStream into writeStream, 
		/// reading and writing (respectively) from each stream's present Position.
		/// </summary>
		/// <param name="writeStream">The stream to write to.</param>
		/// <param name="readStream">The stream to read from.</param>
		/// <param name="length">The count of bytes to read from readStream
		/// into writeStream (or less if readStream ends first),
		/// or NULL to read the entirety of bytes from read stream.</param>
		public static void Write(this Stream writeStream, Stream readStream, int length)
		{
			__WRITE_STREAM_TO_STREAM(writeStream, readStream, length);
		}

		#endregion
	}
}
