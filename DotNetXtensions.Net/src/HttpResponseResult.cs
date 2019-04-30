
namespace DotNetXtensions.Net
{
	public enum HttpResponseResult
	{
		None = 0,
		Success = 20,
		NotModified = 30,
		Fail = 40,
		TimeOut = 50,
	}

	public static class HttpResponseResultX
	{
		/// <summary>
		/// Indicates if value is Fail or TimeOut.
		/// </summary>
		public static bool IsFailAny(this HttpResponseResult val)
		{
			return val == HttpResponseResult.Fail || val == HttpResponseResult.TimeOut;
		}

		/// <summary>
		/// Indicates if value is Success or NotModified.
		/// </summary>
		public static bool IsSuccessOrNotModified(this HttpResponseResult val)
		{
			switch(val) {
				case HttpResponseResult.Success:
				case HttpResponseResult.NotModified:
					return true;
			}
			return false;
		}

	}
}
