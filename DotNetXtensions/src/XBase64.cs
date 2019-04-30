// For Html Encode/Decode, include reference to System.Web.dll, 
// and add conditional compilation sign System_Web_dll

using System;
using System.Text;
using System.Collections.Generic;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Base64 extension methods..
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XBase64
	{

		#region Base64

		public static string ToBase64(this string str, bool insertLineBreaks = false)
		{
			if (str == null) throw new ArgumentNullException();

			return Convert.ToBase64String(Encoding.UTF8.GetBytes(str),
				insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks :
				Base64FormattingOptions.None);
		}


		public static string ToBase64(this byte[] data, bool insertLineBreaks = false)
		{
			if (data == null) throw new ArgumentNullException();

			return Convert.ToBase64String(
				data,
				insertLineBreaks ? Base64FormattingOptions.InsertLineBreaks : Base64FormattingOptions.None);
		}

		public static string FromBase64(this string str)
		{
			if (str == null) throw new ArgumentNullException();
			return Encoding.UTF8.GetString(Convert.FromBase64String(str));
		}

		public static byte[] FromBase64ToBytes(this string str)
		{
			if (str == null) throw new ArgumentNullException();
			return Convert.FromBase64String(str);
		}




		// --- UrlSafeBase64 ---

		public static string ToUrlSafeBase64(this string data, bool insertLineBreaks = false)
		{
			if (data.IsNulle()) return data;
			return Encoding.UTF8.GetBytes(data).ToUrlSafeBase64(insertLineBreaks);
		}

		public static string ToUrlSafeBase64(this byte[] data, bool insertLineBreaks = false)
		{
			if (data == null) return null;
			string b64 = ToBase64(data, insertLineBreaks);
			return Base64ToUrlSafeBase64(b64);
		}

		public static string FromUrlSafeBase64(this string urlSafeB64)
		{
			if (urlSafeB64 == null) return null;
			string b64 = UrlSafeBase64ToBase64(urlSafeB64);
			string result = Encoding.UTF8.GetString(Convert.FromBase64String(b64));
			return result;
		}

		public static byte[] FromUrlSafeBase64ToBytes(this string urlSafeB64)
		{
			if (urlSafeB64 == null) return null;
			string b64 = UrlSafeBase64ToBase64(urlSafeB64);
			return Convert.FromBase64String(b64);
		}

		// -------

		/// <summary>
		/// Converts a Base64 encoded string to a URL safe version of Base64,
		/// following the same rules followed by Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Encode(): 
		/// * padding is skipped so the pad character '=' doesn't have to be percent encoded 
		/// * the 62nd (+) and 63rd ('/') regular base64 encoding characters are replaced
		/// as follows: '+' with '-', and '/' with '_', as seems to be the standard.
		/// ---
		/// Helpful to figuring this out were the contributions on this thread:
		/// http://stackoverflow.com/questions/26353710/how-to-achieve-base64-url-safe-encoding-in-c
		/// </summary>
		/// <param name="b64">Base64</param>
		public static string Base64ToUrlSafeBase64(string b64)
		{
			if (b64 == null || b64.Length < 1)
				return b64;

			int len = b64.Length;

			int cutEnd = 0;
			if (b64[len - 1] == '=') {
				cutEnd = len > 1 && b64[len - 2] == '='
					? 2
					: 1;
			}

			if (cutEnd > 0)
				len = len - cutEnd;

			if (len <= 0) // shouldn't happen though, bec b64 encoding i don't think would have trailing '=' with nothing before it...
				return "";

			char[] res = new char[len];
			char c = default(char);

			for (int i = 0; i < len; i++) {
				c = b64[i];
				if (c >= 48)
					res[i] = c; // anything > '0' TRUST (including that it's not '='), we are NOT trying to validate .NET's own Base64 conversion
				else if (c == '/')
					res[i] = '_'; // '-';
				else if (c == '+')
					res[i] = '-';
				else
					throw new ArgumentException("Invalid base64 string.");
			}
			return new string(res);
		}

		/// <summary>
		/// Converts Url safe Base64 back to regular Base64, but in a 
		/// performant way, namely, because there is only 1 string allocation,
		/// whereas the other way requires 2 or 3 (3 if '=' padding needed),
		/// as each string.Replace or concatenation requires another string to be made,
		/// and the Replace is done in one pass.
		/// </summary>
		/// <param name="urlSafeB64">Valid Base64. Nothing is done to validate
		/// it is valid however.</param>
		public static string UrlSafeBase64ToBase64(string urlSafeB64)
		{
			string v = urlSafeB64;
			if (v.IsNulle())
				return v;

			int padding = 0;
			switch (v.Length % 4) {
				case 2: padding = 2; break;
				case 3: padding = 1; break;
			}

			int inLen = v.Length;
			int b64len = v.Length + padding;

			char[] arr = new char[b64len];
			char c = default(char);

			for (int i = 0; i < inLen; i++) {
				c = v[i];
				if (c == '_')
					arr[i] = '/';
				else if (c == '-')
					arr[i] = '+';
				else
					arr[i] = c;
			}

			for (int i = 0; i < padding; i++)
				arr[arr.Length - (i + 1)] = '=';

			string b64 = new string(arr);
			return b64;
		}

		#endregion

	}
}