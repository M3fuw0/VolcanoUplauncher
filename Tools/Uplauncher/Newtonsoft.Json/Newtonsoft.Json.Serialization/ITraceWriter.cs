using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	/*[NullableContext(1)]*/
	public interface ITraceWriter
	{
		TraceLevel LevelFilter { get; }

		void Trace(TraceLevel level, string message,  Exception ex);
	}
}
