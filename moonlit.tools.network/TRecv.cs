#region copyright...

// author: zhanzhang

#endregion

#region using...

using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.NetworkExtends
{ 
    [Command("trecv", "TReceive")]
    public class TRecv : ICommand
    {
        private readonly ILogger _logger;

        [Parameter(Description = "使用广播")]
        public bool Multicast { get; set; }
        [ImportingConstructor]
        public TRecv(ILogger logger)
        {
            _logger = logger;
        }

        protected static Encoding Encoding
        {
            get { return Encoding.GetEncoding("gb2312"); }
        }

        [Parameter(Description = @"本地 IP 地址", Required = true, Prefixs = "h")]
        public IPAddress LocalAddress { get; set; }

        [Parameter(Description = @"本地 IP 地址", Required = true, Prefixs = "l")]
        public int LocalPort { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            var udpclient = new UdpClient(LocalPort, AddressFamily.InterNetwork);
            if (Multicast)
            {
                udpclient.JoinMulticastGroup(LocalAddress);
            }

            while (true)
            {
                IPEndPoint remotePoint = null;
                byte[] bytes = udpclient.Receive(ref remotePoint);
                OutputReceived(bytes, remotePoint);
            }
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("监听 udp:{0}:{1}", LocalAddress, LocalPort); }
        }

        #endregion

        private void OutputReceived(byte[] bytes, IPEndPoint remotePoint)
        {
            string s = Encoding.GetString(bytes);
            _logger.InfoLine(string.Format("收到了[{0}:{1}] {2}个字节:{3}", remotePoint.Address,
                                                               remotePoint.Port, bytes.Length, s));
        }
    }
}