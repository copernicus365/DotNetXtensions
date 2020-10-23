using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions
{
	public class MimeTypeInfo
	{
		public BasicMimeType MimeType { get; private set; }

		/// <summary>
		/// The base mime type for this type. E.g. if this
		/// <see cref="MimeType"/> is <see cref="BasicMimeType.image_jpeg"/>,
		/// this property will be set to <see cref="BasicMimeType.image"/>.
		/// </summary>
		public BasicMimeType GenericMimeType { get; private set; }

		public string MimeTypeString { get; private set; }

		// Note: I would like to increase encapsulation by restricting
		// the 'extension' related properties accessors here as well, but can't necessarily
		// do that at this time without further analysis on actual usage
		// in the builder, in other core usages of the API, etc

		/// <summary>
		/// Chief extension for this mime type.
		/// </summary>
		public string Extension { get; set; }

		public List<string> ExtraMimeStrings { get; set; }

		public List<string> ExtraExtensions { get; set; }


		// for now, doesn't look like the following "main" properties were being used???
		//public bool IsMain { get; set; }
		//public BasicMimeType MainMimeStringType { get; set; }


		public MimeTypeInfo(
			BasicMimeType mimeType,
			string mimeTypeString,
			string extension = null)
		{
			MimeType = mimeType;
			GenericMimeType = mimeType.GetGenericMimeType();
			MimeTypeString = mimeTypeString;
			Extension = extension;

			// not sure for now about the former "main" idea, so removing for now
			// --> removed param: BasicMimeType mainMimeType = BasicMimeType.none)
			//if(mainMimeType != BasicMimeType.none) {
			//	if(mimeTypeString.NotNulle())
			//		throw new ArgumentException();
			//	MainMimeStringType = mainMimeType;
			//}
		}

		public override string ToString()
			=> $"{MimeType} .{Extension} '{MimeTypeString}' ({(ExtraMimeStrings.IsNulle() ? "f" : "t")} / {(ExtraExtensions.IsNulle() ? "f" : "t")})";

		public MimeTypeInfo Copy()
		{
			var mti = MemberwiseClone() as MimeTypeInfo;

			if(ExtraMimeStrings.NotNulle())
				mti.ExtraMimeStrings = ExtraMimeStrings.ToList();
			if(ExtraExtensions.NotNulle())
				mti.ExtraExtensions = ExtraExtensions.ToList();

			return mti;
		}
	}
}
