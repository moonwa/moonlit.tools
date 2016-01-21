#region using...

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using Moonlit.IO;

#endregion

namespace Moonlit.Tools.IOExtends
{
    [Function("将文件从同步路径同步到被同步路径")]
    [CommandUsage("-d 被同步路径 -s 同步路径")]
    [Version(0, 1, Feature = "实现基本功能")]
    [Version(0, 1, 5, Feature = "实现进度显示")]
    [Version(0, 2, Feature = "实现断点同步功能")]
    [Version(0, 2, 1, Feature = "添加自动清理断点服务功能")]
    [Version(0, 2, 2, Feature = "处理对只读文件复制失败功能")]
    [Command("synch")]
    public class Synch : ICommand
    {
        private readonly ILogger _logger;
        private string _currentSrcFileName;

        [ImportingConstructor]
        public Synch(ILogger logger)
        {
            this._logger = logger;
        }

        [Parameter(Required = true)]
        public string Destination { get; set; }

        [Parameter(Required = true)]
        public string Source { get; set; }

        [Parameter]
        public bool Archive { get; set; }

        [Parameter]
        public bool Length { get; set; }

        [Parameter(Prefixs = "t")]
        public bool LastWriteTime { get; set; }

        #region ICommand Members

        public int Execute()
        {
            var synchronizer = new FileSystemSynchronizer
                                   {
                                       SynchArchiveFile = Archive,
                                       SynchDiffLastWriteTimeFile = LastWriteTime,
                                       SynchDiffLengthFile = Length,
                                   };

            using (FileContinueWorkFactory continueWorkFactory = CreateContinueWorkFactory())
            {
                var fileCopy = new StreamFileCopy();
                fileCopy.ProcessChanged += fileCopy_ProcessChanged;
                fileCopy.CopyAttribute = true;
                fileCopy.CopyCreationTime = true;
                fileCopy.CopyLastAccessTime = true;
                fileCopy.CopyLastWriteTime = true;
                fileCopy.BufferSize = 1024 * 1024 * 4;
                fileCopy.ContinueWorkFactory = continueWorkFactory;
                synchronizer.FileSynchronizer = fileCopy;
                synchronizer.ValidateDirectory += synchronizer_ValidateDirectory;
                FileSystemSynchronizer.StatResult result = synchronizer.Stat(new DirectoryInfo(Source),
                                                                             new DirectoryInfo(Destination));

                synchronizer.Synchronize(result, DeleteFileCallback, DeleteDirectoryCallback,
                                         CreateDirectoryCallback, CopyFileCallback);
                return 0;
            }
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("同步 {0} => {1}", Source, Destination); }
        }

        #endregion

        private FileContinueWorkFactory CreateContinueWorkFactory()
        {
            var continueWorkFactory = new FileContinueWorkFactory();
            continueWorkFactory.LoadFromDirectory(Source);
            return continueWorkFactory;
        }

        private void fileCopy_ProcessChanged(ProcessChangedEventArgs e)
        {
            int size = 20;
            int nameLength = (24 - Encoding.GetEncoding("gb2312").GetByteCount(_currentSrcFileName) +
                              _currentSrcFileName.Length);
            string format = string.Format("    {{0,-{0}}} [{{1, 4}}MB - {{2, 4}}MB, {{3, -{1}}} {{4, 6:p}}]", nameLength,
                                          size);
            //string format = "    {0,-" + nameLength + "} [{1, 4}MB - {2, 4}MB, {3, -" + size + "} {4, 6:p}]";

            int per = 20;
            if (e.Total != 0) Convert.ToInt32(e.Current / e.Total * size);
            if (Math.Abs(e.Total - e.Current) < 0.0001)
            {
                if (!string.IsNullOrEmpty(_currentSrcFileName))
                {
                    _logger.InfoLoop(string.Format(format, _currentSrcFileName, (long)e.Current / (1024 * 1024),
                                                  (long)e.Total / (1024 * 1024), new string('#', per), e.Current / e.Total));
                    _logger.InfoLine("");
                    _currentSrcFileName = string.Empty;
                }
            }
            else
            {
                _logger.InfoLoop(string.Format(format, _currentSrcFileName, (long)e.Current / (1024 * 1024),
                                              (long)e.Total / (1024 * 1024), new string('#', per), e.Current / e.Total));
            }
        }

        private bool synchronizer_ValidateDirectory(DirectoryInfo arg1, DirectoryInfo arg2)
        {
            _logger.InfoLoop(string.Format("synchronizing: {0}", arg1.FullName));
            return true;
        }

        private void CopyFileCallback(FileInfo src, FileInfo dst)
        {
            _currentSrcFileName = GetRelativeName(src.FullName);
            if (Archive)
            {
                src.Attributes = ~FileAttributes.Archive & src.Attributes;
            }
        }

        private string GetRelativeName(string s)
        {
            s = Path.GetFileName(s);
            Encoding encoding = Encoding.GetEncoding("gb2312");
            if (encoding.GetByteCount(s) > 24)
            {
                string nameWithoutExt = Path.GetFileNameWithoutExtension(s);
                string ext = Path.GetExtension(s);
                int maxLength = 24 - ext.Length - 2;
                for (int i = Math.Min(maxLength, nameWithoutExt.Length); i >= 0; i--)
                {
                    int len = encoding.GetByteCount(nameWithoutExt.Substring(0, i));
                    if (len <= maxLength)
                    {
                        nameWithoutExt = nameWithoutExt.Substring(0, i);
                        break;
                    }
                }
                s = nameWithoutExt + ".." + ext;
            }
            return s;
            //if (this.ArgSource.Value.EndsWith("\\"))
            //    s = s.Substring(this.ArgSource.Value.Length);
            //else
            //    s = s.Substring(this.ArgSource.Value.Length + 1);
            //return s;
        }

        private void CreateDirectoryCallback(DirectoryInfo di)
        {
            _logger.InfoLine(string.Format("create : {0}\\", di.FullName));
        }

        private void DeleteDirectoryCallback(DirectoryInfo di)
        {
            _logger.InfoLine(string.Format("delete : {0}\\", di.FullName));
        }

        private void DeleteFileCallback(FileInfo fi)
        {
            _logger.InfoLine(string.Format("delete : {0}", fi.FullName));
        }
    }
}