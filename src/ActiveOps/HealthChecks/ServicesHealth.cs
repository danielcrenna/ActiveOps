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
	public sealed class ServicesHealth : IHealthCheck
	{
		private readonly IServiceProvider _serviceProvider;

		public ServicesHealth(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
			CancellationToken cancellationToken = new CancellationToken())
		{
			string description;
			HealthStatus status;
			Exception exception = null;
			IReadOnlyDictionary<string, object> data = null;

			try
			{
				var report = OperationsMethods.ServicesReport(_serviceProvider);

				status = report.MissingRegistrations.Count > 0 ? HealthStatus.Degraded : HealthStatus.Healthy;
				data = status == HealthStatus.Healthy
					? null
					: report.MissingRegistrations.ToDictionary(k => k,
						v => (object) report.Services.FirstOrDefault(x => x.ServiceType == v));

				switch (status)
				{
					case HealthStatus.Unhealthy:
						description =
							"The DI container for this application has a missing registration, hiding a runtime exception.";
						break;
					case HealthStatus.Healthy:
						description = "The DI container is correctly configured for this application.";
						break;
					case HealthStatus.Degraded:
						description =
							"The DI container for this application has a missing registration, hiding a runtime exception.";
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (Exception e)
			{
				status = HealthStatus.Unhealthy;
				description = "The DI container health check faulted.";
				exception = e;
			}

			var result = new HealthCheckResult(status, description, exception, data);

			return Task.FromResult(result);
		}
	}
}