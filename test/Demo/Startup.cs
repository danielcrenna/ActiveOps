// Copyright (c) Daniel Crenna & Contributors. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using ActiveOps;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Demo
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.AddOptionsDebuggerApi(Configuration.GetSection("OptionsDebugFeature"));
			services.AddServicesDebuggerApi(Configuration.GetSection("ServicesDebugFeature"));
			services.AddHealthChecksApi(Configuration.GetSection("HealthChecksDebugFeature"));
			services.AddEnvironmentApi(Configuration.GetSection("EnvironmentDebugFeature"));
			services.AddHostedServicesDebuggerApi(Configuration.GetSection("HostedServicesDebugFeature"));
			services.AddFeaturesDebuggerApi(Configuration.GetSection("FeaturesDebugFeature"));
			services.AddRoutesDebuggerApi(Configuration.GetSection("RoutesDebugFeature"));
			services.AddCachesDebuggerApi(Configuration.GetSection("CachesDebugFeature"));

			services.AddStartupHealthChecks();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseOpsApis();

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}