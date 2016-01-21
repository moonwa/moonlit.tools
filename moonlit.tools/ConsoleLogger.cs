using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Text;
using Moonlit.Windows;

namespace Moonlit.Tools
{ 
    [Export(typeof(ILogger))]
    public sealed class ConsoleLogger : ILogger
    {

        #region ILogger Members


        public void WarnLine(string message)
        {
            WriteLine(message, ConsoleColor.Red);
        }


        public void InfoLine(string message, ConsoleColor? color)
        {
            WriteLine(message, color ?? Console.ForegroundColor);
        }

        public void InfoLoop(string message, ConsoleColor? color)
        {
            var empty = new string(' ', Console.BufferWidth - 1);
            Console.Write("\r" + empty + "\r");

            Write(message);
        }

        public void Clear()
        {
            Console.Clear();
        }
         

        #endregion

        private void Write(string message, ConsoleColor consoleColor)
        {
            ConsoleEx.Write(consoleColor, message);
        }

        private void WriteLine(string message, ConsoleColor consoleColor)
        {
            ConsoleEx.WriteLine(consoleColor, message);
        }

        public void Write(string message)
        {
            Write(message, Console.ForegroundColor);
        } 
    }
}