#region copyright...

// author: zhanzhang

#endregion

#region using...

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.NetworkExtends
{ 
    [Command("tsend", "向远程端口发UDP包")]
    internal class TSend : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public TSend(ILogger logger)
        {
            _logger = logger;
        }

        [Parameter(Description = @"远程端 IP 地址", Required = true, Prefixs = "a")]
        public IPAddress RemoteAddress { get; set; }

        [Parameter(Description = @"远程端端口", Required = true, Prefixs = "r")]
        public int RemotePort { get; set; }

        [Parameter(Description = @"要发送的文件")]
        public string FileName { get; set; }

        [Parameter(Description = @"要发送的消息")]
        public string Message { get; set; }

        private byte[] Data
        {
            get
            {
                if (Message != null)
                {
                    return Encoding.GetEncoding("gb2312").GetBytes(Message);
                }
                else if (FileName != null)
                {
                    return File.ReadAllBytes(FileName);
                }
                throw new Exception("请在 /m 和 /f 中选择一个");
            }
        }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            int send_cnt = SendData(new IPEndPoint(RemoteAddress, RemotePort), Data);

            int total_cnt = Data.Length;
            OutputSendState(send_cnt, total_cnt);
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("发送消息到 udp:{0}:{1}", RemoteAddress, RemotePort); }
        }

        #endregion

        private void OutputSendState(int send_cnt, int total_cnt)
        {
            _logger.InfoLine(string.Format("总共{0}字节，发送了{1}字节", total_cnt, send_cnt));
        }

        private int SendData(EndPoint remote_endpoint, byte[] datas)
        {
            var client = new UdpClient(AddressFamily.InterNetwork);
            client.Connect(new IPEndPoint(RemoteAddress, RemotePort));
            return client.Send(datas, datas.Length);
        }
    }
}