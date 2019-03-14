using System;
using Xunit;
using DotNetXtensions;
using System.Web;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace DotNetXtensions.Test
{
	public class BaseUnitTest
	{
		static BaseUnitTest() => INIT();

		public static string VsOutLogPath { get; private set; }

		static void INIT()
		{
			string logPath = null; // to set on manually here, will override if NotNulle

			if(logPath.IsNulle())
				logPath = _getBinLogPath("VsOutLog");

			DebugWriter.SetConsoleOut(
				writeToOrigConsoleOutStill: true,
				writeToLogFilePath: logPath,
				deleteLogFileContentsFirst: true);

			$@"{nameof(BaseUnitTest)}.{nameof(INIT)} called: {DateTime.Now}

Log path: ""{logPath}""

".Print();
		}

		static string _getBinLogPath(string fileNamePrefix = null)
		{
			var assm = System.Reflection.Assembly.GetExecutingAssembly();
			var exAssNm = assm.GetName();
			string fullNm = exAssNm.Name;

			string path = System.IO.Path.GetDirectoryName(
				exAssNm.CodeBase)?
				.Replace('\\', '/')
				.SubstringAfterStartsWith("file:/")
				.NullIfEmptyTrimmed();

			if (path.NotNulle()) {
				
				if(path.Last() != '/')
					 path = path + '/';

				int lastIdxBin = path.LastIndexOf("/bin/");
				if (lastIdxBin > 0)
					path = path.Substring(0, lastIdxBin + 5);

				if (fileNamePrefix.IsNulle())
					fileNamePrefix = "VsTestOutLog";

				VsOutLogPath = path = $"{path}{fileNamePrefix}.{fullNm}.txt";
			}

			return path;
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

	}
}
