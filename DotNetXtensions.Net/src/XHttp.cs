using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Linq;
using System.Diagnostics;

namespace DotNetXtensions.Net
{
    public static class XHttp
    {

        #region --- User-Agent ---

        /// <summary>
        /// fyi, find values here: https://techblog.willshouse.com/2012/01/03/most-common-user-agents/
        /// notes: 
        /// current value is : Postman useragent value as of June 2015
        /// previous value (IE based):
        /// "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
        /// </summary>
        public const string con_UserAgentHeaderDefault =
            //"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/43.0.2357.81 Safari/537.36";
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";

        public const string con_AcceptHeaderDefault = "*/*";

        public static string DefaultUserAgent = con_UserAgentHeaderDefault;
        public static string DefaultAcceptHeader = con_AcceptHeaderDefault;

        public static HttpRequestHeaders SetUserAgentIfNone(this HttpRequestHeaders headers, string userAgent = null)
        {
            if (headers?.UserAgent?.Count < 1)
            {
                userAgent = userAgent.QQQ(DefaultUserAgent);
                if (userAgent.NotNulle())
                    headers.Add("User-Agent", userAgent);
            }
            return headers;
        }

        public static HttpRequestHeaders SetAcceptIfNone(this HttpRequestHeaders headers, string accept = null)
        {
            if (headers != null && headers.Accept.Count < 1)
            {
                accept = accept.QQQ(DefaultAcceptHeader);
                if (accept.NotNulle())
                    headers.Accept.AddN(accept);
            }
            return headers;
        }

        public static HttpRequestHeaders SetDefaultHeaders(this HttpRequestHeaders headers)
        {
            if (headers != null)
            {
                headers.SetAcceptIfNone();
                headers.SetUserAgentIfNone();
            }
            return headers;
        }

        public static HttpRequestMessage SetDefaultHeaders(this HttpRequestMessage request)
        {
            if (request != null)
                request.Headers.SetDefaultHeaders();
            return request;
        }


        #endregion

        #region --- HttpClient ---

        public static Func<HttpClient> GetHttpClient { get; set; } = () => DefaultHttpClient ?? new HttpClient();

        public static TimeSpan DefaultTimeout {
            get { return _defaultTimeout; }
            set {
                if (value <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException();
                _defaultTimeout = value;
            }
        }

        static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(20);

        static HttpClient _getHttpClient() => GetHttpClient() ?? (DefaultHttpClient ?? new HttpClient());

        static HttpClient DefaultHttpClient = new HttpClient();

        #endregion

        #region --- GetAsync ---

        /// <summary>
        /// A powerful function that performs, via HttpClient, an HTTP GET request,
        /// but while adding the following helpful functions:
        /// 1) If <paramref name="settings"/> (<see cref="HttpNotModifiedProperties"/>) is not null, 
        /// it allows a conditional GET request to be made. 
        /// 2) Timeout allows a true timeout to be set that doesn't have to use the deeply flawed
        /// HttpClient timeout value. This is a great design flaw since their design requires the HttpClient
        /// to be reused globally.
        /// 3) Useful information is returned about the request, the HttpStatus, the basic result (Success or NotModified
        /// or Failed...), and the HttpHeaders, as well as NotModified related information that can be used on the next 
        /// request to make a 304 (NotModified) request (<see cref="HttpResponseInfo"/> inherits <see cref="HttpNotModifiedProperties"/>).
        /// 4) Timing diagnostics on the response times, for both headers and content.
        /// 5) Exceptions are caught, but set on the <see cref="HttpResponseInfo.Ex"/> property.
        /// </summary>
        /// <param name="url">Url to GET.</param>
        /// <param name="settings">Not modified properties in order to make a not-modified request.</param>
        /// <param name="timeout">Timeout if any.</param>
        /// <param name="client">Client if any.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static async Task<HttpResponseInfo> GetAsync(
            string url,
            HttpNotModifiedProperties settings = null,
            TimeSpan? timeout = null,
            HttpClient client = null,
            CancellationToken? cancellationToken = null)
        {
            var swTotal = Stopwatch.StartNew();

            if (url.IsNulle()) throw new ArgumentNullException(nameof(url));

            var s = settings ?? new HttpNotModifiedProperties();
            HttpResponseInfo h = new HttpResponseInfo()
            {
                Date = DateTimeOffset.UtcNow
            };
            h.CopyValuesToThis(s);

            if (client == null)
                client = _getHttpClient();

            CancellationToken cancelToken = cancellationToken ?? new CancellationToken();
            bool condGet = s.ConditionalGet;

            TimeSpan _timeout = timeout ?? TimeSpan.Zero;
            if (_timeout <= TimeSpan.Zero)
                _timeout = DefaultTimeout;
            //if(timeoutSeconds > 0) // problem, HttpClient is supposed to be used globally!!
            //	client.Timeout = TimeSpan.FromSeconds((int)timeoutSeconds);

            bool closeClient = client == null;
            try
            {

                // --- PREP REQUEST ---
                var request = new HttpRequestMessage(HttpMethod.Get, url)
                    .SetDefaultHeaders();

                // set request IfModifiedSince header if available
                if (condGet)
                    request.Headers.IfModifiedSince = s.LastModified;

                // set request ETag if available
                if (condGet && s.ETag.NotNulle())
                {
                    //request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(""))
                    request.Headers.Add("If-None-Match", FixETag(s.ETag));
                }
                // -- GET RESPONSE --


                try
                {
                    var sw = Stopwatch.StartNew();

                    var response = await client
                        .SendAsync(request, cancelToken)
                        //.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancelToken)
                        .TimeoutAfterAsync(_timeout);

                    h.HeaderResponseTime = sw.ElapsedAndReset();

                    TimeSpan remainingTime = _timeout - h.HeaderResponseTime;
                    if (remainingTime <= TimeSpan.Zero)
                    {
                        remainingTime = TimeSpan.FromMilliseconds(1);
                        // shouldn't happen, but if TimeoutAfterAsync didn't throw, make this not 
                        // a negative number at least, which is invalid for the next TimeoutAfterAsync call
                    }

                    h.StatusCode = response.StatusCode;

                    h.ResultHeadersDict = response.GetHeadersDictionary();

                    h.ResultHeaders = h.ResultHeadersDict.__kvHeadersToString();

                    // --- Status code checks ---

                    if (response.StatusCode == HttpStatusCode.NotModified)
                    {
                        h.Result = HttpResponseResult.NotModified;
                        return h;
                    }

                    if (!response.IsSuccessStatusCode)
                    {
                        h.Result = HttpResponseResult.Fail;
                        return h;
                    }

                    // --- ETag ---

                    string responseEtag = FixETag(h.ResultHeadersDict.V("ETag")); // response.Headers.ETag; // IS NULL!! STUPID!! Come on guys!

                    if (condGet && s.ETag.NotNulle() && responseEtag == s.ETag)
                    {
                        h.Result = HttpResponseResult.NotModified;
                        return h;
                    }
                    h.ETag = responseEtag;

                    // --- ContentLength check #1) If Content-Length header is set ---

                    int? contentLengthHeader = h.ResultHeadersDict.V("Content-Length").TrimIfNeeded().ToIntN();
                    if (condGet && s.ContentLengthNotModified(contentLengthHeader))
                    {
                        // contentLengthNotModified(eqCntLenNotMod, contentLengthHeader, s.ContentLength)) {
                        h.Result = HttpResponseResult.NotModified;
                        return h;
                    }

                    // --- READ CONTENT ---

                    sw.Start();

                    byte[] data = await response.Content
                        .ReadAsByteArrayAsync()
                        .TimeoutAfterAsync(remainingTime);

                    h.ContentResponseTime = sw.ElapsedAndStop();

                    if (data == null) data = new byte[0]; // this seems right
                    h.ContentData = data;

                    int dataLen = data.LengthN();

                    // --- ContentLength check #2) If actual data length is equal ---
                    if (condGet && s.ContentLengthNotModified(dataLen))
                    {
                        h.Result = HttpResponseResult.NotModified;
                        return h;
                    }

                    h.ContentLength = dataLen; // methods above should have set, but no hurt

                    h.Result = HttpResponseResult.Success;

                    return h;
                }
                catch (Exception ex)
                {
                    h.Result = HttpResponseResult.Fail;
                    if (ex is TimeoutException)
                        h.Result = HttpResponseResult.TimeOut;
                    else
                        h.Ex = ex; // don't save if just a timeout
                    return h;
                }
            }
            finally
            {
                if (closeClient && client != null)
                    client.Dispose();
                if (h != null && swTotal != null && swTotal.IsRunning)
                    h.TotalResponseTime = swTotal.Elapsed;
            }
        }

        /// <summary>
        /// Indirection to main GetAsync method that returns <see cref="HttpResponseInfo"/>, 
        /// this simply lets it be called as an extenion method on a <see cref="HttpNotModifiedProperties"/>
        /// instance.
        /// </summary>
        public static async Task<HttpResponseInfo> GetAsync(
            this HttpNotModifiedProperties settings,
            string url,
            TimeSpan? timeout = null,
            HttpClient client = null,
            CancellationToken? cancellationToken = null)
        {
            return await GetAsync(url, settings, timeout, client, cancellationToken);
        }

        /// <summary>
        /// Indirection to main GetAsync method that returns <see cref="HttpResponseInfo"/>, this simply returns the string result.
        /// </summary>
        public static async Task<string> GetStringAsync(
            string url,
            HttpNotModifiedProperties settings = null,
            TimeSpan? timeout = null,
            HttpClient client = null,
            CancellationToken? cancellationToken = null)
        {
            var r = await GetAsync(url, settings, timeout, client, cancellationToken);
            return r.GetContentString();
        }

        /// <summary>
        /// Indirection to main GetAsync method that returns <see cref="HttpResponseInfo"/>, this simply returns the data result.
        /// </summary>
        public static async Task<byte[]> GetDataAsync(
            string url,
            HttpNotModifiedProperties settings = null,
            TimeSpan? timeout = null,
            HttpClient client = null,
            CancellationToken? cancellationToken = null)
        {
            var r = await GetAsync(url, settings, timeout, client, cancellationToken);
            return r.ContentData;
        }


        #endregion

        #region --- Misc Helpers ---

        /// <summary>
        /// If ETag is missing quotes we simply surround the value with double quotes to make it valid. 
        /// HttpRequestHeaders (request.Headers) throws FormatException saying "The format of value is invalid" 
        /// otherwise when setting the If-None-Match header. 
        /// We only check the final char, to get around W/"123456789" weak tag versions.
        /// </summary>
        /// <param name="etag">The etag or null. Returns null if nulle.</param>
        public static string FixETag(string etag)
        {
            if (etag.NotNulle())
            {
                if (etag.Last() != '"')
                    etag = '"' + etag + '"';
            }
            return etag;
        }

        public static string Status(this HttpResponseMessage response)
        {
            if (response == null)
                return null;

            string val = "({0}) {1}".FormatX(((int)response.StatusCode), response.ReasonPhrase);
            return val.Last() == ' ' ? val.Trim() : val;
        }

        #endregion

        #region --- GetHeaders ---

        public static List<KeyValuePair<string, string>> GetHeadersFromHttpMessageString(string headersStr, KeyValuePair<string, string>? topLine = null)
        {
            if (headersStr.IsNulle())
                return new List<KeyValuePair<string, string>>();

            string[] lines = headersStr
                .SplitLines(trimLines: true, removeEmptyLines: true);

            if (lines.IsNulle())
                return new List<KeyValuePair<string, string>>();

            // the output string has 3 lines we ignore, so even if 0 headers, should at least be 4
            if (lines.Length < 3 || !lines[1].StartsWithN("{") || !lines.Last().StartsWithN("}"))
                throw new ArgumentOutOfRangeException("This parse expects a certain format when HttpResponseMessage.ToString is called that was not met.");

            int len = lines.Length - 1;
            var list = new List<KeyValuePair<string, string>>(len);

            if (topLine != null)
                list.Add(topLine.Value);

            for (int i = 2; i < len; i++)
            {
                string ln = lines[i];
                int idx = ln.IndexOf(':');
                if (idx < 1)
                    continue;

                string ky = ln.Substring(0, idx).TrimN();
                string val = ln.Substring(idx + 1).TrimN(); // 1 past last index will not throw, just return empty string

                list.Add(new KeyValuePair<string, string>(ky, val));
            }
            return list;
        }



        // All the hard logic here:
        public static List<KeyValuePair<string, string>> GetHeaders(this HttpResponseMessage response)
        {
            string version = "HTTP/" + response.Version + " " + ((int)response.StatusCode) + " " + response.StatusCode;
            return GetHeadersFromHttpMessageString(response.ToStringN(), new KeyValuePair<string, string>("", version));
        }

        public static List<KeyValuePair<string, string>> GetHeaders(this HttpRequestMessage request)
        {
            string method = request.Method.ToString().ToUpper() + " " + request.RequestUri + " " + "HTTP/" + request.Version;
            return GetHeadersFromHttpMessageString(request.ToStringN(), new KeyValuePair<string, string>("", method));
        }

        public static string GetHeadersString(this HttpRequestMessage request, bool doubleLine = false)
        {
            return request.GetHeadersDictionary().__kvHeadersToString(doubleLine);// GetHeaders(request).KVHeadersToString();
        }
        public static string GetHeadersString(this HttpResponseMessage response, bool doubleLine = false)
        {
            return response.GetHeadersDictionary().__kvHeadersToString(doubleLine);// GetHeaders(request).KVHeadersToString();
        }


        public static Dictionary<string, string> GetHeadersDictionary(this HttpResponseMessage responseMsg)
        {
            return _toDictionary(responseMsg.GetHeaders());
        }

        public static Dictionary<string, string> GetHeadersDictionary(this HttpRequestMessage requestMsg)
        {
            return _toDictionary(requestMsg.GetHeaders());
        }


        /// <summary>
        /// I reworked this, am keeping this old code till we can be sure we redid this fine
        /// </summary>
        static Dictionary<string, string> _toDictionaryOld(IEnumerable<KeyValuePair<string, string>> headers, string multiValueSeparator = ", ")
        {
            var dict = new Dictionary<string, string>(20);

            // simply overwrite existing member, no exception, and thus no double check lookup needed
            foreach (var kv in headers)
            {
                string key = kv.Key ?? "";
                string val = dict.V(key);
                if (val.IsNulle())
                    val = kv.Value;
                else
                {
                    if (key.EqualsIgnoreCase("user-agent"))
                        val = val + " " + kv.Value;
                    else
                        val = val + multiValueSeparator + kv.Value;
                }
                dict[key] = val;
            }
            return dict;
        }

        static Dictionary<string, string> _toDictionary(IEnumerable<KeyValuePair<string, string>> headers, string multiValueSeparator = ", ")
        {
            var dict = new Dictionary<string, string>(20);

            // simply overwrite existing member, no exception, and thus no double check lookup needed
            foreach (var kv in headers)
            {
                string key = kv.Key.TrimIfNeeded() ?? "";
                string val = kv.Value.TrimIfNeeded();
                string currVal = dict.V(key);

                if (currVal.NotNulle())
                {
                    string mvSep = key.EqualsIgnoreCase("user-agent")
                        ? " "
                        : multiValueSeparator;
                    val = currVal + mvSep + val;
                }
                dict[key] = val;
            }
            return dict;
        }

        #endregion

        #region --- UrlExists ---

        /// <summary>
        /// Detects if url exists using the input or else will be newly created HttpClient,
        /// using a <see cref="HttpMethod.Head"/> request only. Returns client's IsSuccessStatusCode
        /// value. The whole body is contained in a try catch, with false returned in the catch.
        /// </summary>
        /// <param name="client">HttpClient</param>
        /// <param name="url">Url to check</param>
        public static async Task<bool> UrlExists(this HttpClient client, string url)
        {
            try
            {
                if (client == null)
                    client = _getHttpClient();

                var httpRequestMsg = new HttpRequestMessage(HttpMethod.Head, url);
                var response = await client.SendAsync(httpRequestMsg);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region --- HttpHeaderValueCollection Helpers --

        public static HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> AddN(
            this HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> coll, params string[] mediaTypes)
        {
            if (mediaTypes.NotNulle() && coll != null)
            {
                for (int i = 0; i < mediaTypes.Length; i++)
                    coll.Add(new MediaTypeWithQualityHeaderValue(mediaTypes[i]));
            }
            return coll;
        }

        public static HttpHeaderValueCollection<StringWithQualityHeaderValue> AddN(
            this HttpHeaderValueCollection<StringWithQualityHeaderValue> coll, params string[] mediaTypes)
        {
            if (mediaTypes.NotNulle() && coll != null)
            {
                for (int i = 0; i < mediaTypes.Length; i++)
                    coll.Add(new StringWithQualityHeaderValue(mediaTypes[i]));
            }
            return coll;
        }


        static void __addKVNotNull(List<KeyValuePair<string, string>> kvs, string key, object value)
        {
            string val = value.ToStringN();
            if (val.NotNulle())
                kvs.Add(new KeyValuePair<string, string>(key, val));
        }

        static string __kvHeadersToString(this IEnumerable<KeyValuePair<string, string>> headers, bool doubleLine = false, string keySeparator = ": ")
        {
            StringBuilder sb = new StringBuilder(500);
            string newLine = doubleLine ? "\r\n\r\n" : "\r\n";

            foreach (var kv in headers)
            {
                if (kv.Key.IsNulle())
                {
                    if (kv.Value.IsNulle())
                        continue;
                    else
                        sb.Append(kv.Value);
                }
                else
                    sb.AppendMany(kv.Key, keySeparator, kv.Value);

                sb.Append(newLine);
            }
            string result = sb.ToString();
            return result;
        }

        #endregion

        #region --- HttpResponse/RequestMessage Extensions: SetContent, GetContent, SetContentForms, etc ---

        public static HttpResponseMessage SetContent(this HttpResponseMessage m, string content, string mediaType = "text/plain", Encoding encoding = null)
        {
            if (m != null) m.Content = GetContent(content, mediaType, encoding);
            return m;
        }
        public static HttpResponseMessage SetContent(this HttpResponseMessage m, byte[] content, int offset = 0, int count = 0)
        {
            if (m != null) m.Content = GetContent(content, offset, count);
            return m;
        }
        public static HttpResponseMessage SetContentJson(this HttpResponseMessage m, string content, Encoding encoding = null)
        {
            if (m != null) m.Content = GetContentJson(content, encoding);
            return m;
        }
        public static HttpResponseMessage SetContentForms(this HttpResponseMessage m, FormUrlEncodedContentX formContent)
        {
            if (m != null) m.Content = SetContentForms(formContent);
            return m;
        }
        public static HttpResponseMessage SetContentForms(this HttpResponseMessage m, FormUrlEncodedContent formContent)
        {
            if (m != null) m.Content = formContent;
            return m;
        }

        public static HttpRequestMessage SetContent(this HttpRequestMessage m, string content, string mediaType = "text/plain", Encoding encoding = null)
        {
            if (m != null) m.Content = GetContent(content, mediaType, encoding);
            return m;
        }
        public static HttpRequestMessage SetContent(this HttpRequestMessage m, byte[] content, int offset = 0, int count = 0)
        {
            if (m != null) m.Content = GetContent(content, offset, count);
            return m;
        }
		public static HttpRequestMessage SetContentJson(this HttpRequestMessage m, string content, Encoding encoding = null)
        {
            if (m != null) m.Content = GetContentJson(content, encoding);
            return m;
        }
        public static HttpRequestMessage SetContentForms(this HttpRequestMessage m, FormUrlEncodedContentX formContent)
        {
            if (m != null) m.Content = SetContentForms(formContent);
            return m;
        }
        public static HttpRequestMessage SetContentForms(this HttpRequestMessage m, FormUrlEncodedContent formContent)
        {
            if (m != null) m.Content = formContent;
            return m;
        }


        public static HttpContent GetContent(string content, string mediaType = "text/plain", Encoding encoding = null)
        {
            return new StringContent(content, encoding ?? Encoding.UTF8, mediaType);
        }
        public static HttpContent GetContent(byte[] content, int offset = 0, int count = 0)
        {
            if (offset == 0 && count == 0)
                return new ByteArrayContent(content);
            else
                return new ByteArrayContent(content, offset, count);
        }
        public static HttpContent GetContentJson(string content, Encoding encoding = null)
        {
            return new StringContent(content, encoding ?? Encoding.UTF8, "application/json");
        }
        public static HttpContent SetContentForms(FormUrlEncodedContentX formContent)
        {
            return formContent == null ? null : formContent.ToFormUrlEncodedContent(); ;
        }


		#endregion







		#region --- GetContentXml / SetContentXml ---

		public static HttpResponseMessage SetContentXml(this HttpResponseMessage m, string content, Encoding encoding = null)
		{
			if (m != null) m.Content = GetContentXml(content, encoding);
			return m;
		}
		public static HttpRequestMessage SetContentXml(this HttpRequestMessage m, string content, Encoding encoding = null)
		{
			if (m != null) m.Content = GetContentXml(content, encoding);
			return m;
		}
		public static HttpContent GetContentXml(string content, Encoding encoding = null)
		{
			return new StringContent(content, encoding ?? Encoding.UTF8, "application/xml");
		}

		// --- REMOVED members depending on special XML.cs which now resides in DotNetXtensions.X2

		public static HttpResponseMessage SetContentXml(this HttpResponseMessage m, XElement xml, Encoding encoding = null, bool format = false, bool newLineOnAttributes = false)
		{
			if (m != null) m.Content = GetContentXml(xml, encoding, format, newLineOnAttributes);
			return m;
		}


		public static HttpRequestMessage SetContentXml(this HttpRequestMessage m, XElement xml, Encoding encoding = null, bool format = false, bool newLineOnAttributes = false)
		{
			if (m != null) m.Content = GetContentXml(xml, encoding, format, newLineOnAttributes);
			return m;
		}

		public static HttpContent GetContentXml(XElement xml, Encoding encoding = null, bool format = false, bool newLineOnAttributes = false)
		{
			string content = null;
			if (xml != null) {
				if (newLineOnAttributes)
					content = xml.ToStringFormatted(format, newLineOnAttributes);
				else
					content = xml.ToString(format ? SaveOptions.None : SaveOptions.DisableFormatting);
			}
			return new StringContent(content, encoding ?? Encoding.UTF8, "application/xml");
		}

		#endregion

	}
}
