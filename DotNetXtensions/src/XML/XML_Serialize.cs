using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DotNetXtensions
{
	public static partial class XML
	{
		// SERIALIZE

		/// <summary>
		/// Serializes the object with XmlSerializer, with NO namespaces.
		/// </summary>
		public static XElement Serialize<T>(T obj, bool changeElementsToAttributes = false)
		{
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			XmlSerializer xs = new XmlSerializer(typeof(T));

			XDocument doc = new XDocument();
			using(XmlWriter xw = doc.CreateWriter())
				xs.Serialize(xw, obj, ns);

			XElement x = doc.Root;

			if(changeElementsToAttributes) {
				foreach(var e in x.Elements()) {
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
			if(changeElementsToAttributes) {
				foreach(var a in x.Attributes()) {
					x.Add(new XElement(a.Name, a.Value));
				}
				x.Attributes().Remove();
			}
			XmlSerializer xs = new XmlSerializer(typeof(T));
			T pDeserialized = xs.Deserialize((x.CreateReader())) as T;
			return pDeserialized;
		}

		#region ToStringFormatted

		public static string ToStringFormatted(this XElement xml, bool indent = true, bool newLineOnAttributes = false, string indentChars = "\t", bool omitXmlDeclaration = true)
		{
			if(xml == null) return null;

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
			if(xml == null) return null;

			var result = new StringBuilder();
			using(XmlWriter writer = XmlWriter.Create(result, settings)) {
				xml.WriteTo(writer);
			}
			return result.ToString();
		}

		#endregion ToStringFormatted
	}
}
