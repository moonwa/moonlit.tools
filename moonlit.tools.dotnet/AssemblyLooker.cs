#region

using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Moonlit.Reflection;

#endregion

namespace Moonlit.Tools.DotNet
{
    [Command("assembly", "查看程序集")]
    internal class AssemblyLooker : ICommand
    {
        [Import]
        public ILogger Logger { get; set; }

        [Target]
        public string Target { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
        {
            LookFile(Target);
            return 0;
        }

        #endregion

        private void LookFile(string fileName)
        {
            try
            {
                FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(fileName);
                Assembly assembly = Assembly.LoadFile(fileName);
                AssemblyName assemblyName = assembly.GetName();

                Logger.InfoLine("FileName: " + fileName);
                Logger.InfoLine("AssemblyName: " + assembly.GetName().ToString());
            }
            catch (Exception ex)
            {
                Logger.WarnLine(string.Format("process {0}: {1}", fileName, ex));
            }
        }
    }
}