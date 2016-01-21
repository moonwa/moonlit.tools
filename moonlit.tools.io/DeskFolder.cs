#region using...

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using Moonlit.Configuration.Ini;
using Moonlit.Configuration.Registry;
using Moonlit.IO.Extensions;
using Moonlit.Text;

#endregion

namespace Moonlit.Tools.IOExtends
{
    [CommandUsage("targetDirectory")]
    [Version(0, 1, Feature = "实现基本功能")]
    public class DeskFolder : ICommand
    {
        #region SaveIniMode enum

        public enum SaveIniMode
        {
            IniFile, ClsID
        }

        #endregion

        private readonly ILogger _logger;

        [ImportingConstructor]
        public DeskFolder(ILogger logger)
        {
            this._logger = logger;
        }


        [Target(Name = "目标文件夹")]
        private string Target { get; set; }

        [Parameter(Description = "递归所有子目录")]
        public bool Recursion { get; set; }

        [Parameter("icon", Description = "指定图标文件, 如果文件是 .lst 文件, 则随机取出文件中的文件名", Required = false)]
        public string IconFile { get; set; }

        [Parameter(Description = "储存方式")]
        public SaveIniMode Mode { get; set; }

        [Parameter("iconi", Description = "图标索引")]
        public int IconIndex { get; set; }

        [Parameter(Description = "提示信息")]
        public string ToolTip { get; set; }

        [Parameter(Description = "设置背景图片, 如果文件是 .lst 文件, 则随机取出文件中的文件名")]
        public string BackgroundImage { get; set; }

        [Parameter(Description = "字体颜色")]
        public Color FontColor { get; set; }

        #region ICommand Members

        public int Execute()
        {
            string target = Target;
            WriteIni(target);
            if (Recursion)
            {
                new DirectoryInfo(target).Foreach(di =>
                                                      {
                                                          _logger.InfoLine(string.Format("set {0}", di.FullName));
                                                          WriteIni(di.FullName);
                                                      }, null, DirError);
            }
            return 0;
        }

        #endregion

        private void DirError(DirectoryInfo di, Exception ex)
        {
            _logger.WarnLine(string.Format("访问文件夹 {0} 失败: {1}", di.FullName, ex.Message));
        }

        private void WriteIni(string target)
        {
            try
            {
                DesktopIni ini = CreateDesktopIni();
                if (!target.EndsWith("desktop.ini", StringComparison.InvariantCultureIgnoreCase))
                {
                    target += "\\desktop.ini";
                }
                ini.Load(target);
                if (IconFile != null)
                {
                    ini.Icon.IconFile = GetBackgroundImage(IconFile, Path.GetDirectoryName(target));
                    ini.Icon.IconIndex = IconIndex;
                }
                if (ToolTip != null)
                {
                    ini.TipInfo = ToolTip;
                }
                if (FontColor != Color.Empty)
                {
                    ini.Appearance.FontColor = FontColor;
                }
                if (BackgroundImage != null)
                {
                    ini.Appearance.BackgroundImage = GetBackgroundImage(BackgroundImage, Path.GetDirectoryName(target));
                }
                ini.Save();
            }
            catch (Exception ex)
            {
                _logger.WarnLine(string.Format("处理 {0} 失败: {1}", target, ex.Message));
            }
        }

        private string GetBackgroundImage(string fileName, string targetDir)
        {
            if (fileName.EndsWith(".lst", StringComparison.CurrentCultureIgnoreCase))
            {
                string dir = Path.GetDirectoryName(fileName);
                var filenames = new List<string>(File.ReadAllLines(fileName));
                filenames.Trim();

                RandomNumberGenerator generator = RandomNumberGenerator.Create();
                var bytes = new byte[4];
                generator.GetBytes(bytes);
                int v = (bytes[0] << 24 | bytes[1] << 16 | bytes[2] << 8 | bytes[3]) & 0x7fff;
                v = v % filenames.Count;
                fileName = filenames[v];
            }

            return fileName;
        }

        private DesktopIni CreateDesktopIni()
        {
            switch (Mode)
            {
                case SaveIniMode.IniFile:
                    return new IniFileDesktopIni();
                case SaveIniMode.ClsID:
                    return new RegDesktopIni();
                default:
                    return null;
            }
        }
    }
}