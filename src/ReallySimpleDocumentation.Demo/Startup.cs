using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Marsman.ReallySimpleDocumentation;
using System.IO;

namespace Marsman.ReallySimpleDocumentation.Demo
{
    public class Startup
    {
        private readonly IHostingEnvironment environment;
        private const string apiTitle = "Marsman.ReallySimpleDocumentation Demo API";
        private const string apiShortName  = "demo";
        private const string apiDescription = "A dummy API to demonstrate a thing.";
        private const string apiVersion = "1.0";

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            this.environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
            services.AddHttpContextAccessor();
            services.AddReallySimpleDocumentation(opts =>
            {
                opts.MarkdownFilesPath = Path.Combine(AppContext.BaseDirectory, "Docs");
            })
            .WithXmlFile(Path.Combine(AppContext.BaseDirectory, "ReallySimpleDocumentation.Demo.xml"), true)
            .WithRedoc()
            .WithSwaggerUI()
            .For(apiShortName, apiTitle, apiDescription, apiVersion);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseBreakpointErrorMiddleware();
            app.UseReallySimpleDocumentation()
               .WithRedoc()
               .WithSwaggerUI();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
