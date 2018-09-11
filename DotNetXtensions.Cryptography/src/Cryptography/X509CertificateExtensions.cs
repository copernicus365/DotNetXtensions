using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DotNetXtensions.Cryptography
{
	/// <summary>
	/// Need to find where I got this from stackoverflow, this is a significantly altered version,
	/// but the main logic was still mostly from there...
	/// ---
	/// https://www.sslshopper.com/ssl-converter.html
	/// </summary>
	public static class X509CertificateExtensions
	{
		public const string defaultPrivKeyHeader = "RSA PRIVATE KEY";

		#region --- ExportPrivateKey ---

		public static string ExportPrivateKeyPEM(this X509Certificate2 cert,
			bool includeB64LineBreaks = true,
			string headerFooter = defaultPrivKeyHeader)
		{
			RSACryptoServiceProvider privKeyPrvdr = (RSACryptoServiceProvider)cert.PrivateKey;
			return privKeyPrvdr.ExportPrivateKeyPEM(includeB64LineBreaks, headerFooter);
		}

		public static string ExportPrivateKeyPEM(this RSACryptoServiceProvider csp,
			bool includeB64LineBreaks = true,
			string headerFooter = defaultPrivKeyHeader)
		{
			var sb = new StringBuilder();
			TextWriter outputStream = new StringWriter(sb);
			ExportPrivateKeyPEM(csp, outputStream, includeB64LineBreaks, headerFooter);
			return sb.ToString();
		}

		public static void ExportPrivateKeyPEM(
			this RSACryptoServiceProvider csp,
			TextWriter outputStream,
			bool includeB64LineBreaks = false,
			string headerFooter = defaultPrivKeyHeader)
		{
			if(csp.PublicOnly)
				throw new ArgumentException("CSP does not contain a private key", "csp");

			var p = csp.ExportParameters(true);

			using(var stream = new MemoryStream()) {

				var writer = new BinaryWriter(stream);

				writer.Write((byte)0x30); // SEQUENCE

				using(var innerStream = new MemoryStream()) {
					var innerWriter = new BinaryWriter(innerStream);

					EncodeIntegerBigEndian(innerWriter, true,
						new byte[] { 0x00 },
						p.Modulus,
						p.Exponent,
						p.D,
						p.P,
						p.Q,
						p.DP,
						p.DQ,
						p.InverseQ); // Version

					var length = (int)innerStream.Length;
					EncodeLength(writer, length);

					writer.Write(innerStream.GetBuffer(), 0, length);
				}

				string base64 = Convert.ToBase64String(stream.ToArray(), Base64FormattingOptions.None);
				if(includeB64LineBreaks)
					base64 = base64.InsertBreaks(64, "\r\n");

				bool includeCerHeaderFooter = headerFooter.NotNulle();
				if(includeCerHeaderFooter)
					outputStream.WriteLine($"-----BEGIN {headerFooter}-----");

				outputStream.WriteLine(base64);

				if(includeCerHeaderFooter)
					outputStream.WriteLine($"-----END {headerFooter}-----");
			}
		}

		private static void EncodeLength(BinaryWriter stream, int length)
		{
			if(length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
			if(length < 0x80) {
				// Short form
				stream.Write((byte)length);
			}
			else {
				// Long form
				var temp = length;
				var bytesRequired = 0;
				while(temp > 0) {
					temp >>= 8;
					bytesRequired++;
				}
				stream.Write((byte)(bytesRequired | 0x80));
				for(var i = bytesRequired - 1; i >= 0; i--) {
					stream.Write((byte)(length >> (8 * i) & 0xff));
				}
			}
		}

		private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
		{
			stream.Write((byte)0x02); // INTEGER
			var prefixZeros = 0;
			for(var i = 0; i < value.Length; i++) {
				if(value[i] != 0) break;
				prefixZeros++;
			}
			if(value.Length - prefixZeros == 0) {
				EncodeLength(stream, 1);
				stream.Write((byte)0);
			}
			else {
				if(forceUnsigned && value[prefixZeros] > 0x7f) {
					// Add a prefix zero to force unsigned if the MSB is 1
					EncodeLength(stream, value.Length - prefixZeros + 1);
					stream.Write((byte)0);
				}
				else {
					EncodeLength(stream, value.Length - prefixZeros);
				}
				for(var i = prefixZeros; i < value.Length; i++) {
					stream.Write(value[i]);
				}
			}
		}

		private static void EncodeIntegerBigEndian(BinaryWriter stream, bool forceUnsigned, params byte[][] values)
		{
			for(int i = 0; i < values.Length; i++)
				EncodeIntegerBigEndian(stream, values[i], forceUnsigned);
		}

		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="cert">The cert.</param>
		/// <param name="contentType">One of the X509ContentType values that describes how to format the output data.</param>
		/// <param name="password">The password required to access the X.509 certificate data.</param>
		/// <returns></returns>
		public static string ExportToPEM(
			this X509Certificate2 cert, 
			X509ContentType contentType = X509ContentType.Cert, 
			string password = null)
		{
			byte[] data = password == null
				? cert.Export(contentType)
				: cert.Export(contentType, password);

			string certBase64 = 
				Convert.ToBase64String(data, Base64FormattingOptions.None)
				.InsertBreaks(64, "\r\n");

			string certPEM = $"-----BEGIN CERTIFICATE-----\r\n{certBase64}\r\n-----END CERTIFICATE-----\r\n";
			return certPEM;
		}

	}
}
