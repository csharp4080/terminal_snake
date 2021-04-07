using System;
using System.Collections.Generic;
using term2d;

namespace Term2DGame
{
    class TerminalSnakeGame : Game
    {
        public struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        public static void Main(string[] args)
        {
            Term2D.Start(new TerminalSnakeGame());
        }

        // Constants
        const double INITIAL_SPEED_INTERVAL = 0.1; // initially how many seconds between each movement
        const double MIN_SPEED_INTERVAL = 0.02; // the cap for how fast the interval can become
        const double MAX_SPEED_SCORE = 750; // what score represents the fastest motion interval

        // Game Information
        List<Point> snake = new List<Point>();
        Point apple;
        double timer = 0.0;
        Random rand = new Random();
        int score = 0;
        bool showFPSCounter = false;
        bool alive = true;
        bool paused = false;

        override public void Init(Canvas canvas)
        {
            Point p = new Point(canvas.GetWidth()/2, canvas.GetHeight()/2);
            snake.Add(p);
            replaceApple(canvas);
            Console.Title = "Terminal Snake";
            canvas.DefaultBackgroundColor = ConsoleColor.Black;
            canvas.DefaultForegroundColor = ConsoleColor.DarkGreen;
            canvas.Clear();
        }

        public bool appleEaten()
        {
            if(snake[0].X == apple.X && snake[0].Y == apple.Y)
                return true;
            else
                return false;
        }

        public void replaceApple(Canvas canvas)
        {
            int appleX;
            int appleY;
            do
            {
                appleX = rand.Next() % (canvas.GetWidth() - 1);
                appleY = rand.Next() % (canvas.GetHeight() - 1);
            } while (appleX == 0 || appleY == 0);
            apple = new Point(appleX, appleY);
        }

        int queuedDirection = 1; //1 is up, 2 is down, 3 is left, 4 is right
        int currentDirection = 1;

        override public bool Update(UpdateInfo updateInfo)
        {
            // Update Timer
            if (!paused)
            {
                timer += updateInfo.DeltaTime;
            }
            // Handle Logic
            if (updateInfo.HasUnreadInput)
            {
                switch (updateInfo.LastInput)
                {
                    case ConsoleKey.UpArrow:
                        if(currentDirection != 2)
                            queuedDirection = 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if(currentDirection != 1)
                            queuedDirection = 2;
                        break;
                    case ConsoleKey.LeftArrow:
                        if(currentDirection != 4)
                            queuedDirection = 3;
                        break;
                    case ConsoleKey.RightArrow:
                        if(currentDirection != 3)
                            queuedDirection = 4;
                        break;
                    case ConsoleKey.P:
                        paused = !paused;
                        break;
                    case ConsoleKey.F:
                        showFPSCounter = !showFPSCounter;
                        break;
                    case ConsoleKey.Escape:
                        return false;
                    default:
                        break;
                }
            }
            // Update View
            Canvas canvas = updateInfo.ActiveCanvas;
            
            int x = 0;
            int y = 0;

            double movementInterval = Math.Max(INITIAL_SPEED_INTERVAL - ((double) score / MAX_SPEED_SCORE) * INITIAL_SPEED_INTERVAL, MIN_SPEED_INTERVAL);
            if(timer >= movementInterval)
            {
                switch(queuedDirection)
                {
                    case 1:
                        x = snake[0].X;
                        y = snake[0].Y - 1;
                        break;
                    case 2:
                        x = snake[0].X;
                        y = snake[0].Y + 1;
                        break;
                    case 3:
                        x = snake[0].X - 1;
                        y = snake[0].Y;
                        break;
                    case 4:
                        x = snake[0].X + 1;
                        y = snake[0].Y;
                        break;
                    default:
                        break;
                }
                currentDirection = queuedDirection;
                snake.Insert(0, new Point(x, y));
                if(appleEaten())
                {
                    score += 10;
                    replaceApple(canvas);
                }
                else
                {
                    snake.RemoveAt(snake.Count - 1);
                }
                timer = 0.0;
            }

            canvas.Clear();

            for(int i = 0; i < canvas.GetHeight(); i++)
            {
                canvas.Draw(i, 0, '║', ConsoleColor.White, canvas.DefaultForegroundColor);
                canvas.Draw(i, canvas.GetWidth()-1, '║', ConsoleColor.White, canvas.DefaultForegroundColor);
            }

            for(int i = 0; i < canvas.GetWidth(); i++)
            {
                canvas.Draw(0, i, '═', ConsoleColor.White, canvas.DefaultForegroundColor);
                canvas.Draw(canvas.GetHeight()-1, i, '═', ConsoleColor.White, canvas.DefaultForegroundColor);
            }

            canvas.Draw(0, 0, '╔', ConsoleColor.White, canvas.DefaultForegroundColor);
            canvas.Draw(0, canvas.GetWidth()-1, '╗', ConsoleColor.White, canvas.DefaultForegroundColor);
            canvas.Draw(canvas.GetHeight()-1, 0, '╚', ConsoleColor.White, canvas.DefaultForegroundColor);
            canvas.Draw(canvas.GetHeight()-1, canvas.GetWidth()-1, '╝', ConsoleColor.White, canvas.DefaultForegroundColor);

            if(snake[0].X < 1 || snake[0].X == canvas.GetWidth() - 1)
                return false;

            if(snake[0].Y < 1 || snake[0].Y == canvas.GetHeight() - 1)
                return false;

            for(int i = 1; i < snake.Count; i++)
            {
                if(snake[0].X == snake[i].X && snake[0].Y == snake[i].Y)
                    return false;
            }
            
            for(int i = 0; i < snake.Count; i++)
            {
                // Make Snake Trail Fade Towards Tail
                char snakeChar;
                double snakePosition = (double) i / snake.Count;
                if (snakePosition < 0.4)
                {
                    snakeChar = '█';
                }
                else if (snakePosition < 0.8)
                {
                    snakeChar = '▓';
                }
                else
                {
                    snakeChar = '▒';
                }
                canvas.Draw(snake[i].Y, snake[i].X, snakeChar);
            }
            canvas.Draw(apple.Y, apple.X, '■', ConsoleColor.Red, canvas.DefaultBackgroundColor);
            // Render Status Indicator Text
            canvas.DrawText(0, 2, $" Terminal Snake ══ Score: {score} ", ConsoleColor.White, canvas.DefaultForegroundColor);
            if (showFPSCounter)
            {
                double measuredFPS = 1.0 / updateInfo.DeltaTime;
                canvas.DrawText(canvas.GetHeight() - 1, 2, $" FPS: {measuredFPS:0.0} ", ConsoleColor.White, canvas.DefaultForegroundColor);
            }
            if (paused)
            {
                canvas.DrawText(canvas.GetHeight() / 2, canvas.GetWidth() / 2 - 6, "** PAUSED **", ConsoleColor.Red, ConsoleColor.White);
            }
            return true;
        }
    }
}