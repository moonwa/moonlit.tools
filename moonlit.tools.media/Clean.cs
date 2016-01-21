using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Moonlit.Media;

namespace Moonlit.Tools.MediaExtends
{
    /// <summary>
    /// 命令 : Subtitle
    /// </summary>
    [Function("Clean")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Clean")]
    [Command("Clean")]
    public class Clean : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor()]
        public Clean(ILogger logger)
        {
            _logger = logger;
        }

        [Target(Name = "FileName")]
        public string FileName { get; set; }

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
            foreach (var sub in subs)
            {
                if(string.IsNullOrEmpty(sub.Content))
                    continue;
                _logger.InfoLine(sub.Content);
            }
        }
    }

}