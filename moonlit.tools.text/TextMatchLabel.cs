using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MWW.Configuration.Argument;
using MWW.Collections;
using MWW.Text;
using MWW.Text.Extensions;
using System.IO;
using System.Text.RegularExpressions;

namespace MWW.Project.MWWTools.TextExtends {
    /// <summary>
    /// 命令 : TextMatchLabel
    /// </summary>
    [Function("TextMatchLabel")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("TextMatchLabel")]
    [RealName("TextMatchLabel")]
    internal class TextMatchLabel : CommandBase, ITitleCommand {
        ILogger logger = MyIoc.GetService<ILogger>();
        protected override int Run() {
            Regex reLabelName = new Regex(@"System\.Windows\.Forms\.Label\s+(?<labelName>\S+)\s*;", RegexOptions.Compiled);
            Regex reTextBoxName = new Regex(@"System\.Windows\.Forms\.TextBox\s+(?<textboxName>\S+)\s*;", RegexOptions.Compiled);

            List<string> fileContentLines = File.ReadAllLines(this.ArgSourceFile.Value, Encoding.Default).ToList();
            fileContentLines.Trim();
            List<string> DicxContentLines = File.ReadAllLines(this.ArgDictXFile.Value, Encoding.Default).ToList();
            DicxContentLines.Trim();

            string labID = string.Empty;
            string txtID = string.Empty;

            Dictionary<string, string> labDisplayMap = new Dictionary<string, string>();    // 显示名称 vs 字段名称


            for (int i = 0; i < DicxContentLines.Count; i++) {
                string line = DicxContentLines[i];

                string[] tmp = line.Split('\t');
                string disp = tmp[0];
                string field = tmp[1];

                labDisplayMap.Add(disp, field);
            }

            Dictionary<string, string> labTxtMap = new Dictionary<string, string>();
            for (int i = 0; i < fileContentLines.Count; i++) {
                string line = fileContentLines[i];

                if (string.IsNullOrEmpty(labID)) {
                    Match mcLabel = reLabelName.Match(line);
                    if (!mcLabel.Success) continue;

                    labID = mcLabel.Groups["labelName"].Value;
                } else if (!string.IsNullOrEmpty(labID) && string.IsNullOrEmpty(txtID)) {
                    Match mcTextBox = reTextBoxName.Match(line);
                    if (!mcTextBox.Success) { labID = string.Empty; i--; continue; }

                    txtID = mcTextBox.Groups["textboxName"].Value;
                    labTxtMap.Add(labID, txtID);
                    labID = string.Empty;
                    txtID = string.Empty;
                }
            }
            string sourceFileContent = File.ReadAllText(this.ArgSourceFile.Value, Encoding.Default);

            Regex reLabelText = new Regex(@"this\.(?<labelID>\S+)\.Text\s*=\s*""(?<display>\S+)""\s*;", RegexOptions.Compiled);
            for (int i = 0; i < fileContentLines.Count; i++) {
                string line = fileContentLines[i];

                Match mcLabelText = reLabelText.Match(line);
                if (mcLabelText.Success) {
                    string display = mcLabelText.Groups["display"].Value;
                    string labid = mcLabelText.Groups["labelID"].Value;

                    if (!labDisplayMap.ContainsKey(display)) {
                        continue;
                    }
                    string field = labDisplayMap[display];

                    if (!labTxtMap.ContainsKey(labid)) {
                        throw new Exception(string.Format("{0} 没有找到对应的 TextBox", labid));
                    }
                    string txtid = labTxtMap[labid];
                    sourceFileContent = sourceFileContent.Replace(labid, "lab" + field);
                    sourceFileContent = sourceFileContent.Replace(txtid, "txt" + field);
                }
            }
            Console.WriteLine(sourceFileContent);
            return 0;
        }

        protected override void InitArgument(Parser parser) {
            parser.AddArguments(ArgSourceFile, ArgDictXFile);
        }

        #region 参数 源文件
        /// <summary>
        /// 参数 源文件 名称
        /// </summary>
        private const string Argument_SourceFile = "SourceFile";
        /// <summary>
        /// 参数 源文件
        /// </summary>
        private ValueArgument ArgSourceFile = new ValueArgument(Argument_SourceFile, "源文件", true);
        #endregion


        #region 参数 数据字典
        /// <summary>
        /// 参数 数据字典 名称
        /// </summary>
        private const string Argument_DictXFile = "DictXFile";
        /// <summary>
        /// 参数 数据字典
        /// </summary>
        private ValueArgument ArgDictXFile = new ValueArgument(Argument_DictXFile, "数据字典", true);
        #endregion


        class TempClass {
            public string LabelID { get; set; }
            public string TextBoxID { get; set; }
            public string DicxName { get; set; }
            public string DicxDisplay { get; set; }
        }


        #region ITitleCommand 成员

        public string CommandTitle {
            get { return ""; }
        }

        #endregion
    }
}
