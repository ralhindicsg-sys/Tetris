namespace Tetris
{
    internal class PlayerManager
    {
        public static string? CurrentPlayer { get; private set; }

        public static void NewPlayer()
        {
            while (true)
            {
                Console.Clear();

                int width = Console.WindowWidth;
                int height = Console.WindowHeight;

                string title = "=== CREATE NEW PLAYER ===";
                string hint = "(Press ESC to go back)";
                string inputLabel = "Name: ";

                int centerY = height / 2;


                Console.ForegroundColor = (ConsoleColor)(DateTime.Now.Millisecond / 100 % 16);
                Console.SetCursorPosition((width - title.Length) / 2, centerY - 3);
                Console.Write(title);

               
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.SetCursorPosition((width - hint.Length) / 2, centerY - 1);
                Console.Write(hint);

                Console.ForegroundColor = ConsoleColor.White;
                int inputX = (width - 20) / 2;
                Console.SetCursorPosition(inputX, centerY + 1);
                Console.Write(inputLabel);

                Console.SetCursorPosition(inputX + inputLabel.Length, centerY + 1);

                string name = "";

                while (true)
                {
                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                    {
                        CurrentPlayer = null;
                        Console.Clear();
                        return;
                    }

                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            CurrentPlayer = name;
                            File.AppendAllText("players.txt", name + "\n");
                            return;
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace && name.Length > 0)
                    {
                        name = name.Substring(0, name.Length - 1);
                    }
                    else if (!char.IsControl(key.KeyChar))
                    {
                        name += key.KeyChar;
                    }

                    Console.SetCursorPosition(inputX + inputLabel.Length, centerY + 1);
                    Console.Write(new string(' ', 20));
                    Console.SetCursorPosition(inputX + inputLabel.Length, centerY + 1);
                    Console.Write(name);
                }
            }
        }

        public static void ContinuePlayer()
        {
            Console.Clear();

            if (!File.Exists("players.txt"))
            {
                Console.WriteLine("No players found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                CurrentPlayer = null;
                return;
            }

            string[] players = File.ReadAllLines("players.txt")
                                   .Where(p => !string.IsNullOrWhiteSpace(p))
                                   .ToArray();

            
            if (players.Length == 0)
            {
                Console.WriteLine("No players found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey(true);
                CurrentPlayer = null;
                return;
            }

            Console.WriteLine("Select Player (ESC to go back):\n");

            for (int i = 0; i < players.Length; i++)
                Console.WriteLine($"{i + 1}. {players[i]}");

            Console.Write("\nChoice: ");

            string input = "";

            while (true)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    CurrentPlayer = null;
                    Console.Clear();
                    return;
                    
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    if (int.TryParse(input, out int choice) &&
                        choice >= 1 && choice <= players.Length)
                    {
                        CurrentPlayer = players[choice - 1];
                        return;
                    }

                    input = "";
                    Console.Write("\nInvalid choice. Try again: ");
                    continue;
                }

                if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b");
                    continue;
                }

                if (char.IsDigit(key.KeyChar))
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }
        }

        public static void SaveScore(int score)
        {
            string path = "scores.txt";

            Dictionary<string, int> scores = new Dictionary<string, int>();

            if (File.Exists(path))
            {
                foreach (var line in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var parts = line.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int existingScore))
                    {
                        scores[parts[0]] = existingScore;
                    }
                }
            }

            if (CurrentPlayer != null)
            {
                if (scores.ContainsKey(CurrentPlayer))
                {
                    if (score > scores[CurrentPlayer])
                    {
                        scores[CurrentPlayer] = score;
                    }
                }
                else
                {
                    scores[CurrentPlayer] = score;
                }
            }

            var lines = scores.Select(s => $"{s.Key}:{s.Value}");
            File.WriteAllLines(path, lines);
        }

        public static void ShowScores()
        {
            Console.Clear();

            if (!File.Exists("scores.txt"))
            {
                Console.WriteLine("No scores yet.");
                Console.WriteLine("Press any key...");
                Console.ReadKey(true);
                return;
            }

            var scores = File.ReadAllLines("scores.txt")
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(':'))
                .Where(parts => parts.Length == 2 && int.TryParse(parts[1], out _))
                .Select(parts => new {
                    Name = parts[0],
                    Score = int.Parse(parts[1])
                })
                .OrderByDescending(s => s.Score)
                .ToList();

            if (scores.Count == 0)
            {
                Console.WriteLine("No valid scores found.");
                Console.WriteLine("Press any key...");
                Console.ReadKey(true);
                return;
            }

            Console.WriteLine("=== SCOREBOARD ===\n");

            foreach (var s in scores)
                Console.WriteLine($"{s.Name} - {s.Score}");

            Console.WriteLine("\nPress any key...");
            Console.ReadKey(true);
            Console.Clear();
        }

        public static void ClearPlayers()
        {
            Console.Clear();
            Console.Write("Are you sure you want to delete ALL players? (y/n): ");

            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                File.WriteAllText("players.txt", "");
                Console.WriteLine("\nPlayers cleared.");
            }
            else
            {
                Console.WriteLine("\nCancelled.");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }

        public static void ClearScores()
        {
            Console.Clear();
            Console.Write("Are you sure you want to delete ALL scores? (y/n): ");

            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                File.WriteAllText("scores.txt", "");
                Console.WriteLine("\nScores cleared.");
            }
            else
            {
                Console.WriteLine("\nCancelled.");
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey(true);
        }
    }
}
