using System;
using System.Reflection;
using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools
{
    public interface IParameterSetter
    {
        object GetValue(Parser parser, PropertyInfo property);
    }
}