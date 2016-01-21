#region using...

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.CryptoExtends
{
    /// <summary>
    /// 命令 : Base64
    /// </summary>
    [Command("Base64", "crypt target by base64")]
    internal class Base64 : ICommand
    {
        private readonly ILogger _logger;
        [ImportingConstructor]
        public Base64(ILogger logger)
        {
            _logger = logger;
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        [Target(0, Name = "计算的值")]
        public string Target { get; set; }

        #region ICommand Members

        public async  Task<int> Execute()
        {
            var buf = new List<byte>();
            for (int i = 0; i < Target.Length; i += 2)
            {
                string s = Target.Substring(i, 2);
                buf.Add(byte.Parse(s, NumberStyles.HexNumber));
            }
            string result = Convert.ToBase64String(buf.ToArray());
            _logger.InfoLine(BitConverter.ToString(buf.ToArray()));
            _logger.InfoLine(result);
            return 0;
        }

        #endregion
    }
}