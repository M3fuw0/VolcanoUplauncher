using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal class QueryFilter : PathFilter
	{
		internal QueryExpression Expression;

		public QueryFilter(QueryExpression expression)
		{
			Expression = expression;
		}

		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch)
		{
			foreach (JToken item in current)
			{
				foreach (JToken item2 in (IEnumerable<JToken>)item)
				{
					if (Expression.IsMatch(root, item2))
					{
						yield return item2;
					}
				}
			}
		}
	}
}
