using System;
using System.IO;

namespace DotNetXtensions
{
	/// <summary>
	/// Provides a convenient class for managing project paths and embedded resources,
	/// often most helpful by inheriting from this class, which itself inherits <see cref="EmbeddedResources"/>.
	/// This also depends heavily on <see cref="ProjectPath"/>, but that is only a static class,
	/// and neither read files directly, as this does. Among other use cases, this can be a
	/// convenient base for test classes (for XUnit, etc), when reading
	/// from embedded resources or project files are needed.no
	/// </summary>
	public class ProjectPaths : EmbeddedResources
	{
		/// <summary>
		/// Empty constructor, or to be used when setting the <see cref="BaseDataPath"/>.
		/// See documentation on that property for more information.
		/// </summary>
		/// <param name="dataBasePath">See documentation on <see cref="BaseDataPath"/>.</param>
		public ProjectPaths(string dataBasePath = "data")
			=> BaseDataPath = dataBasePath;

		/// <summary>
		/// Constructor to call when using embedded resources, namely when
		/// going to be calling <see cref="EmbeddedResources.ResourceString(string, bool?)"/>,
		/// etc.
		/// </summary>
		/// <param name="typeForEmbeddedResources">A type that exists in the project to read embedded resources from.</param>
		/// <param name="resourceBasePath">If using embedded resources, should be sub-directory path
		/// after project, separated by periods. If using resources copied to the output directory,
		/// should be the normal relative path (slash separated) after </param>
		/// <param name="cacheResourceGets"></param>
		public ProjectPaths(
			Type typeForEmbeddedResources,
			string resourceBasePath = null,
			bool? cacheResourceGets = null)
			: base(typeForEmbeddedResources, resourceBasePath)
		{
			if(cacheResourceGets != null)
				CacheResourceGetsDefault = cacheResourceGets.Value;
		}


		#region --- DataPath ---


		/// <summary>
		/// When using <see cref="DataString(string)"/> and
		/// <see cref="DataBytes(string)"/>, this path represents the path after
		/// <see cref="Environment.CurrentDirectory"/> which to read files from.
		/// To have this work, see example:
		/// <para />
		/// With a root project directory `data` exists with let's say json files that need
		/// accessed by the test project, in csproj put within an `ItemGroup`:
		/// <![CDATA[<Content Include="data\**\*.*"><CopyToOutputDirectory>Always</CopyToOutputDirectory></Content>]]>
		/// </summary>
		public string BaseDataPath {
			get => _baseDataPath;
			private set {
				_baseDataPath = value.IsNulle()
					? null
					: value; //.ReplaceIfNeeded(".", Environment.NewLine).ReplaceIfNeeded('/', '\\');
			}
		}
		string _baseDataPath = "data";


		/// <summary>
		/// 
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public string DataString(string path)
		{
			string fpath = _getDataPath(BaseDataPath, path);
			string content = File.ReadAllText(fpath);
			//Assert.True(content != null);
			return content;
		}

		public byte[] DataBytes(string path)
		{
			string fpath = _getDataPath(BaseDataPath, path);
			var content = File.ReadAllBytes(fpath);
			//Assert.True(content != null);
			return content;
		}

		static string _getDataPath(string baseDir, string path)
		{
			string currDir = Environment.CurrentDirectory;
			string fpath = baseDir.IsNulle()
				? Path.Combine(currDir, path)
				: Path.Combine(currDir, baseDir, path);
			return fpath;
		}


		#endregion


		#region --- Proj Paths ---

		public static string BaseDirectory
			=> ProjectPath.BaseDirectory;

		public static string RootProjectDirectory
			=> ProjectPath.RootProjectDirectory;

		public static string BinDirectory
			=> ProjectPath.BinDirectory;

		public static string ProjPath(string endPath = null)
			=> ProjectPath.ProjPath(endPath);

		public static string ProjSrcPath(string endPathAfterSrc = null)
			=> ProjectPath.ProjSrcPath(endPathAfterSrc);

		#endregion
	}
}
