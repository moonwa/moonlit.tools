using System;
using System.IO;
using System.Threading.Tasks;

namespace Moonlit.Tools.IOExtends
{ 
    [Command("filetime", "change file time")]
    public class FileTime : ICommand
    {
        #region TimePartTypes enum

        public enum TimePartTypes
        {
           LastAccess,
         Create,
        LastWrite
        }

        #endregion

        [Target(0, Name = "目标")]
        public string TargetName { get; set; }

        [Target(1, Name = "时间")]
        public DateTime Time { get; set; }

        [Parameter("TimePart")]
        public TimePartTypes TimePart { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            switch (TimePart)
            {
                case TimePartTypes.LastAccess:
                    File.SetLastAccessTime(TargetName, Time);
                    break;
                case TimePartTypes.Create:
                    File.SetCreationTime(TargetName, Time);
                    break;
                case TimePartTypes.LastWrite:
                    File.SetLastWriteTime(TargetName, Time);
                    break;
                default:
                    throw new UsageErrorException("修改时间部分输入错误");
            }
            return 0;
        }

        #endregion
    }
}