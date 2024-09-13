using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	public class ErrorContext
	{
		internal bool Traced { get; set; }

		public Exception Error { get; }

		
		/*[field: Nullable(2)]*/
		public object OriginalObject
		{
			/*[NullableContext(2)]*/
			get;
		}

		
		/*[field: Nullable(2)]*/
		public object Member
		{
			/*[NullableContext(2)]*/
			get;
		}

		public string Path { get; }

		public bool Handled { get; set; }

		internal ErrorContext( object originalObject,  object member, string path, Exception error)
		{
			OriginalObject = originalObject;
			Member = member;
			Error = error;
			Path = path;
		}
	}
}
