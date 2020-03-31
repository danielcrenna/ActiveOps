// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using ActiveOps.Internal;
using ActiveOptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TypeKitchen;

namespace ActiveOps
{
	internal static class OperationsMethods
	{
		public static OperationsReports.OptionsReport OptionsReport(IServiceProvider serviceProvider)
		{
			var report = new OperationsReports.OptionsReport();

			var optionsTypes = typeof(IOptions<>).GetImplementationsOfOpenGeneric();

			report.Options = optionsTypes.GroupBy(x => x.Name).Select(x =>
			{
				// i.e., IOptions, IOptionsSnapshot, IOptionsMonitor, etc.
				var scope = x.Key.Substring(0, x.Key.Length - 2 /* `1 */);

				var values = x.Distinct()
					.Where(t => !t.ContainsGenericParameters)
					.Select(t =>
					{
						var valid = serviceProvider.TryBindOptions(t, false, out var options);

						return new OperationsReports.OptionBindingReport
						{
							Type = t.GetInnerGenericTypeName(), Value = options, IsValid = valid
						};
					})
					.OrderByDescending(v => !v.IsValid)
					.ThenBy(v => v.Type)
					.ToList();

				var hasErrors = false;
				foreach (var v in values)
				{
					if (v.IsValid)
					{
						continue;
					}

					hasErrors = true;
					break;
				}

				report.HasErrors |= hasErrors;

				return new OperationsReports.OptionReport {Scope = scope, HasErrors = hasErrors, Values = values};
			}).ToList();

			return report;
		}

		public static OperationsReports.ServicesReport ServicesReport(IServiceProvider serviceProvider)
		{
			var services = serviceProvider.GetRequiredService<IServiceCollection>();

			var missing = new HashSet<string>();
			var report = new OperationsReports.ServicesReport
			{
				MissingRegistrations = missing,
				Services = services.Select(x =>
				{
					var serviceTypeName = x.ServiceType.Name;
					var implementationTypeName = x.ImplementationType?.Name;
					var implementationInstanceName = x.ImplementationInstance?.GetType().Name;

					string implementationFactoryTypeName = null;
					if (x.ImplementationFactory != null)
					{
						try
						{
							var result = x.ImplementationFactory.Invoke(serviceProvider);
							if (result != null)
							{
								implementationFactoryTypeName = result.GetType().Name;
							}
						}
						catch (InvalidOperationException ex)
						{
							if (ex.Source == "Microsoft.Extensions.DependencyInjection.Abstractions")
							{
								var match = Regex.Match(ex.Message, "No service for type '([\\w.]*)'",
									RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline);

								if (match.Success)
								{
									var typeName = match.Groups[1];
									missing.Add(typeName.Value);
								}
							}
							else
							{
								Trace.TraceError($"{ex}");
							}
						}
					}

					return new OperationsReports.ServiceReport
					{
						Lifetime = x.Lifetime,
						ImplementationType = implementationTypeName,
						ImplementationInstance = implementationInstanceName,
						ImplementationFactory = implementationFactoryTypeName,
						ServiceType = serviceTypeName
					};
				}).ToList()
			};

			return report;
		}

		public static OperationsReports.HostedServicesReport HostedServicesReport(IServiceProvider serviceProvider)
		{
			var report = new OperationsReports.HostedServicesReport();
			var hostedServices = serviceProvider.GetServices<IHostedService>();

			foreach (var hostedService in hostedServices)
			{
				report.Services.Add(hostedService.GetType().Name);
			}

			return report;
		}
	}
}