using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetXtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DotNetXtensions.Json
{
	/// <summary>
	/// A JsonConverter for Enums that are serialized to strings. This
	/// will ignore settings to serialize enums to numbers ... for that, just use the
	/// standard library converter (i.e. don't apply this converter): with numbers there is no need to produce custom naming
	/// for the enum values. Internally uses XEnum. XEnum allows you to alter or change the names
	/// associated with an enum value (see XEnum.SetNames). So this is powerful
	/// because it allows the outputted enum names to be set globally (for all that use XEnum
	/// naming at least), and because this converter uses the same, to have those names suddenly used
	/// in Json serialization of that enum type.
	/// </summary>
	/// <typeparam name="T">Type t</typeparam>
	public class StringXEnumConverter<T> 
		: QuickJsonConverter<T, string> where T : struct, IConvertible
	{
		//Func<T, int> toInt;
		//public StringXEnumConverter() : this(null) { }
		//public StringXEnumConverter(Func<T, int> toInt = null)
		//{
		//	if(toInt == null) //throw new ArgumentNullException();
		//		toInt = t => t.GetHashCode();
		//	this.toInt = toInt;
		//}

		public StringXEnumConverter()
		{
			base.Serialize = (t) => {
				string val = XEnum<T>.Name(t);
				return val;
			};
			base.Deserialize = (str) => {
				if (XEnum<T>.TryGetValueIgnoreCase(str, out T val))
					return val;
				throw new ArgumentOutOfRangeException($"Dictionary key {str}");
				//T val = XEnum<T>.ValueIgnoreCase(str);
			};
		}
	}

	public class StringXEnumConverterLowercase<T>
	: StringXEnumConverter<T> where T : struct, IConvertible
	{
		public StringXEnumConverterLowercase()
		{
			Serialize = t => {
				string val = XEnum<T>.Name(t).ToLower();
				return val;
			};
			Deserialize = str => {
				if (XEnum<T>.TryGetValueIgnoreCase(str, out T val))
					return val;
				throw new ArgumentOutOfRangeException($"Dictionary key {str}");
				//T val = XEnum<T>.ValueIgnoreCase(str);
			};
		}
	}

	public class StringXEnumConverterCamelCase<T>
		: StringXEnumConverter<T> where T : struct, IConvertible
	{
		public StringXEnumConverterCamelCase()
		{
			Serialize = t => {
				string val = XEnum<T>.Name(t).ToCamelCaseFromCodePascalCase();
				return val;
			};
			Deserialize = str => {
				if (XEnum<T>.TryGetValueIgnoreCase(str, out T val))
					return val;
				throw new ArgumentOutOfRangeException($"Dictionary key {str}");
				//T val = XEnum<T>.ValueIgnoreCase(str);
			};
		}
	}


}
