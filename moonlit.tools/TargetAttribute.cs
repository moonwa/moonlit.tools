using System;
using System.Reflection;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TargetAttribute : Attribute, IParameter
    {
        public TargetAttribute()
        {
        }

        public TargetAttribute(int index)
        {
            Index = index;
        }

        public string Name { get; set; }

        public int Index { get; set; }
        public bool Required { get; set; }

        public object GetValue(Parser parser, PropertyInfo property)
        {
            if (parser.Targets.Count <= Index )
            {
                if (Required)
                {
                    throw new UsageErrorException($"{property.Name} is required");
                }
                return null;
            }
            return parser.Targets[Index];
        }

        public void Set(Parser parser, PropertyInfo property)
        {
             
        }

        public string Desc(PropertyInfo propertyInfo)
        {
            if (Required)
            {
            return propertyInfo.Name;
            }
            return $"[propertyInfo.Name]";
        }
    }
}