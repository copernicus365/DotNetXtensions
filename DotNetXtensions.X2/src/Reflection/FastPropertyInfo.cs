using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetXtensions.Reflection
{

	/// <summary>
	/// Wraps a PropertyInfo object while caching some key values from it 
	/// for fastest performance. This is mostly to be used as a base class
	/// for child FastPropertyInfo types, namely those which provide a cached
	/// getter and setter on this property.
	/// </summary>
	public class FastPropertyInfo
	{
		public FastPropertyInfo(PropertyInfo pinfo)
		{
			if (pinfo == null) throw new ArgumentNullException();
			PropInfo = pinfo;
			DeclaringType = pinfo.DeclaringType.GetUnderlyingTypeOrSelfIfNullable();
			PropertyType = pinfo.PropertyType.GetUnderlyingTypeOrSelfIfNullable();
			Name = pinfo.Name;

			BasicType = BasicBaseType.other;
			if (PropertyType == typeof(string))
				BasicType = BasicBaseType.stringT;
			else if (PropertyType == typeof(int))
				BasicType = BasicBaseType.intT;
			else if (PropertyType == typeof(bool))
				BasicType = BasicBaseType.boolT;
			else if (PropertyType == typeof(DateTime))
				BasicType = BasicBaseType.datetimeT;
			else if (PropertyType == typeof(DateTimeOffset))
				BasicType = BasicBaseType.datetimeoffsetT;
			else if (PropertyType == typeof(long))
				BasicType = BasicBaseType.longT;
			else if (PropertyType == typeof(byte))
				BasicType = BasicBaseType.byteT;
		}

		/// <summary>
		/// The source PropertyInfo DeclaringType (converts from Nullable if is a nullable type).
		/// This is *cached* on instantiation.
		/// (<code>DeclaringType = pinfo.DeclaringType.GetUnderlyingTypeOrSelfIfNullable();</code>)
		/// </summary>
		public Type DeclaringType { get; set; }

		/// <summary>
		/// The source PropertyInfo type (converts from Nullable if is a nullable type).
		/// This is *cached* on instantiation.
		/// (<code>PropertyType = pinfo.PropertyType.GetUnderlyingTypeOrSelfIfNullable();</code>)
		/// </summary>
		public Type PropertyType { get; set; }

		public BasicBaseType BasicType { get; set; }

		/// <summary>
		/// The source Property name (cached to ensure as fast as possible).
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The source PropertyInfo which this FastPropertyInfo wraps.
		/// </summary>
		public PropertyInfo PropInfo { get; private set; }
	}

	public class FastPropertyInfo<TDeclaringType> : FastPropertyInfo
	{
		public FastPropertyInfo(PropertyInfo pinfo)
			: base(pinfo)
		{
			Getter = pinfo.GetFastUntypedGetter<TDeclaringType>();
			Setter = pinfo.GetFastUntypedSetter<TDeclaringType>();
		}

		public Action<TDeclaringType, object> Setter { get; set; }

		public Func<TDeclaringType, object> Getter { get; set; }
	}

	public class FastPropertyInfo<TDeclaringType, TAttribute> 
		: FastPropertyInfo<TDeclaringType> 
		where TAttribute : Attribute
	{
		public FastPropertyInfo(PropertyInfo pinfo, TAttribute attr = null)
			: base(pinfo)
		{
			CustAttribute = attr;
		}

		public TAttribute CustAttribute { get; set; }
	}

	// --- FastPropertyInfoTyped ---

	/// <summary>
	/// This is a strongly typed version which allows the getter and setter to receive/return
	/// a strongly typed type instead of object.
	/// </summary>
	/// <typeparam name="TDeclaringType">The type of the class that declares this member.</typeparam>
	/// <typeparam name="TPropertyType">The property type.</typeparam>
	public class FastPropertyInfoTyped<TDeclaringType, TPropertyType> : FastPropertyInfo
	{
		public FastPropertyInfoTyped(PropertyInfo pinfo)
			: base(pinfo)
		{
			Getter = pinfo.GetFastGetter<TDeclaringType, TPropertyType>();
			Setter = pinfo.GetFastSetter<TDeclaringType, TPropertyType>();
		}

		public Action<TDeclaringType, TPropertyType> Setter { get; set; }

		public Func<TDeclaringType, TPropertyType> Getter { get; set; }
	}

	public class FastPropertyInfoTyped<TDeclaringType, TPropertyType, TAttribute> 
		: FastPropertyInfoTyped<TDeclaringType, TPropertyType> 
		where TAttribute : Attribute
	{
		public FastPropertyInfoTyped(PropertyInfo pinfo, TAttribute attr = null)
			: base(pinfo)
		{
			CustAttribute = attr;
		}

		public TAttribute CustAttribute { get; set; }
	}

	// --- FastPropertyInfoTypedWParent ---

	public class FastPropertyInfoTypedWParent<TDeclaringType, TPropertyType> : FastPropertyInfoTyped<TDeclaringType, TPropertyType>
	{
		public Func<TDeclaringType> getParentObj { get; set; }

		public FastPropertyInfoTypedWParent(PropertyInfo pinfo, Func<TDeclaringType> getParentObj = null)
			: base(pinfo)
		{
			if (getParentObj != null)
				this.getParentObj = getParentObj;
		}

		public TPropertyType Obj 
		{
			get
			{
				var obj = getParentObj();
				if (obj != null)
					return this.Getter(obj);
				return default(TPropertyType);
			}
			set 
			{
				var obj = getParentObj();
				if (obj != null)
					this.Setter(obj, value);
			}
		}


	}

}