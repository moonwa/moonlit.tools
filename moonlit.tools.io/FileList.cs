using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonlit.Tools.IO
{
    /// <summary>
    /// 命令 : FileList
    /// </summary>
    [Command("FileList", "显示文件列表")]
    internal class FileList : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public FileList(ILogger logger)
        {
            this._logger = logger;
        }



        [Target(0, Name = "目标路径")]
        public string Target { get; set; }
        [Target(1, Name = "过滤器")]
        public string Filter { get; set; }


        public Task<int> Execute()
        {
            var di = new DirectoryInfo(Target);
            if (!di.Exists)
            {
                throw new Exception("目录不存在");
            }
            Show(di, 1);
            return Task.FromResult(0);
        }

        private void Show(DirectoryInfo di, int indent)
        {
            _logger.InfoLine(string.Format($"{new String(' ', indent * 2)}{di.Name}/"));
            foreach (var dir in di.GetFileSystemInfos().OfType<DirectoryInfo>())
            {
                Show(dir, indent + 1);
            }
            var fileInfos = di.GetFileSystemInfos(Filter).OfType<FileInfo>();
            foreach (var fi in fileInfos)
            {
                string encoding = GetEncoding(fi);
                _logger.InfoLine(string.Format($"{new String(' ', indent * 2)}{fi.Name}|{fi.Length}|{encoding}"));
            }

        }

        private string GetEncoding(FileInfo fi)
        {
            using (var fs = fi.Open(FileMode.Open, FileAccess.Read))
            {
                using (var streamReader = new StreamReader(fs, Encoding.Default, true))
                {
                    var content = streamReader.ReadToEnd();
                    return streamReader.CurrentEncoding.EncodingName;
                }
            }

        }
    }
}