using Marsman.ReallySimpleDocumentation.CollectionExtensions;
using Marsman.Reflekt;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Marsman.ReallySimpleDocumentation
{
    public class TypeDocConfig<T>
    {
        private readonly List<string> excludedMembers = new List<string>();
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();
        private string typeDescription;

        public TypeDocConfig<T> WithPropertyDescription(Expression<Func<T, object>> selector, string description)
        {
            var name = Reflekt<T>.PropertyName(selector);
            properties.Add(name, description);
            return this;
        }
        public TypeDocConfig<T> WithTypeDescription(string description)
        {
            typeDescription = description;
            return this;
        }
        public TypeDocConfig<T> ExcludingProperty(Expression<Func<T, object>> selector)
        {
            excludedMembers.Add(Reflekt<T>.PropertyName(selector));
            return this;
        }
        public TypeDocConfig<T> ExcludingEnumMember(T member)
        {
            excludedMembers.Add(Enum.GetName(typeof(T), member));
            return this;
        }

        internal TypeDescription GetTypeDescription()
        {
            var result = new TypeDescription<T>
            {
                Description = typeDescription
            };
            result.PropertyDescriptions.AddRange(properties);
            result.MembersToExclude.AddRange(excludedMembers);
            return result;
        }
    }
}
