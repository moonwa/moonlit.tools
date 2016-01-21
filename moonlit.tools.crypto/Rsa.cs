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
    /// <summary>
    /// 命令 : Rsa
    /// </summary>
    [Command("Rsa", "Crypt target by rsa")]
    internal class Rsa : ICommand
    {
        [Parameter("CryptType", Description = "加密解密算法名称")]
        public CryptType CryptType { get; set; }

        [Parameter("Key", Description = "密钥", Required = true)]
        public string Key { get; set; }
        [Import]
        public ILogger Logger { get; set; }
        [Target(0, Name = "加密的文件名称")]
        public string Target { get; set; }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        #region ICommand Members

        public async Task<int> Execute()
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(File.ReadAllText(Key, Encoding.UTF8)); 

            // 进行加密解密
            //
            byte[] cryptData = null;
            switch (CryptType)
            {
                case CryptType.Encrypt:
                    cryptData = rsa.Encrypt(File.ReadAllBytes(Target), true);
                    break;
                case CryptType.Decrypt:
                    cryptData = rsa.Decrypt(File.ReadAllBytes(Target), true);
                    break;
                default:
                    break;
            }

            if (cryptData != null)
                File.WriteAllBytes(Target, cryptData);
            return 0;
        }

        #endregion
    }
}