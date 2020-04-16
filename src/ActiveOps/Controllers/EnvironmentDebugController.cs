// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime;
using System.Runtime.InteropServices;
using ActiveRoutes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveOps.Controllers
{
	public class EnvironmentDebugController : Controller
	{
		private readonly IWebHostEnvironment _hosting;
		private readonly IConfiguration _config;

		public EnvironmentDebugController(IWebHostEnvironment hosting, IConfiguration config)
		{
			_hosting = hosting;
			_config = config;
		}

		[DynamicHttpGet("")]
		public IActionResult GetEnvironmentAsync()
		{
			static string GetPlatform()
			{
				return
					RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
					RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
					RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "OSX" : "Unknown";
			}

			// See: https://github.com/dotnet/BenchmarkDotNet/issues/448#issuecomment-308424100
			static string GetNetCoreVersion()
			{
				var assembly = typeof(GCSettings).Assembly;
				var assemblyPath = assembly.CodeBase.Split(new[] {'/', '\\'}, StringSplitOptions.RemoveEmptyEntries);
				var netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
				return netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2
					? assemblyPath[netCoreAppIndex + 1]
					: null;
			}

			var process = Process.GetCurrentProcess();
			var hostName = Dns.GetHostName();
			var hostEntry = Dns.GetHostEntry(hostName);

			IDictionary<string, object> configuration = new ExpandoObject();
			foreach (var (k, v) in _config.AsEnumerable())
			{
				configuration.Add(k, v);
			}

			var env = new
			{
				Dns =
					new
					{
						HostName = hostName,
						HostEntry = new
						{
							hostEntry.Aliases, Addresses = hostEntry.AddressList.Select(x => x.ToString())
						}
					},
				OperatingSystem =
					new
					{
						Platform = GetPlatform(),
						Description = RuntimeInformation.OSDescription,
						Architecture = RuntimeInformation.OSArchitecture,
						Version = Environment.OSVersion,
						Is64Bit = Environment.Is64BitOperatingSystem
					},
				Process = new
				{
					process.Id,
					process.ProcessName,
					process.MachineName,
					Arguments = Environment.GetCommandLineArgs(),
					Architecture = RuntimeInformation.ProcessArchitecture,
					MaxWorkingSet = process.MaxWorkingSet.ToInt64(),
					MinWorkingSet = process.MinWorkingSet.ToInt64(),
					process.PagedMemorySize64,
					process.PeakWorkingSet64,
					process.PrivateMemorySize64,
					process.StartTime,
					Is64Bit = Environment.Is64BitProcess
				},
				Environment =
					new
					{
						_hosting.EnvironmentName,
						_hosting.ApplicationName,
						_hosting.ContentRootPath,
						_hosting.WebRootPath,
						Environment.CurrentDirectory,
						Environment.CurrentManagedThreadId
					},
				Framework = new
				{
					Version = $"{RuntimeInformation.FrameworkDescription}",
					NetCoreVersion = GetNetCoreVersion(),
					ClrVersion = Environment.Version.ToString()
				},
				Configuration = configuration
			};

			return Ok(env);
		}
	}
}