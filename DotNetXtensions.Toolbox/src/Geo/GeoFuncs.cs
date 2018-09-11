using System;

namespace DotNetXtensions.Geo
{
	public static class GeoFuncs
	{
		const double PIx = 3.141592653589793;
		const double RADIO = 6378.16;
		const double MilesPerKilometer = 0.621371192;
		const double KilometersPerMile = 1609.344;



		/// <summary>
		/// Convert degrees to Radians
		/// </summary>
		/// <param name="x">Degrees</param>
		/// <returns>The equivalent in radians</returns>
		public static double Radians(double x)
		{
			return (x * PIx) / 180;
		}

		public static double GetDistanceInMiles(double lat1, double long1, double lat2, double long2)
		{
			return GetDistance(lat1, long1, lat2, long2) * MilesPerKilometer;
		}

		/// <summary>
		/// Calculate the distance between two places.
		/// </summary>
		/// <param name="long1"></param>
		/// <param name="lat1"></param>
		/// <param name="long2"></param>
		/// <param name="lat2"></param>
		/// <returns></returns>
		public static double GetDistance(double lat1, double long1, double lat2, double long2)
		{
			double dlon = Radians(long2 - long1);
			double dlat = Radians(lat2 - lat1);

			double a =
				(Math.Sin(dlat / 2) * Math.Sin(dlat / 2))
				+ Math.Cos(Radians(lat1))
				* Math.Cos(Radians(lat2))
				* (Math.Sin(dlon / 2)
				* Math.Sin(dlon / 2));

			double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

			return angle * RADIO;
		}


	}
}
