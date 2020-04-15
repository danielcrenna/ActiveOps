// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActiveOps.Serialization
{
	internal sealed class IgnoreConverter : JsonConverter<object>
	{
		public static HashSet<Type> IgnoreTypes = new HashSet<Type>();

		static IgnoreConverter()
		{
			IgnoreTypes.Add(typeof(Assembly));
			IgnoreTypes.Add(typeof(Module));
			IgnoreTypes.Add(typeof(Type));
			IgnoreTypes.Add(typeof(MethodBase));
			IgnoreTypes.Add(typeof(MemberInfo));
			IgnoreTypes.Add(typeof(RuntimeMethodHandle));
			IgnoreTypes.Add(typeof(Delegate));
			IgnoreTypes.Add(typeof(IServiceProvider));
		}

		public override bool CanConvert(Type typeToConvert)
		{
			return IgnoreTypes.Contains(typeToConvert) || typeof(MulticastDelegate).IsAssignableFrom(typeToConvert);
		}

		public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return JsonSerializer.Deserialize(ref reader, typeToConvert, options);
		}

		public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
		{
			writer.WriteNullValue();
		}
	}
}