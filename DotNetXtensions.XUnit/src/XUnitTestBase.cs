using System;
using System.Collections.Generic;
using System.Linq;
using DotNetXtensions;
using Xunit;

namespace DotNetXtensions.Test
{
	public abstract class XUnitTestBase : EmbeddedResources
	{
		public XUnitTestBase() { }

		public XUnitTestBase(
			Type typeForEmbeddedResources, 
			string resourceBasePath = null,
			bool? cacheResourceGets = null)
			: base(typeForEmbeddedResources, resourceBasePath)
		{
			if(cacheResourceGets != null)
				CacheResourceGetsDefault = cacheResourceGets.Value;
		}

		#region --- Proj Paths ---

		public static string BaseDirectory => ProjectPath.BaseDirectory;

		public static string RootProjectDirectory => ProjectPath.RootProjectDirectory;

		public static string BinDirectory => ProjectPath.BinDirectory;


		public static string ProjPath(string endPath = null)
			=> ProjectPath.ProjPath(endPath);

		public static string ProjSrcPath(string endPathAfterSrc = null)
			=> ProjectPath.ProjSrcPath(endPathAfterSrc);

		#endregion

		#region --- OutLog ---

		static bool _outputSet;
		static object _outputLock = new object();

		public static string VsOutLogPath
			=> ProjectPath.BinPath($"xunit-out-log.txt");

		public static void SetConsoleOutputToTestLog(bool reset = false)
		{
			if(reset)
				_outputSet = false;

			if(!_outputSet) {
				lock(_outputLock) {
					if(!_outputSet) {

						_outputSet = true;
						string logPath = VsOutLogPath;

						DebugWriter.SetConsoleOut(
							writeToOrigConsoleOutStill: true,
							writeToLogFilePath: logPath,
							deleteLogFileContentsFirst: true);

						$@"{nameof(XUnitTestBase)}.{nameof(SetConsoleOutputToTestLog)} called at '{DateTime.Now}'

Log path: ""{logPath}""

".Print();
					}
				}
			}
		}

		#endregion

		#region --- General Helpers ---

		public static KeyValuePair<string, string>[] GetKVsPerLine(string kvInput, char separator = ':')
		{
			var kvs = kvInput
				.SplitLines(trimLines: true, removeEmptyLines: true)
				.Select(ln => {
					var items = ln.Split(separator);
					if(items.Length != 2) throw new ArgumentException();
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

		#endregion

	}
}
