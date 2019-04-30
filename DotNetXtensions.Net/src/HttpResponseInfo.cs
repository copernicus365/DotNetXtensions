using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DotNetXtensions.Net
{
	/// <summary>
	/// A helpful type for http responses that extends <see cref="HttpNotModifiedProperties"/>, which provides
	/// information about an Http Response, such as the <see cref="HttpResponseResult"/> and <see cref="HttpStatusCode"/>
	/// values, whether the request timed out, a headers dictionary, the content of the response, and so forth.
	/// </summary>
	public class HttpResponseInfo : HttpNotModifiedProperties
	{
		public HttpResponseInfo() { }

		public HttpResponseInfo(HttpNotModifiedProperties copyValues)
		{
			CopyValuesToThis(copyValues);
		}

		public HttpResponseResult Result { get; set; }

		public HttpStatusCode? StatusCode { get; set; }

		public bool TimedOut { get { return Result == HttpResponseResult.TimeOut; } }

		public bool Failed { get { return Result.IsFailAny(); } }

		public bool NotModified { get { return Result == HttpResponseResult.NotModified; } }

		/// <summary>
		/// Don't necessarily need this value, but in some situations 
		/// (such as when this is directly persisted) it could be useful.
		/// </summary>
		public DateTimeOffset Date { get; set; }

		public TimeSpan HeaderResponseTime { get; set; }

		public TimeSpan ContentResponseTime { get; set; }

		/// <summary>
		/// Simply sums <see cref="HeaderResponseTime"/> and <see cref="ContentResponseTime"/>.
		/// This does not include any (CPU) time taken by the rest of the overall call, for the 
		/// entire time taken CPU and HTTP see <see cref="TotalResponseTime"/>.
		/// </summary>
		public TimeSpan HttpResponseTime { get { return HeaderResponseTime.Add(ContentResponseTime); } }

		public TimeSpan TotalResponseTime { get; set; }

		/// <summary>
		/// General purpose Message payload. Currently we do not plan on using this value, it is for consumption use only.
		/// </summary>
		public string Message { get; set; }

		public string ResultHeaders { get; set; }

		public Dictionary<string, string> ResultHeadersDict { get; set; }

		public Exception Ex { get; set; }

		public byte[] ContentData { get; set; }

		/// <summary>
		/// To set this value from the byte array (<see cref="ContentData"/>), 
		/// call <see cref="GetContentString(bool)"/>.
		/// </summary>
		public string ContentString { get; set; }

		/// <summary>
		/// Sets <see cref="ContentString"/> as a UTF8 string from <see cref="ContentData"/>, and 
		/// by default DELETES <see cref="ContentData"/> afterwards, then returns the string directly.
		/// </summary>
		/// <param name="deleteContentData">True to delete <see cref="ContentData"/> after calling this. TRUE by default.</param>
		public string GetContentString(bool deleteContentData = true)
		{
			if(ContentData.NotNulle()) {
				ContentString = System.Text.Encoding.UTF8.GetString(ContentData);
				if(deleteContentData)
					ContentData = null;
			}
			return ContentString;
		}

		public HttpResponseInfo Copy()
		{
			HttpResponseInfo copy = (HttpResponseInfo)this.MemberwiseClone();
			return copy;
		}

		public string Report()
		{
			string v = $@"{this.StatusCode} - {this.Result}
     Etag: {this.ETag}, Content-Length: {this.ContentLength}, Last-Modified: {this.LastModified}
     Headers: {this.ResultHeaders}";
			return v;
		}

	}
}
