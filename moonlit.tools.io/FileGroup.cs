using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Moonlit.Tools.IOExtends
{
    /// <summary>
    /// 命令 : FileGroup
    /// </summary> 
    [Command("FileGroup", "文件分组")]
    internal class FileGroup : ICommand
    {
        private readonly ILogger _logger  ;

        [ImportingConstructor]
        public FileGroup(ILogger logger)
        {
            this._logger = logger;
        }

        [Target(0, Name = "目标路径")]
        public string Target { get; set; }

        [Parameter("PrefixLength", Description = "前缀长度")]
        public int PrefixLength { get; set; }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        #region ICommand Members

        public  async Task<int> Execute()
        {
            if (!Directory.Exists(Target))
            {
                throw new UsageErrorException(string.Format("指定目录 [{0}] 不存在!", Target));
            }
            IEnumerable<string> filenames = from x in Directory.GetFiles(Target)
                                            select Path.GetFileName(x).ToLower();

            IEnumerable<IGrouping<string, string>> fileNameGroups = from x in filenames
                                                                    group x by x.Substring(0, PrefixLength)
                                                                    into g
                                                                    select g;
            foreach (var group in fileNameGroups)
            {
                string subDir = Target + "\\" + group.Key;
                if (!Directory.Exists(subDir))
                {
                    Directory.CreateDirectory(subDir);
                }
                foreach (string filename in group)
                {
                    _logger.InfoLine(string.Format("{0} => {1}", Target + "\\" + filename, subDir + "\\" + filename));
                    File.Move(Target + "\\" + filename, subDir + "\\" + filename);
                }
            }
            return 0;
        }

        #endregion
    }
}