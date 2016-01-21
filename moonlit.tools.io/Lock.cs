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
    [Command("lock", "锁定文件")]
    internal class Lock : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Lock(ILogger logger)
        {
            _logger = logger;
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

        [Target(0, Name = "内容或者文件")]
        public string Target { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            if (!File.Exists(Target))
            {
                _logger.InfoLine("not found the file: " + Target);
                return -1;
            }
            _logger.InfoLine("lock the file: " + Target);
            using (var fs = new FileStream(Target, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 1024))
            {
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
            return 0;
        }

        #endregion
    }
}