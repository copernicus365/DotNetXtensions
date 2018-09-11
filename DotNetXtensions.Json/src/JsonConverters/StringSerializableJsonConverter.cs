using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotNetXtensions.Json
{
	/// <summary>
	/// A JsonConverter that handles all <see cref="DotNetXtensions.IStringSerializable"/> types.
	/// The point of <see cref="IStringSerializable"/> is it means the type that implements it 
	/// says there is a way to serialize it to a string and deserialize it back from that string. 
	/// One can add this converter as one of the default converters and suddenly any
	/// types that implement <see cref="IStringSerializable"/> are properly handled. While your type 
	/// still must implement <see cref="IStringSerializable"/>, what is nice is that beyond that it
	/// needs no direct tie-in into Json stuff now, and, we can significantly cut down on loads of 
	/// custom JsonConverters.
	/// <para/>
	/// One major concern I have with the <see cref="Newtonsoft.Json"/> implementation of JsonConverters
	/// is the potential major drag from the CanConvert method. So internally we cache all types that ever 
	/// get sent to that method into a dictionary, recording (on the first hit only for that type) whether
	/// that type implemented <see cref="IStringSerializable"/> or not. Beyond the first hits per unique type,
	/// it's a single Dictionary lookup by Type as key, which is very performant. Again, not our fault this library
	/// does this this way, I feel it needs to globally record and cache which converters can handle which types,
	/// and not hit this thousands millions of times per converter all the time.
	/// <para/>
	/// We DO handle Nullable-T type (we detect this on the first hit of the type).
	/// It will be treated like the non-nullable type in terms of handling it, if
	/// the object is not null (which JsonConverter only hands to us as a boxed object anyways).
	/// </summary>
	public class StringSerializableJsonConverter : JsonConverter
	{
		// IMPORTANT to make this static. It is typical for a converter instance to be made every time,
		// which would greatly lessen the help of this high-performance routine, where we cache already looked
		// up types in a Dictionary. This would have to be done on every instance of the converter (it still
		// would greatly help performance for that instance though). Note that this type is not built 
		// customizable, it does it's thing for IStringSerializable types and them alone, there's no reason therefore
		// we can't cache these types
		static Dictionary<Type, CanConvertType> _canConvertTypesDict = new Dictionary<Type, CanConvertType>();

		public override bool CanConvert(Type t)
		{
			CanConvertType c;
			if(_canConvertTypesDict.TryGetValue(t, out c))
				return c.IsStringSerializable;

			c = CanConvertType.GetCanConvertType(t);

			_canConvertTypesDict[t] = c; // note 't' might still be the nullable type, if so write another dict key below of main type

			if(c.HadNullable)
				_canConvertTypesDict[c.type] = c;
			
			return c.IsStringSerializable;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string strVal = reader.Value as string;
			if(strVal == null)
				return null;

			var dobj = _canConvertTypesDict[objectType]
				.StringSerializableInstance
				.Deserialize(reader.Value as string);
			return dobj;

			//object obj = (reader.Value as string)?
			//	.Deserialize(existingValue as IStringSerializable);
			//return obj;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			string serializedObj = (value as IStringSerializable)?.Serialize();
			writer.WriteValue(serializedObj);
		}

		class CanConvertType
		{
			public Type type;
			public bool IsStringSerializable;
			public bool HadNullable;
			public IStringSerializable StringSerializableInstance;

			public static CanConvertType GetCanConvertType(Type t)
			{
				if(t == null) return null;

				bool hadNullable = false;
				Type t1 = t.GetUnderlyingTypeIfNullable();

				if(t1 != null) {
					// The input type is Nullable-T, reset t to the generic type it wraps. 
					// The caller of this method will still have the original nullable type itself
					// and will set both types as entries in the Dictionary both pointing to this
					// new CanConvertType instance
					t = t1;
					hadNullable = true;
				}

				var c = new CanConvertType() {
					type = t,
					IsStringSerializable = typeof(IStringSerializable).IsAssignableFrom(t),
					HadNullable = hadNullable
				};

				if(c.IsStringSerializable)
					c.StringSerializableInstance = Activator.CreateInstance(t) as IStringSerializable;

				return c;
			}
		}

	}

}
