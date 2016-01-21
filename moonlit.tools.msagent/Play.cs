#region using...

using MWW.Configuration.Argument;

#endregion

namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : Play
    /// </summary>
    [Function("Play")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("action")]
    [RealName("Play")]
    internal class Play : MSAgentOperationBase
    {
        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        protected override int Run()
        {
            Character.Play(GetTarget(0, "动作"));
            return 0;
        }

        protected override void InitArgument(Parser parser)
        {
        }
    }
}