using System;
using System.ComponentModel.Composition;
using System.IO;
using Moonlit.Media.Mpeg;

namespace Moonlit.Tools.MediaExtends
{
    [Function("获取或设置 mpeg 文件信息")]
    [Version(0, 1, Feature = "获取 mpeg 文件信息")]
    [Version(0, 1, 5, Feature = "设置 mpeg 文件信息")]
    [CommandUsage("-p pattern targets")]
    [Command("mpeginfo")]
    public class MgepInfo : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public MgepInfo(ILogger logger)
        {
            _logger = logger;
        }

        [Target(Name = "目标")]
        public string MusicFile { get; set; }

        [Parameter(Description = "专辑")]
        public string Album { get; set; }

        [Parameter(Description = "标题")]
        public string Title { get; set; }

        [Parameter(Description = "艺术家")]
        public string Article { get; set; }

        #region ICommand Members

        public int Execute()
        {
            if (File.Exists(MusicFile))
            {
                Out(MusicFile);
            }
            return 0;
        }

        #endregion

        private void Out(string target)
        {
            MpegInfo mpegInfo = null;
            using (var fs = new FileStream(target, FileMode.Open, FileAccess.ReadWrite))
            {
                try
                {
                    mpegInfo = MpegInfo.Load(fs);
                }
                catch (Exception ex)
                {
                    _logger.WarnLine(string.Format("加载文件 {0} 出错 {1}", Path.GetFileName(target), ex.Message));
                    return;
                }
            }
            if (Article != null)
            {
                mpegInfo.ID3V1.Artist = Article;
                mpegInfo.ID3V2.Artist = Article;
            }
            if (Album != null)
            {
                mpegInfo.ID3V1.Album = Album;
                mpegInfo.ID3V2.Album = Album;
            }
            if (Title != null)
            {
                mpegInfo.ID3V1.Title = Title;
                mpegInfo.ID3V2.Title = Title;
            }
            if (Title != null || Album != null || Title != null)
            {
                using (var fs = new FileStream(target, FileMode.Open, FileAccess.ReadWrite))
                {
                    try
                    {
                        mpegInfo.Save(fs);
                        fs.Flush();
                    }
                    catch (Exception ex)
                    {
                        _logger.WarnLine(string.Format("加载文件 {0} 出错 {1}", Path.GetFileName(target), ex.Message));
                        return;
                    }
                }
            }
        }
    }
}