// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Text.Json.Serialization;
using ActiveOps.Configuration;
using ActiveOps.Controllers;
using ActiveOps.Features;
using ActiveOps.Filters;
using ActiveOps.HealthChecks;
using ActiveOps.Serialization;
using ActiveOptions;
using ActiveRoutes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ActiveOps
{
	public static class Add
	{
		public static IServiceCollection AddOptionsDebuggerApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddOptionsDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddOptionsDebuggerApi(this IServiceCollection services,
			Action<OptionsDebugOptions> configureAction = null)
		{
			services.AddValidOptions();
			services.AddSaveOptions();

			services.AddActiveRouting(builder =>
			{
				AddOptionsDebuggerApi(builder, configureAction);
			});

			return services;
		}

		public static IServiceCollection AddServicesDebuggerApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddServicesDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddServicesDebuggerApi(this IServiceCollection services,
			Action<ServicesDebugOptions> configureAction = null)
		{
			services.TryAddSingleton(services);

			services.AddActiveRouting(builder =>
			{
				builder.AddServicesDebuggerApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddHostedServicesDebuggerApi(this IServiceCollection services,
			IConfiguration config)
		{
			return services.AddHostedServicesDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddHostedServicesDebuggerApi(this IServiceCollection services,
			Action<HostedServicesDebugOptions> configureAction = null)
		{
			services.TryAddSingleton(services);

			services.AddActiveRouting(builder =>
			{
				builder.AddHostedServicesDebuggerApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddFeaturesDebuggerApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddFeaturesDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddFeaturesDebuggerApi(this IServiceCollection services,
			Action<FeaturesDebugOptions> configureAction = null)
		{
			services.TryAddSingleton(services);

			services.AddActiveRouting(builder =>
			{
				builder.AddFeaturesDebuggerApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddHealthChecksApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddHealthChecksApi(config.FastBind);
		}

		public static IServiceCollection AddHealthChecksApi(this IServiceCollection services,
			Action<HealthChecksDebugOptions> configureAction = null)
		{
			services.AddActiveRouting(builder =>
			{
				builder.AddHealthChecksApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddEnvironmentApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddEnvironmentApi(config.FastBind);
		}

		public static IServiceCollection AddEnvironmentApi(this IServiceCollection services,
			Action<EnvironmentDebugOptions> configureAction = null)
		{
			services.AddActiveRouting(builder =>
			{
				builder.AddEnvironmentApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddRoutesDebuggerApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddRoutesDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddRoutesDebuggerApi(this IServiceCollection services,
			Action<RoutesDebugOptions> configureAction = null)
		{
			services.AddActiveRouting(builder =>
			{
				builder.AddRoutesDebuggerApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddCachesDebuggerApi(this IServiceCollection services, IConfiguration config)
		{
			return services.AddCachesDebuggerApi(config.FastBind);
		}

		public static IServiceCollection AddCachesDebuggerApi(this IServiceCollection services,
			Action<CachesDebugOptions> configureAction = null)
		{
			services.AddActiveRouting(builder =>
			{
				builder.AddCachesDebuggerApi(configureAction);
			});

			return services;
		}

		public static IServiceCollection AddStartupHealthChecks(this IServiceCollection services)
		{
			services.TryAddTransient<IStartupFilter, HealthCheckStartupFilter>();

			var builder = services.AddHealthChecks();
			builder.AddCheck<ServicesHealth>(nameof(ServicesHealth), HealthStatus.Unhealthy, new[] {"ops", "startup"});
			builder.AddCheck<OptionsHealth>(nameof(OptionsHealth), HealthStatus.Unhealthy, new[] {"ops", "startup"});
			return services;
		}

		private static void AddOptionsDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<OptionsDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder.AddJsonOptions(o =>
			{
				o.JsonSerializerOptions.IgnoreNullValues = true;
				o.JsonSerializerOptions.Converters.Add(new IgnoreConverter());
				o.JsonSerializerOptions.Converters.Add(new EnumDictionaryConverter());
				o.JsonSerializerOptions.Converters.Add(new IpAddressConverter());
			});

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, OptionsDebugController, OptionsDebugFeature, OptionsDebugOptions>();
		}

		private static void AddServicesDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<ServicesDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder.AddJsonOptions(o =>
			{
				o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			});

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, ServicesDebugController, ServicesDebugFeature, ServicesDebugOptions
				>();
		}

		private static void AddHostedServicesDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<HostedServicesDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, HostedServicesDebugController, HostedServicesDebugFeature,
					HostedServicesDebugOptions>();
		}

		private static void AddFeaturesDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<FeaturesDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, FeaturesDebugController, FeaturesDebugFeature, FeaturesDebugOptions
				>();
		}

		private static void AddHealthChecksApi(this IMvcCoreBuilder mvcBuilder,
			Action<HealthChecksDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, HealthChecksDebugController, HealthChecksDebugFeature,
					HealthChecksDebugOptions>();
		}

		private static void AddEnvironmentApi(this IMvcCoreBuilder mvcBuilder,
			Action<EnvironmentDebugOptions> configureAction = null)
		{
			if (configureAction != null)
			{
				mvcBuilder.Services.Configure(configureAction);
			}

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, EnvironmentDebugController, EnvironmentDebugFeature,
					EnvironmentDebugOptions>();
		}

		private static void AddRoutesDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<RoutesDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, RoutesDebugController, RoutesDebugFeature, RoutesDebugOptions>();
		}

		private static void AddCachesDebuggerApi(this IMvcCoreBuilder mvcBuilder,
			Action<CachesDebugOptions> configureAction = null)
		{
			if (configureAction != null)
				mvcBuilder.Services.Configure(configureAction);

			mvcBuilder
				.AddActiveRoute<OperationsBuilder, CachesDebugController, CachesDebugFeature, CachesDebugOptions>();
		}
	}
}