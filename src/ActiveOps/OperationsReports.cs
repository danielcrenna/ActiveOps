// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveOps
{
	internal static class OperationsReports
	{
		public class HostedServicesReport
		{
			public List<string> Services { get; set; } = new List<string>();
		}


		public class ServicesReport
		{
			public HashSet<string> MissingRegistrations { get; set; }
			public List<ServiceReport> Services { get; set; }
		}

		public class ServiceReport
		{
			public ServiceLifetime Lifetime { get; set; }
			public string ImplementationType { get; set; }
			public string ImplementationInstance { get; set; }
			public string ImplementationFactory { get; set; }
			public string ServiceType { get; set; }
		}

		public class OptionsReport
		{
			public bool HasErrors { get; set; }
			public List<OptionReport> Options { get; set; }
		}

		public class OptionReport
		{
			public string Scope { get; set; }
			public bool HasErrors { get; set; }
			public List<OptionBindingReport> Values { get; set; }
		}

		public class OptionBindingReport
		{
			public string Type { get; set; }
			public bool IsValid { get; set; }
			public object Value { get; set; }
		}
	}
}