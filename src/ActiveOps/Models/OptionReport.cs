// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;

namespace ActiveOps.Models
{
	[DebuggerDisplay("[{Scope}, HasErrors={HasErrors}]: {Values}")]
	public class OptionReport
	{
		public string Scope { get; set; }
		public bool HasErrors { get; set; }
		public IList<OptionBindingReport> Values { get; set; }
	}
}