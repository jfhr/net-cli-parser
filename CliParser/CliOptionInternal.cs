using System;
using System.Reflection;

namespace CliParser
{
    internal class CliOptionInternal
    {
        private readonly PropertyInfo property;

        public CliOptionAttribute Attribute { get; }
        public bool Required => Attribute.Required;
        public string[] Flags => Attribute.Flags;
        public string Description => Attribute.Description;
        public MethodInfo Getter => property.GetMethod;
        public MethodInfo Setter => property.SetMethod;
        public Type PropertyType => property.PropertyType;


        public CliOptionInternal(PropertyInfo property)
        {
            Attribute = property.GetCustomAttribute<CliOptionAttribute>();
            this.property = property;
        }
    }
}