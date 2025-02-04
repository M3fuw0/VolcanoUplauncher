using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	public class JTokenEqualityComparer : IEqualityComparer<JToken>
	{
		public bool Equals(JToken x, JToken y)
		{
			return JToken.DeepEquals(x, y);
		}

		public int GetHashCode(JToken obj)
		{
			return obj?.GetDeepHashCode() ?? 0;
		}
	}
}
