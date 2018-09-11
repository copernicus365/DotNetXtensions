using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Contains XML type functions and extensions.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XML
	{
		#region LegalXmlChars

		/// <summary>
		/// Determines if any invalid XML 1.0 characters exist within the string, using 
		/// XML.IsLegalXmlChar method, and if so it returns a new string with the invalid chars removed, 
		/// else the same string is returned (with no wasted StringBuilder allocated, etc).
		/// If string is null or empty immediately returns the same value. 
		/// </summary>
		/// <param name="s">Xml string.</param>
		/// <param name="index">The index to begin checking at.</param>
		public static string ToValidXmlCharactersString(this string s, int index = 0)
		{
			if(s.IsNulle())
				return s;

			int firstInvalidChar = IndexOfFirstInvalidXMLChar(s, index);
			if (firstInvalidChar < 0)
				return s;

			index = firstInvalidChar;

            int len = s.Length;
			var sb = new StringBuilder(len);

			if (index > 0)
				sb.Append(s, 0, index);

			for (int i = index; i < len; i++)
				if (IsLegalXmlChar(s[i]))
					sb.Append(s[i]);

			return sb.ToString();
		}

		/// <summary>
		/// Gets the index of the first invalid XML 1.0 character in this string, else returns -1.
		/// </summary>
		/// <param name="s">Xml string.</param>
		/// <param name="index">Start index.</param>
		public static int IndexOfFirstInvalidXMLChar(string s, int index = 0)
		{
			if (s != null && s.Length > 0 && index < s.Length) {

				if (index < 0) index = 0;
				int len = s.Length;

				for (int i = index; i < len; i++)
					if (!IsLegalXmlChar(s[i])) // inlining aspects of this check might add even more perf, but let us trust the framework
						return i;
			}
			return -1;
		}

		/// <summary>
		/// Indicates whether a given character is valid according to the XML 1.0 spec.
		/// This code represents an optimized version of Tom Bogle's on SO: 
		/// http://stackoverflow.com/a/13039301/264031.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsLegalXmlChar(this char c)
		{
			if (c > 31 && c <= 55295)
				return true;
			if (c < 32)
				return c == 9 || c == 10 || c == 13;
			return (c >= 57344 && c <= 65533) || c > 65535;
			// final comparison is useful only for integral comparison, if char c -> int c, useful for utf-32 I suppose
			//c <= 1114111 */ // impossible to get a code point bigger than 1114111 because Char.ConvertToUtf32 would have thrown an exception
		}

		#endregion

		#region ToStringFormatted

		public static string ToStringFormatted(this XElement xml, bool indent = true, bool newLineOnAttributes = false, string indentChars = "\t", bool omitXmlDeclaration = true)
		{
			if (xml == null) return null;

			XmlWriterSettings settings = new XmlWriterSettings() {
				Indent = indent,
				NewLineOnAttributes = newLineOnAttributes,
				OmitXmlDeclaration = omitXmlDeclaration,
				IndentChars = indentChars
			};

			return xml.ToStringFormatted(settings);
		}

		public static string ToStringFormatted(this XElement xml, XmlWriterSettings settings)
		{
			if (xml == null) return null;

			StringBuilder result = new StringBuilder();
			using (XmlWriter writer = XmlWriter.Create(result, settings)) {
				xml.WriteTo(writer);
			}
			return result.ToString();
		}

		/// <summary>
		/// Writes this XML to string while allowing invalid XML chars to either be
		/// simply removed during the write process, or else encoded into entities, 
		/// instead of having an exception occur, as the standard XmlWriter.Create 
		/// XmlWriter does (which is the default writer used by XElement).
		/// </summary>
		/// <param name="xml">XElement.</param>
		/// <param name="deleteInvalidChars">True to have any invalid chars deleted, else they will be entity encoded.</param>
		/// <param name="indent">Indent setting.</param>
		/// <param name="indentChar">Indent char (leave null to use default)</param>
		public static string ToStringIgnoreInvalidChars(this XElement xml, bool deleteInvalidChars = true, bool indent = true, char? indentChar = null)
		{
			if (xml == null) return null;

			StringWriter swriter = new StringWriter();
			using (XmlTextWriterIgnoreInvalidChars writer = new XmlTextWriterIgnoreInvalidChars(swriter, deleteInvalidChars)) {

				// -- settings --
				// unfortunately writer.Settings cannot be set, is null, so we can't specify: bool newLineOnAttributes, bool omitXmlDeclaration
				writer.Formatting = indent ? Formatting.Indented : Formatting.None;

				if (indentChar != null)
					writer.IndentChar = (char)indentChar;
			
				// -- write --
				xml.WriteTo(writer); 
			}

			return swriter.ToString();
		}

		#endregion ToStringFormatted

		#region GetNamespaceIgnorantXElement

		/// <summary>
		/// Allows one to serialize an XElement from a XML document that has a
		/// root xmlns namespace set, but where that root namespace gets highly
		/// performantly set to empty, which allows one to work with that XElement
		/// in the simpler, namespace ignorant manner when performant LINQ to XML queries.
		/// This is done via RootNamespaceIgnorantXmlTextReader, so it is very performant,
		/// whereas ClearDefaultNamespace must only after serialization traverse every
		/// element in the document and (when needed) change that elements XName.
		/// See RootNamespaceIgnorantXmlTextReader documentation for more details.
		/// </summary>
		/// <param name="xmlString">String of XML.</param>
		/// <param name="loadOptions"></param>
		public static XElement GetNamespaceIgnorantXElement(string xmlString, LoadOptions loadOptions = LoadOptions.None)
		{
			return RootNamespaceIgnorantXmlTextReader.GetNamespaceIgnorantXElement(xmlString);
		}

		/// <summary>
		/// See overload's documentation.
		/// </summary>
		/// <param name="xml">XML data.</param>
		/// <param name="loadOptions"></param>
		public static XElement GetNamespaceIgnorantXElement(byte[] xml, LoadOptions loadOptions = LoadOptions.None)
		{
			return RootNamespaceIgnorantXmlTextReader.GetNamespaceIgnorantXElement(xml);
		}

		#endregion

		///// <summary>
		///// Calls TextFuncs.ClearXmlTags, see notes there.
		///// </summary>
		//public static string ClearXmlTags(string value)
		//{
		//	return TextFuncs.ClearXmlTags(value);
		//}

		#region SERIALIZE

		/// <summary>
		/// Serializes the object with XmlSerializer, with NO namespaces.
		/// </summary>
		public static XElement Serialize<T>(T obj, bool changeElementsToAttributes = false)
		{
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			XmlSerializer xs = new XmlSerializer(typeof(T));

			XDocument doc = new XDocument();
			using (XmlWriter xw = doc.CreateWriter())
				xs.Serialize(xw, obj, ns);

			XElement x = doc.Root;

			if (changeElementsToAttributes) {
				foreach (var e in x.Elements()) {
					x.Add(new XAttribute(e.Name, e.Value));
				}
				x.Elements().Remove();
			}

			return x;
		}

		/// <summary>
		/// Deserializes the object with XmlSerializer, with no namespaces. 
		/// </summary>
		public static T Deserialize<T>(this XElement x, bool changeElementsToAttributes = false) where T : class
		{
			if (changeElementsToAttributes) {
				foreach (var a in x.Attributes()) {
					x.Add(new XElement(a.Name, a.Value));
				}
				x.Attributes().Remove();
			}
			XmlSerializer xs = new XmlSerializer(typeof(T));
			T pDeserialized = xs.Deserialize((x.CreateReader())) as T;
			return pDeserialized;
		}

		#endregion

		/// <summary>
		/// Returns null if <paramref name="value"/> is null,
		/// else returns a new XAttribute with the value set accordingly.
		/// This can help in creating much cleaner XML elements
		/// where conditional attributes are to be only included
		/// if the value is not null. This way one can do this inline
		/// within the functional creation of the XElement, rather than 
		/// having to add these afterwards with if statements. This works
		/// because null objects that are added to the constructor 
		/// of an XElement are simply ignored (*thankfully*).
		/// <code>
		/// <![CDATA[
		/// string lastName = null;
		/// XElement x = new XElement("cool",
		///		new XAttribute("firstName", "Joey"),
		///		XML.AttributeN("lastName", name)); // will only add a 'lastName' attribute if lastName is not null
		/// ]]>
		/// </code>
		/// </summary>
		/// <param name="name">XName</param>
		/// <param name="value">Value. If null, null will be returned.</param>
		public static XAttribute AttributeN(XName name, object value)
		{
			if (value == null) return null;
			return new XAttribute(name, value);
		}

		public static XAttribute AttributeN(this XElement elem, XName attrName)
		{
			if (elem == null)
				return null;
			return elem.Attribute(attrName);
		}

		/// <summary>
		/// Returns null if <paramref name="value"/> is null,
		/// else returns a new XElement with the value set accordingly.
		/// See notes on XML.AttributeN for more information.
		/// </summary>
		/// <param name="name">XName</param>
		/// <param name="value">Value. If null, null will be returned.</param>
		public static XElement ElementN(XName name, object value)
		{
			if (value == null) return null;
			return new XElement(name, value);
		}




		#region ToTypeOrDefault

		[DebuggerStepThrough]
		public static string ValueN(this XElement e)
		{
			if (e == null) return null;
			return (string)e;
		}

		[DebuggerStepThrough]
		public static string ValueN(this XAttribute e)
		{
			if (e == null) return null;
			return (string)e;
		}



		// -------------- ToInt

		[DebuggerStepThrough]
		public static int ToInt(this XElement e, int defaultVal = 0)
		{
			return fix(e).ToInt(defaultVal);
		}

		[DebuggerStepThrough]
		public static int ToInt(this XAttribute e, int defaultVal = 0)
		{
			return fix(e).ToInt(defaultVal);
		}

		[DebuggerStepThrough]
		public static int? ToIntN(this XElement e)
		{
			return fix(e).ToIntN();
		}

		[DebuggerStepThrough]
		public static int? ToIntN(this XAttribute e)
		{
			return fix(e).ToIntN();
		}



		// -------------- ToLong

		[DebuggerStepThrough]
		public static long ToLong(this XElement e, long defaultVal = 0)
		{
			return fix(e).ToLong(defaultVal);
		}

		[DebuggerStepThrough]
		public static long ToLong(this XAttribute e, long defaultVal = 0)
		{
			return fix(e).ToLong(defaultVal);
		}

		[DebuggerStepThrough]
		public static long? ToLongN(this XElement e)
		{
			return fix(e).ToLongN();
		}

		[DebuggerStepThrough]
		public static long? ToLongN(this XAttribute e)
		{
			return fix(e).ToLongN();
		}




		// -------------- ToDecimal

		[DebuggerStepThrough]
		public static decimal ToDecimal(this XElement e, decimal defaultVal = 0)
		{
			return fix(e).ToDecimal(defaultVal);
		}

		[DebuggerStepThrough]
		public static decimal ToDecimal(this XAttribute e, decimal defaultVal = 0)
		{
			return fix(e).ToDecimal(defaultVal);
		}

		[DebuggerStepThrough]
		public static decimal? ToDecimalN(this XElement e)
		{
			return fix(e).ToDecimalN();
		}

		[DebuggerStepThrough]
		public static decimal? ToDecimalN(this XAttribute e)
		{
			return fix(e).ToDecimalN();
		}




		// -------------- ToDouble

		[DebuggerStepThrough]
		public static double ToDouble(this XElement e, double defaultVal = 0)
		{
			return fix(e).ToDouble(defaultVal);
		}

		[DebuggerStepThrough]
		public static double ToDouble(this XAttribute e, double defaultVal = 0)
		{
			return fix(e).ToDouble(defaultVal);
		}

		[DebuggerStepThrough]
		public static double? ToDoubleN(this XElement e)
		{
			return fix(e).ToDoubleN();
		}

		[DebuggerStepThrough]
		public static double? ToDoubleN(this XAttribute e)
		{
			return fix(e).ToDoubleN();
		}





		// -------------- ToDateTime

		private static readonly DateTime _dtDefault = default(DateTime);
		private static readonly DateTimeOffset _dtoffDefault = default(DateTimeOffset);

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XElement e)
		{
			return fix(e).ToDateTime(_dtDefault);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XElement e, DateTime defaultVal)
		{
			return fix(e).ToDateTime(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XAttribute e)
		{
			return fix(e).ToDateTime(_dtDefault);
		}

		[DebuggerStepThrough]
		public static DateTime ToDateTime(this XAttribute e, DateTime defaultVal)
		{
			return fix(e).ToDateTime(defaultVal);
		}


		[DebuggerStepThrough]
		public static DateTime? ToDateTimeN(this XElement e)
		{
			return fix(e).ToDateTimeN();
		}

		[DebuggerStepThrough]
		public static DateTime? ToDateTimeN(this XAttribute e)
		{
			return fix(e).ToDateTimeN();
		}




		// -------------- ToDateTimeOffset

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XElement e)
		{
			return fix(e).ToDateTimeOffset(_dtoffDefault);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XElement e, DateTimeOffset defaultVal)
		{
			return fix(e).ToDateTimeOffset(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XAttribute e)
		{
			return fix(e).ToDateTimeOffset(_dtoffDefault);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset ToDateTimeOffset(this XAttribute e, DateTimeOffset defaultVal)
		{
			return fix(e).ToDateTimeOffset(defaultVal);
		}

		[DebuggerStepThrough]
		public static DateTimeOffset? ToDateTimeOffsetN(this XElement e)
		{
			return fix(e).ToDateTimeOffsetN();
		}

		[DebuggerStepThrough]
		public static DateTimeOffset? ToDateTimeOffsetN(this XAttribute e)
		{
			return fix(e).ToDateTimeOffsetN();
		}




		// -------------- ToBool

		[DebuggerStepThrough]
		public static bool ToBool(this XElement e, bool defaultVal = false)
		{
			return fix(e).ToBool(defaultVal);
		}

		[DebuggerStepThrough]
		public static bool ToBool(this XAttribute e, bool defaultVal = false)
		{
			return fix(e).ToBool(defaultVal);
		}

		[DebuggerStepThrough]
		public static bool? ToBoolN(this XElement e)
		{
			return fix(e).ToBoolN();
		}

		[DebuggerStepThrough]
		public static bool? ToBoolN(this XAttribute e)
		{
			return fix(e).ToBoolN();
		}






		private static string fix(XElement e)
		{
			if (e == null)
				return null;
			string val = e.Value;
			return string.IsNullOrWhiteSpace(val) ? null : val;
		}

		private static string fix(XAttribute e)
		{
			if (e == null)
				return null;
			string val = e.Value;
			return string.IsNullOrWhiteSpace(val) ? null : val;
		}



		#endregion



		#region SetDefaultNamespace / ClearDefaultNamespace

		/// <summary>
		/// Sets a default namespace by iterating over all elements (and self)
		/// and setting the namespace on each one that is empty.
		/// </summary>
		/// <param name="elem">Element</param>
		/// <param name="ns">Namespace</param>
		public static void SetDefaultNamespace(this XElement elem, XNamespace ns)
		{
			SetDefaultNamespace(elem, ns, false);
		}

		/// <summary>
		/// Sets a default namespace by iterating over all elements (and self)
		/// and setting the namespace on each one that is empty, or on all if
		/// <paramref name="overwriteAll"/> is true.
		/// </summary>
		/// <param name="elem">Element</param>
		/// <param name="ns">Namespace</param>
		/// <param name="overwriteAll">True to replace all namespaces.
		/// Will be a bit more performant as the namespace for each element does
		/// not have to be compared upon first.</param>
		public static void SetDefaultNamespace(this XElement elem, XNamespace ns, bool overwriteAll)
		{
			foreach (XElement e in elem.DescendantsAndSelf())
				if (overwriteAll || e.Name.Namespace.NamespaceName == "")
					e.Name = ns + e.Name.LocalName; //e.Name = ns.GetName(head.Name.LocalName);
		}

		/// <summary>
		/// If the input XElement has an xmlns namespace value set other than empty ("" - none),
		/// this function sets that namespace value to "", and then iterates through all
		/// descendant elements setting their namespace values to empty as well *if* they too
		/// had the same namespace as the root (input) XElement. The main reason for this
		/// function is to address one of the worse aspects of XElement -- simply having a
		/// 'xmlns' value set on the root of the document suddenly
		/// means you always have to specify namespaces when doing LINQ to XML queries, even if
		/// you only have the one default namespace.
		/// <para/>
		/// This function first determines if the namespace on the input XElement is an empty
		/// namespace (""), and if so, it quickly returns. In those cases where no root xmlns
		/// namespace is set, this allows one to call this function without a performance hit.
		/// </summary>
		/// <param name="elem">XElement</param>
		public static bool ClearDefaultNamespace(this XElement elem)
		{
			string rootNSName = elem.Name.NamespaceName;
			if (rootNSName == null || rootNSName == "")
				return false;

			XAttribute rootXmlnsAtt = elem.Attribute("xmlns"); // .SetAttributeValue("xmlns", "")
			if (rootXmlnsAtt != null)
				rootXmlnsAtt.Remove();

			XNamespace ns = XNamespace.None;

			elem.Name = ns + elem.Name.LocalName;

			foreach (XElement e in elem.Descendants()) {
				XName name = e.Name;
				if (name.Namespace.NamespaceName == rootNSName)
					e.Name = ns + name.LocalName;
			}
			return true;
		}

		#endregion

	}

	#region Classes XmlTextWriterIgnoreInvalidChars and RootNamespaceIgnorantXmlTextReader

	/// <summary>
	/// An XmlTextReader that treats all element namespaces that match the
	/// root xml's xmlns namespace as an empty string, which in essense then
	/// acts to set the XML's xmlns as it was empty all along (without, however,
	/// deleting the root element's xmlns attribute value - when there is one -
	/// which is not possible with the readonly XmlTextReader).
	/// <para />
	/// The reason for making this type was for using it in serializing an
	/// XElement so as to be able to essentially ignore the root xmlns
	/// value. Doing that alleviates one of the greatest pains associated with
	/// the otherwise excellent XElement - being able to easily do LINQ to XML queries
	/// without messing with namespaces, particularly in those cases where one
	/// knows that the XML document they are working on simply needed a root xmlns root
	/// value set.
	/// <para />
	/// One must remember to clear the XElement's root xmlns attribute value to empty ("")
	/// when there is one. And on that note, it should be noted that if any other elements
	/// in the document had an xmlns value set that is the same as the root value, that
	/// they would also have to clear those attribute values to empty. Otherwise, those XElements
	/// will report errors when trying to serialize (call ToString) the XElement. This is however
	/// a fringe case scenario, as in most cases, the root  namespace will be specified once
	/// in the root, and not in child elements thereforth.
	/// <example><code>
	/// XElement e = XElement.Load(new RootNamespaceIgnorantXmlTextReader(xmlString).Init());
	/// if (e.Attribute("xmlns") != null)
	/// 	e.Attribute("xmlns").Remove();
	/// </code></example>
	/// </summary>
#if DNXPublic
	public
#endif
	class RootNamespaceIgnorantXmlTextReader : XmlTextReader
	{
		public const LoadOptions DefLoadOptions = LoadOptions.None;

		#region CONSTRUCTORS

		public RootNamespaceIgnorantXmlTextReader(Uri uri)
			: base(uri.OriginalString) { }

		public RootNamespaceIgnorantXmlTextReader(System.IO.TextReader reader)
			: base(reader) { }

		public RootNamespaceIgnorantXmlTextReader(string xmlText)
			: base(new StringReader(xmlText)) { }

		public RootNamespaceIgnorantXmlTextReader(System.IO.Stream stream)
			: base(stream) { }

		public RootNamespaceIgnorantXmlTextReader(byte[] bytes)
			: base(new System.IO.MemoryStream(bytes)) { }

		#endregion CONSTRUCTORS


		/// <summary>
		/// MUST call this Init method after constructor is called (new object). We
		/// could not run the following logic in the constructor because it requires calling
		/// a virtual method in the base class, which is not good, (see why here: https://msdn.microsoft.com/en-us/library/ms182331.aspx)
		/// ---
		/// The only need is to record the root Xml xmlns namespace value, if there is one.
		/// This does require, it must be noted, a call to MoveToContent.
		/// </summary>
		public RootNamespaceIgnorantXmlTextReader Init()
		{
			MoveToContent();

			string rootNS = base.NamespaceURI;
			if(rootNS == null || rootNS == "")
				m_RootNameSpace = "";
			m_RootNameSpace = rootNS;
			m_NSLen = m_RootNameSpace.Length;

			return this;
		}


		/// <summary>
		/// Root xmlns namespace if there is one. This will be used
		/// in our override of NamespaceURI, where we'll return every
		/// NamespaceURI except those that match this root xmlns value
		/// (in which case we'll return empty - "").
		/// </summary>
		private string m_RootNameSpace = "";

		/// <summary>
		/// The string length of the final set m_RootNameSpace. Used for faster
		/// comparisons in NamespaceURI property.
		/// </summary>
		private int m_NSLen;


		/// <summary>
		/// With the root namespace recorded in m_RootNameSpace, we now only have
		/// to discover if a given element upon which this property is being called
		/// matches that root namespace. If so, we just return empty (""). ELSE,
		/// the base.NamespaceURI is simply returned. Comparisons are done extremely
		/// performantly, so one should notice very little effect from the XmlReader.
		/// </summary>
		public override string NamespaceURI
		{
			get
			{
				if (m_NSLen == 0) // no root xmlns is set, so always return the nsUri
					return base.NamespaceURI;

				// thus a root xmlns is set, goal is to always return empty if this elem
				// has that same ns, ELSE return this element's ns
				string ns = base.NamespaceURI;

				if (ns == null || ns.Length == 0)
					return "";

				if (ns.Length == m_NSLen && ns == m_RootNameSpace)
					return "";

				return ns;
			}
		}

		#region GetNamespaceIgnorantXElement

		public static XElement GetNamespaceIgnorantXElement(string xmlString, LoadOptions loadOptions = DefLoadOptions)
		{
			return GetNamespaceIgnorantXElement(new RootNamespaceIgnorantXmlTextReader(xmlString).Init(), loadOptions);
		}

		public static XElement GetNamespaceIgnorantXElement(byte[] xml, LoadOptions loadOptions = DefLoadOptions)
		{
			return GetNamespaceIgnorantXElement(new RootNamespaceIgnorantXmlTextReader(xml).Init(), loadOptions);
		}

		public static XElement GetNamespaceIgnorantXElement(System.IO.Stream xml, LoadOptions loadOptions = DefLoadOptions)
		{
			return GetNamespaceIgnorantXElement(new RootNamespaceIgnorantXmlTextReader(xml).Init(), loadOptions);
		}

		public static XElement GetNamespaceIgnorantXElement(System.IO.TextReader textReader, LoadOptions loadOptions = DefLoadOptions)
		{
			return GetNamespaceIgnorantXElement(new RootNamespaceIgnorantXmlTextReader(textReader).Init(), loadOptions);
		}

		public static XElement GetNamespaceIgnorantXElement(Uri uri, LoadOptions loadOptions = DefLoadOptions)
		{
			return GetNamespaceIgnorantXElement(new RootNamespaceIgnorantXmlTextReader(uri).Init(), loadOptions);
		}

		public static XElement GetNamespaceIgnorantXElement(RootNamespaceIgnorantXmlTextReader xmlTextReader, LoadOptions loadOptions = DefLoadOptions)
		{
			XElement e = XElement.Load(xmlTextReader, loadOptions);
			if (e.Attribute("xmlns") != null)
				e.Attribute("xmlns").Remove();
			return e;
		}

		#endregion GetNamespaceIgnorantXElement
	}

#if DNXPublic
	public
#endif
	class XmlTextWriterIgnoreInvalidChars : XmlTextWriter
	{
		public bool DeleteInvalidChars { get; set; }

        public XmlTextWriterIgnoreInvalidChars(TextWriter w, bool deleteInvalidChars = true) : base(w)
		{
			DeleteInvalidChars = deleteInvalidChars;
		}

		public override void WriteString(string text)
		{
			if (text != null && DeleteInvalidChars)
				text = XML.ToValidXmlCharactersString(text);
			base.WriteString(text);
		}
	}

	#endregion
}