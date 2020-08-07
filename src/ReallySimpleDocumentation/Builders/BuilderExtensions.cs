using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public static class BuilderExtensions
    {
        internal static string ReplaceApiTemplateVariables(this string input, IUIOptions config, SwaggerDocOptions options)
        {
            var css = new List<string>(config.AdditionalStylesheets.Where(x => !string.IsNullOrWhiteSpace(x)));
            if (config.ServeDefaultCss) css.Add(config.DefaultCssRoute);
            css = css.Select(x => $"<link rel='stylesheet' href='{x}'>").ToList();

            var js = new List<string>(config.AdditionalJavascript.Where(x => !string.IsNullOrWhiteSpace(x)));
            if (config.ServeDefaultJavascript) js.Add(config.DefaultJavascriptRoute);
            js = js.Select(x => $"<script src='{x}'></script>").ToList();

            var favIconElement = string.IsNullOrWhiteSpace(config.FaviconUrl) ? string.Empty : $"<link rel='icon' href='{config.FaviconUrl}'>";

            if (config is RedocUiOptions ruio)
            {
                input = input.Replace("{{RedocBundleUrl}}", ruio.RedocBundleUrl);
            }

            return input.Replace("{{ApiShortName}}", options.ShortName)
                        .Replace("{{ApiTitle}}", options.Title)
                        .Replace("{{ApiDescription}}", options.DefaultDescription)
                        .Replace("{{FavIconUrl}}", config.FaviconUrl ?? string.Empty)
                        .Replace("{{FavIconElement}}", favIconElement)
                        .Replace("{{LogoUrl}}", config.LogoUrl)
                        .Replace("{{LogoAltText}}", config.LogoAltText)
                        .Replace("{{LogoBackgroundColor}}", config.LogoBackgroundColor)
                        .Replace("{{CssElement}}", string.Join(Environment.NewLine, css))
                        .Replace("{{ScriptElement}}", string.Join(Environment.NewLine, js));
        }

        public static ReallySimpleDocumentationApplicationBuilder UseReallySimpleDocumentation(this IApplicationBuilder app, Action<SwaggerOptions> additionalSwaggerConfig = null)
        {
            var options = app.ApplicationServices.GetService<IOptions<SwaggerDocOptions>>()?.Value;
            if (options == null)
                throw new InvalidOperationException(
                    "SwaggerDocOptions not found - cannot use ReallySimpleDocumentation without adding ReallySimpleDocumentation. Ensure you have called the For method in your chain after calling AddReallySimpleDocumentation");

            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{{client}}/{{documentName}}/swagger.json";
                additionalSwaggerConfig?.Invoke(c);
            });
            return new ReallySimpleDocumentationApplicationBuilder(app, options);
        }

        public static ReallySimpleDocumentationBuilder AddReallySimpleDocumentation(this IServiceCollection services,
                                                                  Action<WikiMarkdownOptions> markdownOptions)
        {
            services.Configure(markdownOptions);
            services.AddSingleton<IWikiMarkdownHandler, WikiMarkdownHandler>();
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<UniqueOperationNamesFilter>();
                c.OperationFilter<ResponseDescriptionFilter>();
                c.SchemaFilter<EnumDescriptionSchemaFilter>();
                c.SchemaFilter<TypeDescriptionFilter>();
                c.SchemaFilter<SwaggerIgnoreFilter>();
                c.DocumentFilter<SwaggerDocFilter>();
                c.SchemaFilter<SwaggerRequiredFilter>();
                c.ParameterFilter<GuidParameterFilter>();

                c.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;
                c.SwaggerGeneratorOptions.IgnoreObsoleteActions = false;
            });

            return new ReallySimpleDocumentationBuilder(services);
        }
    }
}
