using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal class ArrayIndexFilter : PathFilter
	{
		public int? Index { get; set; }

		/*[NullableContext(1)]*/
		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken item in current)
			{
				if (Index.HasValue)
				{
					JToken tokenIndex = GetTokenIndex(item, errorWhenNoMatch, Index.GetValueOrDefault());
					if (tokenIndex != null)
					{
						yield return tokenIndex;
					}
				}
				else if (item is JArray || item is JConstructor)
				{
					foreach (JToken item2 in (IEnumerable<JToken>)item)
					{
						yield return item2;
					}
				}
				else if (errorWhenNoMatch)
				{
					throw new JsonException("Index * not valid on {0}.".FormatWith(CultureInfo.InvariantCulture, item.GetType().Name));
				}
			}
		}
	}
}
