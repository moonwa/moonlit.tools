using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Moonlit.Tools.TextExtends
{
    [Command("Tb2Xml", "将以\\t分隔的数据转换为Xml")]
    public sealed class Tb2Xml : ICommand
    {
        [Parameter(Description = "XML的字段, 格式为字段名,[e | a]|", Required = true)]
        public string XmlFields { get; set; }

        [Parameter(Description = "输入文件, 文件以\\t分隔", Required = true)]
        public string FileName { get; set; }

        [Parameter(Description = "元素名", Required = true)]
        public string Element { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            var fields = (from field in XmlFields.Split('|')
                          let field2 = field.Split(',')
                          select new {FieldName = field2[0].Trim(), IsElement = field2[1].Trim() == "e"}).ToList();
            ;

            var xdoc = new XmlDocument();
            XmlElement root = xdoc.CreateElement(Element + "s");
            foreach (string s in File.ReadAllLines(FileName, Encoding.Default))
            {
                string[] contents = s.Split('\t');

                XmlElement xele = xdoc.CreateElement(Element);

                for (int i = 0; i < fields.Count; i++)
                {
                    var field = fields[i];
                    XmlNode xnode;
                    if (field.IsElement)
                    {
                        xnode = xdoc.CreateElement(field.FieldName);
                        xele.AppendChild(xnode);
                    }
                    else
                    {
                        xnode = xdoc.CreateAttribute(field.FieldName);
                        xele.Attributes.Append((XmlAttribute) xnode);
                    }
                    if (i < contents.Length)
                    {
                        xnode.InnerText = contents[i];
                    }
                }
                root.AppendChild(xele);
            }

            xdoc.AppendChild(root);
            xdoc.Save(new XmlTextWriter(Console.Out));
            return 0;
        }

        #endregion

        #region ITitleCommand Members

        public string CommandTitle
        {
            get { return string.Format("将文件 {0} 转换为 xml", FileName); }
        }

        #endregion
    }
}