using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWW.Configuration.Argument;

namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : LanguageID
    /// </summary>
    [Function("LanguageID")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("LanguageID")]
    [RealName("LanguageID")]
    internal class LanguageID : MSAgentOperationBase
    {
        ILogger logger = MyIoc.GetService<ILogger>();
        protected override int Run()
        {
            this.Character.LanguageID = this.LangID;
            return 0;
        }
        private int LangID
        {
            get
            {
                return this.SafeGetTarget<int>(0, 0x0409);
            }
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }
}
