#region using...

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Moonlit.Tools.IOExtends
{
    [Command("Tee", "Copy standard input to each FILE, and also to standard output.")]
    public class Tee : ICommand
    {
        [Target(0, Name = "文件名")]
        private string Filename { get; set; }

        [Parameter(Description = "追加到文件尾")]
        public bool Appened { get; set; }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            FileMode mode = Appened ? FileMode.OpenOrCreate : FileMode.Create;
            using (var fwrite = new FileStream(Filename, mode, FileAccess.Write, FileShare.Read))
            {
                DumpContent(fwrite);
            }

            return 0;
        }

        #endregion

        public void DumpContent(FileStream fwrite)
        {
            var writer = new StreamWriter(fwrite, Encoding.UTF8);
            fwrite.Seek(0, SeekOrigin.End);
            var buf = new char[1024];
            while (true)
            {
                int readSize = Console.In.Read(buf, 0, 1024);
                if (readSize == 0)
                    break;
                writer.Write(buf, 0, readSize);
                Console.Write(buf, 0, readSize);
                writer.Flush();
            }
        }
    }
}