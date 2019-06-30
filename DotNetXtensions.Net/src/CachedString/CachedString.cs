using System;
using System.Threading.Tasks;
using DotNetXtensions.Net;

namespace DotNetXtensions
{
	/// <summary></summary>
	/// <remarks>
	/// Note: In the future, should we rename this to CachedHttpString (adding Http)?
	/// </remarks>
	public abstract class CachedString : ICachedString
	{
		public abstract string DocUrl { get; set; }

		public abstract TimeSpan ExpiresAfter { get; set; }

		public virtual bool ValidateDownload(string doc) => doc.NotNulle();

		public string Doc { get; set; }

		public DateTimeOffset LastUpdated { get; private set; }

		public DateTimeOffset ExpiresOn => LastUpdated + ExpiresAfter;

		public string ETag { get; set; }

		public async Task<bool> ResetIfNeeded(bool forceReset = false)
		{
			if (!forceReset && Doc.NotNulle() && ExpiresOn > DateTime.UtcNow)
				return false;

			return await Reset();
		}

		/// <summary>
		/// Resets the doc regardless if <see cref="ExpiresOn"/> has expired yet.
		/// Note, this still *does* do a conditional get with the <see cref="ETag"/>
		/// (where the ETag has to be set and Doc has to be not nulle). To ignore that
		/// as well set <paramref name="ignoreEtag"/> to true.
		/// </summary>
		/// <param name="docUrl">The url to download. Enter this to override
		/// the current <see cref="DocUrl"/>.</param>
		/// <param name="ignoreEtag">Set to true to ignore the etag and force
		/// a full download no matter what.</param>
		/// <returns>True if a full reset occurred, false if conditional get
		/// determined current document was already up to date.</returns>
		public async Task<bool> Reset(string docUrl = null, bool ignoreEtag = false)
		{
			if (docUrl.IsNulle())
				docUrl = DocUrl.NullIfEmptyTrimmed();

			if (docUrl.IsNulle())
				throw new ArgumentException($"{nameof(DocUrl)} was null.");

			if (!Uri.TryCreate(docUrl, UriKind.Absolute, out Uri uriResult))
				throw new ArgumentException($"{nameof(DocUrl)} was not a valid URI.");

			string etag = ignoreEtag || Doc.IsNulle() // never even set etag if doc is null
				? null
				: ETag;

			string _doc = null;

			var notModProps = new HttpNotModifiedProperties() { ETag = etag };

			var httpRespInfo = await XHttp.GetAsync(
				DocUrl,
				settings: notModProps,
				timeout: TimeSpan.FromSeconds(10));

			if (httpRespInfo.NotModified)
				return false; // NotModified can never be true if we didn't send in the etag, but...

			_doc = httpRespInfo.Failed
				? null
				: httpRespInfo.GetContentString();

			bool isValid = ValidateDownload(_doc);
			if (!isValid)
				throw new Exception("Downloaded string is null or invalid.");

			Doc = _doc;

			this.ETag = httpRespInfo.ETag;

			LastUpdated = DateTimeOffset.UtcNow;

			return true;
		}

	}
}
