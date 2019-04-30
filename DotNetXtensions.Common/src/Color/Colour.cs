// Copyright (c) Sherif Elmetainy (Code Art). 
// Licensed under the MIT License, See License.txt in the repository root for license information.

// Nick: source: https://github.com/sherif-elmetainy/DotnetGD/blob/master/src/CodeArt.DotnetGD/Color.cs
// Code has been altered by me (Nicholas Petersen) in a few important places, but
// will have to code compare to remember exactly what. Those I think were some important
// contributions, but the bulk of the code is still untouched from above (Sherif)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DotNetXtensions // original namespace was: namespace CodeArt.DotnetGD
{
	/// <summary>
	/// Color structure (has similarities to the one used in System.Drawing). 
	/// The color structure is immutable and holds a 32-bit unsigned interger color value with 8 bits for each of the alpha, red, green and blue.
	/// </summary>
	/// <remarks>
	/// In libgd true color is represented as 32 bit signed integer. The sign bit is ignored and always zero.
	/// Also in libgd alpha = 0 means opaque while alpha = 0x7f means transparent. 
	/// This Color structure has alpha=0 means transparent and alpha = 0xff means opaque. 
	/// The reason behind the difference is that most .NET developers are familiar with Color structure in System.Drawing
	/// and also because in HTML alpha=0xff also means opaque. Since the target audience of this wrapper is most likely developing
	/// web application for ASP.NET 5 and/or a .NET developer familiar Color in system.Drawing, I choose to make it different than libgd 
	/// and have the library internally convert the alpha part.
	/// 
	/// The above is only relevant in because of the following:
	/// 1- There is a small loss in alpha resolution cause by libgd (it's 7 bits instead of 8 bits)
	/// 2- When accessing image raw pointer, colors will not work like this structurce and will rather be libgd true color in case of truecolor image
	/// and color index in case of non true color images.
	/// </remarks>
	public struct Colour : IEquatable<Colour>, IStringSerializable<Colour>
	{
		private const byte AlphaOpaque = 0xff;


		/// <summary>
		/// 32-bit color value with 8 bits used for each of the components (aarrggbb)
		/// </summary>
		public uint Argb { get; }

		public uint Rgb { get; }



		/// <summary>
		/// The Alpha component of the color. 0xff is fully opaque, 0x00 is fully transparent.
		/// </summary>
		public byte A => (byte)((Argb & 0xFF000000) >> 24);

		/// <summary>
		/// The Red component of the color
		/// </summary>
		public byte R => (byte)((Argb & 0x00FF0000) >> 16);

		/// <summary>
		/// The green component of the color
		/// </summary>
		public byte G => (byte)((Argb & 0x0000FF00) >> 8);

		/// <summary>
		/// The blue component of the color
		/// </summary>
		public byte B => (byte)(Argb & 0x000000FF);



		#region --- CONSTRUCTORS ---

		/// <summary>
		/// Initializes a new instance of Color
		/// </summary>
		/// <param name="argb">32-bit color value with 8 bits used for each of the components (aarrggbb)</param>
		public Colour(uint argb)
		{
			Argb = argb;
			Rgb = Argb & noAlpha;
			//A = (byte)((Argb & 0xFF000000) >> 24);
			//R = (byte)((Argb & 0x00FF0000) >> 16);
			//G = (byte)((Argb & 0x0000FF00) >> 8);
			//B = (byte)(Argb & 0x000000FF);
		}
		const uint noAlpha = 0b00000000_11111111_11111111_11111111;

		/// <summary>
		/// Initializes a new instance of Color
		/// </summary>
		/// <param name="a">alpha component</param>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public Colour(byte a, byte r, byte g, byte b)
		{
			// It's far more performant to have this separate final constructor as ARGB values are already known
			// and don't need to be computed
			Argb = ((r & 0xffu) << 16) | ((g & 0xffu) << 8) | (b & 0xffu) | ((a & (uint)AlphaOpaque) << 24);
			Rgb = Argb & noAlpha;
			//A = a;
			//R = r;
			//G = g;
			//B = b;
		}

		/// <summary>
		/// Initializes a new instance of Color from red, green and blue. Alpha component is considered apaque (0xff).
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public Colour(byte r, byte g, byte b) 
			: this(AlphaOpaque, r, g, b) { }


		/// <summary>
		/// Initializes a new instance of Color from red, green and blue. Alpha component is considered apaque (0xff).
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public Colour(long r, long g, long b)
			: this(AlphaOpaque, (byte)r, (byte)g, (byte)b) { }

		/// <summary>
		/// Initializes a new instance of Color from red, green and blue. Alpha component is considered apaque (0xff).
		/// </summary>
		/// <param name="r">red component</param>
		/// <param name="g">green component</param>
		/// <param name="b">blue component</param>
		public Colour(ulong r, ulong g, ulong b) 
			: this(AlphaOpaque, (byte)r, (byte)g, (byte)b) { }
		
		/// <summary>
		/// Initializes a new instance of color from a string.
		/// </summary>
		/// <param name="htmlColor">html color can be either known color name (such as "red", "yellow", "darkblue", etc), 
		/// "#aarrggbb" as in html (# is required), "#rrggbb" (# is required, alpha is considered ff) or #rgb </param>
		public Colour(string htmlColor)
		{
			this = ColourNameConverter.FromHtmlColor(htmlColor);
		}

		/// <summary>
		/// Initializes the color from hue, saturation and brightness.
		/// </summary>
		/// <param name="hue">The angle in degrees around the axis of colors cylinder in the RGB color model. Valid values are from 0 inclusive to 360 exclusive.</param>
		/// <param name="saturation">The distance from the center of the cylinder. Valid values from 0 inclusive to 1 inclusive.</param>
		/// <param name="brightness">Brightness or lightness. Valid values from 0 inclusive to 1 inclusive.</param>
		public Colour(float hue, float saturation, float brightness)
		{
			if (saturation < 0 || saturation > 1) throw new ArgumentOutOfRangeException(nameof(saturation), saturation, "Saturation must be between 0 and 1.");
			if (brightness < 0 || brightness > 1) throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Brightness must be between 0 and 1.");
			if (hue < 0 || hue >= 360) throw new ArgumentOutOfRangeException(nameof(hue), hue, "Brightness must be between 0 and 360.");
			var c = (1 - Math.Abs(2 * brightness - 1)) * saturation;
			var x = c * (1 - Math.Abs((int)Math.Round(hue) / 60 % 2 - 1));
			var m = brightness - c / 2;
			float r, g, b;
			if (hue < 60) {
				r = c;
				g = x;
				b = 0;
			}
			else if (hue < 120) {
				r = x;
				g = c;
				b = 0;
			}
			else if (hue < 180) {
				r = 0;
				g = c;
				b = x;
			}
			else if (hue < 240) {
				r = 0;
				g = x;
				b = c;
			}
			else if (hue < 300) {
				r = x;
				g = 0;
				b = c;
			}
			else {
				r = c;
				g = 0;
				b = x;
			}
			this = new Colour((byte)Math.Round((r + m) * 255), (byte)Math.Round((g + m) * 255), (byte)Math.Round((b + m) * 255));
		}

		#endregion



		/// <summary>
		/// Compares Color to another object.
		/// </summary>
		/// <param name="obj">object to compare.</param>
		/// <returns>True if obj is a Color object having the same value, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			return (obj as Colour?)?.Argb == Argb;
		}

		/// <summary>
		/// Gets hashcode of the color object.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return unchecked((int)Argb);
		}

		/// <summary>
		/// Compares Color to another object.
		/// </summary>
		/// <param name="other">color to compare</param>
		/// <returns>True if the colors have the same value, false otherwise.</returns>
		public bool Equals(Colour other)
		{
			return other.Argb == Argb;
		}

		///// <summary>
		///// String representation of the color (in format #aarrggbb as in HTML)
		///// </summary>
		///// <returns> String representation of the color (in format #aarrggbb as in HTML)</returns>
		//public override string ToString()
		//{
		//	return $"#{Argb:x8}";
		//}

		public override string ToString()
		{
			bool doAlpha = A != byte.MaxValue;
			string val = doAlpha
				? $"#{Argb:x8}"
				: $"#{Rgb:x6}";
			return val;
		}


		#region --- IStringSerializable ---

		public string Serialize()
		{
			return ToString();
		}

		public Colour Deserialize(string htmlColor)
		{
			return ColourNameConverter.FromHtmlColor(htmlColor);
		}

		object IStringSerializable.Deserialize(string htmlColor)
		{
			return Deserialize(htmlColor);
		}

		#endregion




		/// <summary>
		/// Color name such as "red", "green", "yellow", is the color is a known value, #aarrggbb otherwise.
		/// </summary>
		public string Name => ColourNameConverter.GetName(this);

		/// <summary>
		/// compares 2 colors for equality
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator ==(Colour c1, Colour c2)
		{
			return c1.Equals(c2);
		}

		/// <summary>
		/// compares 2 colors for inquality.
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool operator !=(Colour c1, Colour c2)
		{
			return !c1.Equals(c2);
		}




		#region --- COLOURS ---

		public static Colour Transparent => new Colour(0x00ffffff);
		public static Colour AliceBlue => new Colour(0xfff0f8ff);
		public static Colour AntiqueWhite => new Colour(0xfffaebd7);
		public static Colour Aqua => new Colour(0xff00ffff);
		public static Colour Aquamarine => new Colour(0xff7fffd4);
		public static Colour Azure => new Colour(0xfff0ffff);
		public static Colour Beige => new Colour(0xfff5f5dc);
		public static Colour Bisque => new Colour(0xffffe4c4);
		public static Colour Black => new Colour(0xff000000);
		public static Colour BlanchedAlmond => new Colour(0xffffebcd);
		public static Colour Blue => new Colour(0xff0000ff);
		public static Colour BlueViolet => new Colour(0xff8a2be2);
		public static Colour Brown => new Colour(0xffa52a2a);
		public static Colour BurlyWood => new Colour(0xffdeb887);
		public static Colour CadetBlue => new Colour(0xff5f9ea0);
		public static Colour Chartreuse => new Colour(0xff7fff00);
		public static Colour Chocolate => new Colour(0xffd2691e);
		public static Colour Coral => new Colour(0xffff7f50);
		public static Colour CornflowerBlue => new Colour(0xff6495ed);
		public static Colour Cornsilk => new Colour(0xfffff8dc);
		public static Colour Crimson => new Colour(0xffdc143c);
		public static Colour Cyan => new Colour(0xff00ffff);
		public static Colour DarkBlue => new Colour(0xff00008b);
		public static Colour DarkCyan => new Colour(0xff008b8b);
		public static Colour DarkGoldenrod => new Colour(0xffb8860b);
		public static Colour DarkGray => new Colour(0xffa9a9a9);
		public static Colour DarkGreen => new Colour(0xff006400);
		public static Colour DarkKhaki => new Colour(0xffbdb76b);
		public static Colour DarkMagenta => new Colour(0xff8b008b);
		public static Colour DarkOliveGreen => new Colour(0xff556b2f);
		public static Colour DarkOrange => new Colour(0xffff8c00);
		public static Colour DarkOrchid => new Colour(0xff9932cc);
		public static Colour DarkRed => new Colour(0xff8b0000);
		public static Colour DarkSalmon => new Colour(0xffe9967a);
		public static Colour DarkSeaGreen => new Colour(0xff8fbc8b);
		public static Colour DarkSlateBlue => new Colour(0xff483d8b);
		public static Colour DarkSlateGray => new Colour(0xff2f4f4f);
		public static Colour DarkTurquoise => new Colour(0xff00ced1);
		public static Colour DarkViolet => new Colour(0xff9400d3);
		public static Colour DeepPink => new Colour(0xffff1493);
		public static Colour DeepSkyBlue => new Colour(0xff00bfff);
		public static Colour DimGray => new Colour(0xff696969);
		public static Colour DodgerBlue => new Colour(0xff1e90ff);
		public static Colour Firebrick => new Colour(0xffb22222);
		public static Colour FloralWhite => new Colour(0xfffffaf0);
		public static Colour ForestGreen => new Colour(0xff228b22);
		public static Colour Fuchsia => new Colour(0xffff00ff);
		public static Colour Gainsboro => new Colour(0xffdcdcdc);
		public static Colour GhostWhite => new Colour(0xfff8f8ff);
		public static Colour Gold => new Colour(0xffffd700);
		public static Colour Goldenrod => new Colour(0xffdaa520);
		public static Colour Gray => new Colour(0xff808080);
		public static Colour Green => new Colour(0xff008000);
		public static Colour GreenYellow => new Colour(0xffadff2f);
		public static Colour Honeydew => new Colour(0xfff0fff0);
		public static Colour HotPink => new Colour(0xffff69b4);
		public static Colour IndianRed => new Colour(0xffcd5c5c);
		public static Colour Indigo => new Colour(0xff4b0082);
		public static Colour Ivory => new Colour(0xfffffff0);
		public static Colour Khaki => new Colour(0xfff0e68c);
		public static Colour Lavender => new Colour(0xffe6e6fa);
		public static Colour LavenderBlush => new Colour(0xfffff0f5);
		public static Colour LawnGreen => new Colour(0xff7cfc00);
		public static Colour LemonChiffon => new Colour(0xfffffacd);
		public static Colour LightBlue => new Colour(0xffadd8e6);
		public static Colour LightCoral => new Colour(0xfff08080);
		public static Colour LightCyan => new Colour(0xffe0ffff);
		public static Colour LightGoldenrodYellow => new Colour(0xfffafad2);
		public static Colour LightGreen => new Colour(0xff90ee90);
		public static Colour LightGray => new Colour(0xffd3d3d3);
		public static Colour LightPink => new Colour(0xffffb6c1);
		public static Colour LightSalmon => new Colour(0xffffa07a);
		public static Colour LightSeaGreen => new Colour(0xff20b2aa);
		public static Colour LightSkyBlue => new Colour(0xff87cefa);
		public static Colour LightSlateGray => new Colour(0xff778899);
		public static Colour LightSteelBlue => new Colour(0xffb0c4de);
		public static Colour LightYellow => new Colour(0xffffffe0);
		public static Colour Lime => new Colour(0xff00ff00);
		public static Colour LimeGreen => new Colour(0xff32cd32);
		public static Colour Linen => new Colour(0xfffaf0e6);
		public static Colour Magenta => new Colour(0xffff00ff);
		public static Colour Maroon => new Colour(0xff800000);
		public static Colour MediumAquamarine => new Colour(0xff66cdaa);
		public static Colour MediumBlue => new Colour(0xff0000cd);
		public static Colour MediumOrchid => new Colour(0xffba55d3);
		public static Colour MediumPurple => new Colour(0xff9370db);
		public static Colour MediumSeaGreen => new Colour(0xff3cb371);
		public static Colour MediumSlateBlue => new Colour(0xff7b68ee);
		public static Colour MediumSpringGreen => new Colour(0xff00fa9a);
		public static Colour MediumTurquoise => new Colour(0xff48d1cc);
		public static Colour MediumVioletRed => new Colour(0xffc71585);
		public static Colour MidnightBlue => new Colour(0xff191970);
		public static Colour MintCream => new Colour(0xfff5fffa);
		public static Colour MistyRose => new Colour(0xffffe4e1);
		public static Colour Moccasin => new Colour(0xffffe4b5);
		public static Colour NavajoWhite => new Colour(0xffffdead);
		public static Colour Navy => new Colour(0xff000080);
		public static Colour OldLace => new Colour(0xfffdf5e6);
		public static Colour Olive => new Colour(0xff808000);
		public static Colour OliveDrab => new Colour(0xff6b8e23);
		public static Colour Orange => new Colour(0xffffa500);
		public static Colour OrangeRed => new Colour(0xffff4500);
		public static Colour Orchid => new Colour(0xffda70d6);
		public static Colour PaleGoldenrod => new Colour(0xffeee8aa);
		public static Colour PaleGreen => new Colour(0xff98fb98);
		public static Colour PaleTurquoise => new Colour(0xffafeeee);
		public static Colour PaleVioletRed => new Colour(0xffdb7093);
		public static Colour PapayaWhip => new Colour(0xffffefd5);
		public static Colour PeachPuff => new Colour(0xffffdab9);
		public static Colour Peru => new Colour(0xffcd853f);
		public static Colour Pink => new Colour(0xffffc0cb);
		public static Colour Plum => new Colour(0xffdda0dd);
		public static Colour PowderBlue => new Colour(0xffb0e0e6);
		public static Colour Purple => new Colour(0xff800080);
		public static Colour Red => new Colour(0xffff0000);
		public static Colour RosyBrown => new Colour(0xffbc8f8f);
		public static Colour RoyalBlue => new Colour(0xff4169e1);
		public static Colour SaddleBrown => new Colour(0xff8b4513);
		public static Colour Salmon => new Colour(0xfffa8072);
		public static Colour SandyBrown => new Colour(0xfff4a460);
		public static Colour SeaGreen => new Colour(0xff2e8b57);
		public static Colour SeaShell => new Colour(0xfffff5ee);
		public static Colour Sienna => new Colour(0xffa0522d);
		public static Colour Silver => new Colour(0xffc0c0c0);
		public static Colour SkyBlue => new Colour(0xff87ceeb);
		public static Colour SlateBlue => new Colour(0xff6a5acd);
		public static Colour SlateGray => new Colour(0xff708090);
		public static Colour Snow => new Colour(0xfffffafa);
		public static Colour SpringGreen => new Colour(0xff00ff7f);
		public static Colour SteelBlue => new Colour(0xff4682b4);
		public static Colour Tan => new Colour(0xffd2b48c);
		public static Colour Teal => new Colour(0xff008080);
		public static Colour Thistle => new Colour(0xffd8bfd8);
		public static Colour Tomato => new Colour(0xffff6347);
		public static Colour Turquoise => new Colour(0xff40e0d0);
		public static Colour Violet => new Colour(0xffee82ee);
		public static Colour Wheat => new Colour(0xfff5deb3);
		public static Colour White => new Colour(0xffffffff);
		public static Colour WhiteSmoke => new Colour(0xfff5f5f5);
		public static Colour Yellow => new Colour(0xffffff00);
		public static Colour YellowGreen => new Colour(0xff9acd32);

		#endregion

	}

	/// <summary>
	/// Helper class for converting color to string and vice versa
	/// </summary>
	internal static class ColourNameConverter
	{
		/// <summary>
		/// Dictionary for mapping color names to colors
		/// </summary>
		private static readonly Dictionary<string, Colour> NameToColorDictionary = InitializeNameToColorDictionary();


		private static Dictionary<string, Colour> InitializeNameToColorDictionary()
		{
			var dict = new Dictionary<string, Colour>(StringComparer.OrdinalIgnoreCase);
			foreach (var property in typeof(Colour).GetProperties(BindingFlags.Public | BindingFlags.Static).Where(p => p.PropertyType == typeof(Colour))) {
				var color = (Colour)property.GetValue(null);
				var name = property.Name.ToLowerInvariant();
				dict.Add(name, color);
				// gray has 2 common spellings
				if (name.EndsWith("gray")) {
					dict.Add(name.Replace("gray", "grey"), color);
				}
			}

			return dict;
		}

		/// <summary>
		/// Dictionary to map colors to names
		/// </summary>
		private static readonly Dictionary<Colour, string> ColorToNameDictionary = InitializeColorToNameDictionary();


		private static Dictionary<Colour, string> InitializeColorToNameDictionary()
		{
			var dict = new Dictionary<Colour, string>();
			foreach (var property in typeof(Colour).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.PropertyType == typeof(Colour))) {
				var color = (Colour)property.GetValue(null);
				dict.Add(color, property.Name.ToLowerInvariant());
			}
			return dict;
		}

		/// <summary>
		/// Using HTML color get the Color struct
		/// </summary>
		/// <param name="htmlColor">valid values are names like "red", "orange", "darkgreen", etc, or html formats "#aarrggbb", "#rrggbb" or "#rgb".</param>
		/// <returns></returns>
		public static Colour FromHtmlColor(string htmlColor)
		{
			if (string.IsNullOrWhiteSpace(htmlColor))
				throw new ArgumentNullException(nameof(htmlColor));
			htmlColor = htmlColor.Trim();
			if (htmlColor[0] == '#') {
				// ReSharper disable once ConvertIfStatementToSwitchStatement
				if (htmlColor.Length == 9) // #aarrggbb
				{
					return new Colour(uint.Parse(htmlColor.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
				}
				if (htmlColor.Length == 7) // #rrggbb
				{
					return new Colour(0xff000000 | uint.Parse(htmlColor.Substring(1), NumberStyles.HexNumber, CultureInfo.InvariantCulture));
				}
				if (htmlColor.Length == 4) // #rgb
				{
					return new Colour(uint.Parse($"ff{htmlColor[1]}{htmlColor[1]}{htmlColor[2]}{htmlColor[2]}{htmlColor[3]}{htmlColor[3]}", NumberStyles.HexNumber, CultureInfo.InvariantCulture));
				}
			}
			Colour color;
			if (NameToColorDictionary.TryGetValue(htmlColor, out color)) {
				return color;
			}
			throw new ArgumentOutOfRangeException(nameof(htmlColor), htmlColor, "Invalid html color value.");
		}

		public static string GetName(Colour color)
		{
			string name;
			return ColorToNameDictionary.TryGetValue(color, out name) ? name : color.ToString();
		}
	}

}