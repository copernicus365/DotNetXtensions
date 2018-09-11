using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DotNetXtensions.Net
{
	public class HttpNotModifiedProperties
	{
		public int? ContentLength { get; set; }

		public string ETag { get; set; }

		public DateTimeOffset? LastModified { get; set; }

		public bool EqualContentLengthIsNotModified { get; set; } = Default_EqualContentLengthIsNotModified;
		public static bool Default_EqualContentLengthIsNotModified = false;

		/// <summary>
		/// If true, means that at least one of the <see cref="HttpNotModifiedProperties"/> values
		/// are set, meaning the appropriate Http caller should consider equality of that property 
		/// value with an Http response as representing a not-modified resource.
		/// </summary>
		public bool ConditionalGet { //{ get; set; } = true;
			get {
				return !DisableConditionalGet && (ETag.NotNulle() || ContentLength > 0 || (LastModified > DateTimeOffset.MinValue))
					? true
					: false;
			}
		}

		public bool DisableConditionalGet { get; set; }

		/// <summary>
		/// Returns true if input length is greater than 0 (and non-null), and if that matches 
		/// the current ContentLength value, and if settings allows equal content length to indicate not-modified.
		/// </summary>
		public bool ContentLengthNotModified(int? headerContentLength)
		{
			return EqualContentLengthIsNotModified && headerContentLength > 0 && ContentLength == headerContentLength;
		}


		/////// <summary>
		/////// Special property that may be used in addition to the official <see cref="HttpNotModifiedProperties"/>,
		/////// to be used if desired when the response represents a list of items (like an RSS feed), with this property 
		/////// representing the count of items in the list.
		/////// </summary>
		////public int? ItemsCount { get; set; }

		/////// <summary>
		/////// Special property that may be used in addition to the official <see cref="HttpNotModifiedProperties"/>,
		/////// to be used if desired when the response represents a list of items (like an RSS feed), with this property 
		/////// representing the first item id.
		/////// </summary>
		////public string FirstItemId { get; set; }

		/////// <summary>
		/////// Special property that may be used in addition to the official <see cref="HttpNotModifiedProperties"/>,
		/////// with this property representing a hash that if equal would represent the response is not-modified (example:
		/////// all the ids in the feed could be hashed, if all the same, it could represent no new items).
		/////// </summary>
		////public string Hash { get; set; }






		/// <summary>
		/// Resets the three HTTP values (ETag, ContentLength, and LastModified) to null values
		/// but leaves the other (3) in places (e.g. FirstItemId).
		/// </summary>
		public HttpNotModifiedProperties ResetHttpNotModifiedValues()
		{
			ETag = null;
			ContentLength = null;
			LastModified = null;
			return this;
		}

		public void CopyValuesToThis(HttpNotModifiedProperties v) //, bool? equalContentLengthIsNotModified = null)
		{
			if(v != null) {
				ContentLength = v.ContentLength;
				ETag = v.ETag;
				LastModified = v.LastModified;
				EqualContentLengthIsNotModified = v.EqualContentLengthIsNotModified;
				//FirstItemId = v.FirstItemId;
				//Hash = v.Hash;
				//ItemsCount = v.ItemsCount;
			}
		}

	}
}
