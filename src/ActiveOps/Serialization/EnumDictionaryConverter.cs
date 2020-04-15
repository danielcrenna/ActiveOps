// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActiveOps.Serialization
{
	internal sealed class EnumDictionaryConverter : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
			       typeToConvert.GetGenericArguments()[0].IsEnum;
		}

		public override JsonConverter CreateConverter(
			Type type,
			JsonSerializerOptions options)
		{
			var keyType = type.GetGenericArguments()[0];
			var valueType = type.GetGenericArguments()[1];

			var converter = (JsonConverter) Activator.CreateInstance(
				typeof(DictionaryEnumConverterInner<,>).MakeGenericType(keyType, valueType),
				BindingFlags.Instance | BindingFlags.Public,
				null,
				new object[] {options},
				null);

			return converter;
		}

		private class DictionaryEnumConverterInner<TKey, TValue> :
			JsonConverter<Dictionary<TKey, TValue>> where TKey : struct, Enum
		{
			private readonly Type _keyType;
			private readonly JsonConverter<TValue> _valueConverter;
			private readonly Type _valueType;

			public DictionaryEnumConverterInner(JsonSerializerOptions options)
			{
				// For performance, use the existing converter if available.
				_valueConverter = (JsonConverter<TValue>) options
					.GetConverter(typeof(TValue));

				// Cache the key and value types.
				_keyType = typeof(TKey);
				_valueType = typeof(TValue);
			}

			public override Dictionary<TKey, TValue> Read(
				ref Utf8JsonReader reader,
				Type typeToConvert,
				JsonSerializerOptions options)
			{
				if (reader.TokenType != JsonTokenType.StartObject)
				{
					throw new JsonException();
				}

				var dictionary = new Dictionary<TKey, TValue>();

				while (reader.Read())
				{
					if (reader.TokenType == JsonTokenType.EndObject)
					{
						return dictionary;
					}

					// Get the key.
					if (reader.TokenType != JsonTokenType.PropertyName)
					{
						throw new JsonException();
					}

					var propertyName = reader.GetString();

					// For performance, parse with ignoreCase:false first.
					if (!Enum.TryParse(propertyName, false, out TKey key) &&
					    !Enum.TryParse(propertyName, true, out key))
					{
						throw new JsonException(
							$"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");
					}

					// Get the value.
					TValue v;
					if (_valueConverter != null)
					{
						reader.Read();
						v = _valueConverter.Read(ref reader, _valueType, options);
					}
					else
					{
						v = JsonSerializer.Deserialize<TValue>(ref reader, options);
					}

					// Add to dictionary.
					dictionary.Add(key, v);
				}

				throw new JsonException();
			}

			public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> dictionary,
				JsonSerializerOptions options)
			{
				writer.WriteStartObject();

				foreach (var (k, v) in dictionary)
				{
					writer.WritePropertyName(k.ToString());

					if (_valueConverter != null)
					{
						_valueConverter.Write(writer, v, options);
					}
					else
					{
						JsonSerializer.Serialize(writer, v, options);
					}
				}

				writer.WriteEndObject();
			}
		}
	}
}