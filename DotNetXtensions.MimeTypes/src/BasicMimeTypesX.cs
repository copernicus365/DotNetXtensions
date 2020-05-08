using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;
using MTI = DotNetXtensions.MimeTypeInfo;

namespace DotNetXtensions
{
	/// <summary>
	/// A type for working with basic mime types.
	/// </summary>
	public static partial class BasicMimeTypesX
	{
		/// <summary>
		/// Returns the official mimetype string value for this enum value, e.g. "text/html" for text_html.
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		public static string MimeTypeString(this BasicMimeType mimeType)
			=> MTIDict_ByEnums.V(mimeType)?.MimeTypeString;

		///// <summary>
		///// Returns the official mimetype string value for this enum value, e.g. "text/html" for text_html.
		///// </summary>
		///// <param name="mimeType"></param>
		///// <returns></returns>
		//[Obsolete("Use GetMimeTypeString()")]
		//public static string ToMimeTypeString(this BasicMimeType mimeType)
		//	=> MTIDict_ByEnums.V(mimeType)?.MimeTypeString;


		/// <summary>
		/// Gets the extension of the input url or file path by using <see cref="UriPathInfo"/>, 
		/// and then gets the corresponding mimetype, if any.
		/// </summary>
		public static BasicMimeType GetMimeTypeFromPathOrUrl(string url)
		{
			if(url.NotNulle()) {
				UriPathInfo info = new UriPathInfo(url);
				string ext = info.Extension;
				// old: GetExtFromUrl(url);
				if(ext.NotNulle())
					return GetMimeTypeFromFileExtension(ext);
			}
			return BasicMimeType.none;
		}

		/// <summary>
		/// Gets the extension of the input url or file path by using <see cref="UriPathInfo"/>, 
		/// and then gets the corresponding mimetype, if any.
		/// Direct indirection call to <see cref="GetMimeTypeFromPathOrUrl(string)"/>,
		/// this is for simple discoverability sake.
		/// </summary>
		/// <param name="path">Path or Url, absolute or relative or simply a single file name.</param>
		/// <param name="none">Ignore; simply allows easy findability to this call</param>
		public static BasicMimeType GetMimeTypeFromPathOrUrl(this BasicMimeType none, string path)
			=> GetMimeTypeFromPathOrUrl(path);



		/// <summary>
		/// Determines the mimetype for this extension (e.g. "jpg" or "pdf"). Is case insensitive.
		/// Indirection method for discoverability sake.
		/// </summary>
		/// <param name="ext">Extension, such as "jpg". Must NOT have the period.</param>
		public static BasicMimeType GetMimeTypeFromFileExtension(string ext)
		{
			return ext == null || ext.Length > 50
				? BasicMimeType.none
				: MTIDict_ByExtensions.V(ext)?.MimeType ?? BasicMimeType.none; // FileExtensionToMimeTypeDictionary.V(ext, BasicMimeType.none);
		}

		/// <summary>
		/// Determines the mimetype for this extension (e.g. "jpg" or "pdf"). Is case insensitive.
		/// Indirection method for discoverability sake.
		/// </summary>
		/// <param name="ext">Extension, such as "jpg". Must NOT have the period.</param>
		/// <param name="none">Ignore; simply allows easy findability to this call</param>
		public static BasicMimeType GetMimeTypeFromFileExtension(this BasicMimeType none, string ext)
			=> GetMimeTypeFromFileExtension(ext);

		/// <summary>
		/// Gets the most qualified mime type, with the first input (<paramref name="primary"/>) overriding the
		/// secondary input.
		/// </summary>
		/// <param name="primary">The primary value.</param>
		/// <param name="secondary">A secondary mime type, which will only override the primary if it is more qualified.</param>
		public static BasicMimeType GetMostQualifiedMimeType(this BasicMimeType primary, BasicMimeType secondary)
		{
			if(primary == BasicMimeType.none)
				return secondary;

			if(secondary == BasicMimeType.none)
				return primary;

			if(primary.IsGenericType())
				return secondary.IsGenericType() ? primary : secondary;

			return primary; // primary at this point HAS a subtype, so no matter if secondary is fully set, primary wins at this point
		}

		/// <summary>
		/// Attempts to get a <see cref="BasicMimeType"/> match for the input mimetype string. Note that this dictionary contains
		/// generic versions of the major mimetypes (minus subtype), where only the first part of the mimetype 
		/// is specified. Thus a search on any of the following values: "image", "image/", or "image/null",
		/// will return the generic image type <see cref="BasicMimeType.image"/>. 
		/// If the specific value is not found and <paramref name="allowGenericMatchOnNotFound"/> is true 
		/// (FALSE by default), then we will attempt to get only the generic part of the mimetype as just discussed
		/// (e.g. if input `"image/someunsualimagetype"` fails, we will lookup `"image/"`).
		/// </summary>
		/// <param name="mimeTypeStr">String mime type, see notes above.</param>
		/// <param name="allowGenericMatchOnNotFound">See notes above.</param>
		public static BasicMimeType ParseMimeType(string mimeTypeStr, bool allowGenericMatchOnNotFound = false)
		{
			if(mimeTypeStr != null && mimeTypeStr.Length < 50) {

				if(MTIDict_ByMimeNames.TryGetValue(mimeTypeStr, out MTI mti))
					return mti.MimeType;

				if(allowGenericMatchOnNotFound) {
					int maxCount = Math.Min(13, mimeTypeStr.Length - 2);
					for(int i = 0; i < maxCount; i++) {
						if(mimeTypeStr[i] == '/') {
							string subType = mimeTypeStr.Substring(0, i + 1);

							if(MTIDict_ByMimeNames.TryGetValue(subType, out mti))
								return mti.MimeType;
						}
					}
				}
			}
			return BasicMimeType.none;
		}


		///// <summary>
		///// Attempts to get a <see cref="BasicMimeType"/> match for the input mimetype string.
		///// Indirection call to <see cref="ParseMimeType(string, bool)"/>, see there
		///// for further documentation.
		///// </summary>
		///// <param name="none">Ignore; simply allows easy findability to <see cref="ParseMimeType(string, bool)"/></param>
		///// <param name="mimeTypeStr">String mime type, see notes above.</param>
		///// <param name="allowGenericMatchOnNotFound">See notes above.</param>
		//public static BasicMimeType GetMimeTypeFromString(this BasicMimeType none, string mimeTypeStr, bool allowGenericMatchOnNotFound = false)
		//	=> ParseMimeType(mimeTypeStr, allowGenericMatchOnNotFound);


		public static BasicMimeType GetGenericMimeType(this BasicMimeType m)
		{
			int val = (int)m;

			if(val < 400) { // if so, is text or nothing
				if(val >= 100)
					return BasicMimeType.text;
				else
					return BasicMimeType.none;
			}
			else if(val < 500)
				return BasicMimeType.image;
			else if(val < 600)
				return BasicMimeType.audio;
			else if(val < 700)
				return BasicMimeType.video;
			else //if (val < 800)
				return BasicMimeType.application;
		}


		public static string GetExtension(this BasicMimeType mimeType)
			=> MTIDict_ByEnums.V(mimeType)?.Extension;

		public static MTI GetMimeInfo(this BasicMimeType mimeType)
			=> MTIDict_ByEnums.V(mimeType);

		// --- +++ ---

		public static MTI AddExtraExtension(this MTI m, string ext)
		{
			string x = ext.NullIfEmptyTrimmed();
			if(x == null)
				throw new ArgumentNullException();

			if(m.ExtraExtensions == null)
				m.ExtraExtensions = new List<string>();

			m.ExtraExtensions.Add(x);
			return m;
		}

		public static MTI AddExtraExtensions(this MTI m, params string[] exts)
		{
			if(m.ExtraExtensions == null)
				m.ExtraExtensions = new List<string>();

			foreach(string ext in exts) {
				string x = ext.NullIfEmptyTrimmed();
				if(x != null)
					m.ExtraExtensions.Add(x);
			}
			return m;
		}

		public static MTI ExtraMimeStrings(this MTI m, params string[] mimeStrs)
		{
			if(m.ExtraMimeStrings == null)
				m.ExtraMimeStrings = new List<string>();

			foreach(string mim in mimeStrs) {
				string _mim = mim.NullIfEmptyTrimmed();
				if(_mim != null)
					m.ExtraMimeStrings.Add(_mim);
			}
			return m;
		}

	}
}
