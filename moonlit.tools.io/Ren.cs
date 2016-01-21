using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moonlit.Tools.IOExtends
{ 
    [Command("ren", "批量文件改名")]
    internal sealed class Ren : ICommand
    {
        private readonly ExportProvider _container;

        [ImportingConstructor]
        public Ren(ExportProvider container)
        {
            _container = container;
        }

        private Regex _pattern;

        [Parameter("Recursive", Description = "是否递归所有子目录", Prefixs = "s")]
        public bool IsRecursive { get; set; }

        [Parameter(Description = "文件名规则", Required = false)]
        public string FileNamePattern { get; set; }

        private Regex Pattern
        {
            get
            {
                if (_pattern == null)
                {
                    _pattern = new Regex(FileNamePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                return _pattern;
            }
        }


        [Parameter(Description = "根目录", Required = true)]
        public DirectoryInfo Root { get; set; }

        [Parameter(Description = "是否对目录重命名")]
        public bool IsRenDirectory { get; set; }

        [Parameter("extend", Description = "包含扩展名")]
        public bool IsExtendName { get; set; }

        [Parameter(Description = "替换文件名", Prefixs = "o", Required = true)]
        public string Replace { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            RunDirectory(Root, IsRecursive);
            return 0;
        }

        #endregion

        private void RunDirectory(DirectoryInfo source, bool isIncursive)
        {
            RenHelper ren = null;
            if (IsRenDirectory)
            {
                var directoryRenHelper  = _container.GetExportedValue<DirectoryRenHelper>();
                directoryRenHelper.Source = source;
                directoryRenHelper.Destination = Replace;
                directoryRenHelper.Pattern = Pattern;
                ren = directoryRenHelper;
            }
            else
            {
                var fileRenHelper = _container.GetExportedValue<FileRenHelper>();
                fileRenHelper.Source = source;
                fileRenHelper.Destination = Replace;
                fileRenHelper.Pattern = Pattern;
                fileRenHelper.IncludeExtendName = IsExtendName;
                ren = fileRenHelper;
            }
            ren.Run();

            if (isIncursive)
            {
                source.Refresh();
                foreach (DirectoryInfo dir in source.GetDirectories())
                {
                    RunDirectory(dir, true);
                }
            }
        }
    }

    internal abstract class RenHelper
    {
        private readonly ILogger _logger;

        protected RenHelper(ILogger logger)
        {
            _logger = logger;
        }

        private const string Partten_SerializeNumber = "$SEARIALIZENUMBER$";
        public DirectoryInfo Source { get; set; }
        public string Destination { get; set; }
        public Regex Pattern { get; set; }

        public void Run()
        {
            List<FileSystemInfo> fsinfos = (from dir in GetSourceFileSystemInfos(Source)
                                            where Pattern.Match(GetName(dir)).Success
                                            select dir).ToList();

            if (fsinfos.Count() == 0)
            {
                return;
            }
            int width = Convert.ToInt32(Math.Ceiling(Math.Log10(fsinfos.Count())));

            for (int idx = 0; idx < fsinfos.Count; idx++)
            {
                FileSystemInfo file = fsinfos[idx];
                string destFileName = string.Empty;
                destFileName = Source.FullName + "\\" + Pattern.Replace(GetName(file), Destination);
                destFileName = destFileName.Replace(Partten_SerializeNumber, idx.ToString().PadLeft(width, '0'));
                destFileName = OnCompletedName(file, destFileName);
                if (string.Compare(destFileName, file.FullName, true) == 0)
                {
                    continue;
                }
                _logger.InfoLine(file.Name + " => " + destFileName);
                Move(file, destFileName);
            }
        }

        protected abstract List<FileSystemInfo> GetSourceFileSystemInfos(DirectoryInfo directoryInfo);
        protected abstract string GetName(FileSystemInfo fsinfo);
        protected abstract void Move(FileSystemInfo fsinfo, string destName);
        protected abstract string OnCompletedName(FileSystemInfo fsinfo, string destName);
    }
    [Export]
    internal class DirectoryRenHelper : RenHelper
    {
        [ImportingConstructor]
        public DirectoryRenHelper(ILogger logger)
            : base(logger)
        {
        }

        protected override List<FileSystemInfo> GetSourceFileSystemInfos(DirectoryInfo directoryInfo)
        {
            return directoryInfo.GetDirectories().ToList<FileSystemInfo>();
        }

        protected override string GetName(FileSystemInfo fsinfo)
        {
            return fsinfo.Name;
        }

        protected override void Move(FileSystemInfo fsinfo, string destName)
        {
            ((DirectoryInfo) fsinfo).MoveTo(destName);
        }

        protected override string OnCompletedName(FileSystemInfo fsinfo, string destName)
        {
            return destName;
        }
    }
    [Export]
    internal class FileRenHelper : RenHelper
    {
        [ImportingConstructor]
        public FileRenHelper(ILogger logger)
            : base(logger)
        {
        }

        public bool IncludeExtendName { get; set; }

        protected override List<FileSystemInfo> GetSourceFileSystemInfos(DirectoryInfo directoryInfo)
        {
            return directoryInfo.GetFiles().ToList<FileSystemInfo>();
        }

        protected override string GetName(FileSystemInfo fsinfo)
        {
            if (IncludeExtendName)
            {
                return fsinfo.Name;
            }
            else
            {
                return Path.GetFileNameWithoutExtension(fsinfo.Name);
            }
        }

        protected override void Move(FileSystemInfo fsinfo, string destName)
        {
            ((FileInfo) fsinfo).MoveTo(destName);
        }

        protected override string OnCompletedName(FileSystemInfo fsinfo, string destName)
        {
            if (!IncludeExtendName)
            {
                return destName + Path.GetExtension(fsinfo.Name);
            }
            return destName;
        }
    }
}