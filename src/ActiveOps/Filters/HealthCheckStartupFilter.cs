// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using ActiveOps.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ActiveOps.Filters
{
	/// <summary>
	///     Fails startup if health checks fail. This is a fat canary.
	/// </summary>
	public class HealthCheckStartupFilter : IStartupFilter
	{
		private readonly IOptionsSnapshot<OptionsDebugOptions> _optionsDebug;
		private readonly HealthCheckService _service;
		private readonly IOptionsSnapshot<ServicesDebugOptions> _serviceDebug;

		public HealthCheckStartupFilter(HealthCheckService service,
			IOptionsSnapshot<ServicesDebugOptions> serviceDebug,
			IOptionsSnapshot<OptionsDebugOptions> optionsDebug)
		{
			_service = service;
			_serviceDebug = serviceDebug;
			_optionsDebug = optionsDebug;
		}

		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			if (!AnyStartupHealthChecksIncluded())
				return next;

			var report = _service.CheckHealthAsync(r => r.Tags.Contains("startup")).GetAwaiter().GetResult();

			return report.Status == HealthStatus.Unhealthy
				? throw new Exception("Application failed to start due to failing startup health checks.")
				: next;
		}

		private bool AnyStartupHealthChecksIncluded()
		{
			return _optionsDebug.Value.Enabled && _optionsDebug.Value.IncludeInHealthChecks ||
			       _serviceDebug.Value.Enabled && _serviceDebug.Value.IncludeInHealthChecks;
		}
	}
}