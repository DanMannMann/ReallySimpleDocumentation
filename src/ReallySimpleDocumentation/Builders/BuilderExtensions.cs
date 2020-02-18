using Flurl;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marsman.ReallySimpleDocumentation
{

    public static class BuilderExtensions
    {
        internal static string ReplaceApiTemplateVariables(this string input, IUIOptions config, SwaggerDocOptions options)
        {
            return input.Replace("{{ApiShortName}}", options.ShortName)
                        .Replace("{{ApiTitle}}", options.Title)
                        .Replace("{{ApiDescription}}", options.DefaultDescription)
                        .Replace("{{FavIconUrl}}", config.FaviconUrl)
                        .Replace("{{LogoUrl}}", config.LogoUrl)
                        .Replace("{{LogoAltText}}", config.LogoAltText)
                        .Replace("{{LogoBackgroundColor}}", config.LogoBackgroundColor)
                        .Replace("{{RedocCustomCssUrl}}", config.CssUrl)
                        .Replace("{{RedocCustomJsUrl}}", config.JavascriptUrl);
        }

        public static void AddRange<T>(this List<T> list, params T[] items)
        {
            list.AddRange(items);
        }

        public static void AddRange<T1, T2>(this Dictionary<T1,T2> dict, params (T1 key, T2 value)[] items)
        {
            foreach (var item in items)
            {
                dict.Add(item.key, item.value);
            }
        }

        public static void AddRange<T1, T2>(this Dictionary<T1, T2> dict, IEnumerable<KeyValuePair<T1,T2>> items)
        {
            foreach (var item in items)
            {
                dict.Add(item.Key, item.Value);
            }
        }

        public static ReallySimpleDocumentationApplicationBuilder UseReallySimpleDocumentation(this IApplicationBuilder app, Action<SwaggerOptions> additionalSwaggerConfig = null)
        {
            var options = app.ApplicationServices.GetService<IOptions<SwaggerDocOptions>>()?.Value;
            if (options == null)
                throw new InvalidOperationException(
                    "SwaggerDocOptions not found - cannot use UseReallySimpleDocumentation without adding UseReallySimpleDocumentation - ensure you have called the For method in your chain after calling AddFacDocumentation");

            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{{client}}/{{documentName}}/swagger.json";
                additionalSwaggerConfig?.Invoke(c);
            });
            return new ReallySimpleDocumentationApplicationBuilder(app, options);
        }

        public static ReallySimpleDocumentationBuilder AddReallySimpleDocumentation(this IServiceCollection services,
                                                                  Action<WikiMarkdownOptions> markdownOptions,
                                                                  bool expandGenericSchemaIds = true)
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
                if (expandGenericSchemaIds)
                {
                    c.CustomSchemaIds(x =>
                    {
                        if (x.IsConstructedGenericType)
                        {
                            return $"{x.Name.Substring(0, x.Name.LastIndexOf('`'))}<{string.Join(",", x.GetGenericArguments().Select(y => y.Name))}>";
                        }
                        return x.Name;
                    });
                }

                c.SchemaRegistryOptions.DescribeAllEnumsAsStrings = false;
                c.SchemaRegistryOptions.UseReferencedDefinitionsForEnums = true;
                c.SwaggerGeneratorOptions.DescribeAllParametersInCamelCase = true;
                c.SwaggerGeneratorOptions.IgnoreObsoleteActions = false;
            });

            return new ReallySimpleDocumentationBuilder(services);
        }
    }
}
