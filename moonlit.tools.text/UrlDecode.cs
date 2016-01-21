using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web;

namespace Moonlit.Tools.TextExtends
{
    /// <summary>
    /// 命令 : UrlDecode
    /// </summary>
    [Command("urldecode", "decode the url")]
    internal class UrlDecode : ICommand
    {
        private readonly ILogger _logger ;

        [ImportingConstructor]
        public UrlDecode(ILogger logger)
        {
            this._logger = logger;
        }

        [Target(Name = "内容", Required = true)]
        public string Content { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            _logger.InfoLine(HttpUtility.UrlDecode(Content));
            return 0;
        }

        #endregion

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return string.Format("url 解码 '{0}'", Content); }
        }

        #endregion
    }
}