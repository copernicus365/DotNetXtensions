namespace DotNetXtensions.Test;

public class ProjectPathTests : DnxTestBase
{
	const string projtype = "net6.0"; // "netcoreapp3.1", "netcoreapp2.2";
	const string projName = "DotNetXtensions.Test";
	static string buildCnfg =
#if DEBUG
			"Debug";
#else
			"Release";
#endif

	[Fact]
	public void Test_BinDirectory()
		=> True(ProjectPath.BinDirectory.EndsWith($"/{projName}/bin/"));

	[Fact]
	public void Test_BaseDirectory()
		=> True(ProjectPath.BaseDirectory.EndsWith($"/{projName}/bin/{buildCnfg}/{projtype}/"));

	[Fact]
	public void Test_RootProjectDirectory()
		=> True(ProjectPath.RootProjectDirectory.EndsWith($"/{projName}/"));
}
