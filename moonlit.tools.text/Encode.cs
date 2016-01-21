using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moonlit.Text;

namespace Moonlit.Tools.TextExtends
{
    [Command("Encode", "对文件或标准输入进行编码")]
    public class Encode : ICommand
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public Encode(ILogger logger)
        {
            _logger = logger;
            Filter = "*";
        }

        private Encoding CurrentEncoding
        {
            get { return Encoding.GetEncoding(EncodingName); }
        }


        [Target(Name = "内容", Required = true)]
        public string Content { get; set; }

        #region ICommand Members

        public async Task<int> Execute()
        {
            try
            {
                switch (WorkType)
                {
                    case WorkTypes.List:
                        ListAllEncodings();
                        return 0;
                    case WorkTypes.Test:
                    case WorkTypes.Encode:
                    default:
                        break;
                }
                DoEncoding();
            }
            catch (IOException ioex)
            {
                _logger.WarnLine("文件打开失败:" + ioex.Message);
                return -1;
            }
            return 0;
        }

        #endregion


        private void ListAllEncodings()
        {
            _logger.InfoLine("您当前支持的编码方式有:");
            _logger.InfoLine(string.Format("{0,35}{1,28}", "编码名称", "名称"));
            EncodingInfo[] encodings = Encoding.GetEncodings();
            foreach (EncodingInfo info in encodings)
            {
                _logger.InfoLine(string.Format("{0,40}{1,30}", info.DisplayName, info.Name));
            }
        }


        private void EncodeSubDirectory(string dirName)
        {
            string[] files = Directory.GetFiles(dirName, Filter ?? "*");
            foreach (string file in files)
            {
                DoEncodingFile(file);
            }
            foreach (string dir in Directory.GetDirectories(dirName))
            {
                EncodeSubDirectory(dir);
            }
        }

        private void DoEncoding()
        {
            string srcFileName = Content;
            if (Directory.Exists(srcFileName))
            {
                EncodeSubDirectory(srcFileName);
            }
            else
            {
                DoEncodingFile(srcFileName);
            }
        }

        private void DoEncodingFile(string srcFileName)
        {
            switch (WorkType)
            {
                case WorkTypes.Test:
                    TestFile(srcFileName);
                    break;
                case WorkTypes.Encode:
                    EncodeFile(srcFileName);
                    break;
                default:
                    break;
            }
        }

        private void TestFile(string srcFileName)
        {
            _logger.InfoLine(string.Format("[{0}] {1}", GetEncoding(srcFileName).HeaderName,
                                                               srcFileName));
        }
        private Encoding GetEncoding(string file)
        {
            using (var fs = File.Open(file, FileMode.Open))
            {
                using (var streamReader = new StreamReader(fs, Encoding.Default, true))
                {
                    var content = streamReader.ReadToEnd();
                    return streamReader.CurrentEncoding;
                }
            }
        }
        private void EncodeFile(string srcFileName)
        {
            if (GetEncoding(srcFileName).HeaderName != EncodingName)
            {
                Console.WriteLine(srcFileName);
                var text = File.ReadAllText(srcFileName, GetEncoding(srcFileName));
                File.WriteAllText(srcFileName, text, CurrentEncoding);
            }
        }

        #region Nested type: WorkType

        public enum WorkTypes
        {
            List,
            Test,
            Encode
        }

        #endregion

        [Parameter(Description = "编码方式")]
        public string EncodingName { get; set; }
        [Parameter(Description = "工作方式", Required = true)]
        public WorkTypes WorkType { get; set; }
        [Parameter(Description = "过滤器", Required = false)]
        public string Filter { get; set; }
    }
}