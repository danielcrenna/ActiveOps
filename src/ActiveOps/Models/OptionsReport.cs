// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ActiveOps.Models
{
	public class OptionsReport
	{
		public bool HasErrors { get; set; }
		public IList<OptionReport> Options { get; set; }
	}
}