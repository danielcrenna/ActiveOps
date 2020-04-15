// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveRoutes;

namespace ActiveOps.Configuration
{
	public class EnvironmentDebugOptions : IFeatureToggle, IFeatureNamespace, IFeatureScheme, IFeaturePolicy
	{
		public string RootPath { get; } = "/env";
		public string Policy { get; set; }
		public string Scheme { get; set; }
		public bool Enabled { get; set; }
	}
}