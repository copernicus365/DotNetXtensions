using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using DotNetXtensions;
using DotNetXtensions.Cryptography;

namespace DotNetXtensions.Test
{
	public class ProjectPathTests : DnxTestBase
	{
		const string projtype = "netcoreapp2.2";
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
}
