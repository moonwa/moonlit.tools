#region using...

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.CryptoExtends
{
    [Command("md5", "crypto file or string by md5")]
    public sealed class Md5 : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Md5(ILogger logger)
        {
            _logger = logger;
        }

        [Target(0, Name = "计算目标")]
        public string Target { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
        {
            string arg = Target;
            MD5 md5 = MD5.Create();
            byte[] bytes;
            if (File.Exists(arg))
            {
                using (FileStream fs = File.OpenRead(arg))
                {
                    bytes = md5.ComputeHash(fs);
                }
            }
            else
            {
                bytes = Encoding.UTF8.GetBytes(arg);
                bytes = md5.ComputeHash(bytes);
            }
            _logger.InfoLine(BitConverter.ToString(bytes).Replace("-", ""));
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get
            {
                if (File.Exists(Target))
                {
                    return string.Format("计算文件 {0} Md5 值", Target);
                }
                return string.Format("计算字符串 '{0}' Md5 值", Target);
            }
        }

        #endregion
    }
}