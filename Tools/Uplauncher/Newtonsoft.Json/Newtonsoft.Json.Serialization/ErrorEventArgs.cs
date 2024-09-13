using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	public class ErrorEventArgs : EventArgs
	{
		
		/*[field: Nullable(2)]*/
		public object CurrentObject
		{
			/*[NullableContext(2)]*/
			get;
		}

		public ErrorContext ErrorContext { get; }

		public ErrorEventArgs( object currentObject, ErrorContext errorContext)
		{
			CurrentObject = currentObject;
			ErrorContext = errorContext;
		}
	}
}
