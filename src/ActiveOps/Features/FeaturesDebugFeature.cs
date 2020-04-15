// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ActiveOps.Controllers;
using ActiveRoutes;

namespace ActiveOps.Features
{
	internal sealed class FeaturesDebugFeature : DynamicFeature
	{
		public FeaturesDebugFeature() => ControllerTypes = new List<Type> {typeof(FeaturesDebugController)};
		public override IList<Type> ControllerTypes { get; }
	}
}