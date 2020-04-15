// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveRoutes;
using Microsoft.AspNetCore.Mvc;

namespace ActiveOps.Controllers
{
	public class HostedServicesDebugController : Controller
	{
		[DynamicHttpGet("")]
		public IActionResult Get()
		{
			return Ok(OperationsMethods.HostedServicesReport(HttpContext.RequestServices));
		}
	}
}