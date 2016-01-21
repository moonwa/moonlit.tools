using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonlit.Tools.TextExtends
{
    [Command("Find", "搜索文本")]
    public class Find : ICommand
    {
        [Import]
        public ILogger Logger { get; set; }
        [Parameter(Description = "文件")]
        public string Source { get; set; }

        [Parameter(Description = "表达式", Required = true)]
        public string Pattern { get; set; }

        [Target(Name = "内容")]
        public string Content { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            string content = ReadContent();
            MatchCollection matches = Regex.Matches(content, Pattern);
            foreach (Match matche in matches)
            {
                Logger.InfoLine(matche.Value);
            }
            return 0;
        }

        #endregion

        private string ReadContent()
        {
            if (Source != null)
            {
                return File.ReadAllText(Source, Encoding.GetEncoding("gb2312"));
            }
            else
                return Content;
        }
    }
}