using System;
using DotNetXtensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using System.Reflection;
using System.IO;

namespace DotNetXtensions.Test
{
	public abstract class XUnitTestBase
	{
		public XUnitTestBase() : this(false) { }

		public XUnitTestBase(bool setOutput)
		{
			if(setOutput)
				SetConsoleOutputToTestLog();

			if(UnitTestType != null) {
				_UnitTestAssm = Assembly.GetAssembly(UnitTestType);
				_AssemblyName = _UnitTestAssm.GetName().Name;
				_EmbeddedResourcesCache = new Dictionary<string, string>();
			}
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

		static bool _outputSet;
		static object _outputLock = new object();

		#endregion

		#region --- Embedded Resources ---

		readonly string _AssemblyName;
		readonly Assembly _UnitTestAssm;
		Dictionary<string, string> _EmbeddedResourcesCache;

		/// <summary>
		/// In order to use embedded resource members herein, caller
		/// must implement (override) this type. Unfortunately we can't auto-detect
		/// the type which inherits this <see cref="XUnitTestBase"/>. This let's 
		/// us get the assembly in which the embedded resources are contained.
		/// </summary>
		public virtual Type UnitTestType { get; set; }

		public string ResourcePath(string path)
		{
			if(ResourceBasePath.IsNulle())
				return $"{_AssemblyName}.{path}";
			return $"{_AssemblyName}.{ResourceBasePath}.{path}";
		}

		/// <summary>
		/// The default resource base path. By default is set to "src.resources".
		/// Note: In the final scenario the final resource path that is used to get 
		/// embedded resourceds will be prefixed with the assembly name, but that part 
		/// is auto-handled by us. Of which, see <see cref="UnitTestType"/> and its documentation.
		/// </summary>
		public virtual string ResourceBasePath { get; set; } = "src.DataResources";



		/// <summary>
		/// Gets the embedded resource value string, using 
		/// <see cref="ResourceBasePath"/> to contruct the full path,
		/// or if it's null expects the full resource path.
		/// </summary>
		/// <param name="nameAfterBasePath"></param>
		/// <param name="cache"></param>
		/// <returns></returns>
		public string ResourceString(string nameAfterBasePath, bool cache = true)
		{
			string fullResPath = ResourcePath(nameAfterBasePath);
			string val = GetResourceString(_UnitTestAssm, fullResPath, cache ? _EmbeddedResourcesCache : null);
			return val;
		}

		public static string GetResourceString(
			Assembly assm, 
			string resourceName,
			Dictionary<string, string> _EmbeddedResourcesCache = null)
		{
			bool cache = _EmbeddedResourcesCache != null;
			if(cache) {
				if(_EmbeddedResourcesCache.TryGetValue(resourceName, out string val))
					return val;
			}

			// don't erase these lines, for debugging...
			//var assm = Assembly.GetExecutingAssembly();
			//string[] names = assm.GetManifestResourceNames();

			using(Stream stream = assm.GetManifestResourceStream(resourceName))
			using(StreamReader reader = new StreamReader(stream)) {
				string val = reader.ReadToEnd();

				if(cache)
					_EmbeddedResourcesCache[resourceName] = val;

				return val;
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
