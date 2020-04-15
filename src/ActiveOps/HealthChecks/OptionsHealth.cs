// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ActiveOps.HealthChecks
{
	public class OptionsHealth : IHealthCheck
	{
		private readonly IServiceProvider _serviceProvider;

		public OptionsHealth(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = new CancellationToken())
		{
			string description;
			HealthStatus status;
			Exception exception = null;
			IReadOnlyDictionary<string, object> data = null;

			try
			{
				var report = OperationsMethods.OptionsReport(_serviceProvider);

				status = report.HasErrors ? HealthStatus.Unhealthy : HealthStatus.Healthy;
				data = status != HealthStatus.Healthy
					? report.Options.Where(x => x.HasErrors).ToDictionary(k => k.Scope, v => (object) v.Values)
					: null;

				switch (status)
				{
					case HealthStatus.Unhealthy:
						description =
							"The options configuration for this application has one or more binding errors, hiding runtime exceptions.";
						break;
					case HealthStatus.Healthy:
						description = "The options configuration for this application is binding correctly.";
						break;
					case HealthStatus.Degraded:
						throw new NotImplementedException();
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (Exception e)
			{
				status = HealthStatus.Unhealthy;
				description = "The options configuration health check faulted.";
				exception = e;
			}

			var result = new HealthCheckResult(status, description, exception, data);

			return Task.FromResult(result);
		}
	}
}