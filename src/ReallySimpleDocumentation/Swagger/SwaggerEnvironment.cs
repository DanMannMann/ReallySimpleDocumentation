using System;

namespace Marsman.ReallySimpleDocumentation
{
    [Flags]
    public enum SwaggerEnvironment
    {
        None = 0,
        Development = 1,
        QA = 2,
        Sandbox = 4,
        Production = 8,
        All = ~None
    }
}
