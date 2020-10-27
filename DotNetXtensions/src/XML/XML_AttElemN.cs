using System.Xml.Linq;

namespace DotNetXtensions
{
	public static partial class XML
	{
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
			if(value == null) return null;
			return new XAttribute(name, value);
		}

		public static XAttribute AttributeN(this XElement elem, XName attrName)
		{
			if(elem == null)
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
			if(value == null) return null;
			return new XElement(name, value);
		}

	}
}
