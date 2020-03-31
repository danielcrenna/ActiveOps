// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ActiveOps
{
	/// <summary>
	///     Fails startup if health checks fail. This is a fat canary.
	/// </summary>
	public class HealthCheckStartupFilter : IStartupFilter
	{
		private readonly IOptionsMonitor<HealthCheckOptions> _options;
		private readonly HealthCheckService _service;

		public HealthCheckStartupFilter(HealthCheckService service, IOptionsMonitor<HealthCheckOptions> options)
		{
			_options = options;
			_service = service;
		}

		public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
		{
			var report = _service.CheckHealthAsync(r => r.Tags.Contains("startup")).GetAwaiter().GetResult();

			if (report.Status == HealthStatus.Unhealthy)
			{
				throw new Exception("Application failed to start due to failing startup health checks.");
			}

			return next;
		}
	}
}