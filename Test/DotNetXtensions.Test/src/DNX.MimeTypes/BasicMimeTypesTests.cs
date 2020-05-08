using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;
using Xunit;

namespace DNX.Test
{
	public class BasicMimeTypesTests : DotNetXtensions.Test.DnxTestBase
	{
		[Theory]
		[InlineData(BasicMimeType.image_jpeg, "image/jpeg", "jpg")]
		[InlineData(BasicMimeType.image_png, "image/png", "png")]
		[InlineData(BasicMimeType.image_gif, "image/gif", "gif")]
		[InlineData(BasicMimeType.image_svg, "image/svg+xml", "svg")]
		[InlineData(BasicMimeType.application_pdf, "application/pdf", "pdf")]
		public void TestBunchOfFullValidTypes(
			BasicMimeType inMimeType, string inMimeTypeStr, string inExt)
		{
			// ASSERT: NOT testing None || Generic types || null mstring || extensions
			True(inMimeTypeStr.NotNulle());
			True(inExt.NotNulle());
			False(inMimeType.IsGenericTypeOrNone());

			BasicMimeType mtypeFromStr = BasicMimeTypesX.ParseMimeType(inMimeTypeStr);

			True(mtypeFromStr == inMimeType);

			BasicMimeType extMtype = BasicMimeTypesX.GetMimeTypeFromFileExtension(inExt);

			True(mtypeFromStr == extMtype);

			string gottenMTypeStrFromInEnum = inMimeType.MimeTypeString();

			True(inMimeTypeStr == gottenMTypeStrFromInEnum);
		}

		[Theory]
		[InlineData(BasicMimeType.image, "image/none")]
		[InlineData(BasicMimeType.text, "text/none")]
		[InlineData(BasicMimeType.audio, "audio/none")]
		public void TestGenericTypes(BasicMimeType inMimeType, string inMimeTypeStr)
		{
			True(inMimeType != BasicMimeType.none);
			True(inMimeType.IsGenericType());
			True(inMimeTypeStr.NotNulle());
			True(inMimeTypeStr.EndsWith("/none"));

			BasicMimeType mtypeFromStr = BasicMimeTypesX.ParseMimeType(inMimeTypeStr);
			// none; see next line. Tests that MUST have `allowGenericMatchOnNotFound: true`,
			// and that default is the reverse
			True(mtypeFromStr == BasicMimeType.none); 

			mtypeFromStr = BasicMimeTypesX.ParseMimeType(inMimeTypeStr, allowGenericMatchOnNotFound: true);
			True(inMimeType == mtypeFromStr);
		}

		/// <summary>
		/// Tests invalid (null, empty, etc) or none matching extension,
		/// goal being NO exceptions even if input ext is null
		/// </summary>
		/// <param name="ext"></param>
		[Theory]
		[InlineData("jpgr")]
		[InlineData("blah")]
		[InlineData("")]
		[InlineData(null)]
		public void Test_InvalidOrMissing_Extensions(string ext)
		{
			BasicMimeType extMtype = BasicMimeTypesX.GetMimeTypeFromFileExtension(ext);
			True(extMtype == BasicMimeType.none);
		}

	}
}
