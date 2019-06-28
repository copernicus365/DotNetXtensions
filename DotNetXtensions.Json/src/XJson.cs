using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetXtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Linq;
using Newtonsoft.Json.Serialization;
using DotNetXtensions.Json;

namespace DotNetXtensions
{
	/// <summary>
	/// (NOTE: In future should we combine this with <see cref="JSON"/> class? There's pros and cons
	/// to each. The stuff here is a lot more technical and not about simply LINQ constructing. Cogitating...)
	/// </summary>
#if !DNXPrivate
	public
#endif
	static class XJson
	{

		#region --- ToJson ---

		public static string ToJson(
			this object obj,
			bool indent = false,
			bool enumsAsString = false,
			DefaultValueHandling? defValueHandling = null,
			bool treatAsList = false,
			bool camelCase = false,
			bool? camelCaseEnumText = null,
			bool? singleQuotes = null)
		{
			if(treatAsList)
				obj = new object[] { obj };

			var settings = (JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings())
				.IndentJson(indent)
				.CamelCase(camelCase)
				.EnumsAsStrings(enumsAsString, camelCaseEnumText ?? camelCase);

			if(defValueHandling != null)
				settings.DefaultValueHandling(defValueHandling.Value);

			bool useCustomWriter = settings.Formatting != Formatting.None || singleQuotes == true;
			string jsn = useCustomWriter
				? _customSerializeJson(obj, settings, singleQuotes: singleQuotes)
				: JsonConvert.SerializeObject(obj, settings);

			return jsn;
		}

		public static string ToJson(
			this object obj, 
			JsonSerializerSettings settings, 
			bool? indent = null, 
			bool? treatAsList = null,
			bool? singleQuotes = null)
		{
			if(treatAsList == true)
				obj = new object[] { obj };

			// do NOT set this on the settings instance! Allow this to override it which thankfully the JsonConvert.SerializeObject call below allows
			Formatting formatting = settings.Formatting;
			if(indent != null)
				formatting = indent == true ? Formatting.Indented : Formatting.None;

			bool useCustomWriter = formatting != Formatting.None || singleQuotes == true;

			string jsn;
			if(useCustomWriter) {
				settings.Formatting = formatting;
				jsn = _customSerializeJson(obj, settings, singleQuotes: singleQuotes);
			}
			else
				jsn = JsonConvert.SerializeObject(obj, Formatting.None, settings);

			return jsn;
		}

		static string _customSerializeJson(object obj, JsonSerializerSettings settings, bool? singleQuotes)
		{
			using(var sw = new StringWriter())
			using(var jw = new JsonTextWriter(sw)) {
				if(settings.Formatting == Formatting.Indented) {
					jw.Formatting = Formatting.Indented;
					jw.IndentChar = '	';
					jw.Indentation = 1;
				}

				if(singleQuotes == true)
					jw.QuoteChar = '\'';

				var js = JsonSerializer.Create(settings);

				js.Serialize(jw, obj);
				sw.Flush();
				string val = sw.ToString();
				return val;
			}

		}

		#endregion


		#region --- Settings Helper Methods ---

		public static JsonSerializerSettings CamelCase(this JsonSerializerSettings settings, bool camelCase = true)
		{
			if(settings != null) {
				if(camelCase)
					settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				else {
					IContractResolver r = settings.ContractResolver;
					if(r != null && r is CamelCasePropertyNamesContractResolver)
						r = null;
				}
			}
			return settings;
		}

		public static JsonSerializerSettings IndentJson(this JsonSerializerSettings settings, bool indent = true)
		{
			if(settings != null) {
				settings.Formatting = indent == true
					? Formatting.Indented
					: Formatting.None;
			}
			return settings;
		}

		public static JsonSerializerSettings DefaultValueHandling(this JsonSerializerSettings settings, DefaultValueHandling defValueHandling)
		{
			if(settings != null)
				settings.DefaultValueHandling = defValueHandling;
			return settings;
		}

		/// <summary>
		/// Makes enums to be serialized as string values rather than by their numeric value
		/// by adding a StringEnumConverter to the Converters if not already present.
		/// </summary>
		public static JsonSerializerSettings EnumsAsStrings(
			this JsonSerializerSettings settings,
			bool enumsAsStrings = true,
			bool camelCaseEnumText = false)
		{
			if(settings != null) {
				var list = settings.Converters;
				if(enumsAsStrings) {
					if(list == null)
						list = settings.Converters = new List<JsonConverter>();

					var enumConv = list.FirstN(c => c is StringEnumConverter) as StringEnumConverter;
					if(enumConv == null) {
						enumConv = new StringEnumConverter();
						list.Add(enumConv);
					}
#pragma warning disable CS0618 // Type or member is obsolete
					enumConv.CamelCaseText = camelCaseEnumText;
#pragma warning restore CS0618 // Type or member is obsolete
				}
				else if(list.NotNulle()) {
					int foundIdx = list.FindIndex(c => c is StringEnumConverter);
					if(foundIdx >= 0)
						list.RemoveAt(foundIdx);
				}
			}
			return settings;
		}

		public static List<JsonConverter> RemoveConverters(this JsonSerializerSettings settings)
		{
			if(settings == null || settings.Converters.IsNulle())
				return null;
			List<JsonConverter> convertersList = settings.Converters?.ToList();
			settings.Converters = null;
			return convertersList;
		}





		public static JsonSerializerSettings AddConverter(this JsonSerializerSettings settings, JsonConverter converter)
		{
			if(converter != null) {
				if(settings.Converters == null)
					settings.Converters = new List<JsonConverter>();
				settings.Converters.Add(converter);
			}
			return settings;
		}


		public static JsonSerializerSettings AddConvertersFirst(this JsonSerializerSettings settings, params JsonConverter[] converters)
		{
			if(converters.NotNulle()) {
				var list = (settings.Converters as List<JsonConverter>) ?? settings.Converters?.ToList();
				if(list.IsNulle()) {
					settings.Converters = list = new List<JsonConverter>();
					list.AddRange(converters);
				}
				else
					list.InsertRange(0, converters);
			}
			return settings;
		}


		public static JsonSerializerSettings AddConverters(this JsonSerializerSettings settings, params JsonConverter[] converters)
		{
			if(converters.NotNulle()) {
				if(settings.Converters == null)
					settings.Converters = new List<JsonConverter>();
				foreach(var converter in converters)
					settings.Converters.Add(converter);
			}
			return settings;
		}

		#endregion



		public static T DeserializeJson<T>(this byte[] json, JsonSerializerSettings settings = null)
		{
			string jsonStr = json == null ? null : json.GetString();
			return jsonStr.DeserializeJson<T>(settings);
		}

		public static T DeserializeJson<T>(this string json, JsonSerializerSettings settings = null)
		{
			var val = JsonConvert.DeserializeObject<T>(json, settings);
			return val;
		}

		/// <summary>
		/// Populates the JSON values onto the target object.
		/// </summary>
		public static void Populate(this JsonSerializer serializer, string json, object targetObject)
		{
			if(serializer != null)
				serializer.Populate(new StringReader(json), targetObject);
		}

		/// <summary>
		/// ?
		/// </summary>
		public static string Serialize(this JsonSerializer serializer, object value)
		{
			if(serializer == null)
				return null;
			var writer = new StringWriter();

			serializer.Serialize(new JsonTextWriter(writer), value);

			writer.Flush();

			return writer.ToString();
		}


		#region --- Value handling ---

		public static bool HasValue(this JProperty prop)
			=> prop != null && prop.ValueObjectNullIfDef() != null;

		public static object ValueObjectNullIfDef(this JProperty prop)
		{
			if(prop != null && prop.Value is JValue value) {
				object valueObj = value.Value;
				if(valueObj == null)
					return null;

				switch(value.Type) {
					case JTokenType.String:
						return valueObj.Equals("");
					case JTokenType.Integer:
					case JTokenType.Float:
						return valueObj.Equals(0);
					case JTokenType.Boolean:
						return valueObj.Equals(false);
				}
			}
			return null;
		}

		public static object ValueObject(this JProperty prop)
		{
			if(prop.Value is JValue value) {
				object valueObj = value.Value;
				return valueObj;
			}
			return null;
		}

		#endregion
	}
}

namespace DotNetXtensions.Json
{
#if !DNXPrivate
	public
#endif
	static class XJson
	{

		public static JObject ToJObject(this object obj, JsonSerializer jsonSerializer = null)
		{
			JObject jobj = jsonSerializer == null
				? JObject.FromObject(obj)
				: JObject.FromObject(obj, jsonSerializer);
			return jobj;
		}

		public static JToken ToJToken(this object obj, JsonSerializer jsonSerializer = null)
		{
			JToken jToken = jsonSerializer == null
				? JToken.FromObject(obj)
				: JToken.FromObject(obj, jsonSerializer);
			return jToken;
		}

	}
}
