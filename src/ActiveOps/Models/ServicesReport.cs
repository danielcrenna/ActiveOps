// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace ActiveOps.Models
{
	public class ServicesReport
	{
		public HashSet<string> MissingRegistrations { get; set; }
		public IList<ServiceReport> Services { get; set; }
	}
}