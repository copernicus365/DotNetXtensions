using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetXtensions.Json
{

	/// <summary>
	/// Type that allows one to create instances of simple JsonConverters on the fly,
	/// without having to make a unique type (class), simply be setting the Serialize and 
	/// Deserialize Funcs. Internally doesn't do anything
	/// more than call writer.WriteValue within WriteJson.
	/// </summary>
	/// <typeparam name="T">Main Type of object.</typeparam>
	/// <typeparam name="TSerialized">The type of the serialized form (so should be 
	/// a standard base type like string, int, DateTime, etc).</typeparam>
	public class QuickJsonConverter<T, TSerialized> : JsonConverter
	{

		public QuickJsonConverter()
		{

		}

		/// <summary>
		/// Serializer. This will not be called if the input parameter T is null. 
		/// </summary>
		public virtual Func<T, TSerialized> Serialize { get; set; }

		/// <summary>
		/// Deserializer. This will not be called if the input parameter TSerialized is null.
		/// </summary>
		public virtual Func<TSerialized, T> Deserialize { get; set; }




		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			TSerialized serializedObj = value == null 
				? default(TSerialized) 
				: Serialize((T)value);
			writer.WriteValue(serializedObj);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object val = reader.Value;
			T obj = val == null 
				? default(T)
				: Deserialize((TSerialized)val);
			return obj;
		}

		public override bool CanConvert(Type objectType)
		{
			timesCanConvertHit++;
			bool canConv = objectType == _typeT || objectType == _typeTNullable;
			return canConv;
		}


		static Type _typeT = typeof(T);
		static Type _typeTNullable;
		static bool isNullableType;

		static QuickJsonConverter()
		{
			Type t1 = _typeT.GetUnderlyingTypeIfNullable();
			if(t1 != null) {
				isNullableType = true;
				_typeTNullable = _typeT;
				_typeT = t1;
			}
		}

		public static int timesCanConvertHit; // diagnostic...


	}
}
