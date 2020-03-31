// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using ActiveOps.Configuration;
using ActiveRoutes.Meta;
using Microsoft.Extensions.Options;

namespace ActiveOps
{
	internal class OperationsMetaProvider : IMetaProvider
	{
		private readonly IOptions<OperationsApiOptions> _options;
		private readonly IEnumerable<IMetaParameterProvider> _parameterProviders;

		public OperationsMetaProvider(IEnumerable<IMetaParameterProvider> parameterProviders,
			IOptions<OperationsApiOptions> options)
		{
			_parameterProviders = parameterProviders;
			_options = options;
		}

		public void Populate(string baseUri, MetaCollection collection, IServiceProvider serviceProvider)
		{
			var versionString = typeof(OperationsMetaProvider).Assembly.GetName().Version.ToString();
			var options = _options.Value;

			if (!options.EnableRouteDebugging &&
			    !options.EnableOptionsDebugging &&
			    !options.EnableEnvironmentEndpoint &&
			    !options.EnableServicesDebugging &&
			    !options.EnableHostedServicesDebugging &&
			    !options.EnableMetricsEndpoint &&
			    !options.EnableHealthChecksEndpoints &&
			    !options.EnableFeatureDebugging &&
			    !options.EnableCacheDebugging)
			{
				return;
			}

			const string auth = "bearer";

			var folder = new MetaFolder
			{
				name = "Operations",
				description = new MetaDescription
				{
					content = "Provides diagnostic tools for server operators at runtime.",
					type = MediaTypeNames.Text.Markdown,
					version = null
				},
				variable = new List<dynamic>(),
				item = new List<MetaItem>(),
				@event = new List<dynamic>(),
				auth = auth,
				protocolProfileBehavior = new { }
			};

			var rootPath = options.RootPath.TrimStart('/');

			if (options.EnableRouteDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Route Diagnostics",
					Description = "Used to detect resolution issues in API path routing.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.RouteDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableOptionsDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Configuration Diagnostics",
					Description =
						"Used to detect configuration binding errors and other issues with configuration, as well as inspect current values of all configurations.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.OptionsDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableEnvironmentEndpoint)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Environment Diagnostics",
					Description = "Used to obtain diagnostic runtime information from the running node instance.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.EnvironmentEndpointPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableServicesDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Services Diagnostics",
					Description =
						"Used to detect errors in dependency injection (DI) or inversion of control (IoC) in the application container.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.ServicesDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableHostedServicesDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Hosted Services Diagnostics",
					Description = "Used to introspect services managed by the underlying host.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.HostedServicesDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableMetricsEndpoint)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Metrics Sample",
					Description = "Used to sample all registered metrics in the system for reporting purposes.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.MetricsEndpointPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableHealthChecksEndpoints)
			{
				if (!string.IsNullOrWhiteSpace(options.HealthChecksPath))
				{
					var descriptor = new EndpointDescriptor
					{
						Auth = auth,
						Name = "Health Checks (full)",
						Description =
							"Used to monitor an API for its ability to respond to requests. This method checks all registered health checks for internal systems.",
						Method = HttpMethod.Get,
						Url = $"{baseUri}/{rootPath + options.HealthChecksPath}",
						Version = versionString
					};

					folder.item.Add(MapFrom(descriptor, serviceProvider));
				}

				if (!string.IsNullOrWhiteSpace(options.HealthCheckLivePath))
				{
					var descriptor = new EndpointDescriptor
					{
						Auth = auth,
						Name = "Health Check (live-only)",
						Description =
							"Used to monitor an API for its ability to respond to requests. This method does not check internal systems.",
						Method = HttpMethod.Get,
						Url = $"{baseUri}/{rootPath + options.HealthCheckLivePath}",
						Version = versionString
					};

					folder.item.Add(MapFrom(descriptor, serviceProvider));
				}
			}

			if (options.EnableFeatureDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Feature Diagnostics",
					Description = "Used to diagnose feature toggles, A/B testing, and cohorts.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.FeatureDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			if (options.EnableCacheDebugging)
			{
				var descriptor = new EndpointDescriptor
				{
					Auth = auth,
					Name = "Cache Diagnostics",
					Description = "Used to diagnose cache size, throughput, contention, and memory pressure.",
					Method = HttpMethod.Get,
					Url = $"{baseUri}/{rootPath + options.CacheDebuggingPath}",
					Version = versionString
				};
				folder.item.Add(MapFrom(descriptor, serviceProvider));
			}

			collection.item.Add(folder);

			collection.item.Sort();
		}

		private MetaItem MapFrom(EndpointDescriptor descriptor, IServiceProvider serviceProvider)
		{
			var operation = new MetaOperation
			{
				url = MetaUrl.FromRaw(descriptor.Url),
				auth = descriptor.Auth,
				proxy = new { },
				certificate = new { },
				method = descriptor.Method.ToString(),
				description =
					new MetaDescription
					{
						content = descriptor.Description, type = "text/markdown", version = descriptor.Version
					},
				header = new List<MetaParameter>
				{
					new MetaParameter
					{
						key = "Content-Type",
						value = "application/json",
						disabled = false,
						description = "" /*new MetaDescription
                            {
                                content = "",
                                type = "text/markdown",
                                version = descriptor.Version
                            }*/
					}
				},
				body = default
			};

			foreach (var provider in _parameterProviders)
				provider.Enrich(descriptor.Url, operation, serviceProvider);

			var item = new MetaItem
			{
				id = Guid.NewGuid(),
				name = descriptor.Name,
				description =
					new MetaDescription {content = descriptor.Description, type = "text/markdown", version = null},
				variable = new List<dynamic>(),
				@event = new List<dynamic>(),
				request = operation,
				response = new List<dynamic>(),
				protocolProfileBehavior = new { }
			};
			return item;
		}
	}
}