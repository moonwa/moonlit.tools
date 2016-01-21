using Moonlit.Configuration.ConsoleParameter;

namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : Hide
    /// </summary>
    [Function("Hide")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Hide")]
    [RealName("Hide")]
    internal class Hide : MSAgentOperationBase
    {
        ILogger logger = MyIoc.GetService<ILogger>();
        protected override int Run()
        {
            this.Character.Hide(0);
            return 0;
        }

        protected override void InitArgument(Parser parser)
        {
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }
}
