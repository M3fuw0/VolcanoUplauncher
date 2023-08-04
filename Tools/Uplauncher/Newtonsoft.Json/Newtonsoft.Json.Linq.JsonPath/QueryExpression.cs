using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	internal abstract class QueryExpression
	{
		internal QueryOperator Operator;

		public QueryExpression(QueryOperator @operator)
		{
			Operator = @operator;
		}

		/*[NullableContext(1)]*/
		public abstract bool IsMatch(JToken root, JToken t);
	}
}
