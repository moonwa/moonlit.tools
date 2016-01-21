using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlit.Tools.IOExtends
{
    /// <summary>
    /// 命令 : Dump
    /// </summary> 
    [Command("trace", "跟踪文件")]
    internal class Trace : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Trace(ILogger logger)
        {
            this._logger = logger;
        }

        [Target(0, Name = "文件或目录")]
        public string FileName { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            System.Threading.ManualResetEvent mre = new ManualResetEvent(true);
            string path = Path.GetDirectoryName(FileName);
            _logger.InfoLine("trace the path: " + path);
            var watcher = new FileSystemWatcher(path);
            watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName |
                                   NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                                   NotifyFilters.Security | NotifyFilters.Size;
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += watcher_Renamed;
            watcher.Changed += watcher_Changed;
            watcher.Created += watcher_Created;
            watcher.Deleted += watcher_Deleted;
            watcher.Error += watcher_Error;
            watcher.EnableRaisingEvents = true;

            mre.WaitOne();
            return 0;
        }

        #endregion

        private void watcher_Error(object sender, ErrorEventArgs e)
        {
            _logger.InfoLine(string.Format(@"error: {0}", e.GetException().Message));
        }

        private void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _logger.InfoLine(string.Format(@"deleted: {0}", e.FullPath));
        }

        private void watcher_Created(object sender, FileSystemEventArgs e)
        {
            _logger.InfoLine(string.Format(@"created: {0}", e.FullPath));
        }

        private void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            _logger.InfoLine(string.Format(@"{0}: {1}", e.ChangeType.ToString().ToLower(), e.FullPath));
        }

        private void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            _logger.InfoLine(string.Format(@"rename: {0} {1}", e.OldFullPath, e.FullPath));
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }
}