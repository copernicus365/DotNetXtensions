using System.Xml.Linq;

namespace DotNetXtensions
{
	public static partial class XML
	{
		// --- SetDefaultNamespace / ClearDefaultNamespace ---

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
			foreach(XElement e in elem.DescendantsAndSelf())
				if(overwriteAll || e.Name.Namespace.NamespaceName == "")
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
			if(rootNSName == null || rootNSName == "")
				return false;

			XAttribute rootXmlnsAtt = elem.Attribute("xmlns"); // .SetAttributeValue("xmlns", "")
			if(rootXmlnsAtt != null)
				rootXmlnsAtt.Remove();

			XNamespace ns = XNamespace.None;

			elem.Name = ns + elem.Name.LocalName;

			foreach(XElement e in elem.Descendants()) {
				XName name = e.Name;
				if(name.Namespace.NamespaceName == rootNSName)
					e.Name = ns + name.LocalName;
			}
			return true;
		}

	}
}
