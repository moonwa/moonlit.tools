using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Moonlit.Tools.TextExtends
{
    [Command("Xml2Csv", "将XML转换为 csv 文件")]
    internal class Xml2Csv : ICommand
    {
        private readonly ILogger _logger ;

        [ImportingConstructor]
        public Xml2Csv(ILogger logger)
        {
            this._logger = logger;
        }

        private Encoding Encode
        {
            get { return Encoding.GetEncoding(EncodingName); }
        }

        [Parameter(Description = "要转换的文件名", Required = true)]
        public string FileName { get; set; }

        [Parameter(Description = "编码格式", Required = true)]
        public string EncodingName { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            using (var reader = new StreamReader(FileName, Encode))
            {
                XmlReader xmlReader = new XmlTextReader(reader);
                xmlReader.ReadStartElement();
                while (xmlReader.Read())
                {
                    var xdoc = new XmlDocument();
                    string content = xmlReader.ReadOuterXml();
                    if (string.IsNullOrEmpty(content))
                    {
                        break;
                    }
                    xdoc.LoadXml(content);
                    for (int i = 0; i < xdoc.DocumentElement.ChildNodes.Count; i++)
                    {
                        var xele = xdoc.DocumentElement.ChildNodes[i] as XmlElement;
                        if (i != 0)
                        {
                            _logger.InfoLine(",");
                        }
                        _logger.InfoLine(xele.InnerText);
                    }
                    _logger.InfoLine("");
                }
                xmlReader.Close();
                return 0;
            }
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("正在将 xml 文件 {0} 转换为 cvs 文件", FileName); }
        }

        #endregion
    }
}