using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal class XDeclarationWrapper : XObjectWrapper, IXmlDeclaration, IXmlNode
	{
		internal XDeclaration Declaration { get; }

		public override XmlNodeType NodeType => XmlNodeType.XmlDeclaration;

		public string Version => Declaration.Version;

		public string Encoding
		{
			get => Declaration.Encoding;
            set => Declaration.Encoding = value;
        }

		public string Standalone
		{
			get => Declaration.Standalone;
            set => Declaration.Standalone = value;
        }

		public XDeclarationWrapper(XDeclaration declaration)
			: base(null)
		{
			Declaration = declaration;
		}
	}
}
