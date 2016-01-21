using System;
using System.Reflection;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TargetAttribute : Attribute, IParameterSetter
    {
        public TargetAttribute()
        {
        }

        public TargetAttribute(int index)
        {
            Index = index;
        }

        public int Index { get; set; }
        public string Name { get; set; }
        public bool Required { get; set; }

        public object GetValue(Parser parser, PropertyInfo property)
        {
            return parser.Targets[Index];
        }
    }
}