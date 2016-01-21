#region using...

using AgentObjects;
using MWW.Configuration.Argument;

#endregion

namespace Moonlit.Tools.MSAgentExtends
{
    internal abstract class MSAgentOperationBase : CommandBase
    {
        /// <summary>
        /// 参数 角色 名称
        /// </summary>
        private const string Argument_Character = "Character";

        /// <summary>
        /// 参数 角色
        /// </summary>
        private readonly ValueArgument ArgCharacter = new ValueArgument(Argument_Character, "角色", "merlin");

        protected IAgentCtlCharacterEx Character
        {
            get { return MSAgentInstance.GetCharacters(ArgCharacter.Value); }
        }

        protected override void InitArgument(Parser parser)
        {
            parser.AddArguments(ArgCharacter);
        }
    }
}