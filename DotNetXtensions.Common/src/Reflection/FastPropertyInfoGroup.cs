using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetXtensions.Reflection
{

	public class FastPropertyInfoGroup<TDeclaringType, TAttribute, TId>
		where TAttribute : Attribute
		where TDeclaringType : class
	{

		public PropertyInfo[] Properties { get; set; }

		public Type DeclaringType { get; set; }

		public Type CustomAttributeType { get; set; }

		public FastPropertyInfo<TDeclaringType, TAttribute>[] CustomProperties { get; set; }

		public Dictionary<TId, FastPropertyInfo<TDeclaringType, TAttribute>> CustomPropertiesDict;

		public object Tag1 { get; set; }

		public object Tag2 { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="getId">Func to get the id from a property. 
		/// Leave null to not have the dictionary set.</param>
		public FastPropertyInfoGroup(Func<FastPropertyInfo<TDeclaringType, TAttribute>, TId> getId = null)
		{
			DeclaringType = typeof(TDeclaringType).GetUnderlyingTypeOrSelfIfNullable();

			CustomAttributeType = typeof(TAttribute).GetUnderlyingTypeOrSelfIfNullable();

			Properties = typeof(TDeclaringType).GetProperties();

			CustomPropertiesDict = new Dictionary<TId, FastPropertyInfo<TDeclaringType, TAttribute>>();

			var cplist = new List<FastPropertyInfo<TDeclaringType, TAttribute>>();

			foreach (var pinfo in Properties) {
			
				TAttribute attr = pinfo.GetCustomAttribute(CustomAttributeType) as TAttribute;

				if (attr != null) {
					var fpi = pinfo.GetFastPropertyInfo<TDeclaringType, TAttribute>(attr);

					cplist.Add(fpi);

					if(getId != null)
						CustomPropertiesDict.Add(getId(fpi), fpi);
				}
			}
			CustomProperties = cplist.ToArray();
		}

	}

	public class FastPropertyInfoGroup<TDeclaringType, TAttribute>
		: FastPropertyInfoGroup<TDeclaringType, TAttribute, string>
		where TAttribute : Attribute
		where TDeclaringType : class
	{
		public FastPropertyInfoGroup(Func<FastPropertyInfo<TDeclaringType, TAttribute>, string> getId = null)
			: base(getId != null ? getId : fpi => fpi.Name) { }
	}
}