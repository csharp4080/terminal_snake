using System;
using System.Collections.Generic;
using term2d;

namespace Term2DGame
{
    class TerminalSnakeGame : Game
    {
        public static void Main(string[] args)
        {
            Console.Title = "Terminal Snake";
            Term2D.Start(new TerminalSnakeGame());
        }

        /// <summary>
        ///     Represents a single point in the game,
        ///     with coordinates relative to the
        ///     top left corner of the gameplay area.
        /// </summary>
        struct Point
        {
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }

            public int X { get; set; }
            public int Y { get; set; }
        }

        /// <summary>
        ///     Represents directions the snake
        ///     can move in.
        /// </summary>
        enum SnakeDirection
        {
            UP, DOWN, LEFT, RIGHT
        }

        // Constants
        const double INITIAL_SPEED_INTERVAL = 0.1; // initially how many seconds between each movement
        const double MIN_SPEED_INTERVAL = 0.02; // the cap for how fast the interval can become
        const double MAX_SPEED_SCORE = 750; // what score represents the fastest motion interval

        // Overall State Information
        bool running = true;
        bool showFPSCounter = false;
        Random rand = new Random();

        // Gameplay State Information
        List<Point> snake;
        Point apple;
        int score;
        double timer;
        bool alive;
        bool paused;
        Canvas canvas;

        override public void Init(Canvas canvas)
        {
            // Save Canvas For Easy Future Reference
            this.canvas = canvas;
            // Reset Snake At Center Point
            snake = new List<Point>();
            Point center = new Point(canvas.GetWidth()/2, canvas.GetHeight()/2);
            snake.Add(center);
            // Place Apple @ Random Valid Point
            replaceApple();
            // Reset Score, Movement Timer
            score = 0;
            timer = 0.0;
            // Reset Game State
            alive = true;
            paused = false;
            running = true;
            // Prepare Canvas
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

        public void replaceApple()
        {
            int appleX;
            int appleY;
            bool collision = false;
            do
            {
                appleX = rand.Next() % (canvas.GetWidth() - 1);
                appleY = rand.Next() % (canvas.GetHeight() - 1);

                for(int i = 0; i < snake.Count-1; i++)
                {
                    if(appleX == snake[i].X && appleY == snake[i].Y)
                    {    
                        collision = true;
                        break;
                    }

                    else
                        collision = false;
                }
            } while (appleX == 0 || appleY == 0 || collision);
            apple = new Point(appleX, appleY);
        }

        SnakeDirection queuedDirection = SnakeDirection.UP;
        SnakeDirection currentDirection = SnakeDirection.UP;

        override public bool Update(UpdateInfo updateInfo)
        {
            // Update View
            Canvas canvas = updateInfo.ActiveCanvas;

            // Update Timer
            if (!paused && alive)
            {
                timer += updateInfo.DeltaTime;
            }

            // Handle Logic
            int x = 0;
            int y = 0;

            // Calculate Interval Between Each Snake Movement
            double movementInterval = Math.Max(INITIAL_SPEED_INTERVAL - ((double) score / MAX_SPEED_SCORE) * INITIAL_SPEED_INTERVAL, MIN_SPEED_INTERVAL);
            // Compensate For Blocks Having Larger Height Than Width
            if (currentDirection == SnakeDirection.LEFT || currentDirection == SnakeDirection.RIGHT)
            {
                movementInterval /= 1.25;
            }
            if(timer >= movementInterval)
            {
                switch(queuedDirection)
                {
                    case SnakeDirection.UP:
                        x = snake[0].X;
                        y = snake[0].Y - 1;
                        break;
                    case SnakeDirection.DOWN:
                        x = snake[0].X;
                        y = snake[0].Y + 1;
                        break;
                    case SnakeDirection.LEFT:
                        x = snake[0].X - 1;
                        y = snake[0].Y;
                        break;
                    case SnakeDirection.RIGHT:
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
                    replaceApple();
                }
                else
                {
                    snake.RemoveAt(snake.Count - 1);
                }
                timer = 0.0;
            }

            if(snake[0].X < 1 || snake[0].X == canvas.GetWidth() - 1)
                alive = false;

            if(snake[0].Y < 1 || snake[0].Y == canvas.GetHeight() - 1)
                alive = false;

            for(int i = 1; i < snake.Count; i++)
            {
                if(snake[0].X == snake[i].X && snake[0].Y == snake[i].Y)
                {
                    alive = false;
                    break;
                }
            }

            // Render Frame
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
            
            // Draw Snake
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

            // Draw Apple
            canvas.Draw(apple.Y, apple.X, '■', ConsoleColor.Red, canvas.DefaultBackgroundColor);
            
            // Draw Status Indicators
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
            if (!alive)
            {
                canvas.DrawText(canvas.GetHeight() / 2 - 2, canvas.GetWidth() / 2 - 6, "** snek ded **", ConsoleColor.White, ConsoleColor.Red);
                canvas.DrawText(canvas.GetHeight() / 2, canvas.GetWidth() / 2 - 13, "ESC: Exit  SPACE: New Game");
            }
            return running;
        }

        public override void OnKeyEvent(ConsoleKeyInfo keyInfo)
        {
            // Handle Console Key Inputs
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    if(currentDirection != SnakeDirection.DOWN)
                        queuedDirection = SnakeDirection.UP;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if(currentDirection != SnakeDirection.UP)
                        queuedDirection = SnakeDirection.DOWN;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if(currentDirection != SnakeDirection.RIGHT)
                        queuedDirection = SnakeDirection.LEFT;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if(currentDirection != SnakeDirection.LEFT)
                        queuedDirection = SnakeDirection.RIGHT;
                    break;
                case ConsoleKey.P:
                    paused = !paused;
                    break;
                case ConsoleKey.F:
                    showFPSCounter = !showFPSCounter;
                    break;
                case ConsoleKey.Spacebar:
                    if (!alive)
                    {
                        Init(canvas);
                    }
                    break;
                case ConsoleKey.Escape:
                    running = false;
                    break;
                default:
                    break;
            }
        }
    }
}