#region using...

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.IOExtends
{
    /// <summary>
    /// 命令 : Dump
    /// </summary> 
    [Command("hex","将文本转换二进制数据")] 
    internal class HexText : ICommand
    {
        [Target(Name = "内容或者文件")]
        public string Input { get; set; }

        [Target(1, Name = "输出文件")]
        public string Output { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
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