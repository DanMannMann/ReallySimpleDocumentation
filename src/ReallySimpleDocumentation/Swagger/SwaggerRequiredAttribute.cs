using System;

namespace Marsman.ReallySimpleDocumentation
{
    /// <summary>
    /// <para>Makes a property show up as required in swagger, even if model validation doesn't apply any validation via a [Required] attribute.</para>
    /// <para>This is useful for when we need to document that things are required but defer the validation to the domain (e.g. when doing a batch job
    /// that should be able to partially succeed)</para>
    /// </summary>
    public class SwaggerRequiredAttribute : Attribute
    {
        public SwaggerRequiredAttribute(bool reqd = true)
        {
            Required = reqd;
        }

        public bool Required { get; }
    }
}
