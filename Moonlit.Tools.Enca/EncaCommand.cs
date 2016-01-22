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
        [STAThread]
        public static int Main(string[] args)
        {
            return RunCommand<EncaCommand>(args);
        }

         

        protected override int Run()
        {
            if (string.IsNullOrEmpty(Target))
            {
                throw new UsageErrorException("type target please");
            }
            this.Logger.InfoLine($"File: {Target} is encoding as " + GetEncoding(Target)?.BodyName);
            return 0;
        }
        [Target(Required = true)]
        public string Target { get; set; }
        private Encoding GetEncoding(string file)
        {
            using (var fs = File.Open(file, FileMode.Open))
            {
                using (var streamReader = new StreamReader(fs, Encoding.Default, true))
                {
                    var content = streamReader.Read();
                    return streamReader.CurrentEncoding;
                }
            }
        }
    }
}
