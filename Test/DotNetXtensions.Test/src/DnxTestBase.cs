using DotNetXtensions.Json;

using Newtonsoft.Json;

namespace DotNetXtensions.Test;

public class DnxTestBase : XUnitTestBase
{
	public DnxTestBase() : this("data") { }

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="setOutput"></param>
	/// <param name="resourceBasePath">
	/// Resource path, default is "src.resources", if you want the root to be rather
	/// for ex: "src.resources.ical", then pass that in, allowing calls to ResouceString
	/// (etc) to be shortened. If you want to avoid any (extremely minor) cost of
	/// initing underlying EmbeddedResourcesHelper types when not needed, just pass in null here.</param>
	public DnxTestBase(string dataBasePath)
		: base(dataBasePath) //resourceBasePath.IsNulle() ? null : typeof(DnxTestBase), resourceBasePath)
	{
		CacheResourceGetsDefault = true;

		InitDefJsonSettings();
	}

	static bool _jsonInited;

	public void InitDefJsonSettings()
	{
		if(!_jsonInited) {
			_jsonInited = true;

			JsonConvert.DefaultSettings = () => new JsonSerializerSettings() {
				NullValueHandling = NullValueHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
				DateParseHandling = DateParseHandling.DateTimeOffset,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				// http://stackoverflow.com/questions/17368484/json-net-how-to-override-serialization-for-a-type-that-defines-a-custom-jsonconv
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			}
				.IndentJson()
				.CamelCase(false)
				.AddConverter(new StringSerializableJsonConverter())
				.EnumsAsStrings(camelCaseEnumText: false);

			WarmupJson();

			// Avoids test breaking issues
			void WarmupJson()
			{
				int[] numArr = { 1, 2, 3, 4 };

				string json = numArr.ToJson(indent: false);

				True(json == "[1,2,3,4]");

				int[] arr = json.DeserializeJson<int[]>();

				True(arr != null && arr.SequenceIsEqual(numArr));
			}
		}
	}
}
