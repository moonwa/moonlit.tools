using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Moonlit.Tools.NetworkExtends
{
    [Function("下载")]
    [CommandUsage("url output ")]
    public class Download : ICommand
    {
        private readonly ILogger _logger;
        private const string Patten = @"(?<url>.+)\s*==>\s*(?<local>.+)";

        [ImportingConstructor]
        public Download(ILogger logger)
        {
            _logger = logger;
        }

        [Parameter(Description = @"下载地址", Prefixs = "s")]
        public string Source { get; set; }

        [Parameter(Description = @"下载地址", Prefixs = "s")]
        public string Output { get; set; }
         

        #region ICommand Members

        public int Execute()
        {
            try
            { 
                DoDownload(Source, Output);
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return -1;
            }
        }

        #endregion

        private void DoDownload(string source, string output)
        {
            WebRequest req = WebRequest.Create(source);
            WebResponse rsp = req.GetResponse();
            Stream stream = rsp.GetResponseStream();

            string path = Path.GetDirectoryName(output);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (var fs = new FileStream(output, FileMode.Create))
            {
                int readCount = -1;
                var buffer = new byte[1024*64];
                while (readCount != 0)
                {
                    readCount = stream.Read(buffer, 0, buffer.Length);
                    fs.Write(buffer, 0, readCount);
                }

                fs.Flush();
            }
            _logger.InfoLine(source + " ==> " + output);
        }
    }
}