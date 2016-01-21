using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Moonlit.Tools.TextExtends
{
    [Command("encodenum", "对指定字符串进行编码，返回二进制数组")]
    internal class Encodenum : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Encodenum(ILogger logger)
        {
            _logger = logger;
        }

        [Parameter(Description = "编码格式", Required = true)]
        public string Encoding { get; set; }

        [Parameter(Description = "编码格式", Required = true)]
        public EncodeDirection EncodeOrDecode { get; set; }

        [Target(Name = "编码内容")]
        public string Content { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            int ret = 0;
            Encoding encoding = System.Text.Encoding.GetEncoding(Encoding);
            switch (EncodeOrDecode)
            {
                case EncodeDirection.Encode:
                    DoEncoding(Content, encoding);
                    break;
                case EncodeDirection.Decode:
                    DoDecoding(Content, encoding);
                    break;
                default:
                    break;
            }

            return ret;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("{0} 字符串 '{1}'", EncodeOrDecode == EncodeDirection.Decode ? "解码" : "编码", Content); }
        }

        #endregion

        private void DoEncoding(string msg, Encoding encoder)
        {
            byte[] bytes = encoder.GetBytes(msg);
            string result = BitConverter.ToString(bytes);
            _logger.InfoLine(result);
        }

        private void DoDecoding(string numbers, Encoding decoder)
        {
            numbers = numbers.Trim(new[] {'-', ' ', '%', '\t', '\r', '\n'});
            string[] bytestrs = numbers.Split(new[] {'%', '-'});
            var bytes = new byte[bytestrs.Length];

            for (int i = 0; i < bytestrs.Length; i++)
            {
                bytes[i] = Convert.ToByte(bytestrs[i], 16);
            }
            string result = decoder.GetString(bytes);
            _logger.InfoLine(result);
        }
    }
}