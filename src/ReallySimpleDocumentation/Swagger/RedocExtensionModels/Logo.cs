using Microsoft.OpenApi.Any;
using System;

namespace Marsman.ReallySimpleDocumentation
{
    public class Logo : OpenApiObject
    {
        public string Url
        {
            get => Get("url");
            set => AddOrUpdate("url", () => new OpenApiString(value));
        }
        public string BackgroundColor
        {
            get => Get("backgroundColor");
            set => AddOrUpdate("backgroundColor", () => new OpenApiString(value));
        }
        public string AltText
        {
            get => Get("altText");
            set => AddOrUpdate("altText", () => new OpenApiString(value));
        }
        

        private string Get(string v)
        {
            return ContainsKey(v) ? (this[v] as OpenApiString)?.Value : null;
        }

        private void AddOrUpdate(string v, Func<OpenApiString> p)
        {
            if (ContainsKey(v))
            {
                this[v] = p();
            }
            else
            {
                Add(v, p());
            }
        }
    }
}
