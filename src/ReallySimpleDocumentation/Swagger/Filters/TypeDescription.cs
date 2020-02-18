using System;
using System.Collections.Generic;

namespace Marsman.ReallySimpleDocumentation
{
    public class TypeDescription<T> : TypeDescription
    {
        public TypeDescription() : base(typeof(T))
        {
        }

        public override Type Type { get => typeof(T); }
    }

    public class TypeDescription
    {
        public TypeDescription(Type type)
        {
            Type = type;
        }

        public virtual Type Type { get; }
        public string Description { get; set; }
        public Dictionary<string, string> PropertyDescriptions { get; } = new Dictionary<string, string>();
        public List<string> MembersToExclude { get; } = new List<string>();
    }
}
