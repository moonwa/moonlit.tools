using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web;

namespace Moonlit.Tools.TextExtends
{
    [Command("UrlEncode", "对URL进行编码")]
    public class UrlEncode : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public UrlEncode(ILogger logger)
        {
            _logger = logger;
        }

        [Target(Name = "内容", Required = true)]
        public string Content { get; set; }
        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("url编码 '{0}'", Content); }
        }

        #endregion

        public async Task<int> Execute()
        {
            _logger.InfoLine(HttpUtility.UrlEncode(Content));
            return 0;
        }

    }
}