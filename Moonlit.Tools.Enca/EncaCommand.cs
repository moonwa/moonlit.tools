using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonlit.Tools.Enca
{
    public class EncaCommand : Command
    {
        protected override int Run()
        {
            this.Logger.InfoLine($"File: {Target} Encoding " + GetEncoding(Target));
            return 0;
        }
        [Target]
        public string Target { get; set; }
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
    }
}
