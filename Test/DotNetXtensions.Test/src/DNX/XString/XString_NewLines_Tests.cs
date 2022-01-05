namespace DNX.Test.Strings;

public class XString_NewLines_Tests : DnxTestBase
{
	const string text1_rn = " hello\r\n there  \n \r\nworld\r\n";
	const string text1_n = " hello\n there  \n \nworld\n";


	[Fact]
	public void NewLine1()
		=> True(text1_rn.ToUnixLines() == text1_n);

	[Fact]
	public void NewLine_NoChange()
		=> True(text1_n.ToUnixLines() == text1_n);


	[Fact]
	public void NewLine_IfNeededTrue()
	=> True(text1_rn.ToUnixLines(ifNeeded: true) == text1_n);


	[Fact]
	public void NewLine_NoChange_IfNeeded()
		=> True(text1_n.ToUnixLines(ifNeeded: true) == text1_n);

}
