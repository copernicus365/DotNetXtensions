
namespace DotNetXtensions
{
	public static partial class BasicMimeTypesX
	{
		// --- type of mime queries ---

		/// <summary>
		/// Indicates if the given type is only one of the generic types
		/// without a subtype (doesn't include none). E.g. BasicMimeType.audio.
		/// Internally, the numbering system allows us to know this, in that any enum with a 100 value (v % 100 == 0)
		/// is set as a generic mime type.
		/// </summary>
		public static bool IsGenericType(this BasicMimeType mimeType)
			=> (int)mimeType % 100 == 0;

		public static bool IsGenericTypeOrNone(this BasicMimeType mimeType)
			=> mimeType == BasicMimeType.none || (int)mimeType % 100 == 0;

		/// <summary>
		/// Returns true if linkType is other than none or html 
		/// (the reverse of linkType.IsWebPageOrNone()).
		/// </summary>
		public static bool IsEnclosureType(this BasicMimeType mimeType)
		{
			return mimeType.IsWebPageOrNone() == false;
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
	}
}
