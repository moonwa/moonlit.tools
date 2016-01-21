using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Moonlit.Tools.NetworkExtends
{
    /// <summary>
    /// 命令 : Wakeup
    /// </summary> 
    [Command("wakeup", "唤醒远程服务器")]
    internal class Wakeup : ICommand
    {
        [Target]
        public string Mac { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            var buf = new List<byte>();
            for (int i = 0; i < 6; i++)
            {
                buf.Add(0xff);
            }
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < Mac.Length; j += 3)
                {
                    byte b = Byte.Parse(Mac.Substring(j, 2), NumberStyles.HexNumber);
                    buf.Add(b);
                }
            }
            var client = new UdpClient(7010);
            client.Send(buf.ToArray(), buf.Count, "255.255.255.255", 7010);
            return 0;
        }

        #endregion

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }
}