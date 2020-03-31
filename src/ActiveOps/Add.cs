// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using ActiveOps.Configuration;
using ActiveOptions;
using ActiveRoutes.Meta;
using Metrics;
using Metrics.Reporters.ServerTiming;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ActiveOps
{
	public static class Add
	{
		public static IServiceCollection AddOperationsApi(this IServiceCollection services, IConfiguration config)
		{
			return AddOperationsApi(services, config.FastBind);
		}

		public static IServiceCollection AddOperationsApi(this IServiceCollection services,
			Action<OperationsApiOptions> configureAction = null)
		{
			if (configureAction != null)
				services.Configure(configureAction);

			var options = new OperationsApiOptions();
			configureAction?.Invoke(options);

			if (options.EnableHealthChecks)
				services.AddTransient<IStartupFilter, HealthCheckStartupFilter>();

			services.AddValidOptions();
			services.AddSaveOptions();

			services.AddScoped<IMetaProvider, OperationsMetaProvider>();

			services.AddMetrics(c =>
			{
				c.AddCheck<OperationsHealthChecks.ServicesHealth>(nameof(OperationsHealthChecks.ServicesHealth),
					HealthStatus.Unhealthy, new[] {"ops", "startup"});

				c.AddCheck<OperationsHealthChecks.OptionsHealth>(nameof(OperationsHealthChecks.OptionsHealth),
					HealthStatus.Unhealthy, new[] {"ops", "startup"});

				c.AddServerTimingReporter(o =>
				{
					o.Enabled = true;
					o.Filter = "*";
					o.Rendering = ServerTimingRendering.Verbose;
					o.AllowedOrigins = "*";
				});
			});

			return services;
		}
	}
}