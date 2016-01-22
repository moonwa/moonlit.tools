using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Moonlit.Configuration.ConsoleParameter;
using Moonlit.Reflection;

namespace Moonlit.Tools
{
    internal class Usage
    {
        private readonly Type _commandType;
        private readonly ILogger _logger;

        public Usage(Type commandType, ILogger logger)
        {
            _commandType = commandType;
            _logger = logger;
        }

        public void Show(string message)
        {
            _logger.WarnLine(message); 
            var targets = string.Join(" ", _commandType
                .GetProperties()
                .Select(x => new { Attr = x.GetCustomAttribute<TargetAttribute>(), Property = x })
                .Where(x=>x.Attr != null)
                .OrderBy(x => x.Attr.Index).Select(x => x.Attr.Desc(x.Property)));
            var args = string.Join(" ", _commandType
                .GetProperties()
                .Select(x => new { Attr = x.GetCustomAttribute<ParameterAttribute>(), Property = x })
                .Where(x=>x.Attr != null)
                .Select(x => x.Attr.Desc(x.Property)));
            var appName = Assembly.GetEntryAssembly().GetName().Name;
            _logger.InfoLine($"usage: {appName} {targets} {args}");
        }
    }
}