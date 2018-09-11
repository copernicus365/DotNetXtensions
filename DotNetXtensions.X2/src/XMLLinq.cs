using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

#if DNXPublic
namespace DotNetXtensions
#else
namespace DotNetXtensionsPrivate
#endif
{
	/// <summary>
	/// Contains Linq to Xml extension methods.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XMLLinq
	{
		#region At

		// non null-checking category

		/// <summary>
		/// Filters the collection of elements, returning only those elements that
		/// have an attribute of the given name.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="name">The name of the attribute to match.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> At(this IEnumerable<XElement> elems, XName name)
		{
			return elems.Where(e => e.Attribute(name) != null);
		}

		/// <summary>
		/// Filters the collection of elements, returning only those elements that
		/// have an attribute of the given name and value.<para/><para/>
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="name">The name of the attribute to match.</param>
		/// <param name="value">The value of the specified attribute</param>
		/// <returns></returns>
		public static IEnumerable<XElement> At(this IEnumerable<XElement> elems, XName name, string value)
		{
			return elems.Where(e => (string)e.Attribute(name) == value);
		}

		#endregion At

		#region AttributeNames

		/// <summary>
		/// Gets a collection of the names of the attributes in this element.<para/><para/>
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns></returns>
		public static string[] AttributeNames(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return elem.Attributes().Select(atrb => atrb.Name.ToString()).ToArray();
		}

		#endregion AttributeNames

		#region Depth | GetStartElement | InnerXml | Line Info

		#region Depth

		/// <summary>
		/// Gets the depth of this node within its cooresponding XElement or XDocument.
		/// Zero (0) indicates it is the root.
		/// </summary>
		/// <param name="node">This node.</param>
		/// <returns>The depth of this node.</returns>
		public static int Depth(this XNode node)
		{
			if(node == null) throw new ArgumentNullException("node");

			// There is no faster way to query this.  Further, this route avoids any
			// potentially costly new references (XNode ancestorNode = node.Parent; over and over...)
			if(node.Parent == null) return 0;
			if(node.Parent.Parent == null) return 1;
			if(node.Parent.Parent.Parent == null) return 2;
			if(node.Parent.Parent.Parent.Parent == null) return 3;
			if(node.Parent.Parent.Parent.Parent.Parent == null) return 4;
			if(node.Parent.Parent.Parent.Parent.Parent.Parent == null) return 5;
			if(node.Parent.Parent.Parent.Parent.Parent.Parent.Parent == null) return 6;
			if(node.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent == null) return 7;

			//continuing where we left off with depth 8 (checking 9th parent)
			int depth = 8;
			XNode ancestorNode = node.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent;
			while(ancestorNode.Parent != null) {
				ancestorNode = ancestorNode.Parent;
				depth++;
			}
			return depth;
		}

		#endregion Depth

		#region GetStartElement

		/// <summary>
		/// Returns a new XElement which is a duplicate of this element's start element (i.e. its opening tag).
		/// It will have no content so it will be a closed element.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns>A new XElement which is a duplicate of this element's start element.</returns>
		public static XElement GetStartElement(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return new XElement(elem.Name, elem.Attributes());
		}

		#endregion GetStartElement

		#region InnerXml

		/// <summary>
		/// Returns the inner-XML content of this element as a string.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns>The Inner-XML content of this element.</returns>
		public static string InnerXml(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			// Though it may be expected that this creation of an xmlReader is very costly,
			// it is actually done extremely fast.
			XmlReader xmlReader = elem.CreateReader();
			xmlReader.MoveToContent();
			return xmlReader.ReadInnerXml();
		}

		#endregion InnerXml

		#region Line Info: HasLineInfo, LineNumber, LinePosition

		/// <summary>
		/// Indicates whether line information can be obtained from this element.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns>A value indicating if line information can be obtained.</returns>
		public static bool HasLineInfo(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return ((IXmlLineInfo)elem).HasLineInfo();
		}

		/// <summary>
		/// Gets the current line-number of this element.
		/// <para /> <para />
		/// In typical scenarios, settings such as LoadOptions.SetLineInfo will need to have
		/// been specified -- if not, the line number will simply return zero (0).
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns>The line-number.</returns>
		public static int LineNumber(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return ((IXmlLineInfo)elem).LineNumber;
		}

		/// <summary>
		/// Gets the current line-position of this element.
		/// <para /> <para />
		/// In typical scenarios, settings such as LoadOptions.PreserveWhitespace and
		/// LoadOptions.SetLineInfo will need to have been specified -- if not, the line postion
		/// will simply return zero (0).
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <returns>The line-postion.</returns>
		public static int LinePosition(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return ((IXmlLineInfo)elem).LinePosition;
		}

		#endregion Line Info: HasLineInfo, LineNumber, LinePosition

		#endregion Depth | GetStartElement | InnerXml | Line Info

		#region Element(s) / Descendant(s)

		#region __ElementsDescendants

		private static IEnumerable<XElement> __ElementsDescendants(this XElement elem, XName elemName, XName attrName, bool elementsNotDesc)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");

			IEnumerable<XElement> _enum = null;
			if(elementsNotDesc)
				_enum = (elemName == null) ? elem.Elements() : elem.Elements(elemName);
			else
				_enum = (elemName == null) ? elem.Descendants() : elem.Descendants(elemName);

			return _enum.Where(e => e.Attribute(attrName) != null);
		}

		private static IEnumerable<XElement> __ElementsDescendants(this IEnumerable<XElement> elems, XName elemName, XName attrName, bool elementsNotDesc)
		{
			if(elems == null) throw new ArgumentNullException("elems");
			if(attrName == null) throw new ArgumentNullException("attrName");

			IEnumerable<XElement> _enum = null;
			if(elementsNotDesc)
				_enum = (elemName == null) ? elems.Elements() : elems.Elements(elemName);
			else
				_enum = (elemName == null) ? elems.Descendants() : elems.Descendants(elemName);

			return _enum.Where(e => e.Attribute(attrName) != null);
		}

		private static IEnumerable<XElement> __ElementsDescendants(this XElement elem, XName elemName, XName attrName, string attrValue, bool elementsNotDesc)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");
			if(attrValue == null) throw new ArgumentNullException("attrValue");

			IEnumerable<XElement> _enum = null;
			if(elementsNotDesc)
				_enum = (elemName == null) ? elem.Elements() : elem.Elements(elemName);
			else
				_enum = (elemName == null) ? elem.Descendants() : elem.Descendants(elemName);

			return _enum.Where(e => (string)e.Attribute(attrName) == attrValue);
		}

		private static IEnumerable<XElement> __ElementsDescendants(this IEnumerable<XElement> elems, XName elemName, XName attrName, string attrValue, bool elementsNotDesc)
		{
			if(elems == null) throw new ArgumentNullException("elems");
			if(attrName == null) throw new ArgumentNullException("attrName");
			if(attrValue == null) throw new ArgumentNullException("attrValue");

			IEnumerable<XElement> _enum = null;
			if(elementsNotDesc)
				_enum = (elemName == null) ? elems.Elements() : elems.Elements(elemName);
			else
				_enum = (elemName == null) ? elems.Descendants() : elems.Descendants(elemName);

			return _enum.Where(e => (string)e.Attribute(attrName) == attrValue);
		}

		#endregion __ElementsDescendants

		// =========

		#region Element

		#region Element -- OWN IMPLEMENTATION

		// OWN IMPLEMENTATION::
		/// <summary>
		/// Returns the first child element of this element.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns></returns>
		public static XElement Element(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return elem.Elements().FirstOrDefault();
		}

		#endregion Element -- OWN IMPLEMENTATION

		/// <summary>
		/// Returns the first XElement of the child elements of this element that matches
		/// the specified attribute name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static XElement Element(this XElement elem, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elem, elemName, attrName, true).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the child elements of these elements that matches
		/// the specified attribute name.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static XElement Element(this IEnumerable<XElement> elems, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elems, elemName, attrName, true).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the child elements of this element that matches
		/// the specified attribute name and value.<para/><para/>
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute.</param>
		/// <returns></returns>
		public static XElement Element(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elem, elemName, attrName, attrValue, true).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the child elements of these elements that matches
		/// the specified attribute name and value.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute.</param>
		/// <returns></returns>
		public static XElement Element(this IEnumerable<XElement> elems, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elems, elemName, attrName, attrValue, true).FirstOrDefault();
		}


		#endregion

		#region Descendant

		#region Descendant -- OWN IMPLEMENTATION

		// OWN IMPLEMENTATION::
		/// <summary>
		/// Returns the first descendant element of this element.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns></returns>
		public static XElement Descendant(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			return elem.Descendants().FirstOrDefault();
		}

		// OWN IMPLEMENTATION::
		/// <summary>
		/// Returns the first descendant element with the specified name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the descendant element to match.</param>
		/// <returns></returns>
		public static XElement Descendant(this XElement elem, XName elemName)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");

			return elem.Descendants(elemName).FirstOrDefault();
		}

		#endregion Descendant -- OWN IMPLEMENTATION

		/// <summary>
		/// Returns the first XElement of the descendant elements of this element that matches
		/// the specified attribute name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static XElement Descendant(this XElement elem, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elem, elemName, attrName, false).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the descendant elements of these elements that matches
		/// the specified attribute name.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static XElement Descendant(this IEnumerable<XElement> elems, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elems, elemName, attrName, false).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the descendant elements of this element that matches
		/// the specified attribute name and value.<para/><para/>
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute (may be NULL).</param>
		/// <returns></returns>
		public static XElement Descendant(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elem, elemName, attrName, attrValue, false).FirstOrDefault();
		}

		/// <summary>
		/// Returns the first XElement of the descendant elements of these elements that matches
		/// the specified attribute name and value.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute (may be NULL).</param>
		/// <returns></returns>
		public static XElement Descendant(this IEnumerable<XElement> elems, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elems, elemName, attrName, attrValue, false).FirstOrDefault();
		}

		#endregion Descendant

		#region Elements

		/// <summary>
		/// Returns a filtered collection of the child elements of this element, in document order.
		/// Only XElements that match the specified element name and attribute name
		/// are included in the collection.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Elements(this XElement elem, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elem, elemName, attrName, true);
		}

		/// <summary>
		/// Returns a filtered collection of the child elements of these elements, in document order.
		/// Only XElements that match the specified element and attribute names are included in the collection.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Elements(this IEnumerable<XElement> elems, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elems, elemName, attrName, true);
		}

		/// <summary>
		/// Returns a filtered collection of the child elements of this element, in document order.
		/// Only XElements that match the specified element name and attribute name and value
		/// are included in the collection.<para/><para/>
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Elements(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elem, elemName, attrName, attrValue, true);
		}

		/// <summary>
		/// Returns a filtered collection of the child elements of these elements, in document order.
		/// Only XElements that match the specified element name and attribute name and value
		/// are included in the collection.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Elements(this IEnumerable<XElement> elems, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elems, elemName, attrName, attrValue, true);
		}

		#endregion Elements

		#region Descendants

		/// <summary>
		/// Returns a filtered collection of the decendant elements of this element, in document order.
		/// Only XElements that match the specified element and attribute names are included in the collection.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Descendants(this XElement elem, XName elemName, XName attrName)
		{
			return __ElementsDescendants(elem, elemName, attrName, false);
		}

		/// <summary>
		/// Returns a filtered collection of the descendant elements of these elements, in document order.
		/// Only XElements that match the specified element and attribute names are included in the collection.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Descendants(this IEnumerable<XElement> elems, XName elemName, string attrName)
		{
			return __ElementsDescendants(elems, elemName, attrName, false);
		}

		/// <summary>
		/// Returns a filtered collection of the decendant elements of this element, in document order.
		/// Only XElements that match the specified element name and attribute name and value
		/// are included in the collection.<para/><para/>
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute (may be NULL).</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Descendants(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elem, elemName, attrName, attrValue, false);
		}

		/// <summary>
		/// Returns a filtered collection of the descendant elements of these elements, in document order.
		/// Only XElements that match the specified element name and attribute name and value
		/// are included in the collection.
		/// </summary>
		/// <param name="elems">This IEnumerable of type XElement.</param>
		/// <param name="elemName">The name of the element to match, or NULL.</param>
		/// <param name="attrName">The name of the attribute to match.</param>
		/// <param name="attrValue">The value of the specified attribute (may be NULL).</param>
		/// <returns></returns>
		public static IEnumerable<XElement> Descendants(this IEnumerable<XElement> elems, XName elemName, XName attrName, string attrValue)
		{
			return __ElementsDescendants(elems, elemName, attrName, attrValue, false);
		}

		#endregion Descendants

		#endregion Element(s) / Descendant(s)

		#region LastElement

		// NOTE: You don't need "FirstElement", because that is what "Element" already does!

		/// <summary>
		/// Gets the last child element of this element.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns></returns>
		public static XElement LastElement(this XElement elem)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			XNode node = elem.LastNode;

			while(node != null) {
				if(node.NodeType == XmlNodeType.Element)
					return node as XElement;
				node = node.PreviousNode;
			}
			return null;
		}

		/// <summary>
		/// Gets the last child element of this element that matches the specified
		/// element name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name that the retrieved element must match.</param>
		/// <returns></returns>
		public static XElement LastElement(this XElement elem, XName elemName)
		{
			if(elemName == null) throw new ArgumentNullException("elemName");

			XElement lastElem = LastElement(elem); // null checks "elem"

			if(lastElem == null) return null;

			if(lastElem.Name == elemName)
				return lastElem;
			else
				return __NextPreviousElements(lastElem, elemName, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets the last child element of this element that matches the
		/// specified  element name (if not null) and that has an attribute of the specified name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name that the retrieved element must match, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the retrieved element must contain.</param>
		/// <returns></returns>
		public static XElement LastElement(this XElement elem, XName elemName, XName attrName)
		{
			if(attrName == null) throw new ArgumentNullException("attrName");

			XElement lastElem = LastElement(elem);  // null checks "elem"

			if(lastElem == null) return null;

			// elemName == null::: We allow elemName to be null so the query can search just be attrNm
			if(elemName == null || lastElem.Name == elemName)
				if(lastElem.Attribute(attrName) != null)
					return lastElem;

			return __NextPreviousElements(lastElem, elemName, attrName, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets the last child element of this element that matches the
		/// specified element name (if not null) and that has an attribute of the specified name and value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name that the retrieved element must match, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the retrieved element must contain.</param>
		/// <param name="attrValue">The value that the attribute must have.</param>
		/// <returns></returns>
		public static XElement LastElement(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			if(attrName == null) throw new ArgumentNullException("attrName");
			if(attrValue == null) throw new ArgumentNullException("attrValue");

			XElement lastElem = LastElement(elem);  // null checks "elem"

			if(lastElem == null) return null;

			// elemName == null::: We allow elemName to be null so the query can search just be attrNm and value
			if(elemName == null || lastElem.Name == elemName) {
				if((string)lastElem.Attribute(attrName) == attrValue)
					return lastElem;
			}
			return __NextPreviousElements(lastElem, elemName, attrName, attrValue, false).FirstOrDefault();
		}

		#endregion LastElement

		#region HasAttribute

		// non null-checking category

		/// <summary>
		/// Indicates whether or not this element has an attribute with the specified name.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="attrName">The name of the attribute.</param>
		/// <returns></returns>
		public static bool HasAttribute(this XElement elem, XName attrName)
		{
			return elem.Attribute(attrName) != null;
		}

		/// <summary>
		/// Indicates whether or not this element has an attribute with the specified name
		/// and value.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="attrName">The name of the attribute.</param>
		/// <param name="attrValue">The value of the attribute.</param>
		/// <returns></returns>
		public static bool HasAttribute(this XElement elem, XName attrName, string attrValue)
		{
			return ((string)elem.Attribute(attrName)) == attrValue;
		}

		#endregion HasAttribute

		#region HasElement / HasDescendant

		#region __HasElementOrDescendant

		// Handles all of the implementation code for HasElement / HasDescendant.

		private static bool __HasElementOrDescendant(this XElement elem, XName elemName, bool elemNotDesc)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");

			XElement child = elemNotDesc ?
				elem.Element(elemName) : elem.Descendant(elemName);

			return child == null ? false : true;
		}

		private static bool __HasElementOrDescendant(this XElement elem, XName elemName, XName attrName, bool elemNotDesc)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");

			XElement child = null;
			if(elemName == null)
				child = elemNotDesc ? elem.Element() : elem.Descendant();
			else
				child = elemNotDesc ? elem.Element(elemName) : elem.Descendant(elemName);

			return (child != null && child.Attribute(attrName) != null) ? true : false;
		}

		private static bool __HasElementOrDescendant(this XElement elem, XName elemName, XName attrName, string attrValue, bool elemNotDesc)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");
			if(attrValue == null) throw new ArgumentNullException("attrValue");

			XElement child = null;
			if(elemName == null)
				child = elemNotDesc ? elem.Element() : elem.Descendant();
			else
				child = elemNotDesc ? elem.Element(elemName) : elem.Descendant(elemName);

			return (child != null && (string)child.Attribute(attrName) == attrValue) ? true : false;
		}

		private static bool __HasElementOrDescendant(this XElement elem, XName elemName, XName attrName, string attrValue, bool nullableAttributeParams, bool elemNotDesc)
		{
			if(!nullableAttributeParams)
				return HasElement(elem, elemName, attrName, attrValue);

			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null && attrValue != null) throw new ArgumentNullException("attrName", "If attrValue is not null, then attrName cannot be null.");

			XElement child = null;
			if(elemName == null)
				child = elemNotDesc ? elem.Element() : elem.Descendant();
			else
				child = elemNotDesc ? elem.Element(elemName) : elem.Descendant(elemName);

			if(child == null) return false;
			if(attrName == null) return true; // trust us, this is right

			// if attrName is *not* null, then as checked above, it means *both* atName and atValue are not null
			return ((string)child.Attribute(attrName) == attrValue) ? true : false;
		}

		#endregion __HasElementOrDescendant

		#region HasElement

		/// <summary>
		/// Determines if this element contains a child element with the given name.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name.</param>
		/// <returns></returns>
		public static bool HasElement(this XElement elem, XName elemName)
		{
			return __HasElementOrDescendant(elem, elemName, true);
		}

		/// <summary>
		/// Determines if this element contains a child element with the given name and attribute.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name.</param>
		/// <returns></returns>
		public static bool HasElement(this XElement elem, XName elemName, XName attrName)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, true);
		}

		/// <summary>
		/// Determines if this element contains a child element with the given name and attribute.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name.</param>
		/// <param name="attrValue">The child element's attribute value.</param>
		/// <returns></returns>
		public static bool HasElement(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, attrValue, true);
		}

		/// <summary>
		/// Determines if this element contains a child element with the given name and (optionally)
		/// an attribute with the specified name and value.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, the attribute will be ignored.</param>
		/// <param name="attrValue">The child element's attribute value, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, attrName must also be null.</param>
		/// <param name="nullableAttributeParams">If true, attrName, or attrName and attrValue may be null,
		/// which will be interpreted as a request to ignore (i.e. to not test off of) that value.  An ArgumentNullException
		/// is thrown if attrValue is not null but attrName is null. This parameter is often useful when the element only
		/// sometimes needs to be tested on one of its attribute values.</param>
		/// <returns></returns>
		public static bool HasElement(this XElement elem, XName elemName, XName attrName, string attrValue, bool nullableAttributeParams)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, attrValue, nullableAttributeParams, true);
		}

		#endregion HasElement

		#region HasDescendant

		/// <summary>
		/// Determines if this element contains a descendant element with the given name.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name.</param>
		/// <returns></returns>
		public static bool HasDescendant(this XElement elem, XName elemName)
		{
			return __HasElementOrDescendant(elem, elemName, false);
		}

		/// <summary>
		/// Determines if this element contains a descendant element with the given name and attribute.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name.</param>
		/// <returns></returns>
		public static bool HasDescendant(this XElement elem, XName elemName, XName attrName)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, false);
		}

		/// <summary>
		/// Determines if this element contains a descendant element with the given name and attribute.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name.</param>
		/// <param name="attrValue">The child element's attribute value.</param>
		/// <returns></returns>
		public static bool HasDescendant(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, attrValue, false);
		}

		/// <summary>
		/// Determines if this element contains a descendant element with the given name and (optionally)
		/// an attribute with the specified name and value.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The child element's name, or NULL.</param>
		/// <param name="attrName">The child element's attribute name, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, the attribute will be ignored.</param>
		/// <param name="attrValue">The child element's attribute value, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, attrName must also be null.</param>
		/// <param name="nullableAttributeParams">If true, attrName, or attrName and attrValue may be null,
		/// which will be interpreted as a request to ignore (i.e. to not test off of) that value.  An ArgumentNullException
		/// is thrown if attrValue is not null but attrName is null. This parameter is often useful when the element only
		/// sometimes needs to be tested on one of its attribute values.</param>
		/// <returns></returns>
		public static bool HasDescendant(this XElement elem, XName elemName, XName attrName, string attrValue, bool nullableAttributeParams)
		{
			return __HasElementOrDescendant(elem, elemName, attrName, attrValue, nullableAttributeParams, false);
		}

		#endregion HasDescendant

		#endregion HasElement / HasDescendant

		#region HasThisTagSignature

		/// <summary>
		/// Determines if this element has the specified name and an attribute with the specified name.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The name this XElement must have.</param>
		/// <param name="attrName">The name of the attribute this element must have.</param>
		/// <returns>True if the signature matches, else false.</returns>
		public static bool HasThisTagSignature(this XElement elem, XName elemName, XName attrName)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");
			if(attrName == null) throw new ArgumentNullException("attrName");

			return (elem.Name == elemName && elem.Attribute(attrName) != null) ? true : false;
		}

		/// <summary>
		/// Determines if this element has the specified name and an attribute with the specified name and value.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The name this XElement must have.</param>
		/// <param name="attrName">The name of the attribute this element must have.</param>
		/// <param name="attrValue">The value that the specified attribute must have.</param>
		/// <returns>True if the signature matches, else false.</returns>
		public static bool HasThisTagSignature(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");
			if(attrName == null) throw new ArgumentNullException("attrName");

			return (elem.Name != elemName && elem.Attribute(attrName).Value == attrValue) ? true : false;
		}

		/// <summary>
		/// Determines if this element has the specified name and (optionally) an attribute with the specified
		/// name and value.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="elemName">The name this XElement must have.</param>
		/// <param name="attrName">The name of the attribute this element must have, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, the attribute will be ignored.</param>
		/// <param name="attrValue">The value that the specified attribute must have, or
		/// NULL if <i>nullableAttributeParams</i> is true. If null, attrName must also be null.</param>
		/// <param name="nullableAttributeParams">If true, attrName, or attrName and attrValue may be null,
		/// which will be interpreted as a request to ignore (i.e. to not test off of) that value.  An ArgumentNullException
		/// is thrown if attrValue is not null but attrName is null. This parameter is often useful when the element only
		/// sometimes needs to be tested on one of its attribute values.</param>
		/// <returns>True if the signature matches, else false.</returns>
		public static bool HasThisTagSignature(this XElement elem, XName elemName, XName attrName, string attrValue, bool nullableAttributeParams)
		{
			if(!nullableAttributeParams)
				return elem.HasThisTagSignature(elemName, attrName, attrValue);

			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");
			if(attrName == null && attrValue != null) throw new ArgumentNullException("attrName", "If attrValue is not null, then attrName cannot be null.");

			if(elem.Name != elemName)
				return false;

			if(attrValue != null) // If attrValue isn't null, then as checked above, attrName isn't null either.  In most cases, this first condition will be true.
			{
				if(elem.Attribute(attrName).Value != attrValue)
					return false;
			}
			else if(attrName != null) {
				if(elem.Attribute(attrName) == null)
					return false;
			}

			return true;
		}

		#endregion HasThisTagSignature

		#region NextElement | PreviousElement ::: ElementsAfterSelf | ElementsBeforeSelf

		#region __NextPreviousElement(s)

		// These five contain the entire implementation for all NextElement(s) / PreviousElement(s) Methods

		private static XElement __NextPreviousElement(XElement elem, bool nextNotPrev)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			XNode node = nextNotPrev ? elem.NextNode : elem.PreviousNode;

			while(node != null) {
				if(node.NodeType == XmlNodeType.Element)
					return node as XElement;

				node = nextNotPrev ? node.NextNode : node.PreviousNode;
			}
			return null;
		}

		private static IEnumerable<XElement> __NextPreviousElements(this XElement elem, bool nextNotPrev)
		{
			if(elem == null) throw new ArgumentNullException("elem");

			XNode node = nextNotPrev ? elem.NextNode : elem.PreviousNode;
			while(node != null) {
				if(node.NodeType == XmlNodeType.Element)
					yield return node as XElement;

				node = nextNotPrev ? node.NextNode : node.PreviousNode;
			}
		}

		private static IEnumerable<XElement> __NextPreviousElements(this XElement elem, XName elemName, bool nextNotPrev)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(elemName == null) throw new ArgumentNullException("elemName");

			XNode node = nextNotPrev ? elem.NextNode : elem.PreviousNode;
			while(node != null) {
				if(node.NodeType == XmlNodeType.Element) {
					XElement e = node as XElement;
					if(e.Name == elemName)
						yield return e;
				}
				node = nextNotPrev ? node.NextNode : node.PreviousNode;
			}
		}

		private static IEnumerable<XElement> __NextPreviousElements(this XElement elem, XName elemName, XName attrName, bool nextNotPrev)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");

			XNode node = nextNotPrev ? elem.NextNode : elem.PreviousNode;
			while(node != null) {
				if(node.NodeType == XmlNodeType.Element) {
					XElement e = node as XElement;
					if(elemName == null || e.Name == elemName)
						if(e.Attribute(attrName) != null)
							yield return e;
				}
				node = nextNotPrev ? node.NextNode : node.PreviousNode;
			}
		}

		private static IEnumerable<XElement> __NextPreviousElements(this XElement elem, XName elemName, XName attrName, string attrValue, bool nextNotPrev)
		{
			if(elem == null) throw new ArgumentNullException("elem");
			if(attrName == null) throw new ArgumentNullException("attrName");
			if(attrValue == null) throw new ArgumentNullException("attrValue");

			XNode node = nextNotPrev ? elem.NextNode : elem.PreviousNode;
			while(node != null) {
				if(node.NodeType == XmlNodeType.Element) {
					XElement e = node as XElement;
					if(elemName == null || e.Name == elemName)
						if((string)e.Attribute(attrName) == attrValue)
							yield return e;
				}
				node = nextNotPrev ? node.NextNode : node.PreviousNode;
			}
		}

		#endregion __NextPreviousElement(s)

		// =========

		#region NextElement / PreviousElement

		#region NextElement

		/// <summary>
		/// Gets this element's next sibling element.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns>The next sibling element.</returns>
		public static XElement NextElement(this XElement elem)
		{
			return __NextPreviousElement(elem, true);
		}

		/// <summary>
		/// Gets this element's next sibling element which matches the specified element name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the next element to retrieve.</param>
		/// <returns></returns>
		public static XElement NextElement(this XElement elem, XName elemName)
		{
			return __NextPreviousElements(elem, elemName, true).FirstOrDefault();
		}

		/// <summary>
		/// Gets this element's next sibling element which has the specified element name
		/// (unless null) and that has an attribute of the specified value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the next element to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the next element must contain.</param>
		/// <returns></returns>
		public static XElement NextElement(this XElement elem, XName elemName, XName attrName)
		{
			return __NextPreviousElements(elem, elemName, attrName, true).FirstOrDefault();
		}

		/// <summary>
		/// Gets this element's next sibling element which has the specified element name
		/// (unless null) and that has an attribute of the specified name and value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the next element to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the next element must contain.</param>
		/// <param name="attrValue">The value of the attribute that the next element must contain.</param>
		/// <returns></returns>
		public static XElement NextElement(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __NextPreviousElements(elem, elemName, attrName, attrValue, true).FirstOrDefault();
		}

		#endregion NextElement

		#region PreviousElement

		/// <summary>
		/// Gets this element's previous sibling element.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns>The previous sibling element.</returns>
		public static XElement PreviousElement(this XElement elem)
		{
			return __NextPreviousElement(elem, false);
		}

		/// <summary>
		/// Gets this element's previous sibling element which matches the specified element name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the next element to retrieve.</param>
		/// <returns></returns>
		public static XElement PreviousElement(this XElement elem, XName elemName)
		{
			return __NextPreviousElements(elem, elemName, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets this element's previous sibling element which has the specified element name
		/// (unless null) and that has an attribute of the specified value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the previous element to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the previous element must contain.</param>
		/// <returns></returns>
		public static XElement PreviousElement(this XElement elem, XName elemName, XName attrName)
		{
			return __NextPreviousElements(elem, elemName, attrName, false).FirstOrDefault();
		}

		/// <summary>
		/// Gets this element's previous sibling element which has the specified element name
		/// (unless null) and that has an attribute of the specified name and value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The name of the previous element to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the previous element must contain.</param>
		/// <param name="attrValue">The value of the attribute that the previous element must contain.</param>
		/// <returns></returns>
		public static XElement PreviousElement(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __NextPreviousElements(elem, elemName, attrName, attrValue, false).FirstOrDefault();
		}

		#endregion PreviousElement

		#endregion NextElement / PreviousElement

		#region ElementsAfterSelf

		/// <summary>
		/// Gets this element's next sibling elements which match the specified element name
		/// (unless null) and that have an attribute of the specified name.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the next elements to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the next elements must contain.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> ElementsAfterSelf(this XElement elem, XName elemName, XName attrName)
		{
			return __NextPreviousElements(elem, elemName, attrName, true);
		}

		/// <summary>
		/// Gets this element's next sibling elements which match the specified element name
		/// (unless null) and that have an attribute of the specified name and value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the next elements to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the next elements must contain.</param>
		/// <param name="attrValue">The value of the attribute that the next elements must contain.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> ElementsAfterSelf(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __NextPreviousElements(elem, elemName, attrName, attrValue, true);
		}

		#endregion

		#region ElementsBeforeSelf

		/// <summary>
		/// Gets this element's previous sibling elements which match the specified element name
		/// (unless null) and that have an attribute of the specified value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the previous elements to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the previous elements must contain.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> ElementsBeforeSelf(this XElement elem, XName elemName, XName attrName)
		{
			return __NextPreviousElements(elem, elemName, attrName, false);
		}

		/// <summary>
		/// Gets this element's previous sibling elements which match the specified element name
		/// (unless null) and that have an attribute of the specified name and value.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="elemName">The element name of the previous elements to retrieve, or NULL.</param>
		/// <param name="attrName">The name of the attribute that the previous elements must contain.</param>
		/// <param name="attrValue">The value of the attribute that the previous elements must contain.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> ElementsBeforeSelf(this XElement elem, XName elemName, XName attrName, string attrValue)
		{
			return __NextPreviousElements(elem, elemName, attrName, attrValue, false);
		}

		#endregion ElementsBeforeSelf

		#endregion

		#region SetAttributeName

		/// <summary>
		/// Renames the attribute in this XElement that has the specified name.  An ArgumentNullException is thrown
		/// if the specified attribute does not exist. The only way to rename an attribute is to remove it and then add a new
		/// attribute with the given name.  A key feature to this function however is that the replacement attribute remains
		/// in the same position that the original attribute was in.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="attrName">The original attribute name.</param>
		/// <param name="newAttrName">The new attribute name.</param>
		public static void SetAttributeName(this XElement elem, XName attrName, string newAttrName)
		{
			SetAttributeName(elem, attrName, newAttrName, null);
		}

		/// <summary>
		/// Renames the attribute in this XElement that has the specified name, and optionally replaces
		/// the value of the attribute.  An ArgumentNullException is thrown if the specified attribute does not exist.
		/// The only way to rename an attribute is to remove it and then add a new attribute with the given name.
		/// A key feature to this function however is that the replacement attribute remains in the same position that
		/// the original attribute was in.
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="attrName">The original attribute name.</param>
		/// <param name="newAttrName">The new attribute name.</param>
		/// <param name="newAttrValue">The new attribute value, or null to keep the same value.</param>
		public static void SetAttributeName(this XElement elem, XName attrName, string newAttrName, string newAttrValue)
		{
			// For XAttribute 'at', at.Name is read-only, it cannot be set.  So the only way to rename an element is to
			// remove it and then re-add it.  This gets quite involved when one considers that some implementations
			// of XML require (or at least desire there to be) a set attribute order even though XML itself is agnostic in
			// this regard.  To keep this attribute in the same order requires even more work, as seen below.
			if(elem == null) throw new ArgumentNullException("elem");

			if(elem.Attribute(attrName) == null) // If not null, then attributes must not be null and is > 0.
				throw new ArgumentException(string.Format("No attribute exists in this element by the name of \"{0}\".", attrName));

			XAttribute[] attributes = elem.Attributes().ToArray();

			int attPos = 0;
			for(; attPos < attributes.Length; attPos++)
				if(attributes[attPos].Name == attrName)
					break;

			XAttribute at = attributes[attPos];

			attributes.Remove();
			string newAttributeValue = newAttrValue != null ? newAttrValue : at.Value;
			attributes[attPos] = new XAttribute(newAttrName, newAttributeValue);
			elem.Add(attributes);
		}

		#endregion SetAttributeName

		#region WhereChildHasValue, HasElementWithValue

		/*
		 * A lot of thought has gone into these couple methods.  The objective is something
		 * considered bread and butter (see next), which is why it's been considered here in DNE.
		 * It has not been easy however to find an easy to understand and descriptive name
		 * which is at the same time succint.
		 * For the Objective, let's first consider the following element:
		 * <customer id="46128"><custName>Joe</custName><!--...etc...--></customer>
		 *
		 * Frequently we will be going after a particular customer (in other cases a set of customers)
		 * with a particular "id".  DNE has made this most bread and butter type of query easier:
		 * .Element("customer", "id", "46128").  But often the xml will have "id" as a child element
		 * rather than as an attribute:
		 * <customer><id>46128</id><custName>Joe</custName><!--...etc...--></customer>
		 *
		 * With this second, not-attribute-based xml type, we are still going after the exact same goal:
		 * filter the customer or customers that have a specific id number.  Couldn't there be
		 * a function that's just as simple as DNE's attribute filtering one for performing this?
		 * That is precisely our goal, but the problem is that finding an **easily understood** and yet
		 * succinct name (hopefully staying under the 22 char count of DescendantNodesAndSelf)
		 * for such a function is very tasking.  A direct fit would be the function below:
		 * ElementsWhereChildHasValue.  This has potential for confusion still, which is why we
		 * are not including it at this time.  The following however: elements.WhereChildHasValue(),
		 * solves our main objective, and hopefully others will agree that it is explanatory enough.
		 * The difference with ElementsWhereChildHasValue is that with
		 * WhereChildHasValue, we have to first get a (IEnumerable) collection, and then this
		 * function filters that collection. ElementsWhereChildHasValue does both of these at
		 * once, just like our attribute-filtering overloads of Element() and Elements() do.
		 *
		 * So then, elems.WhereChildHasValue() will hopefully be considered an intuitive enough
		 * function by others.
		 */

		#region Element(s)WhereChildHasValue [NOT INCLUDED]

		//public static XElement ElementWhereChildHasValue(this XElement elem, string elemName, string childElemName, string childElemValue)
		//{
		//    if (elem == null || elemName == null)
		//        throw new ArgumentNullException();

		//    return WhereChildHasValue(elem.Elements(elemName), childElemName, childElemValue).FirstOrDefault();
		//}

		//public static IEnumerable<XElement> ElementsWhereChildHasValue(this XElement elem, string elemName, string childElemName, string childElemValue)
		//{
		//    if (elem == null || elemName == null)
		//        throw new ArgumentNullException();

		//    return WhereChildHasValue(elem.Elements(elemName), childElemName, childElemValue);
		//}

		#endregion Element(s)WhereChildHasValue [NOT INCLUDED]

		/// <summary>
		/// Filters this collection of XElements to those which have a child element of the
		/// specified name and value.<para/><para/>
		/// </summary>
		/// <param name="elems">This sequence of XElements.</param>
		/// <param name="childElemName">The name of the child element.</param>
		/// <param name="childElemValue">The value of the specified child element.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> WhereChildHasValue(this IEnumerable<XElement> elems, XName childElemName, string childElemValue)
		{
			if(elems == null || childElemName == null || childElemValue == null)
				throw new ArgumentNullException();

			return elems.Where(e => (string)e.Element(childElemName) == childElemValue);
		}

		/// <summary>
		/// Determines if this XElement has an element with the specified name and value.<para/><para/>
		/// </summary>
		/// <param name="elem">This element.</param>
		/// <param name="childElemName">The name of the child element.</param>
		/// <param name="childElemValue">The value of the specified child element.</param>
		/// <returns></returns>
		public static bool HasElementWithValue(this XElement elem, XName childElemName, string childElemValue)
		{
			if(elem == null || childElemName == null || childElemValue == null)
				throw new ArgumentNullException();

			foreach(XElement e in elem.Elements(childElemName))
				if((string)e == childElemValue)
					return true;

			return false;
		}

		#endregion WhereChildHasValue, HasElementWithValue

		#region ElementsLocal

		/// <summary>
		/// Gets elements that have the specified local tag name, disregarding namespaces.
		/// This is particularly useful for when an XElement must have an xmlns namespace set
		/// in the root, and yet one knows no other namespaces are used. In those cases, which
		/// happens quite frequently, ElementsLocal can be used without having to specify namespaces
		/// in every single LINQ to XML query.
		/// </summary>
		/// <param name="e">Element.</param>
		/// <param name="localName">Local tag name.</param>
		/// <returns></returns>
		public static IEnumerable<XElement> ElementsLocal(this XElement e, string localName)
		{
			return e.Elements().Where(elm => elm.Name.LocalName == localName);
		}

		/// <summary>
		/// Gets the first element that has the specified local tag name, disregarding namespaces.
		/// </summary>
		/// <param name="e">Element</param>
		/// <param name="localName">Local element name.</param>
		public static XElement ElementLocal(this XElement e, string localName)
		{
			return e.Elements().Where(elm => elm.Name.LocalName == localName).FirstOrDefault();
		}

		#endregion

	}

}
