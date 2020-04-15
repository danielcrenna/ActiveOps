// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ActiveCaching;
using ActiveRoutes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveOps.Controllers
{
	public class CachesDebugController : Controller
	{
		[DynamicHttpGet("")]
		public IActionResult Get()
		{
			// TODO Caches - Formal (IDistributedCache, IOptionsSnapshot, etc.)
			// TODO Caches - Opaque (Dictionary, etc.)

			var totalCacheKeys = 0L;
			var totalCacheMemory = 0L;

			var managed = new List<object>();
			var unmanaged = new List<object>();

			foreach (var cache in HttpContext.RequestServices.GetServices<ICache>())
			{
				if (cache is ICacheManager manager)
				{
					totalCacheMemory += manager.SizeBytes;
					totalCacheKeys += manager.KeyCount;

					managed.Add(new
					{
						Type = manager.GetType().Name,
						Count = manager.KeyCount,
						Size = manager.SizeBytes,
						SizeLimit = manager.SizeLimitBytes
					});

					continue;
				}

				unmanaged.Add(new {Type = cache.GetType().Name});
			}

			foreach (var cache in HttpContext.RequestServices.GetServices<IHttpCache>())
			{
				if (cache is ICacheManager manager)
				{
					totalCacheMemory += manager.SizeBytes;
					totalCacheKeys += manager.KeyCount;

					managed.Add(new
					{
						Type = manager.GetType().Name,
						Count = manager.KeyCount,
						Size = manager.SizeBytes,
						SizeLimit = manager.SizeLimitBytes
					});

					continue;
				}

				unmanaged.Add(new {Type = cache.GetType().Name});
			}

			return Ok(new
			{
				Managed = managed,
				TotalMemory = GC.GetTotalMemory(false),
				TotalCacheKeys = totalCacheKeys,
				TotalCacheMemory = totalCacheMemory,
				Unmanaged = unmanaged
			});
		}
	}
}