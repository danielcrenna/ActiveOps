// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace ActiveOps.Models
{
	public class ServiceReport
	{
		public ServiceLifetime Lifetime { get; set; }
		public string ImplementationType { get; set; }
		public string ImplementationInstance { get; set; }
		public string ImplementationFactory { get; set; }
		public string ServiceType { get; set; }
	}
}