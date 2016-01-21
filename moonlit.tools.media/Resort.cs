using System;
using Moonlit.Media;

namespace Moonlit.Tools.MediaExtends
{

    /// <summary>
    /// 命令 : Subtitle
    /// </summary>
    [Function("Resort")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Resort")]
    [Command("Resort")]
    public class Resort : ICommand
    {
        [Target(Name = "FileName")]
        public string FileName { get; set; }

        [Parameter(Description = "DelayTime")]
        public int StartIndex { get; set; }
        [Parameter(Description = "Type")]
        public SubtitleType Type { get; set; }

        #region ICommand Members

        public int Execute()
        {
            ISubtitle subtitle = SubtitleHelpers.GetSubtitle(this.Type);
            ResortSubtitle(subtitle, FileName);
            return 0;
        }

        #endregion

        private void ResortSubtitle(ISubtitle subtitle, string filename)
        {
            SubtitleCollection subs = subtitle.Build(filename);
            int startIndex = StartIndex;
            foreach (SubtitleItem title in subs)
            {
                title.Order = startIndex++;
            }
            subtitle.Write(filename + ".delay", subs);
        }
    }
}