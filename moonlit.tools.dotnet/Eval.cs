using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Moonlit.Reflection.Extentions;

namespace Moonlit.Tools.ReflectionExtends
{
    [Command("eval", "计算输入字符串")]
    public class Eval : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Eval(ILogger logger)
        {
            _logger = logger;
        }

        [Target(Name = "计算输入字符串")]
        public string Code { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            _logger.InfoLine(string.Format("{0}", Code.Eval()));
            return 0;
        }

        #endregion
    }
}