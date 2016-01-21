using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Moonlit.Tools.DiagnosticsExtends
{
    /// <summary>
    /// 命令 : ProcessShow
    /// </summary>
    [Command("pcc", "操作控制性能计数器类别")]
    internal class PerformanceCounterCategoryCommand : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public PerformanceCounterCategoryCommand(ILogger logger)
        {
            _logger = logger;
        }
        [Parameter]
        public PerformanceCounterOptions Options { get; set; }
        [Target]
        public string Category { get; set; }
        public async Task<int> Execute()
        {
            switch (Options)
            {
                case PerformanceCounterOptions.Delete:
                    _logger.InfoLine("delete performance counter category " + this.Category);
                    System.Diagnostics.PerformanceCounterCategory.Delete(this.Category);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
}