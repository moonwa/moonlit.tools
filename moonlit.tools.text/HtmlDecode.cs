using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web;

namespace Moonlit.Tools.TextExtends
{
    /// <summary>
    /// 命令 : HtmlDecode
    /// </summary>
    [Command("HtmlDecode", "decode text for html")]
    internal class HtmlDecode : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public HtmlDecode(ILogger logger)
        {
            this._logger = logger;
        }

        [Target(Name = "内容", Required = true)]
        public string Content { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            _logger.InfoLine(HttpUtility.HtmlDecode(Content));
            return 0;
        }

        #endregion

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }
}