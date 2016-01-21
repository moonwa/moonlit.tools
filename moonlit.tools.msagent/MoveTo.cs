using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWW.Configuration.Argument;

namespace Moonlit.Tools.MSAgentExtends {
    /// <summary>
    /// 命令 : MoveTo
    /// </summary>
    [Function("MoveTo")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("MoveTo")]
    [RealName("MoveTo")]
    internal class MoveTo : MSAgentOperationBase {
        ILogger logger = MyIoc.GetService<ILogger>();
        protected override int Run()
        {
            this.Character.MoveTo(this.GetTarget<short>(0, "X 坐标"),
                this.GetTarget<short>(1, "Y 坐标"),
                this.GetTarget<int>(2, "速度"));
            return 0;
        }
        #region ITitleCommand 成员

        public string CommandTitle {
            get { return ""; }
        }

        #endregion
    }
}
