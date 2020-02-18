namespace Marsman.ReallySimpleDocumentation
{
    public static class SwaggerEnvironmentExtensions
    {
        public static bool IsOneOf(this SwaggerEnvironment env, params SwaggerEnvironment[] environments)
        {
            foreach (var e in environments)
            {
                if (env.HasFlag(e))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
