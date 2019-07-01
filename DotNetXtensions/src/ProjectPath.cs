using System;
using System.IO;

namespace DotNetXtensions
{
	public static class ProjectPath
	{
		public static string BaseDirectory { get; set; }

		public static string RootProjectDirectory { get; set; }

		public static string BinDirectory { get; set; }



		static ProjectPath() => Init();


		public static void Init(bool getFromAppDomain = true)
		{
			GetAssemblyDirectories(out string baseDir, out string rootProjDir, out string binDir, getFromAppDomain);
			BaseDirectory = baseDir;
			BinDirectory = binDir;
			RootProjectDirectory = rootProjDir;
		}



		public static string ProjPath(string endPath = null)
			=> _combinePath(RootProjectDirectory, endPath);

		public static string ProjSrcPath(string endPathAfterSrc = null)
			=> ProjPath("src/" + endPathAfterSrc);

		public static string BinPath(string endPath = null)
			=> _combinePath(BinDirectory, endPath);

		public static string BaseDirectoryPath(string endPath = null)
			=> _combinePath(BaseDirectory, endPath);


		static string _combinePath(string root, string end)
		{
			if (end.IsNulle())
				return root;

			if (root.IsNulle())
				return end;

			return root + end;
		}


		public static void GetAssemblyDirectories(
			out string baseDirectory,
			out string rootProjDirectory,
			out string binDirectory,
			bool getFromAppDomain = true)
		{
			string dir1 = GetExeBaseDirectory(getFromAppDomain)
				.Replace('\\', '/');

			if (dir1.LastN() != '/')
				dir1 += '/';

			baseDirectory = dir1;
			rootProjDirectory = null;
			binDirectory = null;

			if (dir1.NotNulle()) {
				int binIdx = dir1.LastIndexOf(@"/bin/");
				if (binIdx > 0) {
					binDirectory = dir1.Substring(0, binIdx + 5);
					rootProjDirectory = dir1.Substring(0, binIdx + 1);
				}
			}
		}

		public static string GetExeBaseDirectory(bool getFromAppDomain = true)
		{
			string val;
			if (getFromAppDomain)
				val = AppDomain.CurrentDomain.BaseDirectory;
			else {
				string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				val = Path.GetDirectoryName(path);
			}
			return val;
		}

	}
}
