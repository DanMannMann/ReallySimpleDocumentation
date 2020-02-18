using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class AuthOptions
    {
        public AuthOptions()
        {
        }

        public string Authority { get; set; }
        public IDictionary<string, string> Scopes { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
