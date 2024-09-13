using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	public abstract class CustomCreationConverter< T> : JsonConverter
	{
		public override bool CanWrite => false;

		public override void WriteJson(JsonWriter writer,  object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should only be used while deserializing.");
		}

		/*[return: Nullable(2)]*/
		public override object ReadJson(JsonReader reader, Type objectType,  object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			T val = Create(objectType);
			if (val == null)
			{
				throw new JsonSerializationException("No object created.");
			}
			serializer.Populate(reader, val);
			return val;
		}

		public abstract T Create(Type objectType);

		public override bool CanConvert(Type objectType)
		{
			return typeof(T).IsAssignableFrom(objectType);
		}
	}
}
