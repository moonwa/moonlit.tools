using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Moonlit.Tools.ReflectionExtends
{
    [Command("TypeFind", "在指定程序集中搜索指定类型")]
    public class TypeFind : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TypeFind(ILogger logger)
        {
            _logger = logger;
        }

        [Parameter(Description = @"所搜索的类型", Required = true)]
        public string TypeName { get; set; }

        [Parameter(Description = @"所搜索的程序集", Required = true)]
        public string AssemblyFile { get; set; }

        [Parameter(Description = @"使用继承", Required = true)]
        public bool Inherit { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            Assembly assembly = Assembly.LoadFrom(AssemblyFile);

            IEnumerable<Type> types = from type in assembly.GetTypes()
                                      where string.Compare(type.Name, TypeName, true) == 0
                                      select type;

            foreach (Type t in types)
            {
                _logger.InfoLine(string.Format("类型: {0}", t.FullName));
                foreach (Type subType in assembly.GetTypes())
                {
                    if (t.IsAssignableFrom(subType) && t != subType)
                    {
                        _logger.InfoLine(string.Format("    派生类型: {0}", subType.FullName));
                    }
                }
                _logger.InfoLine("");
            }
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("搜索类型 {0}", TypeName); }
        }

        #endregion
    }
}