namespace DNX.Test.Strings;

public class XString_NulleTests : DnxTestBase
{

	[Theory]
	[InlineData("hello", null, 1)]
	[InlineData("hello", "", 1)]
	[InlineData("hello", "howdy", 1)]
	[InlineData(null, "hello", 2)]
	[InlineData("", "hello", 2)]
	[InlineData("", "", 2)]
	[InlineData("", null, 1, false)]
	[InlineData(null, "", 2)]
	[InlineData(null, null, 2, true)]
	public async Task FirstNotNulle_2Vals(string val1, string val2, int match, bool endResultIsNull = false)
	{
		if(match.NotInRange(1, 2)) throw new ArgumentOutOfRangeException(nameof(match));

		string expMatch = match == 1 ? val1 : val2;

		string res = val1.FirstNotNulle(val2);

		True(res == expMatch);
		True((res == null) == endResultIsNull);
	}



	[Theory]
	[InlineData("hello", null, null, 1)]
	[InlineData("hello", "", "", 1)]

	[InlineData("hello", null, "hi", 1)]
	[InlineData("hello", "", "hi", 1)]

	[InlineData("hello", "hi", null, 1)]
	[InlineData("hello", "hi1", "hi2", 1)]

	[InlineData(null, "hello", "hi", 2)]
	[InlineData("", "hello", "hi", 2)]
	[InlineData("", "hello", "", 2)]

	[InlineData(null, null, "hello", 3)]
	[InlineData("", "", "hello", 3)]
	[InlineData(null, "", "hello", 3)]

	[InlineData(null, "", null, 2)]
	public async Task FirstNotNulle_3Vals(string val1, string val2, string val3, int match)
	{
		string expMatch = null;
		switch(match) {
			case 1: expMatch = val1; break;
			case 2: expMatch = val2; break;
			case 3: expMatch = val3; break;
			default: throw new ArgumentOutOfRangeException(nameof(match));
		}

		string res = val1.FirstNotNulle(val2, val3);

		True(res == expMatch);
		//True((res == null) == endResultIsNull);
	}


}
