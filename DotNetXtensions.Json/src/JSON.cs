using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DotNetXtensions.Json
{
	/// <summary>
	/// At the present time, the members of this static class revolve around making it more 
	/// user-friendly to construct JSON objects without null related exceptions being thrown.
	/// <para />
	/// Longer discussion: 
	/// So for instance, in sending in an array of items into the constructor of a new JObject,
	/// unfortunately the present Newtonsoft way is to throw an exception if any of the items are 
	/// null. This is UNLIKE how System.Xml.Linq.XElement works, which simply IGNORES any null items.
	/// Where this is a BIG DEAL is that VERY frequently, we want to conditionally include an item in pretty 
	/// LINQ constucting the JSON or XML structure, based on a variable. E.g. only include a 
	/// "sound" property if the "message" property is not null or empty. You would do this something like
	/// the following (within a list of params in for instance a JObject constructor):
	/// <c>message == null ? null : new JProperty("sound", "ding")</c>. The current JSON library 
	/// makes this virtually impossible, it throws an exception on seeing any null items.
	/// Forcing one to have to lose the much shorter, prettier, and easier
	/// to read fluid construction in place of declarative if - Add statements. This is largely 
	/// a DEAL STOPPER in my view for using the current Newtonsoft JSON LINQ fluid construction pattern.
	/// Hopefully we can get Newtonsoft to fix this (the fix would be very simple, see for instance the 
	/// JObject constructors here: https://github.com/JamesNK/Newtonsoft.Json/blob/88229ddcf49232ca1898729a8fe319f3c8782f22/Src/Newtonsoft.Json/Linq/JObject.cs), 
	/// but until then this class makes available such functionality in a pretty good way, about as good as you could hope for. 
	/// In fact it is a lot terser than the ternary (if condition return null else object) route anyways.
	/// </summary>
#if DNXPublic
	public
#endif
	static class JSON
	{
		/// <summary>
		/// Returns a new JObject if the input object is not null, else returns null.
		/// </summary>
		public static JObject JObject(object obj)
		{
			if(obj == null)
				return new JObject();
			return new JObject(obj);
		}

		/// <summary>
		/// Returns a new JObject with null-filtered input if the filtered input is NotNulle. Else returns null.
		/// </summary>
		public static JObject JObject(params object[] objects)
		{
			if(objects.NotNulle())
				objects = objects.Where(obj => obj != null).ToArray();
			if(objects.IsNulle())
				return new JObject();
			return new JObject(objects);
		}



		/// <summary>
		/// Returns a new JProperty if the condition is true, else returns null.
		/// </summary>
		public static JProperty JPropertyIf(bool condition, string name, object obj)
		{
			if(!condition)
				return null;
			return new JProperty(name, obj);
		}



		/// <summary>
		/// Returns a new JProperty if the obj is not null, else returns null.
		/// </summary>
		public static JProperty JPropertyNotNull(string name, object obj)
		{
			if(obj == null)
				return null;
			return new JProperty(name, obj);
		}

		/// <summary>
		/// Returns a new JProperty if the string input is not null OR empty, else returns null.
		/// </summary>
		public static JProperty JPropertyNotNulle(string name, string str)
		{
			if(str.IsNulle())
				return null;
			return new JProperty(name, str);
		}
		


		/// <summary>
		/// Returns a new JProperty with null-filtered input if the condition is true and if the filtered 
		/// input is NotNulle. Else returns null.
		/// </summary>
		public static JProperty JPropertyArrayIf(bool condition, string name, params object[] objects)
		{
			if(!condition)
				return null;
			return JPropertyArray(name, objects);
		}

		/// <summary>
		/// Returns a new JProperty with null-filtered input if the filtered 
		/// input is NotNulle. Else returns null.
		/// </summary>
		public static JProperty JPropertyArray(string name, params object[] objects)
		{
			if(objects.IsNulle())
				return null;

			objects = objects.Where(obj => obj != null).ToArray();
			if(objects.IsNulle())
				return null;

			return new JProperty(name, objects);
		}

	}
}
