using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace DotNetXtensions
{
	//#region Classes XmlTextWriterIgnoreInvalidChars and RootNamespaceIgnorantXmlTextReader

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
	public class RootNamespaceIgnorantXmlTextReader : XmlTextReader
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

		#endregion


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
		public override string NamespaceURI {
			get {
				if(m_NSLen == 0) // no root xmlns is set, so always return the nsUri
					return base.NamespaceURI;

				// thus a root xmlns is set, goal is to always return empty if this elem
				// has that same ns, ELSE return this element's ns
				string ns = base.NamespaceURI;

				if(ns == null || ns.Length == 0)
					return "";

				if(ns.Length == m_NSLen && ns == m_RootNameSpace)
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
			if(e.Attribute("xmlns") != null)
				e.Attribute("xmlns").Remove();
			return e;
		}

		#endregion GetNamespaceIgnorantXElement
	}

	public class XmlTextWriterIgnoreInvalidChars : XmlTextWriter
	{
		public bool DeleteInvalidChars { get; set; }

		public XmlTextWriterIgnoreInvalidChars(TextWriter w, bool deleteInvalidChars = true) : base(w)
		{
			DeleteInvalidChars = deleteInvalidChars;
		}

		public override void WriteString(string text)
		{
			if(text != null && DeleteInvalidChars)
				text = XML.ToValidXmlCharactersString(text);
			base.WriteString(text);
		}
	}
}
