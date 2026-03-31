
namespace Tetris
    {
    internal class Program
    {
        static void Main(string[] args)
        {
          Console.CursorVisible = false;
            Game game = new Game();
            game.Run();
        }
    }
}