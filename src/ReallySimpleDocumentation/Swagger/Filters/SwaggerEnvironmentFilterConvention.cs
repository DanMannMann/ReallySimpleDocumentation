using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerEnvironmentFilterConvention : IControllerModelConvention
    {
        private readonly bool? defaultToVisible;
        private readonly StringComparer matchType;
        private readonly string environment;

        public SwaggerEnvironmentFilterConvention(string environment, bool? defaultToVisible = null, bool caseSensitiveMatch = false)
        {
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.defaultToVisible = defaultToVisible;
            this.matchType = caseSensitiveMatch ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
        }

        public void Apply(ControllerModel controller)
        {
            var visible = controller.Attributes
                                    .OfType<SwaggerEnvironmentAttribute>()
                                    .FirstOrDefault()?
                                    .AllowedEnvironments
                                    .Contains(environment, matchType) ?? defaultToVisible;

            if (visible.HasValue)
            {
                controller.ApiExplorer.IsVisible = visible;
                foreach (var action in controller.Actions)
                {
                    var actionAttr = action.Attributes.OfType<SwaggerEnvironmentAttribute>().FirstOrDefault();
                    if (actionAttr == null)
                    {
                        action.ApiExplorer.IsVisible = visible;
                    }
                    else
                    {
                        Apply(action);
                    }
                }
            }
        }

        private void Apply(ActionModel action)
        {
            var visible = action.Attributes
                                .OfType<SwaggerEnvironmentAttribute>()
                                .FirstOrDefault()?
                                .AllowedEnvironments
                                .Contains(environment, matchType) ?? defaultToVisible;

            if (visible.HasValue)
            {
                action.ApiExplorer.IsVisible = visible;
            }

        }
    }
}
