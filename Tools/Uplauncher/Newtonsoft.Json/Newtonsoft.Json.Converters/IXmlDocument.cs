using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(1)]*/
	internal interface IXmlDocument : IXmlNode
	{
		
		IXmlElement DocumentElement
		{
			/*[NullableContext(2)]*/
			get;
		}

		IXmlNode CreateComment( string text);

		IXmlNode CreateTextNode( string text);

		IXmlNode CreateCDataSection( string data);

		IXmlNode CreateWhitespace( string text);

		IXmlNode CreateSignificantWhitespace( string text);

		/*[NullableContext(2)]*/
		/*[return: Nullable(1)]*/
		IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);

		/*[NullableContext(2)]*/
		/*[return: Nullable(1)]*/
		IXmlNode CreateXmlDocumentType(string name, string publicId, string systemId, string internalSubset);

		IXmlNode CreateProcessingInstruction(string target,  string data);

		IXmlElement CreateElement(string elementName);

		IXmlElement CreateElement(string qualifiedName, string namespaceUri);

		IXmlNode CreateAttribute(string name,  string value);

		IXmlNode CreateAttribute(string qualifiedName, string namespaceUri,  string value);
	}
}
