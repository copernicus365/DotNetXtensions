using System;
using System.Collections.Generic;
using System.Linq;

///// <summary>
///// An enum that contains many of the most common or basic mimetypes,
///// thus the name <see cref="BasicMimeType"/>. The purpose is certainly
///// not to enumerate every possible mimetype, which would be impossible
///// and unadvisable. But it is beneficial in many cases to have this, 
///// because for instance, the numeric value of these allows them to be 
///// categorized by their base types (all 'text/*' ones are 100s, 
///// 'image/*' ones are 400s, etc), and importantly, a "base" type for each
///// main category existed when the sub-mimetype isn't one of these, i.e. 
///// see <see cref="BasicMimeType.text"/> or <see cref="BasicMimeType.audio"/>.
///// See <see cref="BasicMimeTypesX"/> for functions for working with these and
///// with mimetypes in general.
///// </summary>

#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	enum BasicMimeType
	{
		none = 0,




		/// <summary><![CDATA[text: >= 100 && < 400]]></summary>
		text = 100,

		/// <summary>text/plain</summary>
		text_plain = 101,

		/// <summary>text/html</summary>
		text_html = 160,

		text_markdown = 170,

		text_css = 175,
		text_scss = 176,
		text_sass = 177,
		text_less = 178,

		/// <summary>text/xml, note there is also application/xml</summary>
		text_xml = 200,



		/// <summary><![CDATA[image: >= 400 && < 500]]></summary>
		image = 400,

		/// <summary>image/jpeg</summary>
		image_jpeg = 403,

		/// <summary>image/png</summary>
		image_png = 406,

		/// <summary>image/gif</summary>
		image_gif = 409,

		/// <summary>image/x-icon</summary>
		image_ico = 412,



		/// <summary><![CDATA[audio: >= 500 && < 600]]></summary>
		audio = 500,

		/// <summary>audio/mpeg (also? audio/mpeg3, audio/x-mpeg3), also: .mp1 .mp2 .mpg .mpeg</summary>
		audio_mp3 = 501,


		/// <summary>audio/aac</summary>
		audio_aac = 503,

		/// <summary>audio/aif</summary>
		audio_aif = 506,

		/// <summary>audio/mp4, also: .m4a</summary>
		audio_mp4 = 509,

		/// <summary>audio/ogg, also: .oga</summary>
		audio_ogg = 512,

		/// <summary>audio/wav</summary>
		audio_wav = 515,

		/// <summary>audio/webm</summary>
		audio_webm = 518,

		/// <summary>audio/x-ms-wma</summary>
		audio_wma = 521,





		/// <summary><![CDATA[video: >= 600 && < 700]]></summary>
		video = 600,

		/// <summary>video/mp4 (MPEG-4); also: .m4v</summary>
		video_mp4 = 601,


		/// <summary>video/3gpp (3GP Mobile)</summary>
		video_3gp = 603,

		/// <summary>video/x-msvideo (A/V Interleave)</summary>
		video_avi = 606,

		/// <summary>video/x-flv (Flash)</summary>
		video_flv = 609,

		/// <summary>video/x-mpegURL (iPhone Index)</summary>
		video_m3u8 = 612,

		/// <summary>video/quicktime (QuickTime)</summary>
		video_mov = 615,

		/// <summary>video/ogg, also: .ogv</summary>
		video_ogg = 618,

		/// <summary>video/MP2T (iPhone Segment)</summary>
		video_ts = 621,

		/// <summary>video/vimeo</summary>
		video_vimeo = 622,

		/// <summary>video/webm</summary>
		video_webm = 624,

		/// <summary>video/x-ms-wmv (Windows Media)</summary>
		video_wmv = 627,

		/// <summary>video/youtube</summary>
		video_youtube = 628,





		/// <summary><![CDATA[application: >= 700 && < 800]]></summary>
		application = 700,

		/// <summary>application/octet-stream</summary>
		application_octetstream = 701,

		/// <summary>application/octet-stream</summary>
		application_json = 710,

		application_javascript = 720,

		application_typescript = 722,

		/// <summary>application/pdf</summary>
		application_pdf = 730,

		//application/rss+xml
		/// <summary>application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, also: .doc, .docx</summary>
		application_word = 735, // need to leave plenty of room for office types here...
		application_excel = 740,
		application_powerpoint = 745,

		/// <summary>application/xml, note there is also text/xml</summary>
		application_xml = 760,

		application_rss_xml = 761,

		application_atom_xml = 762,

	}
}
