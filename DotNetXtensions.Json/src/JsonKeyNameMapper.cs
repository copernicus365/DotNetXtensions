using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DotNetXtensions.Json
{
	public class JsonKeyNameMapper
	{
		public KeyNameMapper KeyMapper { get; set; }

		public string JsonPath { get; set; }

		public JToken GetItems(string json)
		{
			var obj = JToken.Parse(json);
			JToken jArrayRoot = null;
			if (obj == null)
				return null;

			string jsonPath = JsonPath;

			if (jsonPath.NotNulle()) {
				jArrayRoot = obj.SelectToken(jsonPath);
			}
			else {
				if (obj.Type != JTokenType.Array)
					return null;
				jArrayRoot = obj;
			}

			List<JToken> children = jArrayRoot.Children().ToList();
			if (children.IsNulle())
				return null;

			for (int i = 0; i < children.Count; i++) {
				if (children[i] is JObject item && item.Count > 0) {
					ParseToItem(item);
				}
			}

			return jArrayRoot;
		}

		public JObject ParseToItem(JObject item)
		{
			if (item == null)
				return null;

			// #1 - Remove any ignored properties. Need to do this first, otherwise would have to duplicate this operation
			if (KeyMapper.IgnoreKeys.Count > 0) {
				foreach (string igKey in KeyMapper.IgnoreKeys.Keys) {
					bool removed = item.Remove(igKey);
				}
			}

			// #2 - HANDLE MULTI-PROPERTIES :--> Keep first (that has a value!), REMOVE remaining properties immediately from parent item
			foreach (var kv in KeyMapper.NamesToCustomDictWithMultiples) {
				string standardName = kv.Key;
				List<string> mVals = kv.Value;

				bool deleteRest = false;

				JProperty standardProp = item.Property(standardName);
				if (standardProp != null) {
					deleteRest = standardProp.HasValue();
					if (!deleteRest)
						standardProp?.Remove();
				}

				for (int i = 0; i < mVals.Count; i++) {
					string mval = mVals[i];
					if (deleteRest) {
						bool removed = item.Remove(mval);
					}
					else {
						var prop = item.Property(mval);
						if (prop != null) {
							if (prop.HasValue())
								deleteRest = true;
							else
								prop.Remove(); // is probably best considering the next main loop, so this property will never be seen again
						}
					}
				}
			}

			JProperty[] props = item.Children<JProperty>().ToArray();
			if (props.IsNulle())
				return null;

			// There will now be no multi-properties beyond the top-priority one (with a value) within the parent item
			for (int i = 0; i < props.Length; i++) {
				JProperty prop = props[i];
				string pname = prop.Name;

				if (KeyMapper.CustomNamesDict.TryGetValue(pname, out string standardPName)) {

					if (!prop.HasValue())
						continue;

					var currProp = item.Property(standardPName);
					if (currProp != null)
						continue; // -- first in wins, though note: we only allow a CUSTOM add below if it was #1 in a list of 1 or more

					item.Add(propertyName: standardPName, value: prop.Value);
					prop.Remove();
				}
			}
			return item;
		}

		public T[] GetItems<T>(string json, JsonSerializer serializer)
		{
			JToken token = GetItems(json);
			T[] items = token.ToObject<T[]>(serializer);
			return items;
		}
	}
}
