using System.Collections.Generic;
using MTI = DotNetXtensions.MimeTypeInfo;

namespace DotNetXtensions
{
	public static partial class BasicMimeTypesX
	{
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
				.AddExtraExtension("xml"),

			new MTI(BasicMimeType.application_json, "application/json", "json"),
			new MTI(BasicMimeType.application_octetstream, "application/octet-stream"),
			new MTI(BasicMimeType.application_pdf, "application/pdf", "pdf"),
			new MTI(BasicMimeType.application_rss_xml, "application/rss+xml", "rss")
				.AddExtraExtension("xml"),

			new MTI(BasicMimeType.application_word, "application/msword", "docx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
				.AddExtraExtension("doc"),

			new MTI(BasicMimeType.application_excel, "application/vnd.ms-excel", "xlsx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
				.AddExtraExtension("xls"),

			new MTI(BasicMimeType.application_powerpoint, "application/vnd.ms-powerpoint", "pptx")
				.ExtraMimeStrings("application/vnd.openxmlformats-officedocument.presentationml.presentation")
				.AddExtraExtension("ppt"),

			new MTI(BasicMimeType.application_xml, "application/xml")
				.AddExtraExtension("xml"), // making secondary for extension



			// ------- AUDIO -------

			new MTI(BasicMimeType.audio, "audio/null")
				.ExtraMimeStrings("audio", "audio/"),
			new MTI(BasicMimeType.audio_aac, "audio/aac", "aac")
				.AddExtraExtension("m4a"),

			new MTI(BasicMimeType.audio_aif, "audio/aif", "aiff")
				.ExtraMimeStrings("audio/x-aiff")
				.AddExtraExtension("aif"),

			new MTI(BasicMimeType.audio_mp3, "audio/mpeg", "mp3")
				.ExtraMimeStrings("audio/mp3", "audio/mpeg3", "audio/x-mpeg3")
				.AddExtraExtensions("mpga", "mpeg"),

			new MTI(BasicMimeType.audio_mp4, "audio/mp4", "m4a")
				.AddExtraExtensions("mp4a", "mp4"),

			new MTI(BasicMimeType.audio_ogg, "audio/ogg", "oga")
				.AddExtraExtension("ogg"), // .oga = audio only, .ogv for video, .ogg was old format unspecified. we will add ".ogg" as the audio one

			new MTI(BasicMimeType.audio_wav, "audio/wav", "wav"),
			new MTI(BasicMimeType.audio_webm, "audio/webm", null)
				.AddExtraExtension("webm"), // wiki: "WebM is a video file format." So that is it's main deal, leave .webm for video mime below

			new MTI(BasicMimeType.audio_wma, "audio/x-ms-wma", "wma"),



			// ------- IMAGE -------

			new MTI(BasicMimeType.image, "image/null") //, mtStrMain: BasicMimeType.image_jpeg)
				.ExtraMimeStrings("image", "image/"),

			new MTI(BasicMimeType.image_gif, "image/gif", "gif"),
			new MTI(BasicMimeType.image_ico, "image/x-icon"),

			new MTI(BasicMimeType.image_jpeg, "image/jpeg", "jpg")
				.AddExtraExtension("jpeg"),

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
				.AddExtraExtensions("xhtml", "htm"),
			new MTI(BasicMimeType.text_plain, "text/plain", "txt"),
			new MTI(BasicMimeType.text_markdown, "text/markdown", "md")
				.AddExtraExtensions("markdown", "mdown"),
			new MTI(BasicMimeType.text_xml, "text/xml", "xml"), // note: we're making this one the main for xml extension



			// ------- VIDEO -------

			new MTI(BasicMimeType.video, "video/null")
				.ExtraMimeStrings("video", "video/"),
				//.AddExtraExtensions(""), // ???? what was the point of this?
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
	}
}
