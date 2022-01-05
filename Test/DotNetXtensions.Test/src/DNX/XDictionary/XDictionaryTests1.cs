namespace DotNetXtensions.Test;

public class XDictionaryTests1 : DnxTestBase
{
	[Fact]
	public void V_Value_NullInput_Pass()
	{
		string ky = null;
		True(ky == null);
		True(Dict1.V(ky) == false);
		True(Dict1.V(ky, defaultValue: false) == false);
		True(Dict1.V(ky, defaultValue: true) == true);

		True(Dict1.ValueN(ky) == null);
	}

	[Fact]
	public void V_Value_Pass()
	{
		var kv = Dict1.First();
		True(kv.Key.NotNulle());
		True(Dict1.V(kv.Key) == kv.Value);
		True(Dict1.ValueN(kv.Key) == kv.Value);
	}


	static Dictionary<string, bool> Dict1 = new Dictionary<string, bool>() {
		{ "Apples", false },
		{ "Oranges", true },
		{ "Pears", true },
	};
}
