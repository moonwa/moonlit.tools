using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web;

namespace Moonlit.Tools.TextExtends
{
    /// <summary>
    /// 命令 : HtmlEncode
    /// </summary>
    [Command("HtmlEncode", "encode text for html")]
    internal class HtmlEncode : ICommand
    {
        private readonly ILogger _logger ;

        [ImportingConstructor]
        public HtmlEncode(ILogger logger)
        {
            this._logger = logger;
        }

        [Target(Name = "要编码的内容")]
        public string Content { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            _logger.InfoLine(HttpUtility.HtmlEncode(Content));
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