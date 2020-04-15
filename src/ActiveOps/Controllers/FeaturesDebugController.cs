// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ActiveRoutes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ActiveOps.Controllers
{
	public class FeaturesDebugController : Controller
	{
		[DynamicHttpGet("")]
		public IActionResult Get()
		{
			var registered = new Dictionary<string, bool>();
			var unregistered = new HashSet<string>();
			var indeterminate = new HashSet<string>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var featureType in assembly.GetTypes())
				{
					if (!typeof(IFeatureToggle).IsAssignableFrom(featureType))
						continue;

					if (featureType.IsInterface)
						continue;

					Type optionsWrapperType;

					try
					{
						optionsWrapperType = typeof(IOptions<>).MakeGenericType(featureType);
					}
					catch
					{
						indeterminate.Add(featureType.Name);
						continue;
					}

					var instance =
						HttpContext.RequestServices.GetService(optionsWrapperType) ??
						HttpContext.RequestServices.GetService(featureType);

					if (instance == null)
					{
						unregistered.Add(featureType.Name);
						continue;
					}

					var serviceType = instance.GetType();
					if (optionsWrapperType.IsAssignableFrom(serviceType))
					{
						var property = optionsWrapperType.GetProperty(nameof(IOptions<object>.Value));
						if (!(property?.GetValue(instance) is IFeatureToggle feature))
						{
							indeterminate.Add(featureType.Name);
							continue;
						}

						registered[featureType.Name] = feature.Enabled;
						continue;
					}

					if (serviceType == featureType)
					{
						if (instance is IFeatureToggle feature)
						{
							registered[featureType.Name] = feature.Enabled;
						}
					}
				}
			}

			return Ok(new {Registered = registered, Unregistered = unregistered, Indeterminate = indeterminate});
		}
	}
}