using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(2)]*/
	/*[Nullable(0)]*/
	internal class XProcessingInstructionWrapper : XObjectWrapper
	{
		/*[Nullable(1)]*/
		private XProcessingInstruction ProcessingInstruction =>
            /*[NullableContext(1)]*/
            (XProcessingInstruction)WrappedNode;

        public override string LocalName => ProcessingInstruction.Target;

		public override string Value
		{
			get => ProcessingInstruction.Data;
            set => ProcessingInstruction.Data = value;
        }

		/*[NullableContext(1)]*/
		public XProcessingInstructionWrapper(XProcessingInstruction processingInstruction)
			: base(processingInstruction)
		{
		}
	}
}
