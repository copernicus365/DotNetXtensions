namespace DotNetXtensions.Test;

public class ProjectPathTests : DnxTestBase
{
	const string projtype = "net8.0"; // "netcoreapp3.1", "netcoreapp2.2";
	const string projName = "DotNetXtensions.Test";
	static string buildCnfg =
#if DEBUG
			"Debug";
#else
			"Release";
#endif

	[Fact]
	public void Test_BinDirectory()
		=> EndsWith($"/{projName}/bin/", ProjectPath.BinDirectory);

	[Fact]
	public void Test_BaseDirectory()
		=> EndsWith($"/{projName}/bin/{buildCnfg}/{projtype}/", ProjectPath.BaseDirectory);

	[Fact]
	public void Test_RootProjectDirectory()
		=> EndsWith($"/{projName}/", ProjectPath.RootProjectDirectory);
}
