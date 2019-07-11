
namespace DotNetXtensions.Test
{
	public class DnxTestBase : XUnitTestBase
	{
		public DnxTestBase() : this("src.resources") { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="setOutput"></param>
		/// <param name="resourceBasePath">
		/// Resource path, default is "src.resources", if you want the root to be rather
		/// for ex: "src.resources.ical", then pass that in, allowing calls to ResouceString
		/// (etc) to be shortened. If you want to avoid any (extremely minor) cost of
		/// initing underlying EmbeddedResourcesHelper types when not needed, just pass in null here.</param>
		public DnxTestBase(string resourceBasePath) 
			: base(resourceBasePath.IsNulle() ? null : typeof(DnxTestBase), resourceBasePath)
		{
			CacheResourceGetsDefault = true;
		}
	}
}
