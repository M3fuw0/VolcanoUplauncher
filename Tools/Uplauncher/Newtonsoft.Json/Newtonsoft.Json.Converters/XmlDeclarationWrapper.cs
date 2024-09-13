using System.Runtime.CompilerServices;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal class XmlDeclarationWrapper : XmlNodeWrapper, IXmlDeclaration, IXmlNode
	{
		private readonly XmlDeclaration _declaration;

		public string Version => _declaration.Version;

		public string Encoding
		{
			get => _declaration.Encoding;
            set => _declaration.Encoding = value;
        }

		public string Standalone
		{
			get => _declaration.Standalone;
            set => _declaration.Standalone = value;
        }

		public XmlDeclarationWrapper(XmlDeclaration declaration)
			: base(declaration)
		{
			_declaration = declaration;
		}
	}
}
