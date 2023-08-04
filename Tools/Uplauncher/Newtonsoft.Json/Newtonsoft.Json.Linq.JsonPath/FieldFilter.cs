using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	/*[NullableContext(2)]*/
	/*[Nullable(0)]*/
	internal class FieldFilter : PathFilter
	{
		internal string Name;

		public FieldFilter(string name)
		{
			Name = name;
		}

		/*[NullableContext(1)]*/
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken item in current)
			{
				if (item is JObject jObject)
				{
					if (Name != null)
					{
						JToken jToken = jObject[Name];
						if (jToken != null)
						{
							yield return jToken;
						}
						else if (errorWhenNoMatch)
						{
							throw new JsonException("Property '{0}' does not exist on JObject.".FormatWith(CultureInfo.InvariantCulture, Name));
						}
						continue;
					}
					foreach (KeyValuePair<string, JToken> item2 in jObject)
					{
						yield return item2.Value;
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Property '{0}' not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, Name ?? "*", item.GetType().Name));
				}
			}
		}
	}
}
