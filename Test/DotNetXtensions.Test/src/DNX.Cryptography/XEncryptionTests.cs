using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Cryptography;

namespace DotNetXtensions.Test
{
	public class XEncryptionTests : BaseUnitTest
	{
		const string text1 = "Lorem ipsum ... 123";
		const string pass1 = "someP33";

		[Fact]
		public void Test_UrlSafeBase64()
		{
			string encVal = text1.EncryptToUrlSafeBase64(pass1);
			True(encVal != text1 && encVal.Length > text1.Length);

			string valBack = encVal.DecryptUrlSafeBase64ToString(pass1);
			True(valBack == text1);
		}

	}
}
