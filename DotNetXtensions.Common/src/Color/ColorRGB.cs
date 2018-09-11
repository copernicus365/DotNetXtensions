using System;
using System.Collections.Generic;
using System.Globalization;

namespace DotNetXtensions
{
	public struct ColorRGB
	{

		public byte R { get; private set; }

		public byte G { get; private set; }

		public byte B { get; private set; }

		public string HexColor { get; private set; }

		public int DecimalHexColor { get; private set; }


		// --- Calculated Values ---

		/// <summary>
		/// http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color
		/// Formula calculated here thanks to Brian Suda: http://24ways.org/2010/calculating-color-contrast/
		/// </summary>
		public byte Luminance { get; private set; }

		/// <summary>
		/// https://en.wikipedia.org/wiki/YIQ
		/// </summary>
		public byte YIC { get; private set; }

		public bool IsDark { get; private set; }



		/// <summary>
		/// (Source: I (Nicholas Petersen) converted this over from a hodge podge of
		/// JavaScript code I had gotten from other sources and was using for some years. 
		/// This article was influential: https://24ways.org/2010/calculating-color-contrast/)
		/// </summary>
		/// <param name="hex"></param>
		public ColorRGB(string hex)
		{
			HexColor = hex = FixAndBasicValidateHexColorString(hex);

			// get the colors from each two digits, first way only has to parse hex string once so is pry more performant

			DecimalHexColor = Int32.Parse(hex, NumberStyles.HexNumber);

			if(hex.ToUpper() != DecimalHexColor.ToString("X"))
				throw new ArgumentException();

			R = (byte)((DecimalHexColor >> 16) & byte.MaxValue /*255*/);
			G = (byte)((DecimalHexColor >> 8) & byte.MaxValue);
			B = (byte)(DecimalHexColor & byte.MaxValue);

			// or simply substring every two digits
			//R = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
			//G = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
			//B = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);

			// NOTE: in my calculation the highest either Luminance or YIC can be is 255, so I'm keeping them a byte

			Luminance = (byte)(0.2126 * R + 0.7152 * G + 0.0722 * B); // per ITU-R BT.709;

			YIC = (byte)(((R * 299) + (G * 587) + (B * 114)) / 1000);

			// not sure where I got this, but i had this note originally: "uses both luma and yic, gives much better results"
			IsDark = YIC <= 128 || Luminance < 40;
		}


		public static string FixAndBasicValidateHexColorString(string hex)
		{
			if(hex.IsNulle()) throw new ArgumentNullException();
			if(hex[0] == '#') hex = hex.Substring(1); // remove initial '#' if present
			if(hex.Length == 3)
				hex = hex + hex; // this is only valid, I think, if all chars are the same char, but not going to check
			if(hex.Length != 6) throw new ArgumentOutOfRangeException();
			return hex;
		}


		public override string ToString()
		{
			return $"#{HexColor} [{DecimalHexColor + "]",-20} R:{R,-5} G:{G,-5} B:{B,-5} Dark:{(IsDark ? "T" : "F"),-5} Lum:{Luminance,-5} YIC:{YIC,-5}";
		}

	}
}
