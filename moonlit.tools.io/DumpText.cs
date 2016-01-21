#region using...

using System;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace Moonlit.Tools.IOExtends
{
    /// <summary>
    /// 命令 : Dump
    /// </summary>
    [Function("将文本转换二进制数据")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("srcFile dstFile")]
    [Command("DumpText")]
    [Description("dd")]
    internal class DumpText : ICommand
    {
        [Target(Name = "内容或者文件")]
        public string Input { get; set; }

        [Target(1, Name = "输出文件")]
        public string Output { get; set; }

        #region ICommand Members

        public int Execute()
        {
            string text = Input;
            if (File.Exists(Input))
            {
                text = File.ReadAllText(Input, Encoding.Default);
            }
            using (var dstFile = new FileStream(Output, FileMode.Create, FileAccess.Write))
            {
                string[] ss = text.Split();
                foreach (string s in ss)
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    string str = s.ToLower();
                    if (str.StartsWith("\\x"))
                        str = str.Substring(2);

                    byte b = byte.Parse(str, NumberStyles.HexNumber);
                    dstFile.WriteByte(b);
                }
                dstFile.Flush();
            }
            return 0;
        }

        #endregion
    }
}