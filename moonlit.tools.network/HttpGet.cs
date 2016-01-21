using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Moonlit.Tools.NetworkExtends
{
    [Command("httpget", "执行 http get 方法")]
    public class HttpGet : ICommand
    {
        public HttpGet()
        {
            Encoding = Encoding.Default;
        }

        [Target(Name = "url", Required = true)]
        public Uri Uri { get; set; }

        [Parameter(Description = "Cookie")]
        public string Cookie { get; set; }

        [Parameter(Description = "Encoding")]
        public Encoding Encoding { get; set; }

        [Parameter(Description = "Proxy")]
        public WebProxy Proxy { get; set; }

        System.Net.Http.HttpClient _httpClient;
        public  async Task<int> Execute()
        {
            //_httpClient = new HttpClient();
            //_httpClient.GetAsync()
            //CookieContainer container = new CookieContainer();
            //var cookies = CookieHelper.GetCookies(Cookie);
            //foreach (var cookie in cookies)
            //{
            //    cookie.Domain = Uri.Host;
            //    container.Add(cookie);
            //}
            //webClient = new HttpWebClient(container);
            //webClient.Proxy = Proxy;

            //webClient.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            //webClient.Headers.Add(HttpRequestHeader.UserAgent,
            //                      "Mozilla/5.0 (Windows; U; Windows NT 6.1; zh-CN; rv:1.9.2.10) Gecko/20100914 Firefox/3.6.10 ( .NET CLR 3.5.30729; .NET4.0E)");
            //webClient.Encoding = Encoding;
            //webClient.Headers.Add(HttpRequestHeader.Accept,
            //                      "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            //webClient.DownloadStringCompleted += WebClientDownloadStringCompleted;
            //webClient.Headers.Add(HttpRequestHeader.CacheControl, "max-age=0");

            //webClient.DownloadStringAsync(Uri);
            //_event.WaitOne();
             return 0;
        }

        //void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    if (e.Error == null)
        //    {
        //        Host.WriteLine(e.Result);
        //    }
        //    else
        //    {
        //        Host.WriteLine("error: " + e.Error.Message);
        //    }
        //    _event.Set();
        //}
    }
}
