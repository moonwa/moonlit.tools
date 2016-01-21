using System.ComponentModel.Composition;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Moonlit.Tools.DotNet
{
    /// <summary>
    /// 命令 : MachineKey
    /// </summary> 
    [Command("machineKey", "生成 machineKey 加密解密密钥")]
    internal class MachineKey : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MachineKey(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<int> Execute()
        {
            _logger.InfoLine(getASPNET20machinekey());
            return 0;
        } 
        public string getASPNET20machinekey()
        {
            var aspnet20machinekey = new StringBuilder();
            string key64byte = getRandomKey(64);
            string key32byte = getRandomKey(32);
            aspnet20machinekey.Append("<machineKey \r\n");
            aspnet20machinekey.Append("validationKey=\"" + key64byte + "\"\r\n");
            aspnet20machinekey.Append("decryptionKey=\"" + key32byte + "\"\r\n");
            aspnet20machinekey.Append("validation=\"SHA1\" decryption=\"AES\"\r\n");
            aspnet20machinekey.Append("/>\r\n");
            return aspnet20machinekey.ToString();
        }

        public string getASPNET11machinekey()
        {
            var aspnet11machinekey = new StringBuilder();
            string key64byte = getRandomKey(64);
            string key24byte = getRandomKey(24);

            aspnet11machinekey.Append("<machineKey ");
            aspnet11machinekey.Append("validationKey=\"" + key64byte + "\"\r\n");
            aspnet11machinekey.Append("decryptionKey=\"" + key24byte + "\"\r\n");
            aspnet11machinekey.Append("validation=\"SHA1\"\r\n");
            aspnet11machinekey.Append("/>\r\n");
            return aspnet11machinekey.ToString();
        }

        private string getRandomKey(int bytelength)
        {
            var buff = new byte[bytelength];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(buff);
            var sb = new StringBuilder(bytelength*2);
            for (int i = 0; i < buff.Length; i++)
                sb.Append(string.Format("{0:X2}", buff[i]));
            return sb.ToString();
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
         
    }
}