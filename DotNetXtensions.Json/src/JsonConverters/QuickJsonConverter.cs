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
			bool canConv = objectType == TypeT || objectType == TypeTNullable;
			return canConv;
		}


		public static readonly Type TypeT = typeof(T);
		public static readonly Type TypeTNullable;
		public static readonly bool IsNullableType;

		static QuickJsonConverter()
		{
			Type t1 = TypeT.GetUnderlyingTypeIfNullable();
			if(t1 != null) {
				IsNullableType = true;
				TypeTNullable = TypeT;
				TypeT = t1;
			}
		}

		public static int timesCanConvertHit; // diagnostic...


	}
}
