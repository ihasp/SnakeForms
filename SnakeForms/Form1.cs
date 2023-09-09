using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SnakeForms
{
    public partial class Snake : Form
    {
        private const int GridSize = 20;
        private const int Columns = 50;
        private const int Rows = 25;
        private int score = 0;
        private int dx = 0;
        private int dy = 0;
        private int front = 0;
        private int back = 0;

        private readonly Piece[] snake = new Piece[Columns * Rows];
        private readonly List<int> available = new List<int>();
        private readonly bool[,] visit = new bool[Rows, Columns];
        private readonly Random rand = new Random();
        private readonly Timer timer = new Timer();

        public Snake()
        {
            InitializeComponent();
            InitializeGame();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            timer.Interval = 100;
            timer.Tick += Move;
            timer.Start();
        }

        private void Snake_KeyDown_1(object sender, KeyEventArgs e)
        {
            dx = dy = 0;
            switch (e.KeyCode)
            {
                case Keys.Right:
                    dx = GridSize;
                    break;
                case Keys.Left:
                    dx = -GridSize;
                    break;
                case Keys.Up:
                    dy = -GridSize;
                    break;
                case Keys.Down:
                    dy = GridSize;
                    break;
            }
        }

        private void Move(object sender, EventArgs e)
        {
            int x = snake[front].Location.X;
            int y = snake[front].Location.Y;

            if (dx == 0 && dy == 0)
            {
                return;
            }

            if (Gameover(x + dx, y + dy))
            {
                EndGame();
                return;
            }

            if (CollisionFood(x + dx, y + dy))
            {
                HandleFoodCollision(x + dx, y + dy);
            }
            else
            {
                HandleRegularMove(x + dx, y + dy);
            }
        }

        private void HandleFoodCollision(int x, int y)
        {
            score += 1;
            lblScore.Text = "Wynik: " + score.ToString();

            if (Hits(y / GridSize, x / GridSize))
            {
                return;
            }

            Piece head = new Piece(x, y);
            front = (front - 1 + Columns * Rows) % (Columns * Rows);
            snake[front] = head;
            visit[y / GridSize, x / GridSize] = true;
            Controls.Add(head);
            RandomFood();
        }

        private void HandleRegularMove(int x, int y)
        {
            if (Hits(y / GridSize, x / GridSize)) return;
            visit[snake[back].Location.Y / GridSize, snake[back].Location.X / GridSize] = false;
            front = (front - 1 + Columns * Rows) % (Columns * Rows);
            snake[front] = snake[back];
            snake[front].Location = new Point(x, y);
            back = (back - 1 + Columns * Rows) % (Columns * Rows);
            visit[y / GridSize, x / GridSize] = true;
        }

        private void RandomFood()
        {
            available.Clear();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    if (!visit[i, j]) available.Add(i * Columns + j);
                }
            }

            int idx = rand.Next(available.Count) % available.Count;
            lblFood.Left = (available[idx] % Columns) * GridSize;
            lblFood.Top = (available[idx] / Columns) * GridSize;
        }

        private bool Hits(int x, int y)
        {
            if (visit[x, y])
            {
                EndGame();
                return true;
            }
            return false;
        }

        private bool CollisionFood(int x, int y)
        {
            return x == lblFood.Location.X && y == lblFood.Location.Y;
        }

        private bool Gameover(int x, int y)
        {
            return x < 0 || y < 0 || x >= Columns * GridSize || y >= Rows * GridSize;
        }

        private void EndGame()
        {
            timer.Stop();
            MessageBox.Show("Koniec Gry");
        }

        private void InitializeGame()
        {
            Piece head = new Piece((rand.Next() % Columns) * GridSize, (rand.Next() % Rows) * GridSize);
            lblFood.Location = new Point((rand.Next() % Columns) * GridSize, (rand.Next() % Rows) * GridSize);

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    visit[i, j] = false;
                    available.Add(i * Columns + j);
                }
            }

            visit[head.Location.Y / GridSize, head.Location.X / GridSize] = true;
            available.Remove(head.Location.Y / GridSize * Columns + head.Location.X / GridSize);
            Controls.Add(head);
            snake[front] = head;
        }
    }
}
