// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using ActiveRoutes;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ActiveOps.Controllers
{
	public class HealthChecksDebugController : Controller
	{
		private readonly IOptionsSnapshot<HealthCheckOptions> _options;
		private readonly HealthCheckService _service;

		public HealthChecksDebugController(HealthCheckService service, IOptionsSnapshot<HealthCheckOptions> options)
		{
			_service = service;
			_options = options;
		}

		[DynamicHttpGet("ping")]
		public async Task<IActionResult> PingAsync()
		{
			return await GetHealthChecksReportAsync(r => false);
		}

		[DynamicHttpGet("")]
		public async Task<IActionResult> GetAsync()
		{
			var filter = HttpContext.Request.Query.TryGetValue("tags", out var tags)
				? r => r.Tags.IsSupersetOf(tags)
				: _options.Value.Predicate;

			return await GetHealthChecksReportAsync(filter);
		}

		private async Task<IActionResult> GetHealthChecksReportAsync(Func<HealthCheckRegistration, bool> filter)
		{
			var report = await _service.CheckHealthAsync(filter, HttpContext.RequestAborted);

			if (!_options.Value.ResultStatusCodes.TryGetValue(report.Status, out var statusCode))
			{
				throw new InvalidOperationException(
					$"No status code mapping found for {"HealthStatus" as object} value: {report.Status as object}.HealthCheckOptions.ResultStatusCodes must contain an entry for {report.Status as object}.");
			}

			HttpContext.Response.StatusCode = statusCode;

			if (!_options.Value.AllowCachingResponses)
			{
				var headers = HttpContext.Response.Headers;
				headers["Cache-Control"] = "no-store, no-cache";
				headers["Pragma"] = "no-cache";
				headers["Expires"] = "Thu, 01 Jan 1970 00:00:00 GMT";
			}

			return Ok(report);
		}
	}
}