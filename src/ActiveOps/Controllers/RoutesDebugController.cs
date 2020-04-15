// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using ActiveRoutes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ActiveOps.Controllers
{
	public class RoutesDebugController : Controller
	{
		private readonly IActionDescriptorCollectionProvider _provider;

		public RoutesDebugController(IActionDescriptorCollectionProvider provider) => _provider = provider;

		[DynamicHttpGet("")]
		public IActionResult Get()
		{
			var map = _provider.ActionDescriptors.Items.Select(Map);

			static object Map(ActionDescriptor descriptor)
			{
				var controller = descriptor.RouteValues["Controller"];
				var action = descriptor.RouteValues["Action"];
				var constraints = descriptor.ActionConstraints;
				var filters = descriptor.FilterDescriptors.OrderBy(x => x.Order)
					.ThenBy(x => x.Scope)
					.Select(x => x.Filter.GetType().Name);

				return new
				{
					descriptor.Id,
					Path = $"{controller}/{action}",
					Action = action,
					descriptor.DisplayName,
					descriptor.AttributeRouteInfo?.Template,
					descriptor.AttributeRouteInfo?.Name,
					Filters = filters,
					Constraints = constraints
				};
			}

			return Ok(map);
		}
	}
}