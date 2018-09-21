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

		static void INIT()
		{
			string logPath = null; // to set on manually here, will override if NotNulle

			if(logPath.IsNulle())
				logPath = _getBinLogPath("vsOutLog.txt");

			DebugWriter.SetConsoleOut(
				writeToOrigConsoleOutStill: true,
				writeToLogFilePath: logPath,
				deleteLogFileContentsFirst: true);
		}

		static string _getBinLogPath(string fileName)
		{
			string path = System.IO.Path.GetDirectoryName(
				System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)?
				.Replace('\\', '/')
				.SubstringAfterStartsWith("file:/")
				.NullIfEmptyTrimmed();

			if (path.NotNulle()) {
				
				if(path.Last() != '/')
					 path = path + '/';

				int lastIdxBin = path.LastIndexOf("/bin/");
				if (lastIdxBin > 0)
					path = path.Substring(0, lastIdxBin + 5);

				path = path + fileName;
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

		public static bool DictionariesAreEqual(Dictionary<string, string> dict, Dictionary<string, string> expectedMatchDict)
		{
			var d1 = dict;
			var d2 = expectedMatchDict;
			if (d1 == null || d2 == null)
				return d1 == null && d2 == null;

			if (d1.Count != d2.Count)
				return false;

			foreach (string key in d1.Keys) {
				string val = d1[key];

				if (!d2.TryGetValue(key, out string val2))
					return false;

				if (val != val2)
					return false;
			}
			return true;
		}

		public static bool DictionariesAreEqual(Dictionary<string, string[]> dict, Dictionary<string, string[]> expectedMatchDict)
		{
			var d1 = dict;
			var d2 = expectedMatchDict;
			if (d1 == null || d2 == null)
				return d1 == null && d2 == null;

			if (d1.Count != d2.Count)
				return false;

			foreach (string key in d1.Keys) {
				string[] vals = d1[key];

				if (!d2.TryGetValue(key, out string[] vals2))
					return false;

				if (vals == null || vals2 == null)
					return vals == vals2;

				return vals.SequenceEqual(vals2);
			}

			return true;
		}

	}
}
