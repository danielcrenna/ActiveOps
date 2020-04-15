// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace ActiveOps.Models
{
	[DebuggerDisplay("[{Type}, IsValid={IsValid}]: {Value}")]
	public class OptionBindingReport
	{
		public string Type { get; set; }
		public bool IsValid { get; set; }
		public object Value { get; set; }
	}
}