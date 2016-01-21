using System;
using Moonlit.Media;

namespace Moonlit.Tools.MediaExtends
{
    public static class SubtitleHelpers
    {
        public static ISubtitle GetSubtitle(SubtitleType subtitleType)
        {
            switch (subtitleType)
            {
                case SubtitleType.Src:
                    return new SrtSubtitle();
                default:
                    throw new ArgumentOutOfRangeException("subtitleType");
            }
        }
    }
}