namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : Think
    /// </summary>
    [Function("Think")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Think")]
    [RealName("Think")]
    internal class Think : MSAgentOperationBase
    {
        private string Message
        {
            get { return GetTarget(0, "思考内容"); }
        }

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        protected override int Run()
        {
            Character.Think(Message);
            return 0;
        }
    }
}