#region using...

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moonlit.IO;

#endregion

namespace Moonlit.Tools.IOExtends
{  
    [Command("SynchWatcher", "将文件从同步路径同步到被同步路径")]
    public class SynchWatcher : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public SynchWatcher(ILogger logger)
        {
            this._logger = logger;
        }

        [Parameter(Required = true)]
        public string Destination { get; set; }

        [Parameter(Required = true)]
        public string Source { get; set; }
        [Parameter]
        public bool Delete { get; set; }

        System.Threading.AutoResetEvent _resetEvent = new AutoResetEvent(false);

        class Watcher
        {
            private readonly SynchWatcher _synchWatcher;
            private readonly string _srcPath;
            private readonly string _dstPath;

            public Watcher(SynchWatcher synchWatcher, string srcPath, string dstPath)
            {
                _synchWatcher = synchWatcher;
                _srcPath = Path.GetFullPath(srcPath);
                if (!_srcPath.EndsWith("\\"))
                    _srcPath = _srcPath + "\\";
                _dstPath = Path.GetFullPath(dstPath);
                if (!_dstPath.EndsWith("\\"))
                    _dstPath = _dstPath + "\\";
            }

            public void Run()
            {
                if (!Directory.Exists(_dstPath))
                    Directory.CreateDirectory(_dstPath);
                Clear(_dstPath);
                Copy(_srcPath);
                StartWatch(_srcPath);
            }

            private void Copy(string srcPath)
            {
                var fileName = GetFileName(srcPath);
                this._synchWatcher.AddTask(new CreateDirectory(Path.Combine(_dstPath, fileName)));
                foreach (var file in GetFiles(srcPath))
                {
                    _synchWatcher.AddTask(new FileCopyTask(Path.Combine(_srcPath, GetFileName(file)), Path.Combine(_dstPath, GetFileName(file))));
                }
                foreach (var directory in GetDirectories(srcPath))
                {
                    _synchWatcher.AddTask(new CreateDirectory(Path.Combine(_dstPath, GetFileName(directory))));
                    Copy(directory);
                }
            }


            private FileSystemWatcher _watcher;
            private void StartWatch(string srcPath)
            {
                _watcher = new FileSystemWatcher(srcPath);
                _watcher.IncludeSubdirectories = true;
                _watcher.NotifyFilter = NotifyFilters.Size
                    | NotifyFilters.LastWrite
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.CreationTime;
                _watcher.Created += WatcherOnCreated;
                _watcher.Changed += new FileSystemEventHandler(_watcher_Changed);
                _watcher.Deleted += new FileSystemEventHandler(_watcher_Deleted);
                _watcher.Renamed += new RenamedEventHandler(_watcher_Renamed);
                _watcher.EnableRaisingEvents = true;
            }

            void _watcher_Renamed(object sender, RenamedEventArgs e)
            {
                var src = Path.Combine(_dstPath, GetFileName(e.OldFullPath));
                var dst = Path.Combine(_dstPath, GetFileName(e.FullPath));
                this._synchWatcher.AddTask(new RenameTask(src, dst));
            }

            void _watcher_Deleted(object sender, FileSystemEventArgs e)
            {
                var fileName = Path.Combine(_dstPath, GetFileName(e.FullPath));
                if (Directory.Exists(fileName))
                {
                    Clear(fileName);
                }
                if (File.Exists(fileName))
                    DeleteFile(fileName);
            }

            void _watcher_Changed(object sender, FileSystemEventArgs e)
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    var fileName = GetFileName(e.FullPath);
                    if (File.Exists(e.FullPath))
                    {
                        this._synchWatcher.AddTask(new FileCopyTask(Path.Combine(_srcPath, fileName),
                                                                    Path.Combine(_dstPath, fileName)));
                    }
                    if (Directory.Exists(e.FullPath))
                    {
                        this._synchWatcher.AddTask(new CreateDirectory(Path.Combine(_dstPath, fileName)));

                    }
                }
            }
            string GetFileName(string srcName)
            {
                return srcName.Substring(this._srcPath.Length);
            }
            private void WatcherOnCreated(object sender, FileSystemEventArgs e)
            {
                var fileName = GetFileName(e.FullPath);
                if (File.Exists(Path.Combine(_srcPath, e.FullPath)))
                    this._synchWatcher.AddTask(new FileCopyTask(Path.Combine(_srcPath, fileName), Path.Combine(_dstPath, fileName)));
                if (Directory.Exists(Path.Combine(_srcPath, e.FullPath)))
                    Copy(Path.Combine(_srcPath, e.FullPath));
            }

            private void Clear(string path)
            {
                foreach (var file in GetFiles(path))
                {
                    DeleteFile(file);
                }
                foreach (var directory in GetDirectories(path))
                {
                    Clear(directory);

                    DeleteDirectory(directory);
                }
                DeleteDirectory(path);
            }

            private void DeleteFile(string file)
            {
                if (_synchWatcher.Delete)
                    _synchWatcher.AddTask(new DeleteFile(file));
            }

            private void DeleteDirectory(string directory)
            {
                if (_synchWatcher.Delete)
                    _synchWatcher.AddTask(new DeleteDirectory(directory));
            }

            private static string[] GetDirectories(string path)
            {
                while (true)
                {
                    try
                    {
                        if (!Directory.Exists(path))
                            return new string[0];
                        return Directory.GetDirectories(path);
                    }
                    catch (IOException)
                    {

                    }
                }
            }

            private static string[] GetFiles(string path)
            {
                while (true)
                {
                    try
                    {
                        if (!Directory.Exists(path))
                            return new string[0];
                        return Directory.GetFiles(path);
                    }
                    catch (IOException)
                    {

                    }
                }
            }
        }

        internal class CreateDirectory : ITask
        {
            private readonly string _directory;

            public string Name
            {
                get { return this.ToString(); }
            }
            public CreateDirectory(string directory)
            {
                _directory = directory;
            }

            public void Execute()
            {
                Directory.CreateDirectory(_directory);
            }
            public override string ToString()
            {
                return "Create Directory \"" + _directory + "\"";
            }
        }
        List<ITask> _tasks;
        private void AddTask(ITask task)
        {
            lock (_tasks)
            {
                if (!_tasks.Any(x => x.Name == task.Name))
                    _tasks.Add(task);
            }
        }

        private System.Threading.Tasks.Task _worker;
        List<Watcher> _watchers = new List<Watcher>();
        public  async Task<int> Execute()
        {
            this._tasks = new List<ITask>();
            _worker = new Task(Work);
            var srcPaths = this.Source.Split(';');
            var destPaths = this.Destination.Split(';');
            for (int i = 0; i < srcPaths.Length && i < destPaths.Length; i++)
            {
                Watcher watcher = new Watcher(this, srcPaths[i], destPaths[i]);
                watcher.Run();
            }
            _worker.Start();
            _resetEvent.WaitOne();
            return 0;
        }

        private void Work()
        {
            while (true)
            {
                while (this._tasks.Any())
                {
                    var task = _tasks.First();
                    try
                    {
                        task.Execute();
                        lock (_tasks)
                        {
                            _tasks.Remove(task);
                            _logger.InfoLine(task.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnLine("Error: " + task.ToString());
                        _logger.WarnLine(ex.Message);
                        break;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("同步 {0} => {1}", Source, Destination); }
        }

        #endregion

        internal interface ITask
        {
            void Execute();
            string Name { get; }
        }
        internal class FileCopyTask : ITask
        {
            private readonly string _src;
            private readonly string _dst;

            public FileCopyTask(string src, string dst)
            {
                _src = src;
                _dst = dst;
            }

            public void Execute()
            {
                if (File.Exists(_src))
                    File.Copy(_src, _dst, true);
            }

            public string Name
            {
                get { return this.ToString(); }
            }

            public override string ToString()
            {
                return string.Format("Copy \"{0}\" => \"{1}\"", _src, _dst);
            }
        }
        internal class DeleteFile : SynchWatcher.ITask
        {
            public string Name
            {
                get { return this.ToString(); }
            }
            private readonly string _file;

            public DeleteFile(string file)
            {
                _file = file;
            }

            public void Execute()
            {
                File.Delete(_file);
            }
            public override string ToString()
            {
                return string.Format("Delete File \"{0}\"", _file);
            }
        }


        internal class RenameTask : SynchWatcher.ITask
        {
            private readonly string _src;
            private readonly string _dst;

            public RenameTask(string src, string dst)
            {
                _src = src;
                _dst = dst;
            }

            public void Execute()
            {
                if (Directory.Exists(_src))
                    Directory.Move(_src, _dst);
                if (File.Exists(_src))
                    File.Move(_src, _dst);
            }

            public string Name
            {
                get { return this.ToString(); }
            }
            public override string ToString()
            {
                return string.Format("Rename \"{0}\" => \"{1}\"", _src, _dst);
            }
        }
        private class DeleteDirectory : ITask
        {
            public string Name
            {
                get { return this.ToString(); }
            }
            private readonly string _directory;

            public DeleteDirectory(string directory)
            {
                _directory = directory;
            }

            public void Execute()
            {
                Directory.Delete(_directory);
            }
            public override string ToString()
            {
                return string.Format("Delete Directory \"{0}\"", _directory);
            }
        }
    }
}