using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris
{
    internal class Tetromino
    {
        public int[,] Shape { get; private set; }
        public ConsoleColor Color { get; private set; }
        public int H => Shape.GetLength(0);
        public int W => Shape.GetLength(1);

        static Random rand = new Random();

        static List<Tetromino> pieces = new List<Tetromino>
     {
     new Tetromino(new int[,]{{1,1,1,1}}, ConsoleColor.Cyan),
     new Tetromino(new int[,]{{1,1},{1,1}}, ConsoleColor.Yellow),
     new Tetromino(new int[,]{{0,1,0},{1,1,1}}, ConsoleColor.Magenta),
     new Tetromino(new int[,]{{1,0,0},{1,1,1}}, ConsoleColor.Green),
     new Tetromino(new int[,]{{0,0,1},{1,1,1}}, ConsoleColor.Blue),
     new Tetromino(new int[,]{{0,1,1},{1,1,0}}, ConsoleColor.Red),
     new Tetromino(new int[,]{{1,1,0},{0,1,1}}, ConsoleColor.DarkYellow),
 };

        Tetromino(int[,] s, ConsoleColor c)
        {
            Shape = s;
            Color = c;
        }

        public static Tetromino Random()
        {
            Tetromino t = pieces[rand.Next(pieces.Count)];
            return new Tetromino((int[,])t.Shape.Clone(), t.Color);
        }

        public void Rotate()
        {
            int[,] r = new int[W, H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                    r[x, H - y - 1] = Shape[y, x];
            Shape = r;
        }
    }
}
