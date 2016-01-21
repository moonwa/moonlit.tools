using System;
using Moonlit.Media;

namespace Moonlit.Tools.MediaExtends
{
    /// <summary>
    /// 命令 : Subtitle
    /// </summary>
    [Function("Delay")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Delay")]
    [Command("Delay")]
    public class Delay : ICommand
    {
        [Target(Name = "FileName")]
        public string FileName { get; set; }

        [Parameter(Description = "DelayTime")]
        public TimeSpan DelayTime { get; set; }
        [Parameter(Description = "Type")]
        public SubtitleType Type { get; set; }

        #region ICommand Members

        public int Execute()
        {
            ISubtitle subtitle = SubtitleHelpers.GetSubtitle(this.Type);

            RunDelay(subtitle, FileName);
            return 0;
        }

        #endregion

        private void RunDelay(ISubtitle subtitle, string filename)
        {
            TimeSpan delayValue = DelayTime;
            SubtitleCollection subs = subtitle.Build(filename);
            foreach (SubtitleItem title in subs)
            {
                title.Start = title.Start.Add(delayValue);
                title.End = title.End.Add(delayValue);
            }
            subtitle.Write(filename + ".delay", subs);
        }
    }
}