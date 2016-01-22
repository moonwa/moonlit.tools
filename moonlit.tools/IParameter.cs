using System;
using System.Reflection;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    public interface IParameter
    {
        object GetValue(Parser parser, PropertyInfo property);
        void Set(Parser parser,   PropertyInfo property);
        string Desc(PropertyInfo propertyInfo);
    }
}