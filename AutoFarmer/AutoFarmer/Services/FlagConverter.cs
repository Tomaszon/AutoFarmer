using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AutoFarmer.Services
{
	public class FlagConverter<T> : JsonConverter where T : Enum, new()
	{
		public override bool CanConvert(Type objectType)
		{
			throw new NotImplementedException();
		}

		public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
		{
			string? raw = reader.Value as string;

			if (raw is { })
			{
				dynamic result = new T();

				var array = raw.Split('|');

				foreach (string item in array)
				{
					result |= (T)Enum.Parse(typeof(T), item);
				}

				return result;

			}

			return null;
		}

		public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
		{
			if (value is { })
			{
				writer.WriteRawValue($"\"{value.ToString()?.Replace(", ", "|")}\"");
			}
		}
	}
}
