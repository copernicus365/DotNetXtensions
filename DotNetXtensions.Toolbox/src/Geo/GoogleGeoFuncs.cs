using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DotNetXtensions.Geo
{
	public static class GoogleGeoFuncs
	{
		public static string UrlBase_GoogleApiGeocode = "http://maps.googleapis.com/maps/api/geocode/";
		public static string UrlBase_GoogleApiTimeZone = "https://maps.googleapis.com/maps/api/timezone/";



		public static string Url_GoogleApiGeocode(string addressQueryUrlEncodedValue, bool getXmlNotJson = true)
		{
			string val = $"{UrlBase_GoogleApiGeocode}{(getXmlNotJson ? "xml" : "json")}?address={addressQueryUrlEncodedValue}&sensor=true";

			//string vl2 = string.Format("{0}{1}?address={2}&sensor=true",
			//	UrlBase_GoogleApiGeocode,
			//	(getXmlNotJson ? "xml" : "json"),
			//	addressQueryUrlEncodedValue);

			return val;
		}

		public static string Url_GoogleApiTimeZone(decimal lat, decimal lng, bool getXmlNotJson = true)
		{
			// ?? what was 'timestamp=1331161200' ??
			string val = $"{UrlBase_GoogleApiTimeZone}{(getXmlNotJson ? "xml" : "json")}?location={lat},{lng}&timestamp=1331161200&sensor=true";
			return val;
		}



		/// <summary>
		/// This parses the xml string response from Url_GoogleApiGeocode
		/// and gets the lat/long from it into the out parameters
		/// (and returns the XML document if needed for further values).
		/// </summary>
		public static bool TryParseGoogleGeocodeResponseXML(string xmlString, out decimal lat, out decimal lng, out XElement xml)
		{
			lat = 0;
			lng = 0;
			xml = null;
			try {
				xml = XElement.Parse(xmlString);
				if (xml.Name != "GeocodeResponse")
					return false;

				XElement location = xml.Element("result")
					.Element("geometry")
					.Element("location");

				lat = (decimal)location.Element("lat");
				lng = (decimal)location.Element("lng");

				if (lat == 0 && lng == 0)
					return false;
			}
			catch {
				return false;
			}
			return true;
		}

		public static bool TryParseGoogleTimeZoneResponseXML(string xmlStringFromGoogle, out string tzid, out XElement xml)
		{
			// xmlStringFromGoogle should look like this:
			/*
				<TimeZoneResponse>
				<status>OK</status>
				<raw_offset>-28800.0000000</raw_offset>
				<dst_offset>0.0000000</dst_offset>
				<time_zone_id>America/Los_Angeles</time_zone_id>
				<time_zone_name>Pacific Standard Time</time_zone_name>
				</TimeZoneResponse>
			*/

			tzid = null;
			xml = null;
			try {
				xml = XElement.Parse(xmlStringFromGoogle, LoadOptions.None);
			}
			catch (Exception ex) {
				return false;
			}

			if (xml.Name != "TimeZoneResponse")
				return false;

			tzid = (string)xml.Element("time_zone_id");
			if (tzid.IsNulle())
				return false;

			return true;
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
