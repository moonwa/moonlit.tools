#region copyright...

// author: zhanzhang

#endregion

#region using...

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

#endregion

namespace Moonlit.Tools.NetworkExtends
{
    [Function("为下载工具提供下载列表")]
    [CommandUsage("url output /d downloadPattern")]
    public class DownloadList : ICommand
    {
        private readonly ILogger _logger;

        [Target(1, Name = "输出文件")]
        public string Url { get; set; }

        [Target(1, Name = "输出文件")]
        public string Output { get; set; }


        [Parameter(Description = "下载表达式, 以逗号分隔", Required = true)]
        public string DownloadPattern { get; set; }

        [Parameter]
        public string ToolName { get; set; }

        [Parameter(Description = "输出表达式", Required = true)]
        public string OutputPattern { get; set; }

        #region ICommand Members

        public int Execute()
        {
            var urlParttens = new List<string>(DownloadPattern.Split(','));

            Download(urlParttens, Url);

            return 0;
        }

        #endregion

        [ImportingConstructor]
        public DownloadList(ILogger logger)
        {
            _logger = logger;
        }

        private void Download(IList<string> urlParttens, string url)
        {
            _logger.Intend();
            string curUrlPartten = urlParttens[0];
            string content = GetContent(url);
            _logger.Info("complete the " + url);
            string path = url.Substring(0, url.LastIndexOf("/")) + "/";
            if (urlParttens.Count == 1)
            {
                int matchCount = 0;
                Match match = Regex.Match(content, curUrlPartten, RegexOptions.IgnoreCase);
                while (match.Success)
                {
                    matchCount++;
                    File.AppendAllText(Output, path + match.Groups["url"].Value + "|" + url + "\r\n");
                    match = match.NextMatch();
                }
                if (matchCount == 0)
                {
                    throw new Exception("fail");
                }
                _logger.InfoLine(" " + matchCount + " time's");
            }
            else
            {
                var newUrlPatterns = new List<string>(urlParttens.Skip(1));

                Match match = Regex.Match(content, curUrlPartten, RegexOptions.IgnoreCase);
                while (match.Success)
                {
                    Download(newUrlPatterns, path + match.Groups["url"].Value);
                    match = match.NextMatch();
                }
            }
            _logger.Deintend();
        }

        private string GetContent(string url)
        {
            var req = WebRequest.Create(url) as HttpWebRequest;
            Debug.Assert(req != null);

            var resp = req.GetResponse() as HttpWebResponse;
            Debug.Assert(resp != null);

            string encoding = resp.ContentEncoding;
            Encoding e = string.IsNullOrEmpty(encoding) ? Encoding.Default : Encoding.GetEncoding(encoding);
            using (Stream stream = resp.GetResponseStream())
            using (var reader = new StreamReader(stream, e))
            {
                return reader.ReadToEnd();
            }
        }

        #region Nested type: ToolsType

        private enum ToolsType
        {
            Thunder
        }

        #endregion
    }
}