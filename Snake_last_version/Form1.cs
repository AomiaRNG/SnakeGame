using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Snake
{
    public enum Direction
    {
        Right,
        Left,
        Down,
        Up,
        None
    }

    struct Snake_element
    {
        public Point cube_pos;
        public int width;
        public int height;
        public Direction direction;
    }
    public partial class Form1 : Form
    {
        private const int width = 20;
        private const int height = 20;
        private Point correct_cube;
        private Direction correct_direction;
        private int width_apple = 0;
        private int height_apple = 0;
        private Point apple_location = new Point(0,0);        
        private Image apple = Image.FromFile(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +  @"\photo\apple.jpg");

        

        private Point cube_pos = new Point();
        private int apple_eaten_count = 0;
        List<Snake_element> Snake_list = new List<Snake_element>();
        public Form1()
        {
            
            Snake_element first = new Snake_element();
            correct_cube = new Point(width / 2, height / 2);
            first.cube_pos = correct_cube;
            first.height = 0;
            first.width = 0;
            Snake_list.Add(first);
            InitializeComponent();
            Form1_Resize(null, null);
            Create_food();

        }

        public void key(object sender, KeyEventArgs e)
        {            
            Direction now = Direction_by_key(e.KeyCode);
            if (((int)now + (int)correct_direction) % (int)Direction.None != 1)
            {
                Snake_element a = Snake_list[0];
                a.direction = now;
                Snake_list[0] = a;                 
            }
        }

        public Direction Direction_by_key(Keys KeyCode)
        {
            Direction dr_in_past = Snake_list[0].direction;
            
            Direction direction_local = Direction.None;
            switch (KeyCode)
            {
                case Keys.Right:
                    direction_local = Direction.Right;
                    break;
                case Keys.Left:
                    direction_local = Direction.Left;
                    break;
                case Keys.Down:
                    direction_local = Direction.Down;
                    break;
                case Keys.Up:
                    direction_local = Direction.Up;
                    break;
                default:
                    direction_local = dr_in_past;
                    break;
            }
            return direction_local;
        }

        private void Create_food()
        {
            Random rand = new Random();
            bool canCreate = false;
            int x = 0, y = 0;            
            while (!canCreate)
            {                
                x = rand.Next(0, width);
                y = rand.Next(0, height);
                canCreate = true;
                for (int i = 0; i < Snake_list.Count; i++)
                {
                    if (Snake_list[i].cube_pos.X == x && Snake_list[i].cube_pos.Y == y)
                        canCreate = false;
                }                
            }
            apple_location.X = x;
            apple_location.Y = y;
        }

        private void End_game()
        {
            if (Snake_list[0].cube_pos.X > width - 1 || Snake_list[0].cube_pos.X < 0 || Snake_list[0].cube_pos.Y > height - 1 || Snake_list[0].cube_pos.Y < 0)
                Close();
            if (Snake_list.Count >= 5)
            for(int i = 0; i < Snake_list.Count; i++)
            {
                for(int j = i + 1; j < Snake_list.Count; j++)
                {
                    if (Snake_list[i].cube_pos == Snake_list[j].cube_pos)
                        Close();
                }
            }
        }

        private void UpdateSnakeLocation()
        {
            End_game();
            Snake.Location = new Point(correct_cube.X * pictureBox1.Width / width + 1, correct_cube.Y * pictureBox1.Height / height + 1);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;            
            var pen = new Pen(Color.Black);
            int x, y;
            for (int i = 1; i < width; i++)
            {
                x = pictureBox1.Width * i / width;
                g.DrawLine(pen, x, 0, x, pictureBox1.Height);
            }
            for (int i = 1; i < height; i++)
            {
                y = pictureBox1.Height * i / height;
                g.DrawLine(pen, 0, y, pictureBox1.Width, y);
            }

            g.DrawImage(apple, apple_location.X * pictureBox1.Width / width + 1, apple_location.Y * pictureBox1.Height / height + 1, width_apple - 1, height_apple - 1);
            
            for (int i = 0; i < Snake_list.Count; i++)
            {
                Brush brush = Brushes.Blue;
                Brush brush2 = Brushes.Green;
                Snake_element a = Snake_list[i];
                a.width = pictureBox1.Width * (Snake_list[i].cube_pos.X + 1) / width - pictureBox1.Width * Snake_list[i].cube_pos.X / width - 1;
                a.height = pictureBox1.Height * (Snake_list[i].cube_pos.Y + 1) / height - pictureBox1.Height * Snake_list[i].cube_pos.Y / height - 1;
                cube_pos.X = (Snake_list[i].cube_pos.X) * pictureBox1.Width / width + 1;
                cube_pos.Y = (Snake_list[i].cube_pos.Y) * pictureBox1.Height / height + 1;
                g.FillRectangle(i == 0 ? brush : brush2, cube_pos.X, cube_pos.Y, a.width, a.height);
                width_apple = pictureBox1.Width * (apple_location.X + 1) / width - pictureBox1.Width * apple_location.X / width;
                height_apple = pictureBox1.Height * (apple_location.Y + 1) / height - pictureBox1.Height * apple_location.Y / height;
                Snake_list[i] = a;
                
                if (Snake_list.Count - i - 1 != 0)
                {
                    a = Snake_list[Snake_list.Count - i - 1];
                    a.direction = Snake_list[Snake_list.Count - i - 2].direction;
                    Snake_list[Snake_list.Count - i - 1] = a;                    
                }
            }
            
            
            apple_eaten();
            label1.Text = "Счет: " + apple_eaten_count;
        }

        private void apple_eaten()
        {                         
            if (apple_location == Snake_list[0].cube_pos)
            {                
                Create_food();
                apple_eaten_count++;
                Create_new_snake();
            }
        }

        private void Create_new_snake()
        {            
            Snake_element a = new Snake_element();
            a.direction = Snake_list[apple_eaten_count - 1].direction;
            switch (a.direction)
            {
                case Direction.Right:
                    a.cube_pos.X = Snake_list[apple_eaten_count - 1].cube_pos.X - 1;
                    a.cube_pos.Y = Snake_list[apple_eaten_count - 1].cube_pos.Y;
                    break;
                case Direction.Left:
                    a.cube_pos.X = Snake_list[apple_eaten_count - 1].cube_pos.X + 1;
                    a.cube_pos.Y = Snake_list[apple_eaten_count - 1].cube_pos.Y;
                    break;
                case Direction.Down:
                    a.cube_pos.X = Snake_list[apple_eaten_count - 1].cube_pos.X;
                    a.cube_pos.Y = Snake_list[apple_eaten_count - 1].cube_pos.Y - 1;
                    break;
                case Direction.Up:
                    a.cube_pos.X = Snake_list[apple_eaten_count - 1].cube_pos.X;
                    a.cube_pos.Y = Snake_list[apple_eaten_count - 1].cube_pos.Y + 1;
                    break;                    
            }
            a.width = pictureBox1.Width * (a.cube_pos.X + 1) / width - pictureBox1.Width * a.cube_pos.X / width;
            a.height = pictureBox1.Height * (a.cube_pos.Y + 1) / height - pictureBox1.Height * a.cube_pos.Y / height;
            Snake_list.Add(a);                       
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();        
            UpdateSnakeLocation();            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Snake_list.Count; i++)
            {
                Snake_element a = Snake_list[i];
                switch (Snake_list[i].direction)
                {
                    case Direction.Right:
                        a.cube_pos.X++;
                        break;
                    case Direction.Left:
                        a.cube_pos.X--;
                        break;
                    case Direction.Down:
                        a.cube_pos.Y++;
                        break;
                    case Direction.Up:
                        a.cube_pos.Y--;
                        break;
                }
                Snake_list[i] = a;
            }
            correct_direction = Snake_list[0].direction;
            UpdateSnakeLocation();            
        }
        
    }
}
