using System;
using Moonlit.Reflection;

namespace Moonlit.Tools
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class CommandUsageAttribute : Attribute
    {
        public CommandUsageAttribute(string usage)
        {
            Usage = usage;
        }

        public string Usage { get; private set; }

        public static string GetUsage(Type t)
        {
            var usage = t.GetAttribute<CommandUsageAttribute>(false);
            if (usage != null)
            {
                return usage.Usage;
            }
            return "";
        }
    }
}