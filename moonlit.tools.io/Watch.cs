#region using...

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moonlit.Windows;

#endregion

namespace Moonlit.Tools.IOExtends
{
    /// <summary>
    /// Watch for command
    /// </summary> 
    [Command("watch", "定时循环运行程序, 并显示其内容")]
    public class Watch : ICommand
    {
        private readonly ILogger _logger;

        /// <summary>
        /// 最后的输出内容
        /// </summary>
        private string _lastOutputBuffer;

        private ProcessStartInfo _psInfo;

        [Parameter("difference", Description = "只在输出更新时刷新屏幕")]
        public bool IsDifference { get; set; }

        [Parameter("interval", Description = "时间间隔 (00:00:00)", Required = true)]
        public TimeSpan Interval { get; set; }

        [Parameter("noTitle", Description = "屏蔽头信息", Prefixs = "t")]
        public bool IsNoTitle { get; set; }

        [Target(Name = "命令")]
        public string Command { get; set; }

        [Target(Name = "目标命令集合")]
        public List<string> Targets { get; set; }

        [ImportingConstructor]
        public Watch(ILogger logger)
        {
            _logger = logger;
        }

        #region ICommand Members

        public  async Task<int> Execute()
        {
            InitByShell();
            string arguments = GetCmdArgsString(Targets);

            try
            {
                Run(arguments);
            }
            catch (Exception ex)
            {
                _logger.WarnLine(ex.Message);
            }
            return 0;
        }

        #endregion
         
        /// <summary>
        /// 获取命令参数的字符串表达形式
        /// </summary>
        /// <param name="cmds"></param>
        /// <returns></returns>
        private static string GetCmdArgsString(List<string> cmds)
        {
            var cmdArgs = new string[cmds.Count - 1];
            cmds.CopyTo(1, cmdArgs, 0, cmds.Count - 1);
            return string.Join(" ", cmdArgs);
        }

        private void InitByShell()
        {
            if (IsCmdShell(Command))
            {
                Targets = Targets;
                Targets.Insert(0, "/c");
                Targets.Insert(0, "cmd");
            }
        }

        private bool IsCmdShell(string command)
        {
            return false;
        }

        private string BuildTitle()
        {
            string cmd = _psInfo.FileName;
            string args = _psInfo.Arguments;

            if (cmd.ToUpper() == "CMD")
            {
                string[] oldArgsList = args.Split(new char[' ']);
                cmd = oldArgsList[0];
                var newArgsList = new string[oldArgsList.Length - 1];
                Array.Copy(oldArgsList, 1, newArgsList, 0, newArgsList.Length);
                args = string.Join(" ", newArgsList);
            }
            return string.Format("时间间隔:\t{0}秒/次\t\t\t{1}\r\n命令行:\t{2} {3}",
                                 (Interval.TotalMilliseconds / 1000.0),
                                 DateTime.Now.ToString("记录时间:\tHH时mm分ss秒"),
                                 cmd,
                                 args);
        }

        public void Run(string args)
        {
            _psInfo = new ProcessStartInfo(Command);
            _psInfo.RedirectStandardError = true;
            _psInfo.RedirectStandardOutput = true;
            _psInfo.UseShellExecute = false;
            _psInfo.Arguments = args;
            ExecuteCommand(_psInfo);
        }

        private void ExecuteCommand(ProcessStartInfo psInfo)
        {
            var tempBuffer = new char[1024];
            while (true)
            {
                var outputBuffer = new StringBuilder();
                Process process = Process.Start(psInfo);
                do
                {
                    Debug.Assert(process != null);

                    int readCnt = process.StandardOutput.Read(tempBuffer, 0, 1024);
                    if (readCnt <= 0) break;
                    outputBuffer.Append(tempBuffer, 0, readCnt);
                } while (true);

                process.WaitForExit();
                if (process.ExitCode != 0)
                    break;

                string newOut = outputBuffer.ToString();
                if (IsDifference)
                {
                    if (string.Compare(_lastOutputBuffer, newOut) != 0)
                    {
                        _lastOutputBuffer = newOut;
                        Dump(_lastOutputBuffer);
                    }
                }
                else
                    Dump(newOut);
                Thread.Sleep(Interval);
            }
        }

        private void Dump(string msg)
        {
            _logger.Clear();
            if (!IsNoTitle)
                _logger.InfoLine(BuildTitle());
            _logger.InfoLine(msg);
        }
    }
}