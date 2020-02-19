using Flurl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using WebApiContrib.Core.Results;

namespace Marsman.ReallySimpleDocumentation
{
    public class ReallySimpleDocumentationApplicationBuilder
    {
        private const string SwaggerUiDefaultJsResourceName = "Marsman.ReallySimpleDocumentation.Templates.swaggerui.js";
        private const string SwaggerUiDefaultCssResourceName = "Marsman.ReallySimpleDocumentation.Templates.swaggerui.css";
        private const string RedocDefaultHtmlResourceName = "Marsman.ReallySimpleDocumentation.Templates.redoc.html";
        private const string RedocDefaultJsResourceName = "Marsman.ReallySimpleDocumentation.Templates.redoc.js";
        private const string RedocDefaultCssResourceName = "Marsman.ReallySimpleDocumentation.Templates.redoc.css";
        private readonly IApplicationBuilder app;
        private readonly SwaggerDocOptions options;

        internal ReallySimpleDocumentationApplicationBuilder(IApplicationBuilder app, SwaggerDocOptions options)
        {
            this.app = app;
            this.options = options;
        }

        public ReallySimpleDocumentationApplicationBuilder WithRedoc()
        {
            var opts = app.ApplicationServices.GetService<IOptions<RedocUiOptions>>();
            var config = opts?.Value ?? new RedocUiOptions();
            var assembly = typeof(ReallySimpleDocumentationApplicationBuilder).Assembly;

            if (config.ServeDefaultHtml)
            {
                var htmlRouteTemplate = "/redoc";
                var defaultHtml = string.Empty;
                using (var stream = assembly.GetManifestResourceStream(RedocDefaultHtmlResourceName))
                using (var reader = new StreamReader(stream))
                {
                    defaultHtml = reader.ReadToEnd();
                }
                defaultHtml = defaultHtml.ReplaceApiTemplateVariables(config, options);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == htmlRouteTemplate)
                    {
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(defaultHtml, Encoding.UTF8);
                        return;
                    }
                    await next();
                });
            }

            if (config.ServeDefaultCss)
            {
                var cssRouteTemplate = config.DefaultCssRoute.Trim('/');
                var defaultCss = string.Empty;
                using (var stream = assembly.GetManifestResourceStream(RedocDefaultCssResourceName))
                using (var reader = new StreamReader(stream))
                {
                    defaultCss = reader.ReadToEnd();
                }
                defaultCss = defaultCss.ReplaceApiTemplateVariables(config, options);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == $"/{cssRouteTemplate}")
                    {
                        context.Response.ContentType = "text/css";
                        await context.Response.WriteAsync(defaultCss, Encoding.UTF8);
                        return;
                    }
                    await next();
                });
            }

            if (config.ServeDefaultJavascript)
            {
                var jsRouteTemplate = config.DefaultJavascriptRoute.Trim('/');
                var defaultJs = string.Empty;
                using (var stream = assembly.GetManifestResourceStream(RedocDefaultJsResourceName))
                using (var reader = new StreamReader(stream))
                {
                    defaultJs = reader.ReadToEnd();
                }
                defaultJs = defaultJs.ReplaceApiTemplateVariables(config, options);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == $"/{jsRouteTemplate}")
                    {
                        context.Response.ContentType = "text/javascript";
                        await context.Response.WriteAsync(defaultJs, Encoding.UTF8);
                        return;
                    }
                    await next();
                });
            }

            return this;
        }

        public ReallySimpleDocumentationApplicationBuilder WithSwaggerUI()
        {
            var opts = app.ApplicationServices.GetService<IOptions<SwaggerUiOptions>>();
            var config = opts?.Value ?? new SwaggerUiOptions();
            var assembly = typeof(ReallySimpleDocumentationApplicationBuilder).Assembly;

            if (config.ServeDefaultCss)
            {
                var cssRouteTemplate = config.DefaultCssRoute.StartsWith("/") ? config.DefaultCssRoute.Trim('/') : $"swagger/{config.DefaultCssRoute}";
                var defaultCss = string.Empty;
                using (var stream = assembly.GetManifestResourceStream(SwaggerUiDefaultCssResourceName))
                using (var reader = new StreamReader(stream))
                {
                    defaultCss = reader.ReadToEnd();
                }
                defaultCss = defaultCss.ReplaceApiTemplateVariables(config, options);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == $"/{cssRouteTemplate}")
                    {
                        context.Response.ContentType = "text/css";
                        await context.Response.WriteAsync(defaultCss, Encoding.UTF8);
                        return;
                    }
                    await next();
                });
            }
            
            if (config.ServeDefaultJavascript)
            {
                var jsRouteTemplate = config.DefaultJavascriptRoute.StartsWith("/") ? config.DefaultJavascriptRoute.Trim('/') : $"swagger/{config.DefaultJavascriptRoute}";
                var defaultJs = string.Empty;
                using (var stream = assembly.GetManifestResourceStream(SwaggerUiDefaultJsResourceName))
                using (var reader = new StreamReader(stream))
                {
                    defaultJs = reader.ReadToEnd();
                }
                defaultJs = defaultJs.ReplaceApiTemplateVariables(config, options);
                app.Use(async (context, next) =>
                {
                    if (context.Request.Path == $"/{jsRouteTemplate}")
                    {
                        context.Response.ContentType = "text/javascript";
                        await context.Response.WriteAsync(defaultJs, Encoding.UTF8);
                        return;
                    }
                    await next();
                });
            }

            app.UseSwaggerUI(c =>
            {
                if (config.ServeDefaultCss) c.InjectStylesheet(config.DefaultCssRoute);
                if (config.ServeDefaultJavascript) c.InjectJavascript(config.DefaultJavascriptRoute);
                foreach (var css in config.AdditionalStylesheets) c.InjectStylesheet(css);
                foreach (var js in config.AdditionalJavascript) c.InjectJavascript(js);
                if (!string.IsNullOrWhiteSpace(config.FaviconUrl)) c.HeadContent += $@"<link rel='icon' href='{config.FaviconUrl}'>";
                c.SwaggerEndpoint($"/swaggerui/{options.ShortName}/swagger.json", options.Title);
                c.DocumentTitle = options.Title;

                var authOptions = app.ApplicationServices.GetService<IOptions<AuthOptions>>()?.Value;
                if (authOptions?.Enabled == true)
                {
                    c.OAuthClientId(authOptions.ClientId);
                    c.OAuthAppName(authOptions.ClientName);
                }

                config.SwaggerUIOptions?.Invoke(c);
            });
            return this;
        }
    }
}
