using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DotNetXtensions.Geo.Google
{
	/// <summary>
	/// https://developers.google.com/maps/documentation/timezone/start
	/// </summary>
	public class GoogleTimeZoneInfo
	{
		public int dstOffset { get; set; }
		public int rawOffset { get; set; }
		public string status { get; set; }
		public string timeZoneId { get; set; }
		public string timeZoneName { get; set; }
	}

	/// <summary>
	/// https://developers.google.com/maps/documentation/geocoding/start
	/// </summary>
	public class GoogleGeocodeInfoResponse
	{
		public GoogleGeocodeInfo[] results { get; set; }
		public string status { get; set; }
	}

	public class GoogleGeocodeInfo
	{
		public AddressComponents[] address_components { get; set; }
		public string formatted_address { get; set; }
		public Geometry geometry { get; set; }
		public string place_id { get; set; }
		public string[] types { get; set; }
	}

	public class Geometry
	{
		public LatLong location { get; set; }
		public string location_type { get; set; }
		public Viewport viewport { get; set; }
	}

	public class LatLong
	{
		public decimal lat { get; set; }
		public decimal lng { get; set; }
	}

	public class Viewport
	{
		public LatLong northeast { get; set; }
		public LatLong southwest { get; set; }
	}

	public class AddressComponents
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public string[] types { get; set; }
	}
}
