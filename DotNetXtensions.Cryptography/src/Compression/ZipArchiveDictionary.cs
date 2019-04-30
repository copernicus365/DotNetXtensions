using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetXtensions;
using System.IO;
using System.IO.Compression;
using System.Collections;

namespace DotNetXtensions.Compression
{
	/// <summary>
	/// [NOT implemented yet, work in progress]
	/// A class that inherits from ZipArchive but that additionally 
	/// lightly enhances its functionality by having it implement 
	/// IDictionary[string,T]. This also means, of necessity, that this now
	/// implements IEnumerable and ICollection.
	/// <para/>
	/// For comparison of ZipArchive see its source here: 
	/// https://github.com/dotnet/corefx/blob/39471b56bd0241e60a33b4179a04187b96481547/src/System.IO.Compression/src/System/IO/Compression/ZipArchive.cs
	/// </summary>
	public class ZipArchiveDictionary<T> : ZipArchive, IDictionary<string, T>
	{

		//Func<byte[], T> converter = null;


		/// <summary>
		/// Initializes a new instance of the ZipArchiveDictionary class
		/// from the specified stream. Note that the default parameters
		/// here have default values according to what the ZipArchive source code was doing,
		/// so this way we have a single constructor.
		/// </summary>
		/// <param name="stream">The input or output stream (whether it is readable, writable, 
		/// or both, depends on the ZipArchiveMode ).</param>		
		/// <param name="mode">See the description of the ZipArchiveMode enum. Read requires the stream to support reading, Create requires the stream to support writing, and Update requires the stream to support reading, writing, and seeking.</param>
		/// <param name="leaveOpen">true to leave the stream open upon disposing the ZipArchive, otherwise false.</param>
		/// <param name="tConverter"></param>
		/// <param name="entryNameEncoding">The encoding to use when reading or writing entry names in this ZipArchive.
		/// NOTE: Specifying this parameter to values other than <c>null</c> is discouraged... (see 
		/// ZipArchive for further, very lengthy notes on why).</param>
		public ZipArchiveDictionary(
			Stream stream, 
			ZipArchiveMode mode = ZipArchiveMode.Read, 
			bool leaveOpen = false,
			Func<byte[], T> tConverter = null,
			System.Text.Encoding entryNameEncoding = null) :
			base(stream, mode, leaveOpen, entryNameEncoding)
		{
			throw new NotImplementedException();
		}



		public T this[string key] {
			get {
				throw new NotImplementedException();
			}

			set {
				throw new NotImplementedException();
			}
		}

		public int Count {
			get {
				throw new NotImplementedException();
			}
		}

		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}

		public ICollection<string> Keys {
			get {
				throw new NotImplementedException();
			}
		}

		public ICollection<T> Values {
			get {
				throw new NotImplementedException();
			}
		}

		public void Add(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		public void Add(string key, T value)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		public bool ContainsKey(string key)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<string, T> item)
		{
			throw new NotImplementedException();
		}

		public bool Remove(string key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(string key, out T value)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
