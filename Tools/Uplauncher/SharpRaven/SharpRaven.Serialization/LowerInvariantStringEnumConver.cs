using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SharpRaven.Serialization
{
	public class LowerInvariantStringEnumConverter : StringEnumConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value is Enum)
			{
				string text = value.ToString().ToLowerInvariant();
				writer.WriteValue(text);
			}
		}
	}
}
