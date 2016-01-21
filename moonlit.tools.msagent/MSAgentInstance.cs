using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AgentObjects;

namespace Moonlit.Tools.MSAgentExtends
{
    internal static class MSAgentInstance
    {
        public static AgentClass _agent = new AgentClass();
        static MSAgentInstance()
        {
            _agent.Connected = true;
        }
        private static Dictionary<string, IAgentCtlCharacterEx> _characters = new Dictionary<string, IAgentCtlCharacterEx>();
        public static IAgentCtlCharacterEx GetCharacters(string id)
        {
            if (!_characters.ContainsKey(id))
            {
                lock (typeof(MSAgentInstance))
                {
                    if (!_characters.ContainsKey(id))
                    {
                        IAgentCtlRequest request = _agent.Characters.Load(id, id + ".acs");

                        string AgentStates = "Showing, Hiding, Speaking, Moving";
                        string AgentAnimations = "GetAttention, GetAttentionReturn, Congratulate, Acknowledge, Read, WriteContinued, WriteReturn, wave";


                        IAgentCtlCharacterEx character = _agent.Characters[id];
                        // Agent.get(Request, list) 预载相关 MSAgent 动画数据
                        // MSAgent人物数据文件支持单结构角色文件（.acs，角色数据与动画数据存于同一个文件），
                        // 也支持分离结构角色文件（.acf，角色数据存于.acf中，动画数据存于.aca中）。
                        // 基于本地硬盘和网络调用均可采用这两种模式，
                        // 当调用网络 acf 文件时，由于角色数据与动画数据分别下载，
                        // 所以需要预载相关动画数据，使用 acs 文件（一般没有本地 acf 文件的可能性），不需要预载。
                        IAgentCtlRequest agentStateRequest = character.Get("state", AgentStates, Type.Missing);
                        IAgentCtlRequest agentAnimationRequest = character.Get("animation", AgentAnimations, Type.Missing);
                        _characters[id] = character;
                        Console.WriteLine("yes");
                        _agent.DblClick += new _AgentEvents_DblClickEventHandler(_agent_DblClick);
                        _agent.Click += new _AgentEvents_ClickEventHandler(_agent_Click);
                        _agent.Command += new _AgentEvents_CommandEventHandler(_agent_Command);
                        character.Commands.RemoveAll();
                        
                        character.Commands.Caption = "by zz";
                        character.Commands.Add("Hide", "隐藏", "Hide", true, true);
                        character.Commands.Add("Show", "显示", "Show", true, true);
                        return character;
                    }
                }
            }
            return _characters[id];
        }

        static void _agent_Command(object UserInput)
        {
            Console.WriteLine(UserInput.GetType().Name);
        }

        static void _agent_Click(string CharacterID, short Button, short Shift, short x, short y)
        {
            Console.WriteLine("yeah yeah");
        }

        static void _agent_DblClick(string CharacterID, short Button, short Shift, short x, short y)
        {
            Console.WriteLine("haha");
        }
    }
}
