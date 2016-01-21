using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Moonlit.Tools.NetworkExtends
{
    /// <summary>
    /// 命令 : Arp
    /// </summary> 
    [Command("Arp", "发送ARP请求")]
    internal class Arp : ICommand
    {
        [Import]
        public ILogger Logger { get; set; }
        [Target(1, Name = "目的地址 ip")]
        public string Desc { get; set; }

        [Target(1, Name = "本地服务器的 ip")]
        public string Host { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
        {
            Int32 ldest = inet_addr(Desc); //目的地的ip 
            Int32 lhost = inet_addr(Host); //本地服务器的ip 

            try
            {
                var macinfo = new Int64();
                Int32 len = 6;
                int res = SendARP(ldest, lhost, ref macinfo, ref len);

                for (int i = 0; i < 6; i++)
                {
                    ulong v = (ulong)macinfo & (ulong)(0xffL << (i * 8));
                    Logger.InfoLine(string.Format("{0:x2}-", (v >> (i * 8))));
                }
            }
            catch (Exception err)
            {
                Logger.WarnLine(err.Message);
            }
            return 0;
        }

        #endregion

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        [DllImport("Iphlpapi.dll")]
        private static extern int SendARP(Int32 dest, Int32 host, ref Int64 mac, ref Int32 length);

        [DllImport("Ws2_32.dll")]
        private static extern Int32 inet_addr(string ip);
    }
}