using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text.RegularExpressions;
using Moonlit.IO.Extensions;

namespace Moonlit.Tools.IOExtends
{
    [Function("本地垃圾清理工具")]
    [CommandUsage("target")]
    internal sealed class Clean : ICommand
    {
        private readonly ILogger _logger ;

        [ImportingConstructor]
        public Clean(ILogger logger)
        {
            this._logger = logger;
        }

        [Parameter("Target", Description = "清理目标")]
        public string Target { get; set; }

        [Parameter("Temporary", Description = "清理临时文件夹")]
        public bool Temporary { get; set; }

        [Parameter("Temporary", Description = "文件夹名称正则式")]
        public string Pattern { get; set; }

        [Parameter("EmptyDirectory", Description = "清理所有空目录")]
        public bool EmptyDirectory { get; set; }

        #region ICommand Members

        public int Execute()
        {
            if (Target != null)
            {
                _logger.InfoLine(string.Format("清理目标 {0}", Target));
                CleanTarget(Target);
            }
            if (Temporary)
            {
                DeleteSubs(Environment.GetEnvironmentVariable("TEMP"));
                DeleteSubs(Environment.GetEnvironmentVariable("TMP"));
            }
            return 0;
        }

        #endregion

        private void DeleteSubs(string temp)
        {
            if (!string.IsNullOrEmpty(temp))
            {
                var dir = new DirectoryInfo(temp);
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    try
                    {
                        subDir.ForceDelete(true);
                        _logger.InfoLine(string.Format("清理目录 {0}", subDir.FullName));
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnLine(string.Format("清理目录 {0} 失败: {1}", subDir.FullName, ex.Message));
                    }
                }
                foreach (FileInfo file in dir.GetFiles())
                {
                    try
                    {
                        file.ForceDelete();
                        _logger.InfoLine(string.Format("清理文件 {0}", file.FullName));
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnLine(string.Format("清理文件 {0} 失败: {1}", file.FullName, ex.Message));
                    }
                }
            }
        }

        private void CleanTarget(string target)
        {
            var dir = new DirectoryInfo(target);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("目标目录不存在");
            }

            dir.Reforeach(DirectoryCallback, FileCallback);
        }

        private void DirectoryCallback(DirectoryInfo dir)
        {
            if (!EmptyDirectory && Pattern == null)
            {
                return;
            }
            bool emptyDirCondition = (!EmptyDirectory) ||
                                     (dir.GetFiles().Length == 0 && dir.GetDirectories().Length == 0);
            bool pattern = (Pattern != null) || Regex.Match(dir.Name, Pattern).Success;
            if (emptyDirCondition && pattern)
            {
                _logger.Info(string.Format("删除文件夹 {0}", dir.FullName));
                dir.ForceDelete();
            }
        }

        private void FileCallback(FileInfo file)
        {
        }
    }
}