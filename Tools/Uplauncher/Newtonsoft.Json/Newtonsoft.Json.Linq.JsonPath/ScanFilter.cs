using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	/*[NullableContext(2)]*/
	/*[Nullable(0)]*/
	internal class ScanFilter : PathFilter
	{
		internal string Name;

		public ScanFilter(string name)
		{
			Name = name;
		}

		/*[NullableContext(1)]*/
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken c in current)
			{
				if (Name == null)
				{
					yield return c;
				}
				JToken value = c;
				while (true)
				{
					JContainer container = value as JContainer;
					value = GetNextScanValue(c, container, value);
					if (value == null)
					{
						break;
					}
					if (value is JProperty jProperty)
					{
						if (jProperty.Name == Name)
						{
							yield return jProperty.Value;
						}
					}
					else if (Name == null)
					{
						yield return value;
					}
				}
			}
		}
	}
}
