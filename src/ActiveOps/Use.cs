// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Threading.Tasks;
using ActiveOps.Configuration;
using Metrics.Reporters.ServerTiming;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TypeKitchen;

namespace ActiveOps
{
	public static class Use
	{
		public static IApplicationBuilder UseOperationsApi(this IApplicationBuilder app)
		{
			app.UseServerTimingReporter();
			app.UseRequestProfiling();
			app.UseOperationsEndpoints();
			return app;
		}

		internal static IApplicationBuilder UseRequestProfiling(this IApplicationBuilder app)
		{
			return app.Use(async (context, next) =>
			{
				var options = context.RequestServices.GetService<IOptions<OperationsApiOptions>>();

				if (options?.Value != null && options.Value.EnableRequestProfiling &&
				    !options.Value.MetricsOptions.EnableServerTiming)
				{
					var sw = Pooling.StopwatchPool.Pool.Get();

					context.Response.OnStarting(() =>
					{
						var duration = sw.Elapsed;
						Pooling.StopwatchPool.Pool.Return(sw);
						var header = options.Value.RequestProfilingHeader ?? HttpHeaders.ServerTiming;
						context.Response.Headers.Add(header, $"roundtrip;dur={duration.TotalMilliseconds};desc=\"*\"");
						return Task.CompletedTask;
					});
				}

				await next();
			});
		}

		internal static IApplicationBuilder UseOperationsEndpoints(this IApplicationBuilder app)
		{
			return app.Use(async (context, next) =>
			{
				var options = context.RequestServices.GetRequiredService<IOptions<OperationsApiOptions>>();

				if (options.Value != null &&
				    options.Value.EnableEnvironmentEndpoint &&
				    !string.IsNullOrWhiteSpace(options.Value.EnvironmentEndpointPath) &&
				    context.Request.Path.Value.StartsWith(
					    options.Value.RootPath + options.Value.EnvironmentEndpointPath))
				{
					await OperationsHandlers.GetEnvironmentHandler(app, context);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableRouteDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.RouteDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.RouteDebuggingPath))
				{
					await OperationsHandlers.GetRoutesDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableOptionsDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.OptionsDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.OptionsDebuggingPath))
				{
					await OperationsHandlers.GetOptionsDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableServicesDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.ServicesDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.ServicesDebuggingPath))
				{
					await OperationsHandlers.GetServicesDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableHostedServicesDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.HostedServicesDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath +
				                                          options.Value.HostedServicesDebuggingPath))
				{
					await OperationsHandlers.GetHostedServicesDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableMetricsEndpoint &&
				    !string.IsNullOrWhiteSpace(options.Value.MetricsEndpointPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.MetricsEndpointPath))
				{
					await OperationsHandlers.GetMetricsHandler(context, options.Value, app);
					return;
				}

				if (options.Value != null && options.Value.EnableHealthChecksEndpoints)
				{
					if (!string.IsNullOrWhiteSpace(options.Value.HealthCheckLivePath) &&
					    context.Request.Path.Value.StartsWith(
						    options.Value.RootPath + options.Value.HealthCheckLivePath))
					{
						await OperationsHandlers.GetHealthChecksHandler(context, r => false, app);
						return;
					}

					if (!string.IsNullOrWhiteSpace(options.Value.HealthChecksPath) &&
					    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.HealthChecksPath))
					{
						context.Request.Query.TryGetValue("tags", out var tags);
						await OperationsHandlers.GetHealthChecksHandler(context, r => r.Tags.IsSupersetOf(tags), app);
						return;
					}
				}

				if (options.Value != null &&
				    options.Value.EnableFeatureDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.FeatureDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.FeatureDebuggingPath))
				{
					await OperationsHandlers.GetFeaturesDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableCacheDebugging &&
				    !string.IsNullOrWhiteSpace(options.Value.CacheDebuggingPath) &&
				    context.Request.Path.Value.StartsWith(options.Value.RootPath + options.Value.CacheDebuggingPath))
				{
					await OperationsHandlers.GetCacheDebugHandler(context, app);
					return;
				}

				if (options.Value != null &&
				    options.Value.EnableEnvironmentEndpoint &&
				    !string.IsNullOrWhiteSpace(options.Value.EnvironmentEndpointPath) &&
				    context.Request.Path.Value.StartsWith(
					    options.Value.RootPath + options.Value.EnvironmentEndpointPath))
				{
					await OperationsHandlers.GetEnvironmentHandler(app, context);
					return;
				}

				await next();
			});
		}
	}
}