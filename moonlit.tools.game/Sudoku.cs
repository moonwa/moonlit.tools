using System;
using System.IO;
using System.Text;
using Moonlit.Game.Sudoku;

namespace Moonlit.Tools.MathExtends
{
    /// <summary>
    /// 命令 : Sudoku
    /// </summary>
    [Function("Sudoku")]
    [Version(0, 1, Feature = "实现基本功能")]
    [CommandUsage("Sudoku")]
    [Command("Sudoku")]
    internal class Sudoku : ICommand
    { 
        public int Execute()
        {
            if (!IsEmpty)
                Render();
            else
                RenderEmpty();
            return 0;
        }
        private void RenderEmpty()
        {
            for (int i = 0; i < 9; i++)
            {
                Console.WriteLine("0,0,0,0,0,0,0,0,0");
            }
        }
        [Target]
        public string InputFile { get; set; }
        private void Render()
        {
            int[][] pan = new int[9][];
            int row = 0;
            using (StreamReader reader = new StreamReader(InputFile))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    pan[row] = new int[9];
                    var units = line.Split(',');
                    for (int i = 0; i < 9; i++)
                    {
                        pan[row][i] = Convert.ToInt32(units[i]);
                    }
                    row++;
                }

            }
            var analyzer = new SudokuAssistant(pan);
            var context = analyzer.CreateContext();
            while (analyzer.Next(context))
            {
                if (IsStep)
                    Dump(analyzer.Cells);
            }
            Dump(analyzer.Cells);
        }

        private void Dump(SudokuCell[][] cells)
        {
            foreach (var cellCollection in cells)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var cell in cellCollection)
                {
                    if (stringBuilder.Length != 0)
                        Console.Write(",");
                    Console.Write(cell.ToString());
                }
                Console.WriteLine();
            }
            Console.WriteLine("------------------------------------------------------------------");
        }
         
        [Parameter("step", Description = "display steps")]
        public bool IsStep { get; set; }
        [Parameter("empty", Description = "create empty data")]
        public bool IsEmpty{ get; set; }

        #region ITitleCommand 成员

        public string CommandTitle
        {
            get { return ""; }
        }

        #endregion

      
    }
}
