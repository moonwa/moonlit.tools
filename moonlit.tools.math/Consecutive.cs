using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Moonlit.Arithmetic;

namespace Moonlit.Tools.MathExtends
{
    /// <summary>
    /// 命令 : Consecutive
    /// </summary> 
    [Command("Consecutive", "Consecutive")]
    internal class Consecutive : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Consecutive(ILogger logger)
        {
            _logger = logger;
        }
        [Target(Name = "Value")]
        public int Value { get; set; }
        public async Task<int> Execute()
        {
            var results = new ConsecutiveAnalyzer().Analyze(Value);
            results = results.Reverse();
            if (results.Count() == 0)
            {
                _logger.InfoLine("None");
            }
            else
            {
                foreach (var intse in results)
                {
                    foreach (var i in intse)
                    {
                        _logger.InfoLine(i + " ");
                    }
                    _logger.InfoLine("");
                }
            }
            return 0;
        }
    }
}
