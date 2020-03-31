// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveRoutes;
using Metrics;

namespace ActiveOps.Configuration
{
	public class OperationsApiOptions : IFeatureScheme, IFeaturePolicy, IFeatureNamespace
	{
		public bool EnableRouteDebugging { get; set; } = true;
		public string RouteDebuggingPath { get; set; } = "/routes";

		public bool EnableEnvironmentEndpoint { get; set; } = true;
		public string EnvironmentEndpointPath { get; set; } = "/env";

		public bool EnableOptionsDebugging { get; set; } = true;
		public string OptionsDebuggingPath { get; set; } = "/options";

		public bool EnableServicesDebugging { get; set; } = true;
		public string ServicesDebuggingPath { get; set; } = "/services";

		public bool EnableHostedServicesDebugging { get; set; } = true;
		public string HostedServicesDebuggingPath { get; set; } = "/hostedServices";

		public bool EnableFeatureDebugging { get; set; } = true;
		public string FeatureDebuggingPath { get; set; } = "/features";

		public bool EnableCacheDebugging { get; set; } = true;
		public string CacheDebuggingPath { get; set; } = "/caches";

		public bool EnableRequestProfiling { get; set; } = true;
		public string RequestProfilingHeader { get; set; } = HttpHeaders.ServerTiming;

		public bool EnableHealthChecks { get; set; } = true;
		public bool EnableHealthChecksEndpoints { get; set; } = true;
		public string HealthCheckLivePath { get; set; } = "/ping";
		public string HealthChecksPath { get; set; } = "/health";

		public bool EnableMetricsEndpoint { get; set; } = true;
		public string MetricsEndpointPath { get; set; } = "/metrics";

		public MetricsOptions MetricsOptions { get; set; } = new MetricsOptions();
		public string RootPath { get; set; } = "/ops";
		public string Policy { get; set; }

		public string Scheme { get; set; }
	}
}