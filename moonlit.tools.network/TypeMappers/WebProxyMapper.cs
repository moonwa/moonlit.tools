using System;
using System.Globalization;
using System.Net;
using Moonlit.TypeMappers;

namespace Moonlit.Tools.NetworkExtends.TypeMappers
{
    public class WebProxyMapper : IStringMapper
    {
        public object CovertTo(string o, Type targetType, CultureInfo culture)
        {
            try
            {
                if (targetType == typeof(WebProxy))
                    return new WebProxy(o);
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        } 
    }
}
