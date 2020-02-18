using System;

namespace Marsman.ReallySimpleDocumentation
{
    /// <summary>
    /// <para>Swagger will show the controller or action when running in the specified environments.</para>
    /// <para>Swagger will hide the controller or action when running in any environment not specified, regardless of the default visibility set in the convention.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SwaggerEnvironmentAttribute : Attribute
    {
        public SwaggerEnvironmentAttribute(SwaggerEnvironment allowedEnvironments)
        {
            AllowedEnvironments = allowedEnvironments;
        }

        public SwaggerEnvironment AllowedEnvironments { get; }
    }
}
