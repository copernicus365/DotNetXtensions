using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Extension methods for Type.
	/// </summary>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XType
	{
		/// <summary>
		/// If the input type is a Nullable T, gets the underlying type T.
		/// Else returns null. This is very performant, taking 0.4 of a microsecond in our tests.
		/// </summary>
		/// <param name="type">Type</param>
		public static Type GetUnderlyingTypeIfNullable(this Type type)
		{
			if(type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				//&& type.FullName.StartsWith("System.Nullable") // this was actually a tiny bit slower! .5 microseconds over .4 ms
				Type underType = type.GenericTypeArguments.Single();
				return underType;
			}
			return null;
		}

		/// <summary>
		/// From: http://stackoverflow.com/a/29379834/264031
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			if(assembly == null) throw new ArgumentNullException("assembly");
			try {
				return assembly.GetTypes();
			}
			catch(ReflectionTypeLoadException e) {
				return e.Types.Where(t => t != null);
			}
		}

		/// <summary>
		/// Gets types that implement this interface within the input Assembly.
		/// From: http://stackoverflow.com/a/29379834/264031
		/// </summary>
		/// <param name="interfaceType">Interface type, must be an interface.</param>
		/// <param name="assembly">Assembly</param>
		public static IEnumerable<Type> GetTypesImplementingThisInterface(this Type interfaceType, Assembly assembly)
		{
			if(interfaceType == null || assembly == null)
				return null;

			if(!interfaceType.IsInterface)
				throw new ArgumentException("Input type must be the type of an interface.");

			return assembly.GetLoadableTypes().Where(interfaceType.IsAssignableFrom).ToList();
		}

		/// <summary>
		/// Gets types that implement this interface within the assembly in which the type 
		/// was declared. To get others implemented outside of the declaring assembly see the 
		/// overload where the Assembly can be passed in.
		/// </summary>
		/// <param name="interfaceType">Interface type, must be an interface.</param>
		public static IEnumerable<Type> GetTypesImplementingThisInterface(this Type interfaceType)
		{
			if(interfaceType == null)
				return null;

			var assembly = Assembly.GetAssembly(interfaceType);

			if(assembly == null)
				return null;

			return interfaceType.GetTypesImplementingThisInterface(assembly);
		}

	}
}