using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Xsl;

namespace DotNetXtensions
{
	/// <summary>
	/// Contains XML type functions and extensions.
	/// </summary>
#if DNXPublic
	public
#endif
	static class XMLSchema
	{
		#region GetSchema -- Schema Generation (xsd)

		/// <summary>
		/// Generates an Xml Schema (xsd) from this XDocument or XElement, and returns the
		/// schema as an XDocument.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xdoc">This XDocument.</param>
		/// <returns>The schema as an XDocument.</returns>
		private static XDocument __GetSchema(XElement elem, XDocument xdoc)
		{
			if (elem == null && xdoc == null) throw new ArgumentNullException();

			XmlSchemaInference schemaInfer = new XmlSchemaInference();

			XmlSchemaSet schemaSet = schemaInfer.InferSchema(
				elem == null ? xdoc.CreateReader() : elem.CreateReader());

			XDocument newSchema = new XDocument();

			using (XmlWriter newSchemaWriter = XmlWriter.Create(newSchema.CreateWriter())) {
				foreach (XmlSchema s in schemaSet.Schemas())
					s.Write(newSchemaWriter);
			}
			return newSchema;
		}

		/// <summary>
		/// Generates an Xml Schema (xsd) from this XElement, and returns the
		/// schema as an XDocument.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <returns>The schema as an XDocument.</returns>
		public static XDocument GetSchema(this XElement elem)
		{
			return __GetSchema(elem, null);
		}

		/// <summary>
		/// Generates an Xml Schema (xsd) from this XDocument, and returns the
		/// schema as an XDocument.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <returns>The schema as an XDocument.</returns>
		public static XDocument GetSchema(this XDocument xdoc)
		{
			return __GetSchema(null, xdoc);
		}

		#endregion GetSchema -- Schema Generation (xsd)

		#region Validate -- Schema Validation (xsd)

		/// <summary>
		/// Validates this XDocument or XElement against the specified schema, returning
		/// the error messages in the return string if it is not valid (an exception message per line),
		/// but null if it is valid.  Note that the XElement will be converted to an XDocument
		/// for the purpose of running this validation.
		/// </summary>
		/// <param name="elem">This XElement, or null.</param>
		/// <param name="xdoc">This XDocument, or null.</param>
		/// <param name="schema">The schema document in the form of an XElement.</param>
		/// <param name="schemaUri">The URI of the schema.</param>
		/// <returns>Null if validiation is successful, or the validation error messages
		/// within the return string if not successful.</returns>
		private static string __Validate(XElement elem, XDocument xdoc, XElement schema, string schemaUri)
		{
			if (xdoc == null) {
				if (elem == null) throw new ArgumentNullException();
				// As the user has been notified, if the srcXml is an XElement it will be converted
				// to an XDocument, the only way to validate it if it was not already part of a parent
				// XDocument that was previously verified
				else xdoc = new XDocument(elem);
			}

			XmlSchemaSet schemaSet = new XmlSchemaSet();

			if (schemaUri != null)
				schemaSet.Add(null, schemaUri);
			else {
				if (schema == null) throw new ArgumentNullException("schema");
				schemaSet.Add(null, schema.CreateReader());
			}

			StringBuilder vldErrorsSB = new StringBuilder();
			bool isValid = true;

			xdoc.Validate(schemaSet,
				(sender, vldArgs) => {
					isValid = false;

					XmlSchemaException ex = vldArgs.Exception;
					vldErrorsSB.AppendFormat("Error: {0}\r\n", ex.Message);

					// Nice idea about reporting line info, but no matter if LoadOptions set line info, this exception just reports zero (0).
					//if(((IXmlLineInfo)xdoc.Root).HasLineInfo())
					//vldErrorsSB.AppendFormat(" (Line Num: {0}, Line Pos: {1})", ex.LineNumber, ex.LinePosition);
				});

			if (isValid)
				return null;
			else
				return vldErrorsSB.ToString();
		}

		/// <summary>
		/// Validates this XElement against the specified schema, returning
		/// the error messages in the return string if it is not valid (an exception message per line),
		/// but null if it is valid.  Note that for this XElement to be validated it must be (internally)
		/// converted to an XDocument.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="schemaUri">The URI of the schema.</param>
		/// <returns>Null if validiation is successful, or the validation error messages
		/// within the return string if not successful.</returns>
		public static string Validate(this XElement elem, string schemaUri)
		{
			return __Validate(elem, null, null, schemaUri);
		}

		/// <summary>
		/// Validates this XDocument against the specified schema, returning
		/// the error messages in the return string if it is not valid (an exception message per line),
		/// but null if it is valid.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="schemaUri">The URI of the schema.</param>
		/// <returns>Null if validiation is successful, or the validation error messages
		/// within the return string if not successful.</returns>
		public static string Validate(this XDocument xdoc, string schemaUri)
		{
			return __Validate(null, xdoc, null, schemaUri);
		}

		/// <summary>
		/// Validates this XElement against the specified schema, returning
		/// the error messages in the return string if it is not valid (an exception message per line),
		/// but null if it is valid.  Note that for this XElement to be validated it must be (internally)
		/// converted to an XDocument.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="schema">The schema document in the form of an XElement.</param>
		/// <returns>Null if validiation is successful, or the validation error messages
		/// within the return string if not successful.</returns>
		public static string Validate(this XElement elem, XElement schema)
		{
			return __Validate(elem, null, schema, null);
		}

		/// <summary>
		/// Validates this XDocument against the specified schema, returning
		/// the error messages in the return string if it is not valid (an exception message per line),
		/// but null if it is valid.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="schema">The schema document in the form of an XElement.</param>
		/// <returns>Null if validiation is successful, or the validation error messages
		/// within the return string if not successful.</returns>
		public static string Validate(this XDocument xdoc, XElement schema)
		{
			return __Validate(null, xdoc, schema, null);
		}

		#endregion Validate -- Schema Validation (xsd)

		#region XslTransform, XslTransformData -- XSL Transformation

		private static void __XslTransform(
			XElement elem, XDocument xdoc, string xsltPath, ref string xsltExceptionMess,
			XsltSettings xsltSettings, XsltArgumentList xsltArgList, ref XDocument transformedXDoc,
			ref byte[] transformedData, bool getContent)
		{
			if (xdoc == null && elem == null) throw new ArgumentNullException();

			XslCompiledTransform xslTransform = new XslCompiledTransform(); //XslCompiledTransform(TRUE/FALSE: enableDebug);

			try {
				// 2nd arg: if xsltSettings param is null, default is used;
				// 3rd arg: we have to set XmlResolver; we will invalidate this by specifying null
				xslTransform.Load(xsltPath, xsltSettings, null);// XmlReader.Create(new StringReader(xsltDoc)),

				if (!getContent) {
					transformedXDoc = new XDocument();
					using (XmlWriter xmlWriter = transformedXDoc.CreateWriter()) {
						xslTransform.Transform(
							(xdoc != null ? xdoc.CreateReader() : elem.CreateReader()),
							xsltArgList, xmlWriter);
					}
				}
				else {
					MemoryStream textStream = new MemoryStream();
					xslTransform.Transform((xdoc != null ? xdoc.CreateReader() : elem.CreateReader()),
						xsltArgList, textStream);

					transformedData = textStream.ToArray();
				}
			} catch (XsltCompileException xslLoadEx) {
				xsltExceptionMess = xslLoadEx.Message;
				transformedXDoc = null; transformedData = null;
				return;
			} catch (XsltException xslTransformEx) {
				xsltExceptionMess = xslTransformEx.Message;
				transformedXDoc = null; transformedData = null;
				return;
			}
		}

		#region XslTransform (Returns an XSLT transformed XDocument)

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as an XDocument.  The target type must
		/// be valid XML (such as XHTML).
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <returns>An XDocument containing the XSLT transformed result.</returns>
		public static XDocument XslTransform(this XElement elem, string xsltPath)
		{
			XDocument transformedXDoc = null;
			string nullString = null; byte[] nullBytes = null;

			__XslTransform(elem, null, xsltPath, ref nullString, null, null, ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XElement elem, string xsltPath, ref string xsltExceptionMess)
		{
			XDocument transformedXDoc = null;
			byte[] nullBytes = null;

			__XslTransform(elem, null, xsltPath, ref xsltExceptionMess, null, null, ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XElement elem, string xsltPath, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			XDocument transformedXDoc = null;
			string nullString = null; byte[] nullBytes = null;

			__XslTransform(elem, null, xsltPath, ref nullString, xsltSettings, xsltArgList,
				ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XElement elem, string xsltPath, ref string xsltExceptionMess, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			XDocument transformedXDoc = null;
			byte[] nullBytes = null;

			__XslTransform(elem, null, xsltPath, ref xsltExceptionMess, xsltSettings, xsltArgList,
				ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		// Input: XDocument

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as an XDocument.  The target type must
		/// be valid XML (such as XHTML).
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <returns>An XDocument containing the XSLT transformed result.</returns>
		public static XDocument XslTransform(this XDocument xdoc, string xsltPath)
		{
			XDocument transformedXDoc = null;
			string nullString = null; byte[] nullBytes = null;

			__XslTransform(null, xdoc, xsltPath, ref nullString, null, null, ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XDocument xdoc, string xsltPath, ref string xsltExceptionMess)
		{
			XDocument transformedXDoc = null;
			byte[] nullBytes = null;

			__XslTransform(null, xdoc, xsltPath, ref xsltExceptionMess, null, null, ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XDocument xdoc, string xsltPath, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			XDocument transformedXDoc = null;
			string nullString = null; byte[] nullBytes = null;

			__XslTransform(null, xdoc, xsltPath, ref nullString, xsltSettings, xsltArgList,
				ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as an XDocument, or null if the tranformation failed.
		/// The target tranformation type must be valid XML (e.g. XHTML).
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <returns>An XDocument containing the XSLT transformed result, or null if the tranformation
		/// failed.</returns>
		public static XDocument XslTransform(this XDocument xdoc, string xsltPath, ref string xsltExceptionMess, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			XDocument transformedXDoc = null;
			byte[] nullBytes = null;

			__XslTransform(null, xdoc, xsltPath, ref xsltExceptionMess, xsltSettings, xsltArgList,
				ref transformedXDoc, ref nullBytes, false);

			return transformedXDoc;
		}

		#endregion XslTransform (Returns an XSLT transformed XDocument)

		#region XslTransformData (Returns the XSLT transformed Data)

		// Input type: XElement

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XElement elem, string xsltPath)
		{
			byte[] transformedContent = null;
			string nullString = null;
			XDocument transformedXDoc = null;

			__XslTransform(elem, null, xsltPath, ref nullString, null, null, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XElement elem, string xsltPath, ref string xsltExceptionMess)
		{
			byte[] transformedContent = null;
			XDocument transformedXDoc = null;

			__XslTransform(elem, null, xsltPath, ref xsltExceptionMess, null, null, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XElement elem, string xsltPath, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			byte[] transformedContent = null;
			string nullString = null;
			XDocument transformedXDoc = null;

			__XslTransform(elem, null, xsltPath, ref nullString, xsltSettings, xsltArgList, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XElement using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="elem">This XElement.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XElement elem, string xsltPath, ref string xsltExceptionMess, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			byte[] transformedContent = null;
			XDocument transformedXDoc = null;

			__XslTransform(elem, null, xsltPath, ref xsltExceptionMess, xsltSettings, xsltArgList, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		//

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XDocument xdoc, string xsltPath)
		{
			byte[] transformedContent = null;
			string nullString = null;
			XDocument transformedXDoc = null;

			__XslTransform(null, xdoc, xsltPath, ref nullString, null, null, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XDocument xdoc, string xsltPath, ref string xsltExceptionMess)
		{
			byte[] transformedContent = null;
			XDocument transformedXDoc = null;

			__XslTransform(null, xdoc, xsltPath, ref xsltExceptionMess, null, null, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XDocument xdoc, string xsltPath, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			byte[] transformedContent = null;
			string nullString = null;
			XDocument transformedXDoc = null;

			__XslTransform(null, xdoc, xsltPath, ref nullString, xsltSettings, xsltArgList, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		/// <summary>
		/// Runs an XSLT transformation upon this XDocument using the specified XSLT file,
		/// and returns the transformed result as a byte array (which will often simply contain
		/// UTF-8 encoded text), or null if the tranformation failed.  The target tranformation
		/// type could either be a valid XML type (such as XHTML) or any other transformable type,
		/// such as PDF.
		/// </summary>
		/// <param name="xdoc">This XDocument.</param>
		/// <param name="xsltPath">The URI of the XSLT file.</param>
		/// <param name="xsltExceptionMess">The XsltException message if the transformation fails.</param>
		/// <param name="xsltArgList">An XsltArgumentList object. This value can be null.</param>
		/// <param name="xsltSettings">An XsltSettings object. This value can be null.</param>
		/// <returns>A byte array containing the transformed data.</returns>
		public static byte[] XslTransformData(this XDocument xdoc, string xsltPath, ref string xsltExceptionMess, XsltSettings xsltSettings, XsltArgumentList xsltArgList)
		{
			byte[] transformedContent = null;
			XDocument transformedXDoc = null;

			__XslTransform(null, xdoc, xsltPath, ref xsltExceptionMess, xsltSettings, xsltArgList, ref transformedXDoc, ref transformedContent, true);

			return transformedContent;
		}

		#endregion XslTransformData (Returns the XSLT transformed Data)

		#endregion XslTransform, XslTransformData -- XSL Transformation
	}
}