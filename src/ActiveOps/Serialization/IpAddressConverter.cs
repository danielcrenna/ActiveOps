﻿// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActiveOps.Serialization
{
	internal sealed class IpAddressConverter : JsonConverter<IPAddress>
	{
		public override IPAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return IPAddress.Parse(reader.GetString());
		}

		public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options)
		{
			writer.WriteStringValue(value.ToString());
		}
	}
}