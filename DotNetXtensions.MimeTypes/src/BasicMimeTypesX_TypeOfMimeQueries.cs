
using System.Runtime.CompilerServices;

namespace DotNetXtensions
{
	public static partial class BasicMimeTypesX
	{
		// --- type of mime queries ---

		const int MinBMimeTypeVal = (int)BasicMimeType.text; // == `100`

		/// <summary>
		/// Indicates if the given type is only one of the generic types
		/// without a subtype (doesn't include none). E.g. BasicMimeType.audio.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGenericType(this BasicMimeType mimeType)
			=> (int)mimeType % MinBMimeTypeVal == 0;

		/// <summary>
		/// Indicates if the given type is fully set, i.e. is not
		/// <see cref="BasicMimeType.none"/>, and is not just a generic
		/// version that doesn't have a subtype. An example of a true result
		/// would be <see cref="BasicMimeType.audio_mp3"/>, while
		/// <see cref="BasicMimeType.audio"/> would be false. This is simply
		/// the reverse of <see cref="IsGenericTypeOrNone(BasicMimeType)"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsFullySet(this BasicMimeType mimeType)
			=> !mimeType.IsGenericTypeOrNone();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGenericTypeOrNone(this BasicMimeType mimeType)
			=> mimeType == BasicMimeType.none || mimeType.IsGenericType();

		/// <summary>
		/// Returns true if linkType is other than none or html 
		/// (the reverse of linkType.IsWebPageOrNone()).
		/// </summary>
		public static bool IsEnclosureType(this BasicMimeType mimeType)
			=> mimeType.IsWebPageOrNone() == false;

		/// <summary>
		/// Is none or is any of the text types (lt 200);
		/// </summary>
		public static bool IsTextOrNone(this BasicMimeType mimeType)
			=> (int)mimeType < 200;

		public static bool IsText(this BasicMimeType mimeType)
			=> (int)mimeType < 200 && (int)mimeType >= 100;

		public static bool IsWebPageOrNone(this BasicMimeType mimeType)
			=> mimeType == BasicMimeType.none || mimeType == BasicMimeType.text_html;

		public static bool IsImage(this BasicMimeType mimeType)
			=> (int)mimeType >= 400 && (int)mimeType < 500;

		public static bool IsAudio(this BasicMimeType mimeType)
			=> (int)mimeType >= 500 && (int)mimeType < 600;

		public static bool IsVideo(this BasicMimeType mimeType)
			=> (int)mimeType >= 600 && (int)mimeType < 700;

		public static bool IsAudioOrVideo(this BasicMimeType mimeType)
			=> (int)mimeType >= 500 && (int)mimeType < 700;

		public static bool IsApplication(this BasicMimeType mimeType)
			=> (int)mimeType >= 700 && (int)mimeType < 800;
	}
}
