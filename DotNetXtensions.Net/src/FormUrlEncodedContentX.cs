using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;

namespace DotNetXtensions.Net
{
	/// <summary>
	/// Provides a fluid, user-friendly way of creating a FormUrlEncodedContent object
	/// with its values (chain the Add methods, followed by ToFormUrlEncodedContent,
	/// or use extension methods that can simply take FormUrlEncodedContentX as an object).
	/// </summary>
	public class FormUrlEncodedContentX
	{
		public List<KeyValuePair<string, string>> Values { get; set; }

		public FormUrlEncodedContentX() 
		{
			Values = new List<KeyValuePair<string, string>>();
		}

		public FormUrlEncodedContentX Add(string key, object val)
		{
			return Add(key, val == null ? null : val.ToString());
		}

		public FormUrlEncodedContentX Add(string key, string val)
		{
			Values.Add(new KeyValuePair<string,string>(key, val));
			return this;
		}

		public FormUrlEncodedContent ToFormUrlEncodedContent()
		{
			return new FormUrlEncodedContent(Values);
		}

	}

	public static class XFormUrlEncodedContentX
	{
		public static Task<HttpResponseMessage> PostAsync(this HttpClient client, string requestUri, FormUrlEncodedContentX formContent)
		{
			return client.PostAsync(requestUri, formContent.ToFormUrlEncodedContent());
		}

	}
}
