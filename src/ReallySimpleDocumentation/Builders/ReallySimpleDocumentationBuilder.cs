using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{

    public class ReallySimpleDocumentationBuilder
    {
        private readonly IServiceCollection services;

        internal ReallySimpleDocumentationBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public ReallySimpleDocumentationBuilder WithRedoc(Action<RedocUiOptions> redocOptions = null)
        {
            if (redocOptions != null) services.Configure(redocOptions);
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.DocumentFilter<RedocDocFilter>();
            });
            return this;
        }

        public ReallySimpleDocumentationBuilder WithTypeDoc<T>(Action<TypeDocConfig<T>> typeDoc)
        {
            var tdc = new TypeDocConfig<T>();
            typeDoc.Invoke(tdc);
            services.AddSingleton(tdc.GetTypeDescription());
            return this;
        }

        public ReallySimpleDocumentationBuilder WithSwaggerUI(Action<SwaggerUiOptions> swaggerUiOptions = null)
        {
            var options = new SwaggerUiOptions();
            swaggerUiOptions?.Invoke(options);
            services.AddSingleton(Options.Create(options));
            services.AddSingleton(Options.Create(options.WikiOptions));
            services.AddSingleton<ISwaggerUIWikiFactory, SwaggerUIWikiFactory>();
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.DocumentFilter<SwaggerUIDocFilter>();
                c.OperationFilter<SwaggerUIOpFilter>();
            });
            return this;
        }

        public ReallySimpleDocumentationBuilder WithXmlFile(string xmlPath, bool includeControllerCommentsFromXml)
        {
            if (!services.Any(x => x.ServiceType == typeof(IXmlDocProvider)))
            {
                services.AddSingleton<IXmlDocProvider, XmlDocProvider>();
            }
            services.AddSingleton(new XmlDocSource(xmlPath));
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.IncludeXmlComments(xmlPath, includeControllerCommentsFromXml);
            });
            return this;
        }

        /// <summary>
        /// The details of the API this builder is for. This method must be called last, and must be called.
        /// </summary>
        public void For(string apiShortName, string apiTitle, string apiDescription, string apiVersion, Action<Info> additionalInfoProvider = null)
        {
            services.Configure<SwaggerDocOptions>(opts =>
            {
                opts.Title = apiTitle;
                opts.ShortName = apiShortName;
                opts.DefaultDescription = apiDescription;
                opts.Version = apiVersion;
            });

            var info = new Info
            {
                Title = apiTitle,
                Description = apiDescription,
                Version = apiVersion
            };
            additionalInfoProvider?.Invoke(info);
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.SwaggerDoc(apiShortName, info);
            });
        }

        /// <summary>
        /// The details of the API this builder is for. This method must be called last, and must be called.
        /// </summary>
        public void For(string apiShortName, Action<Info> additionalInfoProvider)
        {
            var info = new Info();
            additionalInfoProvider?.Invoke(info);
            services.Configure<SwaggerDocOptions>(opts =>
            {
                opts.ShortName = apiShortName;
                opts.Title = info.Title;
                opts.DefaultDescription = info.Description;
                opts.Version = info.Version;
            });

            services.Configure<SwaggerGenOptions>(c =>
            {
                c.SwaggerDoc(apiShortName, info);
            });
        }

        public ReallySimpleDocumentationBuilder WithAdditionalSwaggerConfig(Action<SwaggerGenOptions> options)
        {
            services.Configure(options);
            return this;
        }

        public ReallySimpleDocumentationBuilder WithEnvironmentFilter(string environment, bool? defaultToVisible = null, bool caseSensitiveMatch = false)
        {
            services.Configure<MvcOptions>(opts =>
            {
                opts.Conventions.Add(new SwaggerEnvironmentFilterConvention(environment, defaultToVisible, caseSensitiveMatch));
            });
            return this;
        }

        public ReallySimpleDocumentationBuilder WithOAuth2Authentication(string authAuthority, string clientId, string clientName)
        {
            return WithOAuth2Authentication(authAuthority, clientId, clientName, new Dictionary<string,string>());
        }

        public ReallySimpleDocumentationBuilder WithOAuth2Authentication(string authAuthority, string clientId, string clientName, params string[] scopes)
        {
            return WithOAuth2Authentication(authAuthority, clientId, clientName, scopes.ToDictionary(x => x, x => x));
        }

        public ReallySimpleDocumentationBuilder WithOAuth2Authentication(string authAuthority, string clientId, string clientName, params (string key, string value)[] scopes)
        {
            return WithOAuth2Authentication(authAuthority, clientId, clientName, scopes.ToDictionary(x => x.key, x => x.value));
        }

        public ReallySimpleDocumentationBuilder WithOAuth2Authentication(string authAuthority, string clientId, string clientName, IDictionary<string,string> scopes)
        {
            services.Configure<AuthOptions>(opts =>
            {
                opts.Enabled = true;
                opts.Authority = authAuthority;
                opts.Scopes = scopes;
                opts.ClientId = clientId;
                opts.ClientName = clientName;
            });
            services.Configure<OperationScopeOptions>(opts =>
            {
                foreach (var scope in scopes)
                {
                    opts.Scopes.Add(scope);
                }
            });
            services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = authAuthority.EndsWith("/") ? $"{authAuthority}connect/authorize" : $"{authAuthority}/connect/authorize",
                    Scopes = scopes
                });
                c.OperationFilter<OperationScopeAttachmentFilter>();
            });
            return this;
        }
    }
}
