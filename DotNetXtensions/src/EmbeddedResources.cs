using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using DotNetXtensions;

namespace DotNetXtensions
{
	public class EmbeddedResources
	{
		string _assmName;
		Assembly _assm;
		Dictionary<string, string> _resStringCache;
		Dictionary<string, byte[]> _resDataCache;

		public EmbeddedResources() { }

		public EmbeddedResources(Type typeForAssembly, string resourceBasePath = null)
		{
			if(typeForAssembly != null)
				InitEmbeddedResources(typeForAssembly, resourceBasePath);
		}

		public bool CacheResourceGetsDefault { get; set; } = false;


		/// <summary>
		/// Type which is contained within the desired assembly that contains the embedded 
		/// resources to access. See notes on this type in <see cref="InitEmbeddedResources(Type, string)"/>.
		/// </summary>
		public Type TypeForResources { get; private set; }

		string _ResourceBasePath { get; set; }

		/// <summary>
		/// The default resource base path.
		/// Note: In the final scenario the final resource path that is used to get 
		/// embedded resourceds will be prefixed with the assembly name, but that part 
		/// is auto-handled by us. Of which, see <see cref="TypeForResources"/> and its documentation.
		/// </summary>
		public string ResourceBasePath => _ResourceBasePath;

		/// <summary>
		/// Initializes this <see cref="EmbeddedResources"/>, before which its members 
		/// cannot be accessed. Ideally <see cref="TypeForResources"/> would be readonly,
		/// but we want to allow of this type in cases where these members aren't ultimately used. 
		/// Just know that this Init method MUST be called first before any other members
		/// of this type are called.
		/// </summary>
		/// <param name="typeForAssembly"></param>
		/// <param name="resourceBasePath"></param>
		public void InitEmbeddedResources(Type typeForAssembly, string resourceBasePath = null)
		{
			TypeForResources = typeForAssembly ?? throw new ArgumentNullException(nameof(typeForAssembly));

			_assm = Assembly.GetAssembly(TypeForResources);
			_assmName = _assm.GetName().Name;

			_ResourceBasePath = InitResourceBasePath(_assmName, resourceBasePath);

			if(_resStringCache != null)
				_resStringCache = null;

			if(_resDataCache != null)
				_resDataCache = null;
		}

		public static string InitResourceBasePath(string assmName, string resPath)
		{
			resPath = resPath.NullIfEmptyTrimmed();
			if(resPath == null)
				return null; // assmName;

			if(resPath.Last() == '.') {
				resPath = resPath.CutEnd(1).NullIfEmptyTrimmed();
				if(resPath == null)
					return null; // assmName;
			}

			if(resPath.StartsWith(assmName)) {
				return resPath == assmName
					? null
					: resPath;
			}
			else
				return $"{assmName}.{resPath}";
		}


		public string ResourcePath(string path)
		{
			string fullPath;

			if(_ResourceBasePath.IsNulle()) {
				fullPath = (path?.StartsWith(_assmName) ?? false)
					? path
					: $"{_assmName}.{path}";
			}
			else {
				fullPath = $"{_ResourceBasePath}.{path}";
			}

			// NEW: if set, the INIT requires rbp already prefixed with assmNm
			//: $"{_assmName}.{_ResourceBasePath}.{path}";

			if(path.IsNulle()) // rare, but used within to get the base path before final path name
				fullPath = fullPath.CutEnd(1);

			return fullPath;
		}



		/// <summary>
		/// Gets the embedded resource value string, using 
		/// <see cref="_ResourceBasePath"/> to contruct the full path,
		/// or if it's null expects the full resource path.
		/// </summary>
		/// <param name="nameAfterBasePath"></param>
		/// <param name="cache"></param>
		public string ResourceString(string nameAfterBasePath, bool? cache = null)
		{
			string fullResPath = ResourcePath(nameAfterBasePath);

			bool _cache = cache ?? CacheResourceGetsDefault;

			if(_cache && _resStringCache == null)
				_resStringCache = new Dictionary<string, string>();

			string val = GetResourceString(_assm, fullResPath, _cache ? _resStringCache : null);
			return val;
		}

		/// <summary>
		/// Gets the embedded resource value byte array, using 
		/// <see cref="_ResourceBasePath"/> to contruct the full path,
		/// or if it's null expects the full resource path.
		/// </summary>
		/// <param name="nameAfterBasePath"></param>
		/// <param name="cache"></param>
		public byte[] ResourceData(string nameAfterBasePath, bool? cache = null)
		{
			string fullResPath = ResourcePath(nameAfterBasePath);

			bool _cache = cache ?? CacheResourceGetsDefault;
			if(_cache && _resDataCache == null)
				_resDataCache = new Dictionary<string, byte[]>();

			byte[] val = GetResourceData(_assm, fullResPath, _cache ? _resDataCache : null);
			return val;
		}


		/// <summary>
		/// Gets the matching resource string, or NULL if not found.
		/// </summary>
		/// <param name="assm"></param>
		/// <param name="resourceName">"The case-sensitive name of the (embedded) manifest resource".</param>
		/// <param name="cache">True to use the input cache to cache the result.</param>
		public static string GetResourceString(
			Assembly assm,
			string resourceName,
			Dictionary<string, string> cache = null)
		{
			bool useCache = cache != null;
			if(useCache) {
				if(cache.TryGetValue(resourceName, out string val))
					return val;
			}

			using(Stream stream = assm.GetManifestResourceStream(resourceName)) {

				if(stream == null)
					return null;

				using(StreamReader reader = new StreamReader(stream)) {
					string val = reader.ReadToEnd();

					if(val != null && useCache)
						cache[resourceName] = val;

					return val;
				}
			}
		}

		/// <summary>
		/// Gets the matching resource byte array, or NULL if not found.
		/// </summary>
		/// <param name="assm"></param>
		/// <param name="resourceName">"The case-sensitive name of the (embedded) manifest resource".</param>
		/// <param name="cache">True to use the input cache to cache the result.</param>
		public static byte[] GetResourceData(
			Assembly assm,
			string resourceName,
			Dictionary<string, byte[]> cache = null)
		{
			bool useCache = cache != null;
			if(useCache) {
				if(cache.TryGetValue(resourceName, out byte[] val))
					return val;
			}

			using(Stream stream = assm.GetManifestResourceStream(resourceName)) {
				if(stream == null)
					return null;

				byte[] val = stream.ReadBytes();
				if(val != null && useCache)
					cache[resourceName] = val;

				return val;
			}
		}

		/// <summary>
		/// Ideally for diagnostic purposes, gets all manifest resource names
		/// that start with this type's resource path (see <see cref="ResourcePath"/>),
		/// or if <paramref name="onlyResPathMatches"/> is false, returns all 
		/// resources in this assembly.
		/// </summary>
		/// <param name="onlyResPathMatches">False to get all resources in assembly,
		/// else only matches <see cref="ResourcePath"/>.</param>
		public string[] GetAllResourceNames(bool onlyResPathMatches = true)
		{
			string resBasePath = ResourcePath(null);

			string[] names = _assm?.GetManifestResourceNames();
			if(onlyResPathMatches)
				names = names.Where(n => n.StartsWith(resBasePath)).ToArray();

			return names;
		}

	}
}
