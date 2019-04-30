
#if !DNXPrivate
namespace DotNetXtensions
{
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	struct UriPathInfo
	{
		public string FileName;
		public string Extension;
		public string QueryString;

		public UriPathInfo(string path, bool trim = true)
		{
			string f = path;

			FileName = Extension = QueryString = null;

			//const int maxExtLen = 8;
			const int minFullLen = 2;

			if (f == null || f.Length <= minFullLen)
				return;

			int start = 0;
			int len = f.Length;
			char c = default(char);

			for (int i = len - 1; i >= 0; i--) {
				c = f[i];
				if (c == '/' || c == '\\') {
					start = i + 1;
					//len = len - start;
					//if (len <= minFullLen)
					//	return;
					break;
				}
			}

			int qIdx = -1;
			int extIdx = -1;
			int lenMinus1 = len - 1;

			for (int i = start; i < len; i++) {
				c = f[i];
				if (c == '.') {
					if (i < lenMinus1)
						extIdx = i + 1;
				}
				else if (c == '?') {
					//if (i < lenMinus1)
					qIdx = i + 1;
					break;
				}
			}

			if (qIdx >= 0) {
				int qLen = len - qIdx;
				if (qLen > 0) // to remove the '?'
					QueryString = f.Substring(qIdx, qLen);
				len = len - qLen - 1;
			}
			if (extIdx >= 0) {
				Extension = f.Substring(extIdx, len - extIdx);
				len = len - Extension.Length - 1;
			}
			if (len > 0) {
				FileName = f.Substring(start, len - start);
			}

			if (trim) {
				FileName = FileName.NullIfEmptyTrimmed();
				Extension = Extension.NullIfEmptyTrimmed();
				QueryString = QueryString.NullIfEmptyTrimmed();
			}
			else {
				FileName = FileName.NullIfEmpty();
				Extension = Extension.NullIfEmpty();
				QueryString = QueryString.NullIfEmpty();
			}
		}

		public override string ToString()
			=> $"({FileName}).({Extension})?({QueryString})";

		/// <summary>
		/// Old extension getter, was much simpler, had no notion of query string.
		/// It also had arbitrary limits, because it was designed for a particular 
		/// case, not general (like max ext length of 8, url chars had to be only
		/// ascii letter or digit, etc).
		/// Now use <see cref="UriPathInfo.Extension"/>
		/// </summary>
		public static string GetExtFromUrl(string url)
		{
			const int maxExtLen = 8;

			if (url != null) {
				int i = url.Length - 1;
				int j = 0;
				for (; i >= 1 && j < maxExtLen; i--, j++) {
					if (url[i] == '.') {
						if (j < 3)
							return null;
						return url.Substring(i + 1).ToLower();
					}
					if (!url[i].IsAsciiLetterOrDigit())
						return null;
				}
			}
			return null;
		}
	}
}
