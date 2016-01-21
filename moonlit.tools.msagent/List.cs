using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWW.Configuration.Argument;
using System.IO;

namespace Moonlit.Tools.MSAgentExtends {
    /// <summary>
    /// 命令 : List
    /// </summary>
    [Function("显示当前系统中 msagent 的列表")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("")]
    [RealName("list")]
    internal class List : CommandBase {
        ILogger logger = MyIoc.GetService<ILogger>();
        protected override int Run() {
            string[] agentFiles = MsAgentHelper.GetAgentFiles();

            logger.InfoLine(string.Format("there are {0} msagent in your system, the name is:", agentFiles.Length));
            foreach (string agentFile in agentFiles) {
                logger.InfoLine(string.Format("{0} => {1}", Path.GetFileNameWithoutExtension(agentFile), agentFile));
            }
            return 0;
        }

        protected override void InitArgument(Parser parser) {
        }

        #region ITitleCommand 成员

        public string CommandTitle {
            get { return ""; }
        }

        #endregion
    }
}
