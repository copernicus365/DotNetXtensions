using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DotNetXtensions.Compression
{
	/// <summary>
	/// Zip compression archive functions.
	/// </summary>
	public static class XZip
	{


		// --- WRITE ZipArchvie ---

		public static void ZipToArchive(this Stream readStream, Stream writeStream, string fileName)
		{
			if (fileName.IsNulle()) throw new ArgumentNullException();

			if (readStream == null || writeStream == null)
				return;

			using (var archive = new ZipArchive(writeStream, ZipArchiveMode.Create, true)) {

				var demoFile = archive.CreateEntry(fileName);

				using (var entryStream = demoFile.Open())
					entryStream.Write(readStream);
			}
		}

		public static byte[] ZipToArchive(this Stream readStream, string fileName)
		{
			if (fileName.IsNulle()) throw new ArgumentNullException();

			if (readStream == null)
				return new byte[] { };

			byte[] bdata = null;
			using (var ms = new MemoryStream()) {
				readStream.ZipToArchive(ms, fileName);
				ms.Position = 0;
				bdata = ms.ReadBytes();
			}
			return bdata;
		}

		public static byte[] ZipToArchive(this byte[] data, string fileName)
		{
			return ZipToArchive(new MemoryStream(data ?? new byte[] { }), fileName);
		}

		public static byte[] ZipToArchive(this string data, string fileName)
		{
			return ZipToArchive(new MemoryStream(Encoding.UTF8.GetBytes(data ?? "")), fileName);
		}



		// --- OPEN ZipArchive ---

		public static ZipArchive OpenZipArchive(this Stream stream, bool allowUpdates = true)
		{
			if (stream == null)
				return null;
			return new ZipArchive(stream, allowUpdates ? ZipArchiveMode.Update : ZipArchiveMode.Read, true);
		}

		public static ZipArchive OpenZipArchive(this byte[] data, bool allowUpdates = true)
		{
			return OpenZipArchive(new MemoryStream(data), allowUpdates);
		}



		// --- UnzipEntryFromArchive ---

		public static ZipArchiveEntry UnzipEntryFromArchive(this Stream stream, string entryName = null, bool? getFirstEntry = null)
		{
			if (entryName.IsNulle() && getFirstEntry == false)
				throw new ArgumentException("Either the entryName must be non-null, or else getFirstEntry (bool?) must not be set to false.");

			bool _getFirstEntry = entryName.IsNulle();

			// NOT using using statement, closes the stream, and seems to disregard our 'true' command to leave open! grrrr
			var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);
			if (archive == null || archive.Entries.IsNulle())
				return null;

			ZipArchiveEntry entry = _getFirstEntry
				? archive.Entries[0]
				: archive.GetEntry(entryName);
			return entry;
		}

		public static ZipArchiveEntry UnzipEntryFromArchive(this byte[] data, string entryName = null, bool? getFirstEntry = null)
		{
			return UnzipEntryFromArchive(new MemoryStream(data), entryName, getFirstEntry);
		}


		public static byte[] UnzipDataFromArchive(this Stream stream, string entryName = null, bool? getFirstEntry = null)
		{
			ZipArchiveEntry entry = UnzipEntryFromArchive(stream, entryName, getFirstEntry);
			byte[] data = entry == null ? null : entry.Open().ReadBytes();
			if (entry != null)
				entry.Archive?.Dispose();
			return data;
		}

		public static byte[] UnzipDataFromArchive(this byte[] data, string entryName = null, bool? getFirstEntry = null)
		{
			return UnzipDataFromArchive(new MemoryStream(data), entryName, getFirstEntry);
		}

		/// <summary>
		/// Unzips a single file/entry from the input archive.
		/// If both <paramref name="getFirstEntry"/> and <paramref name="entryName"/> 
		/// are not specified (null), the first entry will be gotten if available 
		/// (treating <paramref name="getFirstEntry"/> as true).
		/// </summary>
		/// <param name="data">The zipped archive bytes.</param>
		/// <param name="entryName">The name of the entry / file to find in the archive. 
		/// If this is null, it will treat <paramref name="getFirstEntry"/> as true 
		/// if that value is still null.</param>
		/// <param name="getFirstEntry">True to get the first entry. If
		/// <paramref name="entryName"/> is specified it has precedence over this.</param>
		public static string UnzipTextFromArchive(this byte[] data, string entryName = null, bool? getFirstEntry = null)
		{
			byte[] bdata = UnzipDataFromArchive(new MemoryStream(data), entryName, getFirstEntry);
			return bdata == null ? null : UTF8Encoding.UTF8.GetString(bdata);
		}

		public static string UnzipTextFromArchive(this Stream stream, string entryName = null, bool? getFirstEntry = null)
		{
			byte[] bdata = UnzipDataFromArchive(stream, entryName, getFirstEntry);
			return bdata == null ? null : Encoding.UTF8.GetString(bdata);
		}

	}
}