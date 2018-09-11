using System;
using System.Threading.Tasks;

namespace DotNetXtensions
{
	public interface ICachedString
	{
		string Doc { get; set; }

		string DocUrl { get; set; }

		Task<bool> Reset(string docUrl = null, bool ignoreEtag = false);

		Task<bool> ResetIfNeeded(bool forceReset = false);

		DateTimeOffset ExpiresOn { get; }

		TimeSpan ExpiresAfter { get; set; }

		bool ValidateDownload(string doc);

	}
}
