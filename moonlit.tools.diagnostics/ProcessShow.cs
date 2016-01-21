using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading.Tasks;
using Moonlit.Diagnostics;

namespace Moonlit.Tools.DiagnosticsExtends
{
    /// <summary>
    /// 命令 : ProcessShow
    /// </summary>
    [Command("ps", "show the processes")]
    internal class ProcessShow : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public ProcessShow(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<int> Execute()
        {
            foreach (Process process in Process.GetProcesses())
            {
                _logger.InfoLine(string.Format("{0,7}{1,20}{2,7}", process.Id, process.ProcessName, process.IsManaged()));
            }
            return 0;
        }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion
    }

    [Description("performance counter category")]
    public enum PerformanceCounterOptions
    {
        [Description("delete performance counter category")]
        Delete
    }
}
