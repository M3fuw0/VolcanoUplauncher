using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal class XDocumentWrapper : XContainerWrapper, IXmlDocument, IXmlNode
	{
		private XDocument Document => (XDocument)base.WrappedNode;

		public override List<IXmlNode> ChildNodes
		{
			get
			{
				List<IXmlNode> childNodes = base.ChildNodes;
				if (Document.Declaration != null && (childNodes.Count == 0 || childNodes[0].NodeType != XmlNodeType.XmlDeclaration))
				{
					childNodes.Insert(0, new XDeclarationWrapper(Document.Declaration));
				}
				return childNodes;
			}
		}

		protected override bool HasChildNodes
		{
			get
			{
				if (base.HasChildNodes)
				{
					return true;
				}
				return Document.Declaration != null;
			}
		}

		/*[Nullable(2)]*/
		public IXmlElement DocumentElement
		{
			/*[NullableContext(2)]*/
			get
			{
				if (Document.Root == null)
				{
					return null;
				}
				return new XElementWrapper(Document.Root);
			}
		}

		public XDocumentWrapper(XDocument document)
			: base(document)
		{
		}

		public IXmlNode CreateComment(/*[Nullable(2)]*/ string text)
		{
			return new XObjectWrapper(new XComment(text));
		}

		public IXmlNode CreateTextNode(/*[Nullable(2)]*/ string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		public IXmlNode CreateCDataSection(/*[Nullable(2)]*/ string data)
		{
			return new XObjectWrapper(new XCData(data));
		}

		public IXmlNode CreateWhitespace(/*[Nullable(2)]*/ string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		public IXmlNode CreateSignificantWhitespace(/*[Nullable(2)]*/ string text)
		{
			return new XObjectWrapper(new XText(text));
		}

		/*[NullableContext(2)]*/
		/*[return: Nullable(1)]*/
		public IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone)
		{
			return new XDeclarationWrapper(new XDeclaration(version, encoding, standalone));
		}

		/*[NullableContext(2)]*/
		/*[return: Nullable(1)]*/
		public IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset)
		{
			return new XDocumentTypeWrapper(new XDocumentType(name, publicId, systemId, internalSubset));
		}

		public IXmlNode CreateProcessingInstruction(string target, /*[Nullable(2)]*/ string data)
		{
			return new XProcessingInstructionWrapper(new XProcessingInstruction(target, data));
		}

		public IXmlElement CreateElement(string elementName)
		{
			return new XElementWrapper(new XElement(elementName));
		}

		public IXmlElement CreateElement(string qualifiedName, string namespaceUri)
		{
			return new XElementWrapper(new XElement(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri)));
		}

		public IXmlNode CreateAttribute(string name, /*[Nullable(2)]*/ string value)
		{
			return new XAttributeWrapper(new XAttribute(name, value));
		}

		public IXmlNode CreateAttribute(string qualifiedName, string namespaceUri, /*[Nullable(2)]*/ string value)
		{
			return new XAttributeWrapper(new XAttribute(XName.Get(MiscellaneousUtils.GetLocalName(qualifiedName), namespaceUri), value));
		}

		public override IXmlNode AppendChild(IXmlNode newChild)
		{
			if (newChild is XDeclarationWrapper xDeclarationWrapper)
			{
				Document.Declaration = xDeclarationWrapper.Declaration;
				return xDeclarationWrapper;
			}
			return base.AppendChild(newChild);
		}
	}
}
