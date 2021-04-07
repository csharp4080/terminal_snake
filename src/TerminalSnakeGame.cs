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

        // Game Information
        List<Point> snake = new List<Point>();
        Point apple;
        double timer = 0.0;
        Random rand = new Random();

        override public void Init(Canvas canvas)
        {
            Point p = new Point(canvas.GetWidth()/2, canvas.GetHeight()/2);
            snake.Add(p);
            replaceApple(canvas);
            Console.Title = "term2d Example Game";
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

        int direction = 1; //1 is up, 2 is down, 3 is left, 4 is right

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
                        if(direction != 2)
                            direction = 1;
                        break;
                    case ConsoleKey.DownArrow:
                        if(direction != 1)
                            direction = 2;
                        break;
                    case ConsoleKey.LeftArrow:
                        if(direction != 4)
                            direction = 3;
                        break;
                    case ConsoleKey.RightArrow:
                        if(direction != 3)
                            direction = 4;
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
            int x = 0;
            int y = 0;
            
            if(appleEaten())
            {
                switch(direction)
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

                snake.Insert(0, new Point(x, y));
                replaceApple(canvas);
            }

            if(timer >= 0.1)
            {
                switch(direction)
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
                timer = 0.0;
                snake.Insert(0, new Point(x, y));
                snake.RemoveAt(snake.Count - 1);
            }

            canvas.Clear();
            char border = '█';

            for(int i = 0; i < canvas.GetHeight(); i++)
            {
                canvas.Draw(i, 0, border);
                canvas.Draw(i, canvas.GetWidth()-1, border);
            }

            for(int i = 0; i < canvas.GetWidth(); i++)
            {
                canvas.Draw(0, i, border);
                canvas.Draw(canvas.GetHeight()-1, i, border);
            }

            if(snake[0].X < 1 || snake[0].X == canvas.GetWidth() - 1)
                return false;

            if(snake[0].Y < 1 || snake[0].Y == canvas.GetHeight() - 1)
                return false;

            for(int i = 1; i < snake.Count - 1; i++)
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
            return true;
        }
    }
}