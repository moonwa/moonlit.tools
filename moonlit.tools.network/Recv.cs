#region using...

using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Text;
using Moonlit.Net;

#endregion

namespace Moonlit.Tools.NetworkExtends
{
    [Function("简单的服务器端")]
    [CommandUsage(" -l 端口号")]
    [Version(0, 1, Feature = "实现基本功能")]
    [Command("recv")]
    internal class Recv : ICommand
    {
        private static ILogger Logger;
        [ImportingConstructor]
        public Recv(ILogger logger)
        {
            Logger = logger;
        }

        internal static Reactor<NetServerHandler> ractor = new Reactor<NetServerHandler>();

        [Parameter(Description = @"本地 IP 地址", Required = true, Prefixs = "h")]
        public IPAddress LocalAddress { get; set; }

        [Parameter(Description = @"本地 IP 地址", Required = true, Prefixs = "l")]
        public int LocalPort { get; set; }

        #region ICommand Members

        public int Execute()
        {
            ractor.Accept(LocalPort);
            Console.ReadKey();
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("开始监听端口 Tcp:{0}:{1}", LocalAddress, LocalPort); }
        }

        #endregion

        #region Nested type: NetServerHandler

        internal sealed class NetServerHandler : ReactorHandler
        {
            private string name;

            public override void OnConnected()
            {
                name = DateTime.Now.ToString();
                Recv.Logger.InfoLine(name + " connected");
            }

            public override void OnReceived(byte[] data)
            {
                string s = Encoding.UTF8.GetString(data);
                Recv.Logger.Info(s);
                if (s.StartsWith("close "))
                {
                }
            }

            public override void OnDisconnected()
            {
                Recv.Logger.InfoLine(name + " disconnected");
            }
        }

        #endregion
    }
}