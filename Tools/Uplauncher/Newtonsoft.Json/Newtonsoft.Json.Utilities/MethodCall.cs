using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal delegate TResult MethodCall< T,  TResult>(T target, /*[Nullable(new byte[] { 1, 2 })]*/ params object[] args);
}
