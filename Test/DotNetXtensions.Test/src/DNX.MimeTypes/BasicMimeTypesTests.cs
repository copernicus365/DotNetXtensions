using System;
using System.Collections.Generic;
using System.Linq;

using DotNetXtensions;
using DotNetXtensions.Collections;

using Xunit;

namespace DNX.Test
{
	public class BasicMimeTypesTests : DotNetXtensions.Test.DnxTestBase
	{
		[Fact]
		public void SomeImgs_GetMimeTypeFromFileExtension_ToMimeTypeString()
		{
			(BasicMimeType mimeType, string mimeTypeStr, string ext)[] expectedMTypesArr = {
				(BasicMimeType.image_jpeg, "image/jpeg", "jpg"),
				(BasicMimeType.image_png, "image/png", "png"),
				(BasicMimeType.image_svg, "image/svg+xml", "svg"),
				(BasicMimeType.none, null, "blah"),
			};

			for(int i = 0; i < expectedMTypesArr.Length; i++) {
				(BasicMimeType exMimeType, string exMTypeStr, string exExt) = expectedMTypesArr[i];

				BasicMimeType mtype = BasicMimeTypesX.GetMimeTypeFromFileExtension(exExt);
				string mtypeStr = mtype.ToMimeTypeString();

				True(mtype == exMimeType);

				if(exMimeType == BasicMimeType.none)
					True(mtypeStr == null);
				else
					True(mtypeStr == exMTypeStr);
			}
		}

	}
}
