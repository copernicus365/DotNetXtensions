using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetXtensions.Reflection // Sdx.Malsy.FastInvoke
{
	/// <summary>
	/// From: http://flurfunk.sdx-ag.de/2012/05/c-performance-bei-der-befullungmapping.html
	/// Google translate: "If you like it I would be happy to hear about it. Send an email to "matthias.malsy" @ "sdx-ag.de"."
	/// </summary>
	public static class FastInvoke
	{
		public static Func<T, TReturn> GetFastGetter<T, TReturn>(this PropertyInfo propertyInfo)
		{
			MethodInfo mi = propertyInfo.GetMethod;
			Func<T, TReturn> reflGet = (Func<T, TReturn>)
				mi.CreateDelegate(typeof(Func<T, TReturn>));
			//Delegate.CreateDelegate(typeof(Func<T, TReturn>), propertyInfo.GetGetMethod());
			return reflGet;
		}

		public static Action<T, TProperty> GetFastSetter<T, TProperty>(this PropertyInfo propertyInfo)
		{
			MethodInfo mi = propertyInfo.SetMethod;
			Action<T, TProperty> reflSet = (Action<T, TProperty>)
				mi.CreateDelegate(typeof(Action<T, TProperty>));
				//Delegate.CreateDelegate(typeof(Action<T, TProperty>), propertyInfo.GetSetMethod());
			return reflSet;
		}

		public static FastPropertyInfoTyped<T, TProperty> GetFastPropertyInfoTyped<T, TProperty>(this PropertyInfo propertyInfo)
		{
			return new FastPropertyInfoTyped<T, TProperty>(propertyInfo);
		}

		public static FastPropertyInfo<T> GetFastPropertyInfo<T>(this PropertyInfo propertyInfo)
		{
			return new FastPropertyInfo<T>(propertyInfo);
		}

		public static FastPropertyInfo<T, TAttribute> GetFastPropertyInfo<T, TAttribute>(
			this PropertyInfo propertyInfo, TAttribute custAttr = null) where TAttribute : Attribute
		{
			return new FastPropertyInfo<T, TAttribute>(propertyInfo, custAttr);
		}


		public static Type GetUnderlyingTypeOrSelfIfNullable(this Type type)
		{
			if(type != null)
				return Nullable.GetUnderlyingType(type) ?? type;
			return type;
		}

		public static Action<T, object> GetFastUntypedSetter<T>(this PropertyInfo propertyInfo)
		{
			var targetType = propertyInfo.DeclaringType;
			var methodInfo = propertyInfo.GetSetMethod();
			var exTarget = Expression.Parameter(targetType, "t");
			var exValue = Expression.Parameter(typeof(object), "p");
			// wir betreiben ein an Object.SetPropertyValue(object)
			var exBody = Expression.Call(exTarget, methodInfo,
				Expression.Convert(exValue, propertyInfo.PropertyType));
			var lambda = Expression.Lambda<Action<T, object>>(exBody, exTarget, exValue);
			// (t, p) => t.set_StringValue(Convert(p))

			var action = lambda.Compile();
			return action;
		}

		public static Func<T, object> GetFastUntypedGetter<T>(this PropertyInfo propertyInfo)
		{
			var targetType = propertyInfo.DeclaringType;
			var methodInfo = propertyInfo.GetGetMethod();
			var returnType = methodInfo.ReturnType;

			var exTarget = Expression.Parameter(targetType, "t");
			var exBody = Expression.Call(exTarget, methodInfo);
			var exBody2 = Expression.Convert(exBody, typeof(object));

			var lambda = Expression.Lambda<Func<T, object>>(exBody2, exTarget);
			// t => Convert(t.get_Foo())

			var action = lambda.Compile();
			return action;
		}

		/// <summary>
		/// http://stackoverflow.com/a/491486/264031
		/// </summary>
		public static PropertyInfo GetProperty<TDeclaringType, TProperty>(this Expression<Func<TDeclaringType, TProperty>> selector)
		{
			Expression body = selector;
			if(body is LambdaExpression)
				body = ((LambdaExpression)body).Body;

			switch(body.NodeType) {
				case ExpressionType.MemberAccess:
					return (PropertyInfo)((MemberExpression)body).Member;
				default:
					throw new InvalidOperationException();
			}
		}



		public static FastPropertyInfoTyped<TDeclaringType, TProperty> GetFastPropertyInfoTyped<TDeclaringType, TProperty>(Expression<Func<TDeclaringType, TProperty>> selector)
		{
			var pinfo = selector.GetProperty();
			return pinfo == null ? null : pinfo.GetFastPropertyInfoTyped<TDeclaringType, TProperty>();
		}
		public static FastPropertyInfo<TDeclaringType> GetFastPropertyInfo<TDeclaringType, TProperty>(Expression<Func<TDeclaringType, TProperty>> selector)
		{
			var pinfo = selector.GetProperty();
			return pinfo == null ? null : pinfo.GetFastPropertyInfo<TDeclaringType>();
		}

	}

	public enum BasicBaseType
	{
		other,
		stringT,
		intT,
		boolT,
		datetimeT,
		datetimeoffsetT,
		longT,
		byteT,
		timespanT,
		charT,
	}

}