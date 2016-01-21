//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Composition;
//using System.IO;
//using System.Threading.Tasks;

//namespace Moonlit.Tools.IOExtends
//{
//    /// <summary>
//    /// 命令 : ChangeFiles
//    /// </summary> 
//    [Version(0, 1, Feature = "实现基本功能")] 
//    [Command("ChangeFiles", "取出修改过的文件")]
//    internal class ChangeFiles : ICommand
//    {
//        private readonly ILogger _logger;

//        [ImportingConstructor]
//        public ChangeFiles(ILogger logger)
//        {
//            _logger = logger;
//        }

//        [Target(Name = "获取目录")]
//        public string Input { get; set; }

//        [Target(Name = "输出目录")]
//        public string Output { get; set; }

//        [Parameter("extensionName", Description = "文件扩展名", Required = false)]
//        public string ExtensionName { get; set; }

//        [Parameter("lastModifyTime")]
//        public DateTime IsLastModifyTime { get; set; }

//        #region ITitleCommand 成员

//        public string CommandTitle
//        {
//            get { return ""; }
//        }

//        #endregion

//        #region ICommand Members

//        public  async Task<int> Execute()
//        {
//            if (!Input.EndsWith("\\"))
//            {
//                Input += "\\";
//            }
//            if (!Output.EndsWith("\\"))
//            {
//                Output += "\\";
//            }

//            var di = new DirectoryInfo(Input);

//            di.Foreach(
//                (edi) => { },
//                (fdi) =>
//                    {
//                        if (fdi.LastWriteTime > IsLastModifyTime && CanCopy(fdi.FullName))
//                        {
//                            string outputFileName = Output + fdi.FullName.Substring(Input.Length);
//                            if (!Directory.Exists(Path.GetDirectoryName(outputFileName)))
//                            {
//                                Directory.CreateDirectory(Path.GetDirectoryName(outputFileName));
//                            }
//                            fdi.CopyTo(outputFileName, true);
//                            _logger.WarnLine(fdi.FullName);
//                        }
//                    }
//                );

//            return 0;
//        }

//        #endregion

//        private bool CanCopy(string filename)
//        {
//            if (ExtensionName == null)
//            {
//                return true;
//            }
//            var extensionNames = new List<string>(ExtensionName.Split(','));
//            return extensionNames.IndexOf(Path.GetExtension(filename).Substring(1).ToLower()) != -1;
//        }
//    }
//}