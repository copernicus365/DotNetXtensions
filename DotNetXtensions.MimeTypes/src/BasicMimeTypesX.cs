using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// A type for working with basic mime types.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif

	static class BasicMimeTypesX
	{

		/// <summary>
		/// Returns true if linkType is other than none or html 
		/// (the reverse of linkType.IsWebPageOrNone()).
		/// </summary>
		public static bool IsEnclosureType(this BasicMimeType mimeType)
		{
			return mimeType.IsWebPageOrNone() == false;
		}

		/// <summary>
		/// Indicates if the given type is only one of the generic types
		/// without a subtype (doesn't include none). E.g. BasicMimeType.audio.
		/// Internally, the numbering system allows us to know this, in that any enum with a 100 value (v % 100 == 0)
		/// is set as a generic mime type.
		/// </summary>
		public static bool HasNoSubtype(this BasicMimeType mimeType)
		{
			return (int)mimeType % 100 == 0;
		}

		public static bool HasNoSubtypeOrNone(this BasicMimeType mimeType)
		{
			return mimeType == BasicMimeType.none || (int)mimeType % 100 == 0;
		}

		/// <summary>
		/// Is none or is any of the text types (lt 200);
		/// </summary>
		public static bool IsTextOrNone(this BasicMimeType mimeType)
		{
			return (int)mimeType < 200;
		}

		public static bool IsText(this BasicMimeType mimeType)
		{
			return (int)mimeType < 200 && (int)mimeType >= 100;
		}

		public static bool IsWebPageOrNone(this BasicMimeType mimeType)
		{
			return mimeType == BasicMimeType.none || mimeType == BasicMimeType.text_html;
		}

		public static bool IsImage(this BasicMimeType mimeType)
		{
			return (int)mimeType >= 400 && (int)mimeType < 500;
		}

		public static bool IsAudio(this BasicMimeType mimeType)
		{
			return (int)mimeType >= 500 && (int)mimeType < 600;
		}

		public static bool IsVideo(this BasicMimeType mimeType)
		{
			return (int)mimeType >= 600 && (int)mimeType < 700;
		}

		public static bool IsAudioOrVideo(this BasicMimeType mimeType)
		{
			return (int)mimeType >= 500 && (int)mimeType < 700;
		}

		public static bool IsApplication(this BasicMimeType mimeType)
		{
			return (int)mimeType >= 700 && (int)mimeType < 800;
		}



		/// <summary>
		/// Returns the official mimetype string value for this enum value, e.g. "text/html" for text_html.
		/// </summary>
		/// <param name="mimeType"></param>
		/// <returns></returns>
		public static string ToMimeTypeString(this BasicMimeType mimeType)
		{
			string val = MTIDict_ByEnums.V(mimeType)?.MimeTypeString; // MimeTypeEnumToStringDictionary.V(mimeType);
			return val;
		}


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
		/// and then gets the corresponding mimetype, if any. Indirection method for discoverability sake.
		/// </summary>
		/// <param name="path">Path or Url, absolute or relative or simply a single file name.</param>
		/// <param name="none">Ignore.</param>
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
		/// <param name="none">Ignore</param>
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

			if(primary.HasNoSubtype())
				return secondary.HasNoSubtype() ? primary : secondary;

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
		public static BasicMimeType GetMimeTypeFromString(string mimeTypeStr, bool allowGenericMatchOnNotFound = false)
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
		/// <param name="none">Ignore</param>
		public static BasicMimeType GetMimeTypeFromString(this BasicMimeType none, string mimeTypeStr, bool allowGenericMatchOnNotFound = false)
			=> GetMimeTypeFromString(mimeTypeStr, allowGenericMatchOnNotFound);


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

		public static Dictionary<BasicMimeType, MTI> MTIDict_ByEnums;
		public static Dictionary<string, MTI> MTIDict_ByMimeNames;
		public static Dictionary<string, MTI> MTIDict_ByExtensions;

		static BasicMimeTypesX()
		{
			MTIDict_ByEnums = AllTypes.ToDictionary(v => v.MimeType, v => v);
			MTIDict_ByMimeNames = AllTypes.ToDictionary(v => v.MimeTypeString, v => v);
			MTIDict_ByExtensions = AllTypes.Where(v => v.Ext.NotNulle()).ToDictionary(v => v.Ext, v => v);

			for(int i = 0; i < AllTypes.Length; i++) {
				MTI mti = AllTypes[i];

				if(mti.ExtraMimeStrings.NotNulle()) {
					foreach(string extMimeStr in mti.ExtraMimeStrings) {
						if(!MTIDict_ByMimeNames.ContainsKey(extMimeStr))
							MTIDict_ByMimeNames.Add(extMimeStr, mti);
					}
				}

				if(mti.ExtraExts.NotNulle()) {
					foreach(string ext in mti.ExtraExts) {
						if(!MTIDict_ByExtensions.ContainsKey(ext))
							MTIDict_ByExtensions.Add(ext, mti);
					}
				}
			}
		}


		public static MTI[] AllTypes = new List<MTI>() {

			//new MTI(BasicMimeType.none, null),

			// https://tools.ietf.org/html/rfc4287#section-3.1.1
			// ATOM allows this in some cases, maybe never supposed to be for a full url, but hurts nothing to have here

			// ------- APPLICATION -------

			new MTI(BasicMimeType.application, "application/null")
				.ExtraMimeStrings("application", "application/"),

			new MTI(BasicMimeType.application_javascript, "application/javascript", "js"),
			new MTI(BasicMimeType.application_typescript, "application/typescript", "ts"),

			new MTI(BasicMimeType.application_atom_xml, "application/atom+xml", "atom")
				.ExtraExtensions("xml"),

			new MTI(BasicMimeType.application_json, "application/json", "json"),
			new MTI(BasicMimeType.application_octetstream, "application/octet-stream"),
			new MTI(BasicMimeType.application_pdf, "application/pdf", "pdf"),
			new MTI(BasicMimeType.application_rss_xml, "application/rss+xml", "rss")
				.ExtraExtensions("xml"),

			new MTI(BasicMimeType.application_word, "application/msword", "docx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
				.ExtraExtensions("doc"),

			new MTI(BasicMimeType.application_excel, "application/vnd.ms-excel", "xlsx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
				.ExtraExtensions("xls"),

			new MTI(BasicMimeType.application_powerpoint, "application/vnd.ms-powerpoint", "pptx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.presentationml.presentation")
				.ExtraExtensions("ppt"),

			new MTI(BasicMimeType.application_xml, "application/xml")
				.ExtraExtensions("xml"), // making secondary for extension



			// ------- AUDIO -------

			new MTI(BasicMimeType.audio, "audio/null")
				.ExtraMimeStrings("audio", "audio/"),
			new MTI(BasicMimeType.audio_aac, "audio/aac", "aac")
				.ExtraExtensions("m4a"),

			new MTI(BasicMimeType.audio_aif, "audio/aif", "aiff")
				.ExtraMimeStrings("audio/x-aiff")
				.ExtraExtensions("aif"),

			new MTI(BasicMimeType.audio_mp3, "audio/mpeg", "mp3")
				.ExtraMimeStrings("audio/mp3", "audio/mpeg3", "audio/x-mpeg3")
				.ExtraExtensions("mpga", "mpeg"),

			new MTI(BasicMimeType.audio_mp4, "audio/mp4", "m4a")
				.ExtraExtensions("mp4a", "mp4"),

			new MTI(BasicMimeType.audio_ogg, "audio/ogg", "oga")
				.ExtraExtensions("ogg"), // .oga = audio only, .ogv for video, .ogg was old format unspecified. we will add ".ogg" as the audio one

			new MTI(BasicMimeType.audio_wav, "audio/wav", "wav"),
			new MTI(BasicMimeType.audio_webm, "audio/webm", null)
				.ExtraExtensions("webm"), // wiki: "WebM is a video file format." So that is it's main deal, leave .webm for video mime below

			new MTI(BasicMimeType.audio_wma, "audio/x-ms-wma", "wma"),



			// ------- IMAGE -------

			new MTI(BasicMimeType.image, "image/null") //, mtStrMain: BasicMimeType.image_jpeg)
				.ExtraMimeStrings("image", "image/"),

			new MTI(BasicMimeType.image_gif, "image/gif"),
			new MTI(BasicMimeType.image_ico, "image/x-icon"),

			new MTI(BasicMimeType.image_jpeg, "image/jpeg", "jpg")
				.ExtraExtensions("jpeg"),

			new MTI(BasicMimeType.image_png, "image/png", "png"),

			new MTI(BasicMimeType.image_svg, "image/svg+xml", "svg"),


			// ------- TEXT -------

			new MTI(BasicMimeType.text, "text/null")
				.ExtraMimeStrings("text", "text/"),
			//
			new MTI(BasicMimeType.text_css, "text/css", "css"),
			new MTI(BasicMimeType.text_scss, "text/x-scss", "scss"),
			new MTI(BasicMimeType.text_sass, "text/x-sass", "sass"),
			new MTI(BasicMimeType.text_less, "text/less", "less"),
			//
			new MTI(BasicMimeType.text_html, "text/html", "html")
				.ExtraMimeStrings("application/xhtml+xml", "html", "xhtml")
				.ExtraExtensions("xhtml", "htm"),
			new MTI(BasicMimeType.text_plain, "text/plain", "txt"),
			new MTI(BasicMimeType.text_markdown, "text/markdown", "md")
				.ExtraExtensions("markdown", "mdown"),
			new MTI(BasicMimeType.text_xml, "text/xml", "xml"), // note: we're making this one the main for xml extension



			// ------- VIDEO -------

			new MTI(BasicMimeType.video, "video/null")
				.ExtraMimeStrings("video", "video/")
				.ExtraExtensions(""),
			new MTI(BasicMimeType.video_3gp, "video/3gpp", "3gp"),
			new MTI(BasicMimeType.video_avi, "video/avi", "avi"),
			new MTI(BasicMimeType.video_flv, "video/x-flv", "flv"),
			new MTI(BasicMimeType.video_m3u8, "video/x-mpegURL" /*ext?*/),
			new MTI(BasicMimeType.video_mov, "video/quicktime", "mov"),
			new MTI(BasicMimeType.video_mp4, "video/mp4", "mp4"),
			new MTI(BasicMimeType.video_ogg, "video/ogg", "ogv"),
			new MTI(BasicMimeType.video_ts, "video/MP2T"),
			new MTI(BasicMimeType.video_vimeo, "video/vimeo"),
			new MTI(BasicMimeType.video_webm, "video/webm", "webm"),
			new MTI(BasicMimeType.video_wmv, "video/x-ms-wmv", "wmv"),
			new MTI(BasicMimeType.video_youtube, "video/youtube"),

		}.ToArray();

		public class MTI
		{
			public BasicMimeType MimeType;
			public string MimeTypeString;
			public string Ext;
			public bool IsMain;
			public BasicMimeType MainMimeStringType;
			public BasicMimeType GenericMimeType;

			public List<string> ExtraMimeStrings;
			public List<string> ExtraExts;

			public MTI(BasicMimeType mimeType, string mimeTypeString, string ext = null, BasicMimeType mtStrMain = BasicMimeType.none)
			{
				MimeType = mimeType;
				GenericMimeType = mimeType.GetGenericMimeType();
				MimeTypeString = mimeTypeString;
				Ext = ext;
				if(mtStrMain != BasicMimeType.none) {
					if(mimeTypeString.NotNulle())
						throw new ArgumentException();
					MainMimeStringType = mtStrMain;
				}
			}

			public override string ToString()
				=> $"{MimeType} .{Ext} '{MimeTypeString}' ({(ExtraMimeStrings.IsNulle() ? "f" : "t")} / {(ExtraExts.IsNulle() ? "f" : "t")})";
		}
	}

	static class MTIXte
	{
		public static BasicMimeTypesX.MTI ExtraExtensions(this BasicMimeTypesX.MTI m, params string[] exts)
		{
			if(m.ExtraExts == null)
				m.ExtraExts = new List<string>();

			foreach(string ext in exts) {
				string x = ext.NullIfEmptyTrimmed();
				if(x != null)
					m.ExtraExts.Add(x);
			}
			return m;
		}

		public static BasicMimeTypesX.MTI ExtraMimeStrings(this BasicMimeTypesX.MTI m, params string[] mimeStrs)
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
