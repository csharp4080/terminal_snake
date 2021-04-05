using System;
using term2d;

namespace Term2DGame
{
    class TerminalSnakeGame : Game
    {

        public static void Main(string[] args)
        {
            Term2D.Start(new TerminalSnakeGame());
        }
        override public void Init(Canvas canvas)
        {
            Console.Title = "term2d Example Game";
            canvas.DefaultBackgroundColor = ConsoleColor.Blue;
            canvas.DefaultForegroundColor = ConsoleColor.Red;
            canvas.Clear();
        }

        double timer = 0.0;
        override public bool Update(UpdateInfo updateInfo)
        {
            // Update Timer
            timer += updateInfo.DeltaTime;
            // Handle Logic
            if (updateInfo.HasUnreadInput)
            {
                switch (updateInfo.LastInput)
                {
                    case ConsoleKey.UpArrow:
                        // something
                        break;
                    case ConsoleKey.DownArrow:
                        // something
                        break;
                    case ConsoleKey.LeftArrow:
                        // something
                        break;
                    case ConsoleKey.RightArrow:
                        // something
                        break;
                    case ConsoleKey.Escape:
                        return false;
                    default:
                        break;
                }
            }
            // Update View
            Canvas canvas = updateInfo.ActiveCanvas;
            // clear & draw frame on canvas
            return true;
        }
    }
}