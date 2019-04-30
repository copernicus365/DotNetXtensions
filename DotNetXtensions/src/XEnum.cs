using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#if !DNXPrivate
namespace DotNetXtensions
{
	/// <summary>
	/// Inspired by:
	/// http://computing-tombarreras.blogspot.com/2009/02/fast-enumeration-to-string-conversion.html
	/// http://stackoverflow.com/questions/1414277/simple-form-of-array-class-and-enum-getvalues
	/// https://code.google.com/p/unconstrained-melody/
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public
#else
namespace DotNetXtensionsPrivate
{
#endif
	static class XEnum<T> where T : struct, IConvertible
	{
		// FIELDS
		static int m_Count;
		static bool m_IsSequential;
		//static bool m_CaseInsensitive;
		static string[] m_Names;
		static T[] m_Values;
		static int[] m_NValues;
		static Dictionary<string, T> m_NamesDict;
		static Dictionary<string, T> m_NamesDictCaseInsens;
		static Dictionary<int, string> m_ValuesDict;
		static Dictionary<T, string> m_TDict;

		// GET PROPERTIES
		public static int Count { get { return m_Count; } }
		public static bool IsSequential { get { return m_IsSequential; } }
		public static string[] Names { get { return m_Names; } }
		public static int[] NumericValues { get { return m_NValues; } }
		public static T[] Values { get { return m_Values; } }
		public static Dictionary<string, T> NamesDict {
			get { return m_NamesDict; }
		}
		public static Dictionary<int, string> ValuesDict {
			get { return m_ValuesDict; }
		}
		public static Dictionary<T, string> EnumDict {
			get { return m_TDict; }
		}

		public static bool CaseInsensitive { get; set; } = true;

		//{
		//	get { return m_CaseInsensitive; }
		//	set {
		//		if(value != m_CaseInsensitive) {
		//			m_CaseInsensitive = value;
		//			m_NamesDict = new Dictionary<string, T>(NamesDict, value ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
		//		}
		//	}
		//}

		/// <summary>
		/// STATIC CONSTRUCTOR. Initializes type.
		/// </summary>
		static XEnum()
		{
			INIT();
		}

		static void INIT(T[] values = null, string[] names = null, Func<string, string> transformNames = null, Func<T, string, string> transformNames2 = null)
		{
			if(values != null && names != null && values.Length == names.Length) {
				m_Values = values;
				m_Count = m_Values.Length;
				m_NValues = new int[m_Count];
				m_Names = names;
			}
			else {
				Type t = typeof(T);
				if(!t.IsEnum)
					throw new Exception("Generic type must be an enumeration");

				TypeName = t.Name;

				m_Values = (T[])Enum.GetValues(t);
				m_Count = m_Values.Length;
				m_NValues = new int[m_Count];
				m_Names = new string[m_Count];
				for(int i = 0; i < m_Count; i++)
					m_Names[i] = m_Values[i].ToString();
			}

			if(transformNames != null) {
				for(int i = 0; i < m_Count; i++)
					m_Names[i] = transformNames(m_Names[i]);
			}
			else if(transformNames2 != null) {
				for(int i = 0; i < m_Count; i++)
					m_Names[i] = transformNames2(m_Values[i], m_Names[i]);
			}

			for(int i = 0; i < m_Count; i++)
				m_NValues[i] = m_Values[i].ToInt32(CultureInfo.InvariantCulture);

			m_IsSequential = __IsSequential(m_NValues);

			m_NamesDict = new Dictionary<string, T>(m_Count);
			m_ValuesDict = new Dictionary<int, string>(m_Count);
			m_TDict = new Dictionary<T, string>(m_Count);

			for(int i = 0; i < m_Count; i++) {
				T val = m_Values[i];
				string nm = m_Names[i];
				int iVal = m_NValues[i];

				if(!m_NamesDict.ContainsKey(nm))
					m_NamesDict.Add(nm, val);
				if(!m_ValuesDict.ContainsKey(iVal))
					m_ValuesDict.Add(iVal, nm);
				if(!m_TDict.ContainsKey(val))
					m_TDict.Add(val, nm);
			}
			m_NamesDictCaseInsens = new Dictionary<string, T>(m_NamesDict, StringComparer.OrdinalIgnoreCase);
		}

		public static void SetNames(Dictionary<T, string> names)
		{
			if(names == null) return;
			INIT(names.Keys.ToArray(), names.Values.ToArray());
		}

		public static void SetNames(KeyValuePair<T, string>[] names)
		{
			if(names == null) return;
			INIT(names.Select(kv => kv.Key).ToArray(), names.Select(kv => kv.Value).ToArray()); // .Keys.ToArray(), names.Values.ToArray());
		}

		public static void SetNames(Func<string, string> transformNames)
		{
			if(transformNames == null) return;
			INIT(null, null, transformNames);
		}

		public static void SetNames(Func<T, string, string> transformNames)
		{
			if(transformNames == null) return;
			INIT(null, null, null, transformNames);
		}

		public static string TypeName { get; set; }

		public static string Name(int val)
		{
			if(m_IsSequential)
				return m_Names[val];
			else {
				if(m_Count < 10) {
					// at about 10, in our perf tests GetIndexOfNonSequentialValue (which sequentially
					// iterates m_NValues) becomes slower than a dict lookup, under that faster, 
					// up to twice as fast e.g. with 4 values, which is most common enums under 6
					int idx = GetIndexOfNonSequentialValue(val);
					return idx > -1 ? m_Names[idx] : null;
				}
				else {
					string name;
					m_ValuesDict.TryGetValue(val, out name);
					return name;
				}
			}
		}

		public static string Name(T val)
		{
			return m_TDict[val];
		}

		/// <summary>
		/// Get the value from the internal dictionary indexer, so will throw if
		/// not there. Use the overload where you can enter a default value or else
		/// use TryGetValue.
		/// </summary>
		/// <param name="name">The string name of the value to get.</param>
		/// <param name="caseInsensitive">If set this value will override the default value
		/// that has been set for this type: <see cref="CaseInsensitive"/>. From there
		/// we draw upon either:
		/// <see cref="m_NamesDictCaseInsens"/> or <see cref="m_NamesDict"/> accordingly.</param>
		public static T Value(string name, bool? caseInsensitive = null)
		{
			bool _caseInsensitive = caseInsensitive ?? CaseInsensitive;
			return _caseInsensitive
				? m_NamesDictCaseInsens[name]
				: m_NamesDict[name];
		}

		public static T Value(string name, T defaultValue, bool? caseInsensitive = null)
		{
			bool _caseInsensitive = caseInsensitive ?? CaseInsensitive;
			return _caseInsensitive
				? m_NamesDictCaseInsens.V(name, defaultValue)
				: m_NamesDict.V(name, defaultValue);
		}

		public static T V(string name, T defaultValue = default(T), bool? caseInsensitive = null)
		{
			if (name.IsNulle()) return defaultValue;
			return Value(name, defaultValue, caseInsensitive);
		}

		public static T ValueIgnoreCase(string name)
		{
			return m_NamesDictCaseInsens.V(name);
		}

		private static int GetIndexOfNonSequentialValue(int val)
		{
			for(int i = 0; i < m_Count; i++)
				if(m_NValues[i] == val)
					return i;
			return -1;
		}

		public static bool TryGetValue(string name, out T value)
		{
			return CaseInsensitive
				? m_NamesDictCaseInsens.TryGetValue(name, out value)
				: m_NamesDict.TryGetValue(name, out value);
		}

		public static bool TryGetValueIgnoreCase(string name, out T value)
		{
			return m_NamesDictCaseInsens.TryGetValue(name, out value);
		}

		static bool __IsSequential(int[] nvalues)
		{
			int[] nvals = nvalues.ToArray();

			// CRITICAL BUG FIX!! Found a major bug on 2014/04 
			// (using DotNetXtension.GeoNames State and Country enums)
			// where things did not work as expected, even though the enums
			// had an explicit value set on them. I suspect I may have tested
			// this originally where the test arrays inputed used an OrderedDictionary
			// (this got changed when I dumped our buggy version of that).
			// Anyways, for now on we do NOT sort these values first.
			// IsSequential only if the values **as inputed** start at 0 on up
			// with no gaps
			//Array.Sort(nvals); // NO MORE!!!!!!!

			for(int i = 0; i < nvals.Length; i++) {
				if(nvals[i] != i)
					return false;
			}
			return true;
		}

		public static string EnumToString(bool twoLines = false)
		{
			return !twoLines
				? $"{TypeName}: {{ {Names.JoinToString(", ")} }}"
				: $@"{TypeName}
{{ {Names.JoinToString(", ")} }}";
		}

	}

	public static class XEnum
	{
		// =-=-=-=-=-=-=-=

		public static string GetEnumCode(string enumName, string[] values)
		{
			var b = new StringBuilder(4096);

			b.AppendLine(@"// auto generated code

using System;
using System.Collections.Generic;
using DotNetXtensions;
using DotNetXtensions.Globalization;

");

			b.AppendFormat("public enum {0} {{\r\n", enumName);

			int len = values.Length;
			var kvs = new KeyValuePair<string, string>[len];

			for(int i = 0; i < len; i++) {
				string name = values[i];
				string codeName = name.Replace(c => c.IsAsciiLetterOrDigit() ? c : (c == ' ' ? '_' : char.MinValue));
				kvs[i] = new KeyValuePair<string, string>(codeName, name);
				b.AppendMany("\t", codeName, " = ", i.ToString(), ",\r\n");
			}
			b.Length = b.Length - 3; // cut last: {,\r\n}

			b.AppendLine("}");
			b.AppendLine();

			// =========GET INIT CODE========

			b.AppendFormat("static class {0}_TempClassReplace {{\r\n\r\n", enumName);
			b.AppendLine("// Place this Init function in the associated class, and call it from some init code.");
			b.AppendFormat("private static void Init{0}Enum() {{\r\n", enumName);
			b.AppendFormat("var _names = new Dictionary<{0}, string>({1}) {{\r\n", enumName, len);

			for(int i = 0; i < len; i++) {
				string codeName = kvs[i].Key;
				string name = kvs[i].Value;
				b.AppendMany("{ ", enumName, ".", codeName, ", \"", name, "\" },\r\n");
			}
			b.Length = b.Length - 3; // cut last: {,\r\n}

			b.AppendLine("\r\n};\r\n");

			b.AppendFormat("XEnum<{0}>.SetNames(_names);\r\n", enumName);

			b.AppendLine("}");
			b.AppendLine();

			// we now have the enum code

			string enumConverterStr = string.Format(
@"
		static bool _init_{0};
		public static {0} To{0}(string value)
		{{
			if(!_init_{0}) {{ _init_{0} = true; Init{0}Enum(); }}
			return XEnum<{0}>.Value(value);
		}}

		public static string ToStringFast(this {0} value)
		{{
			if(!_init_{0}) {{ _init_{0} = true; Init{0}Enum(); }}
			return XEnum<{0}>.Name((int)value);
		}}
", enumName);

			b.AppendLine(enumConverterStr);
			b.AppendLine("}");
			b.AppendLine();

			return b.ToString();
		}

		public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue)
			where TEnum : struct
		{
			if(value.IsNulle())
				return defaultValue;

			TEnum result;
			return Enum.TryParse(value, true, out result)
				? result
				: defaultValue;
		}

		public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
		{
			if(value.IsNulle())
				return default(TEnum);

			TEnum result;
			return Enum.TryParse(value, true, out result)
				? result
				: default(TEnum);
		}

		public static TEnum? ToEnumN<TEnum>(this string value) where TEnum : struct
		{
			if(value.IsNulle())
				return null;

			TEnum result;
			if(Enum.TryParse(value, true, out result))
				return result;
			return null;
		}

	}
}
