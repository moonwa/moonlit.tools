using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonlit.Tools.TextExtends
{
    [Command("Replace", "对文本进行正则式替换")]
    internal class Replace : ICommand
    {
        [Target(Name = "目标文件名")]
        public string Target { get; set; }


        [Parameter(Description = "输出的文件名", Required = true)]
        public string OutputFileName { get; set; }

        [Parameter(Description = "被替换的值", Required = true)]
        public string ReplaceTo { get; set; }

        [Parameter(Description = "被匹配的正则式", Required = true)]
        public string Pattern { get; set; }

        [Parameter(Description = "多行")]
        public bool Multiline { get; set; }

        [Parameter(Description = "递归")]
        public bool RecursionArgument { get; set; }

        [Parameter(Description = "忽略大小写")]
        public bool Ignore { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
        {
            string filename = Target;
            if (File.Exists(filename))
            {
                Process(File.ReadAllText(filename, Encoding.Default));
            }
            else
            {
                Process(filename);
            }

            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("正在对 {0} 进行文本替换", Target); }
        }

        #endregion

        private void Process(string s)
        {
            RegexOptions options = GetOptions();
            var regex = new Regex(Pattern.Replace("\\t", "\t"), options);
            s = regex.Replace(s, ReplaceTo.Replace("\\t", "\t"));

            if (OutputFileName != null)
            {
                Output(s, OutputFileName);
            }
            else
            {
                Logger.InfoLine(s);
            }
        }
        [Import]
        public ILogger Logger { get; set; }
        private void Output(string s, string filename)
        {
            File.WriteAllText(filename, s, Encoding.Default);
        }

        private RegexOptions GetOptions()
        {
            RegexOptions options = RegexOptions.None;
            if (Ignore)
            {
                options |= RegexOptions.IgnoreCase;
            }
            if (Multiline)
            {
                options |= RegexOptions.Multiline;
            }
            return options;
        }
    }
}