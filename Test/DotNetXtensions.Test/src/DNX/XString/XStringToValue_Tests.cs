namespace DNX.Test.Strings;

public class XStringToValue_Tests : DnxTestBase
{

	#region --- T_ToInt ---

	[Theory]
	[InlineData(null, 0)]
	[InlineData(null, 5, 5)]
	[InlineData("13", 13)]
	[InlineData("13", 13, 6)]
	[InlineData("a", 0)]
	[InlineData("a", 12, 12)]
	[InlineData("7.3", 5, 5)] // double won't parse as int, so returns default
	[InlineData("13.3", 13, 0, false)]
	public void T_ToInt(string val, int expected, int dflt = default, bool success = true)
		=> isMatch(val.ToInt(dflt), expected, success);

	#endregion

	#region --- T_ToLong ---

	const long num_long = 230_000_000_111;
	const string num_long_str = "230000000111";

	[Theory]
	[InlineData(null, 0)]
	[InlineData(null, num_long, num_long)]
	[InlineData("", num_long, num_long)]
	[InlineData(num_long_str, num_long)]
	[InlineData("13.3", 0)]
	[InlineData("13.3", 13, 0, false)]
	[InlineData(num_long_str, num_long, 5)]
	[InlineData("a", num_long, num_long)]
	public void T_ToLong(string val, long expected, long dflt = default, bool success = true)
		=> isMatch(val.ToLong(dflt), expected, success);

	#endregion

	#region --- T_ToDouble ---

	[Theory]
	[InlineData(null, 0)]
	[InlineData("13.337", 13.337)]
	[InlineData("13.337", 13.337, 6)]
	[InlineData("a", 0)]
	[InlineData("a", 12.447, 12.447)]
	public void T_ToDouble(string val, double expected, double dflt = default, bool success = true)
		=> isMatch(val.ToDouble(dflt), expected, success);

	#endregion

	#region --- T_ToDecimal ---

	[Theory]
	[InlineData(null, 0)]
	[InlineData("13.337", 13.337)]
	[InlineData("13.337", 13.337, 6)]
	[InlineData("a", 0)]
	[InlineData("a", 12.447, 12.447)]
	public void T_ToDecimal(string val, decimal expected, decimal dflt = default, bool success = true)
		=> isMatch(val.ToDecimal(dflt), expected, success);

	#endregion

	#region --- T_ToBool ---

	[Theory]
	[InlineData(null, false)]
	[InlineData("", false)]
	[InlineData("true", true)]
	[InlineData("false", false)]
	[InlineData("True", true)]
	[InlineData("False", false)]
	[InlineData("tRue", true)]
	[InlineData("fAlse", false)]

	[InlineData("false", false, true)]

	[InlineData("1", true)]
	[InlineData("0", false)]
	public void T_ToBool(string val, bool expected, bool dflt = default, bool success = true)
		=> isMatch(val.ToBool(dflt), expected, success);

	#endregion

	#region --- T_ToGuid ---

	const string sample_guid1 = "44bf11c3-befa-4293-aff1-9d002e99e02b";
	const string sample_guid2 = "c7f18bdd-e291-4318-ac1b-4f11f8bcfe5b";

	[Theory]
	[InlineData(null, null)]
	[InlineData("", null)]
	[InlineData(null, sample_guid2, sample_guid2)]
	[InlineData("", sample_guid2, sample_guid2)]
	[InlineData(sample_guid1, sample_guid1)]
	[InlineData(sample_guid1, sample_guid1, sample_guid2)]
	[InlineData("abc", "")]
	[InlineData("abc", sample_guid1, sample_guid1)]
	public void T_ToGuid(string val, string expected, string dflt = default, bool success = true)
	{
		var _dflt = dflt.IsNulle() ? Guid.Empty : new Guid(dflt);
		var _expected = expected.IsNulle() ? Guid.Empty : new Guid(expected);
		isMatch(val.ToGuid(_dflt), _expected, success);
	}

	#endregion

	void isMatch<T>(T answer, T expected, bool success) where T : struct, IEquatable<T>
	{
		bool match = answer.Equals(expected);
		True(match == success);
	}

}
