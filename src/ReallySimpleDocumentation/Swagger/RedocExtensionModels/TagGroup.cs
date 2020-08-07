using Microsoft.OpenApi.Any;
using System;

namespace Marsman.ReallySimpleDocumentation
{
    public class TagGroup : OpenApiObject
    {
        public string Name 
        { 
            get => GetString("name"); 
            set => AddOrUpdate("name", () => new OpenApiString(value));
        }
        public OpenApiArray Tags
        {
            get => GetArray("tags");
            set => AddOrUpdate("tags", () => value);
        }

        private string GetString(string v)
        {
            return ContainsKey(v) ? (this[v] as OpenApiString)?.Value : null;
        }

        private OpenApiArray GetArray(string v)
        {
            return ContainsKey(v) ? (this[v] as OpenApiArray) : null;
        }

        private void AddOrUpdate<T>(string v, Func<T> p) where T : IOpenApiAny
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
