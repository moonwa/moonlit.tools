using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Moonlit.Tools.MSAgentExtends
{
    internal static class MsAgentHelper
    {
        internal static string[] GetAgentFiles()
        {
            string windir = Environment.GetEnvironmentVariable("windir");
            string msagentdir = windir + @"\msagent\chars\";
            string[] agentFiles = Directory.GetFiles(msagentdir, "*.acs");
            return agentFiles;
        }
    }
}
