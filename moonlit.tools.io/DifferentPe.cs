using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Moonlit.Tools.IOExtends
{
    [CommandUsage("-s 快照")]
    [Function("比较目录下所有 dll 在版本一致的情况下是否有修改")]
    [Command("diffpe")]
    [Version(0, 1)]
    internal sealed class DifferentPe : ICommand
    {
        private readonly ExportProvider _container;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public DifferentPe(ExportProvider container, ILogger logger)
        {
            _container = container;
            _logger = logger;
        }

        [Target(0, Name = "根目录", Required = true)]
        public string Root { get; set; }
        [Parameter("snapshot", Description = "快照", Prefixs = "s", Required = false)]
        public bool Issnapshot { get; set; }
        #region ICommand Members

        public int Execute()
        {
            var snapshotConfig = Path.Combine(Root, "diffpe.config");
            if (this.Issnapshot)
            {
                XElement ele = new XElement("snapshot");
                CreateSnapshot(Root, ele);
                ele.Save(snapshotConfig);
                _logger.Info("create snapshot success");
            }
            else
            {
                if (!File.Exists(snapshotConfig))
                {
                    _logger.WarnLine("please create snapshot [-s] first");
                    return -1;
                }
                ComparerSnapshot(Root, XElement.Load(snapshotConfig));
                _logger.Info("compare snapshot success");
            }
            return 0;
        }

        private void ComparerSnapshot(string parent, XElement snapshot)
        {
            foreach (var fi in Directory.GetFiles(parent, "*.dll", SearchOption.AllDirectories).Union(Directory.GetFiles(parent, "*.exe", SearchOption.AllDirectories)))
            {
                string fullName = Path.GetFullPath(fi);
                try
                {
                    var bytes = File.ReadAllBytes(fullName);
                    var assembly = Assembly.Load(bytes);

                    foreach (var xfile in snapshot.Elements("file"))
                    {
                        if ((string)xfile.Attribute("version") == assembly.GetName().Version.ToString()
                            && string.Equals((string)xfile.Attribute("fullname"), fullName, StringComparison.OrdinalIgnoreCase))
                        {
                            bytes[0x88] = 0;
                            bytes[0x89] = 0;
                            MD5 md5 = MD5.Create();
                            var md5Value = Convert.ToBase64String(md5.ComputeHash(bytes));
                            if ((string)xfile.Attribute("md5") != md5Value)
                            {
                                _logger.WarnLine(string.Format("File Changed:  {0}", fullName));
                            }
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        #endregion

        private void CreateSnapshot(string parent, XElement snapshot)
        {
            foreach (var fi in Directory.GetFiles(parent, "*.dll", SearchOption.AllDirectories).Union(Directory.GetFiles(parent, "*.exe", SearchOption.AllDirectories)))
            {
                string fullName = Path.GetFullPath(fi);
                try
                {
                    MethodInfo g;
                    g.GetMethodBody().GetILAsByteArray();
                    var bytes = File.ReadAllBytes(fullName);
                    var assembly = Assembly.Load(bytes);

                    MD5 md5 = MD5.Create();
                    var md5Value = Convert.ToBase64String(md5.ComputeHash(bytes));
                    snapshot.Add(new XElement("file",
                        new XAttribute("version", assembly.GetName().Version.ToString()),
                        new XAttribute("fullname", Path.GetFullPath(fullName)),
                        new XAttribute("md5", md5Value)
                        ));
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }
    }
}