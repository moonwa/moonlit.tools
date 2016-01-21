namespace Moonlit.Tools.MSAgentExtends
{
    /// <summary>
    /// 命令 : Speak
    /// </summary>
    [Function("Speak")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("")]
    [RealName("Speak")]
    internal class Speak : MSAgentOperationBase
    {
        private string Message
        {
            get { return GetTarget(0, "消息"); }
        }

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        protected override int Run()
        {
            Character.Speak(Message, "");
            return 0;
        }
    }
}