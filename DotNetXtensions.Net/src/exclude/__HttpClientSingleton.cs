//using System.Net.Http;

//namespace DotNetXtensions.Net
//{
//    /// <summary>
//    /// http://stackoverflow.com/questions/15705092/do-httpclient-and-httpclienthandler-have-to-be-disposed
//    /// 
//    /// Notes: http://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/
//    /// https://github.com/mspnp/performance-optimization/blob/master/ImproperInstantiation/docs/ImproperInstantiation.md
//    /// 
//    /// But problem: http://byterot.blogspot.co.uk/2016/07/singleton-httpclient-dns.html
//    /// But on .NET Core not applicable problem: https://github.com/dotnet/corefx/issues/11224
//    /// 
//    /// </summary>
//    public static class HttpClientSingleton
//	{

//		static HttpClient _singletonHttpClient;

//		public static HttpClient Client {
//			get {
//				if(_singletonHttpClient == null) {
//					_singletonHttpClient = new HttpClient();
//				}
//				return _singletonHttpClient;
//			} set {
//				_singletonHttpClient = value;
//			}
//		}



//		static HttpClient _staticHttpClientYoutubeOps;
//		static HttpClient youtubeHttpClient {
//			get {
//				if(_staticHttpClientYoutubeOps == null)
//					_staticHttpClientYoutubeOps = new HttpClient();
//				return _staticHttpClientYoutubeOps;
//			}
//		}

//	}
//}
