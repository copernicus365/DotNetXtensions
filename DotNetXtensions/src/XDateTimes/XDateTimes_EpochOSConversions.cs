// XDateTimes_EpochOSConversions

namespace DotNetXtensions
{
	public static partial class XDateTimes
	{
		/// <summary>
		/// Java / Unix time since Epoch converted to .NET ticks,
		/// see <see cref="JavaUnixTimeToDNetTicks(long)"/> and
		/// <see cref="SecondsSinceEpochToDNetTicks"/>.
		/// </summary>
		public const long TICKS_AT_EPOCH = 621355968000000000L;

		/// <summary>
		/// Converts Java Unix Time (milliseconds since Epoch) to
		/// .NET ticks (java.util.Date/Calendar, date.getTime()).
		/// From http://stackoverflow.com/a/29668694/264031
		/// </summary>
		/// <param name="value">Java Unix time since Epoch in milliseconds.</param>
		public static long JavaUnixTimeToDNetTicks(this long value)
			=> TICKS_AT_EPOCH + (value * 10000);

		public static long SecondsSinceEpochToDNetTicks(this long value)
			=> TICKS_AT_EPOCH + (value * 10000000);

		//new DateTime(1970, 1, 1) + TimeSpan.FromSeconds(issuedAt_unixTimeInSecs) 
	}
}
