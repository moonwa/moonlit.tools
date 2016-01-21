using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using Moonlit.Reflection;

namespace Moonlit.Tools
{
    internal class Usage
    {
        private readonly Type _commandType;

        public Usage(Type commandType)
        {
            _commandType = commandType;
        }

        public void Show(string message)
        {

        }
    }
}