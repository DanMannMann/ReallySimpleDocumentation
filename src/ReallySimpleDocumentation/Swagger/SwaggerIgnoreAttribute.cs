using System;

namespace Marsman.ReallySimpleDocumentation
{
    /// <summary>
    /// <para>Swagger will ignore the property.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SwaggerIgnoreAttribute : Attribute
    {
    }
}
