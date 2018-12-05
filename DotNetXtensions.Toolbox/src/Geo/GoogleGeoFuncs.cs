using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DotNetXtensions.Geo.Google;

namespace DotNetXtensions.Geo
{
	public class GoogleGeoFuncs
	{
		private readonly string _apiKey;
		private readonly bool _json = true;

		public GoogleGeoFuncs(string apiKey)
		{
			_apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
		}

		public static string UrlBase_GoogleApiGeocode = "https://maps.googleapis.com/maps/api/geocode/";
		public static string UrlBase_GoogleApiTimeZone = "https://maps.googleapis.com/maps/api/timezone/";

		// https://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&key=AIzaSyBfbPC74G1iBXYzhYIMawvBq6Z-C4cjZXY

		// https://maps.googleapis.com/maps/api/timezone/json?location=38.908133,-77.047119&timestamp=1458000000&key=YOUR_API_KEY

		
		public string GoogleApiGeocodeUrl(string addressQueryUrlEncodedValue)
		{
			string val = $"{UrlBase_GoogleApiGeocode}{_apiGetTyp}?address={addressQueryUrlEncodedValue}&sensor=true&key={_apiKey}";
			return val;
		}

		string _apiGetTyp => _json ? "json" : "xml";

		public string GoogleApiTimeZoneUrl(decimal lat, decimal lng, bool getXmlNotJson = true)
		{
			// ?? what was 'timestamp=1331161200' ??
			string val = $"{UrlBase_GoogleApiTimeZone}{_apiGetTyp}?location={lat},{lng}&timestamp=1331161200&sensor=true&key={_apiKey}";
			return val;
		}

		public bool TryParseGoogleGeocodeResponseJson(
			string jsonStr,
			out decimal lat,
			out decimal lng)
			=> TryParseGoogleGeocodeResponseJson(jsonStr, out lat, out lng, out GoogleGeocodeInfoResponse resp);

		public bool TryParseGoogleGeocodeResponseJson(
			string jsonStr, 
			out decimal lat, 
			out decimal lng,
			out GoogleGeocodeInfoResponse resp)
		{
			lat = lng = 0;
			resp = null;

			try {
				resp = jsonStr.DeserializeJson<GoogleGeocodeInfoResponse>();

				var r = resp?.results?.FirstOrDefault();

				var latLong = r?.geometry?.location;

				if (r == null || latLong == null)
					return false;

				lat = latLong.lat;
				lng = latLong.lng;

				if (lat == 0 && lng == 0)
					return false;

				return true;
			}
			catch {
			}
			return false;
		}

		public bool TryParseGoogleTimeZoneResponseJson(
			string jsonStr, 
			out string tzid)
			=> TryParseGoogleTimeZoneResponseJson(jsonStr, out tzid, out GoogleTimeZoneInfo resp);

		public bool TryParseGoogleTimeZoneResponseJson(
			string jsonStr, 
			out string tzid, 
			out GoogleTimeZoneInfo resp)
		{
			tzid = null;
			resp = null;

			try {
				resp = jsonStr.DeserializeJson<GoogleTimeZoneInfo>();

				tzid = resp?.timeZoneId?.NullIfEmptyTrimmed();

				if (tzid == null)
					return false;

				return true;
			}
			catch {
			}
			return false;
		}


		/// <summary>
		/// Combines an array of address strings into a url encoded string,
		/// with address parts separated by comma.
		/// http://maps.googleapis.com/maps/api/geocode/json?address=1600+Amphitheatre+Parkway,+Mountain+View,+CA&amp;sensor=true_or_false
		/// 
		/// http://maps.googleapis.com/maps/api/geocode/xml?address=2931+Ravogli+Ave+Cincinnati+Ohio+45211&amp;sensor=true
		/// </summary>
		public static string CombineAddressPartsToUrlEncodedString(
			Func<string, string> urlEncode,
			params string[] addressValues)
		{
			if (addressValues.IsNulle())
				return ""; // if none, return empty string

			string value = addressValues
				.Where(s => s.NotNulle()) // winnow out any empty parts
				.JoinToString(", "); // comma separate address parts

			// to url encode, we could do for each address element,
			// but more performant to just do as one already combined sting
			// and then just unencoded the encoded commas
			string finalValue =
				urlEncode(value) // System.Web.HttpUtility.UrlEncode(value)
				.Replace("%2c", ",");

			return finalValue;
		}

	}
}
