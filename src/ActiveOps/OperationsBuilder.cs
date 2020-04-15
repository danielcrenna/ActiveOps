// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveRoutes;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveOps
{
	public class OperationsBuilder : IFeatureBuilder
	{
		public OperationsBuilder(IServiceCollection services) => Services = services;

		public IServiceCollection Services { get; }
	}
}