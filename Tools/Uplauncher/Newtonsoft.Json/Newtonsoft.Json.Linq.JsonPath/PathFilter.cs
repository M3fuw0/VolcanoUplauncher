using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Linq.JsonPath
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal abstract class PathFilter
	{
		public abstract IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, bool errorWhenNoMatch);

		/*[return: Nullable(2)]*/
		protected static JToken GetTokenIndex(JToken t, bool errorWhenNoMatch, int index)
		{
			if (t is JArray jArray)
			{
				if (jArray.Count <= index)
				{
					if (errorWhenNoMatch)
					{
						throw new JsonException("Index {0} outside the bounds of JArray.".FormatWith(CultureInfo.InvariantCulture, index));
					}
					return null;
				}
				return jArray[index];
			}
			if (t is JConstructor jConstructor)
			{
				if (jConstructor.Count <= index)
				{
					if (errorWhenNoMatch)
					{
						throw new JsonException("Index {0} outside the bounds of JConstructor.".FormatWith(CultureInfo.InvariantCulture, index));
					}
					return null;
				}
				return jConstructor[index];
			}
			if (errorWhenNoMatch)
			{
				throw new JsonException("Index {0} not valid on {1}.".FormatWith(CultureInfo.InvariantCulture, index, t.GetType().Name));
			}
			return null;
		}

		/*[NullableContext(2)]*/
		protected static JToken GetNextScanValue(/*[Nullable(1)]*/ JToken originalParent, JToken container, JToken value)
		{
			if (container != null && container.HasValues)
			{
				value = container.First;
			}
			else
			{
				while (value != null && value != originalParent && value == value.Parent.Last)
				{
					value = value.Parent;
				}
				if (value == null || value == originalParent)
				{
					return null;
				}
				value = value.Next;
			}
			return value;
		}
	}
}
