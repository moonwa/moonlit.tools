using System.ComponentModel.Composition;
using Moonlit.StringMappers;
using Moonlit.Tools.NetworkExtends.TypeMappers;
using Moonlit.TypeMappers;

namespace Moonlit.Tools.Network
{
    [Module]
    public class Module : IModule
    {
        private readonly ComposeStringMapper mapper;
        [ImportingConstructor]
        public Module(IStringMapper stringMapper)
        {
            mapper = stringMapper as ComposeStringMapper;
            
        }
        public void Init()
        {
            if (mapper != null)
            {
                mapper.Register(new WebProxyMapper());
            }
        }
    }
}
