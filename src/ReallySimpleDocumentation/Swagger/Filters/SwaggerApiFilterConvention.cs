using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsman.ReallySimpleDocumentation
{
    public class SwaggerApiFilterConvention : IControllerModelConvention
    {
        private readonly SwaggerEnvironment environment;
        private readonly bool? defaultVisible;

        public SwaggerApiFilterConvention(string env, bool? defaultVisible = null)
        {
            if (!Enum.TryParse(env, true, out environment)) throw new InvalidOperationException("unknown enum value");
            this.defaultVisible = defaultVisible;
        }

        public SwaggerApiFilterConvention(string env, Func<SwaggerEnvironment, bool> defaultVisible)
        {
            if (!Enum.TryParse(env, true, out environment)) throw new InvalidOperationException("unknown enum value");
            this.defaultVisible = defaultVisible(environment);
        }

        public void Apply(ControllerModel controller)
        {
            var visible = controller.Attributes
                                    .OfType<SwaggerEnvironmentAttribute>()
                                    .FirstOrDefault()?
                                    .AllowedEnvironments
                                    .HasFlag(environment) ?? defaultVisible;

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
                                .HasFlag(environment) ?? defaultVisible;

            if (visible.HasValue)
            {
                action.ApiExplorer.IsVisible = visible;
            }

        }
    }
}
