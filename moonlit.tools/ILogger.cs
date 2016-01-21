using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;

namespace Moonlit.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILogger
    {

        /// <summary>
        /// 显示一行警告
        /// </summary>
        /// <param name="message">显示的消息.</param>
        void WarnLine(string message);

        /// <summary>
        /// 显示一行消息
        /// </summary>
        /// <param name="message">显示的消息.</param>
        void InfoLine(string message, ConsoleColor? color);

        /// <summary>
        /// 循环显示某消息
        /// </summary>
        /// <param name="message">显示的消息.</param>
        void InfoLoop(string message, ConsoleColor? color);

        void Clear();
    }
    public static class LoggerHelper
    {
        public static void InfoLine(this ILogger logger, string message)
        {
            logger.InfoLine(message, null);
        }
        public static void InfoLoop(this ILogger logger, string message)
        {
            logger.InfoLoop(message, null);
        }
    }
}