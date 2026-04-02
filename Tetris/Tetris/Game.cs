using System.Diagnostics;

namespace Tetris
{
    internal class Game
    {
        Board? board;
        Tetromino? current;
        int x, y;
        int score = 0;
        int level = 1;
        int speed = 400;
        bool paused = false;
        int moveCooldown = 50;
        int rotateCooldown = 150;
        int lastMove = 0;
        int lastRotate = 0;

        List<(int x, int y, int size, int speed, int counter, Tetromino t)> bgBlocks = new();
        Random rand = new Random();

        public void Run()
        {
            MainMenu();
        }

        void MainMenu()
        {
            InitBackground();

            while (true)
            {
                int height = Console.WindowHeight;
                int logoStartY = height / 2 - 10;
                int menuStartY = logoStartY + 6 + 2;
                int menuEndY = menuStartY + 6;

                for (int y = 0; y < Console.WindowHeight; y++)
                {
                    if (y >= logoStartY && y <= menuEndY)
                        continue;

                    Console.SetCursorPosition(0, y);
                    Console.Write(new string(' ', Console.WindowWidth));
                }

                Console.SetCursorPosition(0, 0);

                UpdateBackground();
                DrawBackground();
                DrawMenu();

                if (Console.KeyAvailable)
                {
                    char c = Console.ReadKey(true).KeyChar;

                    switch (c)
                    {
                        case '1':
                            PlayerManager.NewPlayer();
                            if (PlayerManager.CurrentPlayer != null)
                                StartGame();
                            break;

                        case '2':
                            PlayerManager.ContinuePlayer();
                            if (PlayerManager.CurrentPlayer != null)
                                StartGame();
                            break;

                        case '3':
                            PlayerManager.ShowScores();
                            break;

                        case '4':
                            PlayerManager.ClearPlayers();
                            break;

                        case '5':
                            PlayerManager.ClearScores();
                            break;

                        case '6':
                            return;
                    }
                }

                Thread.Sleep(40);
            }
        }

        void DrawMenu()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;

            string[] logo = {
                "████████╗███████╗████████╗██████╗ ██╗███████╗",
                "╚══██╔══╝██╔════╝╚══██╔══╝██╔══██╗██║██╔════╝",
                "   ██║   █████╗     ██║   ██████╔╝██║███████╗",
                "   ██║   ██╔══╝     ██║   ██╔══██╗██║╚════██║",
                "   ██║   ███████╗   ██║   ██║  ██║██║███████║",
                "   ╚═╝   ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚══════╝"
            };

            ConsoleColor[] logoColors = {
                ConsoleColor.Cyan,
                ConsoleColor.Blue,
                ConsoleColor.Magenta,
                ConsoleColor.Red,
                ConsoleColor.Yellow,
                ConsoleColor.Green
            };

            int logoStartY = height / 2 - 10;

            for (int i = 0; i < logo.Length; i++)
            {
                int startX = (width - logo[i].Length) / 2;
                Console.SetCursorPosition(startX, logoStartY + i);
                Console.ForegroundColor = logoColors[i % logoColors.Length];
                Console.Write(logo[i]);
            }

            string[] menu = {
                "1. New Player",
                "2. Continue Player",
                "3. Scoreboard",
                "4. Clear Players",
                "5. Clear Scores",
                "6. Exit"
            };

            int menuStartY = logoStartY + logo.Length + 2;

            for (int i = 0; i < menu.Length; i++)
            {
                int startX = (width - menu[i].Length) / 2;
                Console.SetCursorPosition(startX, menuStartY + i);

                Console.ForegroundColor = menu[i].Contains("Exit")
                    ? ConsoleColor.Red
                    : ConsoleColor.White;

                Console.Write(menu[i]);
            }

            Console.ResetColor();
        }

        void InitBackground()
        {
            bgBlocks.Clear();

            for (int i = 0; i < 25; i++)
            {
                int x = rand.Next(0, Console.WindowWidth - 6);
                int y = rand.Next(-60, 0);

                bgBlocks.Add((x, y, 1, rand.Next(1, 6), 0, Tetromino.Random()));
            }
        }

        void UpdateBackground()
        {
            for (int i = 0; i < bgBlocks.Count; i++)
            {
                var b = bgBlocks[i];

                b.counter++;

                if (b.counter >= b.speed)
                {
                    b.y++;
                    b.counter = 0;
                }

                b.counter++;
                if (rand.Next(20) == 0)
                {
                    b.y++;
                }
                b.x = Math.Clamp(b.x, 0, Console.WindowWidth - 12);

                if (b.y > Console.WindowHeight)
                {
                    b.x = rand.Next(0, Console.WindowWidth - 12);
                    b.y = rand.Next(-60, -10);
                    b.t = Tetromino.Random();
                    b.speed = rand.Next(1, 4);
                    b.counter = 0;
                }

                bgBlocks[i] = b; 
            }
        }

        void DrawBackground()
        {

            foreach (var b in bgBlocks)
            {
                Console.ForegroundColor = b.t.Color;

                for (int y = 0; y < b.t.H; y++)
                {
                    for (int x = 0; x < b.t.W; x++)
                    {
                        if (b.t.Shape[y, x] == 1)
                        {
                            int drawX = b.x + x * 3;
                            int drawY = b.y + y;

                            int height = Console.WindowHeight;
                            int logoStartY = height / 2 - 10;
                            int menuStartY = logoStartY + 6 + 2;
                            int menuEndY = menuStartY + 6;

                            if (drawY >= 0 && drawY < Console.WindowHeight &&
                                (drawY < logoStartY || drawY > menuEndY))
                            {
                                Console.SetCursorPosition(drawX, drawY);
                                Console.Write("[#]");
                            }
                        }
                    }
                }
            }

            Console.ResetColor();
        }

        void StartGame()
        {
            Console.Clear();
            board = new Board();
            score = 0;
            level = 1;
            speed = 400;
            paused = false;

            Spawn();
            Loop();
        }

        void Loop()
        {
            Stopwatch sw = Stopwatch.StartNew();
            int lastFall = 0;
            int fallInterval = speed;

            Console.CursorVisible = false;

            while (true)
            {
                HandleInput();

                if (!paused && sw.ElapsedMilliseconds - lastFall >= fallInterval)
                {
                    y++;
                    if (board.Collision(current, x, y))
                    {
                        y--;
                        board.Lock(current, x, y);
                        int cleared = board.ClearLines();
                        score += cleared * 100;
                        level = score / 500 + 1;
                        fallInterval = Math.Max(100, 400 - level * 40);
                        Spawn();
                        if (board.Collision(current, x, y))
                            break;
                    }
                    lastFall = (int)sw.ElapsedMilliseconds;
                }

                Console.SetCursorPosition(0, 0);
                board.Draw(current, x, y, score, level, paused);

                Thread.Sleep(10);
            }

            PlayerManager.SaveScore(score);
            MainMenu();
        }

        void HandleInput()
        {
            long now = Environment.TickCount;

            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                if ((key == ConsoleKey.A || key == ConsoleKey.D) && now - lastMove >= moveCooldown)
                {
                    if (key == ConsoleKey.A) x--;
                    if (key == ConsoleKey.D) x++;

                    if (board.Collision(current, x, y))
                    {
                        if (key == ConsoleKey.A) x++;
                        if (key == ConsoleKey.D) x--;
                    }

                    lastMove = (int)now;
                }

                if (key == ConsoleKey.W && now - lastRotate >= rotateCooldown)
                {
                    current.Rotate();
                    if (board.Collision(current, x, y))
                    {
                        if (!TryWallKick()) current.Rotate();
                    }
                    lastRotate = (int)now;
                }

                if (key == ConsoleKey.S)
                {
                    y++;
                    if (board.Collision(current, x, y)) y--;
                }

                if (key == ConsoleKey.P) paused = !paused;
                if (key == ConsoleKey.Escape) {
                     PlayerManager.SaveScore(score);
                     Console.Clear();
                     MainMenu();
                 }
            }
        }

        bool TryWallKick()
        {
            int[] kicks = { -1, 1, -2, 2 };
            foreach (int k in kicks)
            {
                x += k;
                if (!board.Collision(current, x, y)) return true;
                x -= k;
            }
            return false;
        }

        void Spawn()
        {
            current = Tetromino.Random();
            x = Board.Width / 2 - 1;
            y = 0;
        }
    }
}
