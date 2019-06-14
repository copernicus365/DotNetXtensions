using System;
using DotNetXtensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DotNetXtensions.Test
{
	public class BaseUnitTest
	{
		static BaseUnitTest() => INIT();

		public static string AssemblyDirPath { get; private set; }
		public static string RootProjPath { get; private set; }

		public static string ProjPath(string endPath)
		{
			string rootPth = RootProjPath;
			if (rootPth.IsNulle())
				return null;
			return rootPth + endPath;
		}

		public static string ProjSrcPath(string endPathAfterSrc)
			=> ProjPath("src/" + endPathAfterSrc);

		public static string VsOutLogPath { get; private set; }

		static void INIT()
		{
			string logPath = null; // to set on manually here, will override if NotNulle

			if (logPath.IsNulle()) {
				(string assemblyDir, string path) = _getBinLogPath("VsOutLog");
				AssemblyDirPath = assemblyDir;
				logPath = path;

				if (assemblyDir.NotNulle()) {
					int binIdx = assemblyDir.LastIndexOf(@"/bin/");
					if (binIdx > 0) {
						string rootProjPath = assemblyDir.Substring(0, binIdx + 1);
						RootProjPath = rootProjPath;
					}
				}
			}

			DebugWriter.SetConsoleOut(
				writeToOrigConsoleOutStill: true,
				writeToLogFilePath: logPath,
				deleteLogFileContentsFirst: true);

			$@"{nameof(BaseUnitTest)}.{nameof(INIT)} called: {DateTime.Now}

Log path: ""{logPath}""

".Print();
		}

		static (string assemblyDir, string path) _getBinLogPath(string fileNamePrefix = null)
		{
			var assm = System.Reflection.Assembly.GetExecutingAssembly();
			var exAssNm = assm.GetName();
			string fullNm = exAssNm.Name;

			string dirPath = System.IO.Path.GetDirectoryName(
				exAssNm.CodeBase)?
				.Replace('\\', '/')
				.SubstringAfterStartsWith("file:/")
				.NullIfEmptyTrimmed();

			string path = null;

			if (dirPath.NotNulle()) {

				path = dirPath;

				if (path.Last() != '/')
					 path += '/';

				int lastIdxBin = path.LastIndexOf("/bin/");
				if (lastIdxBin > 0)
					path = path.Substring(0, lastIdxBin + 5);

				if (fileNamePrefix.IsNulle())
					fileNamePrefix = "VsTestOutLog";

				VsOutLogPath = path = $"{path}{fileNamePrefix}.{fullNm}.txt";
			}

			return (dirPath, path);
		}



		// --- General Helpers ---

		public static KeyValuePair<string, string>[] GetKVsPerLine(string kvInput, char separator = ':') {
			var kvs = kvInput
				.SplitLines(trimLines: true, removeEmptyLines: true)
				.Select(ln => {
					var items = ln.Split(separator);
					if (items.Length != 2) throw new ArgumentException();
					return new KeyValuePair<string, string>(items[0], items[1]);
				})
				.ToArray();
			return kvs;
		}



		public static T[] tarray<T>(params T[] arr) => arr;



		public static DateTime[] AddMinutesArr(DateTime dt, params double[] minsFromBase)
		{
			return minsFromBase.E()
				.Select(diff => dt.AddMinutes(diff))
				.ToArray();
		}

		public void Fail() => Assert.True(false);

		public void False(bool val) => Assert.True(!val);

		public void True(bool val) => Assert.True(val);
	}
}
