using System;
using System.Collections.Generic;
using System.Text;

namespace Tetris
{
    internal class Board
    {
        public const int Width = 10;
        public const int Height = 25;

        int[,] grid = new int[Height, Width];
        ConsoleColor[,] colors = new ConsoleColor[Height, Width];

        public bool Collision(Tetromino t, int px, int py)
        {
            for (int y = 0; y < t.H; y++)
                for (int x = 0; x < t.W; x++)
                    if (t.Shape[y, x] == 1)
                    {
                        int fx = px + x;
                        int fy = py + y;
                        if (fx < 0 || fx >= Width || fy >= Height) return true;
                        if (fy >= 0 && grid[fy, fx] == 1) return true;
                    }
            return false;
        }

        public void Lock(Tetromino t, int px, int py)
        {
            for (int y = 0; y < t.H; y++)
                for (int x = 0; x < t.W; x++)
                    if (t.Shape[y, x] == 1)
                    {
                        grid[py + y, px + x] = 1;
                        colors[py + y, px + x] = t.Color;
                    }
        }

        public int ClearLines()
        {
            int cleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < Width; x++)
                    if (grid[y, x] == 0) full = false;

                if (full)
                {
                    for (int yy = y; yy > 0; yy--)
                        for (int x = 0; x < Width; x++)
                        {
                            grid[yy, x] = grid[yy - 1, x];
                            colors[yy, x] = colors[yy - 1, x];
                        }
                    cleared++;
                    y++;
                }
            }
            return cleared;
        }

        public void Draw(Tetromino t, int px, int py, int score, int level, bool paused)
        {

            int consoleWidth = Console.WindowWidth;
            int consoleHeight = Console.WindowHeight;

            int boardWidth = Width * 3;
            int boardHeight = Height + 2;

            int offsetX = (consoleWidth - boardWidth) / 2;
            int offsetY = (consoleHeight - boardHeight) / 2;
            Console.SetCursorPosition(offsetX, offsetY);
            Console.WriteLine($"Player: {PlayerManager.CurrentPlayer}  Score: {score}  Level: {level} {(paused ? "[PAUSED]" : "             ")}");

            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(offsetX, offsetY + 1 + y);

                for (int x = 0; x < Width; x++)
                {
                    bool piece = false;

                    for (int ty = 0; ty < t.H; ty++)
                        for (int tx = 0; tx < t.W; tx++)
                            if (t.Shape[ty, tx] == 1 && py + ty == y && px + tx == x)
                            {
                                Console.ForegroundColor = t.Color;
                                Console.Write("[#]");
                                piece = true;
                            }

                    if (!piece)
                    {
                        if (grid[y, x] == 1)
                        {
                            Console.ForegroundColor = colors[y, x];
                            Console.Write("[#]");
                        }
                        else
                        {
                            Console.ResetColor();
                            Console.Write(" . ");
                        }
                    }
                }
            }

            Console.ResetColor();
        }
    }
}
