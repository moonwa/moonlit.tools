#region using...

using System;
using MWW.Configuration.Argument;

#endregion

namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : Show
    /// </summary>
    [Function("Show")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Show")]
    [RealName("Show")]
    internal class Show : MSAgentOperationBase
    {
        /// <summary>
        /// 参数 显示速度 名称
        /// </summary>
        private const string Argument_Speed = "Speed";

        /// <summary>
        /// 参数 显示速度
        /// </summary>
        private readonly ValueArgument ArgSpeed = new ValueArgument(Argument_Speed, "显示速度", "0");

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        protected override int Run()
        {
            int speed = Convert.ToInt32(ArgSpeed.Value);
            Character.Show(speed);
            return 0;
        }
    }
}