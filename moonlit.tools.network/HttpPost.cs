using System;
using System.Net;
using System.Text;
using System.Threading;
using Moonlit.Net;
using Moonlit.Net.Web;

namespace Moonlit.Tools.NetworkExtends
{
    [Function("执行 http post 方法")]
    [CommandUsage("uri")]
    [Command("httppost")]
    public class HttpPost : ICommand
    {
        public HttpPost()
        {
            Encoding = Encoding.UTF8;
        }

        [Target(Name = "url", Required = true)]
        public Uri Uri { get; set; }
        [Target(Index = 1, Name = "post", Required = true)]
        public string PostData { get; set; }
        [Parameter(Description = "cookie")]
        public string Cookie { get; set; }
        [Parameter(Description = "encoding")]
        public Encoding Encoding { get; set; }
        [Parameter(Description = "contentType")]
        public string ContentType { get; set; }

        private ManualResetEvent _event;

        public int Execute()
        {
            _event = new ManualResetEvent(false);
            var request = new WebClient();
            if (!string.IsNullOrWhiteSpace(ContentType))
                request.Headers.Add(HttpRequestHeader.ContentType, ContentType);
            WebContainer container = new WebContainer();
            container.Encoding = Encoding;
            var cookies = CookieHelper.GetCookies(Cookie);
            foreach (var cookie in cookies)
            {
                cookie.Domain = Uri.Host;
                container.CookieContainer.Add(cookie);
            }
            var buffer = System.Text.Encoding.UTF8.GetBytes(this.PostData);
            var bytes = request.UploadData(Uri, "POST", buffer);
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(bytes));
            return 0;
        }
    }
}
