using System.IO;
using Moonlit.IO.Compress.Extensions;

namespace Moonlit.Tools.IOExtends
{
    public class Zip : ICommand
    {
        [Target(Name = "压缩的目标", Required = true)]
        public string Input { get; set; }

        #region ICommand Members

        public int Execute()
        {
            if (Directory.Exists(Input))
            {
                var di = new DirectoryInfo(Input);
                di.Zip();
            }
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("正在压缩:{0}", Input); }
        }

        #endregion
    }
}