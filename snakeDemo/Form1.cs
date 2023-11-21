using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging; // add this for JPG Compressor
using System.Media;
using System.IO;
using System.Runtime.CompilerServices;

namespace snakeDemo
{
    public partial class Form1 : Form
    {
        private List <Circle> Snake = new List <Circle> (); 
        private Circle food = new Circle ();

        int maxWidth;
        int maxHeight;

        int score;
        int highscore;

        Random rand = new Random ();

        bool goLeft, goRight, goDown, goUp;
        public Form1()
        {
            InitializeComponent();

            new Setting();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           SoundPlayer sound = new SoundPlayer ("Music.wav");
           sound.PlayLooping ();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Setting.directions != "Right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Setting.directions != "Left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Setting.directions != "Down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Setting.directions != "Up")
            {
                goDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            ReStartGame();

            SoundPlayer sound = new SoundPlayer("Music.wav");
            sound.Stop();
        }

        private void SnapShot(object sender, EventArgs e)
        {
            Label Caption = new Label();
            Caption.Text = "I scored: " + score + " And my highscore is: " + highscore + " on the snake game from Phan Anh Khoa";
            Caption.Font = new Font("Ariel", 12, FontStyle.Bold);
            Caption.ForeColor = Color.Red;
            Caption.AutoSize = false;
            Caption.Width = picCanvas.Width;
            Caption.Height = 30;
            Caption.TextAlign = ContentAlignment.MiddleCenter;
            picCanvas.Controls.Add(Caption);

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = "Snake Game SnapShot Phan Anh Khoa";
            dialog.DefaultExt = "jpg";
            dialog.Filter = "JPG Image file | *.jpg";
            dialog.ValidateNames = true;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                int Width = Convert.ToInt32(picCanvas.Width);
                int Height = Convert.ToInt32(picCanvas.Height);
                Bitmap bmp = new Bitmap(Width, Height); 
                picCanvas.DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
                bmp.Save(dialog.FileName, ImageFormat.Jpeg);
                picCanvas.Controls.Remove(Caption);
            }
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            // setting the directions

            if (goLeft)
            {
                Setting.directions = "Left";
            }
            if (goRight)
            {
                Setting.directions = "Right";
            }
            if (goDown)
            {
                Setting.directions = "Down";
            }
            if (goUp)
            {
                Setting.directions = "Up";
            }
            
            // end of directions

            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Setting.directions) 
                    {
                        case "Left":
                            Snake[i].X--;
                            break;
                        case "Right":
                            Snake[i].X++;
                            break;
                        case "Down":
                            Snake[i].Y++;
                            break;
                        case "Up":
                            Snake[i].Y--;
                            break;
                    }

                    if (Snake[i].X < 0)
                    {
                        Snake[i].X = maxWidth;
                    }
                    if (Snake[i].X > maxWidth)
                    {
                        Snake[i].X = 0;
                    }
                    if (Snake[i].Y < 0)
                    {
                        Snake[i].Y = maxHeight;
                    }
                    if (Snake[i].Y > maxHeight)
                    {
                        Snake[i].Y = 0;
                    }
                    
                    if (Snake[i].X == food.X && Snake[i].Y == food.Y)
                    {
                        EatFood();
                    }

                    for (int k = 10; k < Snake.Count; k++)
                    {
                        if (Snake[i].X == Snake[k].X && Snake[i].Y == Snake[k].Y)
                        {
                            GameOver();
                        }
                    }
                }
                else
                {
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }



            picCanvas.Invalidate();
        }

        private void UpdatePictureBoxGraphics(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush snakeColour;

            // Draw the grid
            Pen gridPen = new Pen(Color.DarkGray);
            for (int i = 0; i <= picCanvas.Width / Setting.Width; i++)
            {
                canvas.DrawLine(gridPen, new Point(i * Setting.Width, 0), new Point(i * Setting.Width, picCanvas.Height));
            }
            for (int j = 0; j <= picCanvas.Height / Setting.Height; j++)
            {
                canvas.DrawLine(gridPen, new Point(0, j * Setting.Height), new Point(picCanvas.Width, j * Setting.Height));
            }

            // Draw the snake and food
            for (int i = 0; i < Snake.Count; i++)
            {
                if (i == 0)
                {
                    snakeColour = Brushes.Black;
                }
                else
                {
                    snakeColour = Brushes.DarkGreen;
                }
                canvas.FillEllipse(snakeColour, new Rectangle(
                    Snake[i].X * Setting.Width,
                    Snake[i].Y * Setting.Height,
                    Setting.Width, Setting.Height));
            }
            canvas.FillEllipse(Brushes.DarkRed, new Rectangle(
                food.X * Setting.Width,
                food.Y * Setting.Height,
                Setting.Width, Setting.Height));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            DialogResult traloi;
            traloi = MessageBox.Show("Chắc không?", "Trả lời",
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (traloi == DialogResult.OK) Application.Exit();
        }

        private void picCanvas_Click(object sender, EventArgs e)
        {
            
        }

        private void ReStartGame()
        {
            maxWidth = picCanvas.Width / Setting.Width - 1;
            maxHeight = picCanvas.Height / Setting.Height - 1;

            Snake.Clear();

            btnStart.Enabled = false;
            btnSnap.Enabled = false;
            btnStop.Enabled = false;

            score = 0;
            lblScore.Text = "Score: " + score;

            Circle head = new Circle { X = 10, Y = 5 };
            Snake.Add(head); // adding a head part of the snake to the list

            for (int i = 0; i < 5; i++) // do dai cua ran
            {
                Circle body = new Circle();
                Snake.Add(body);
            }

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            gameTimer.Start();
        }

        private void EatFood()
        {
            score += 1;

            lblScore.Text = "Score" + score;

            Circle body = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y
            };

            Snake.Add(body);

            food = new Circle { X = rand.Next(2, maxWidth), Y = rand.Next(2, maxHeight) };

            SoundPlayer player = new SoundPlayer("EAT.wav");
            player.Play();
            
        }

        private void GameOver()
        {
            gameTimer.Stop();
            btnStart.Enabled = true;
            btnSnap.Enabled = true;
            btnStop.Enabled = true;

            if (score > highscore)
            {
                highscore = score;

                lblHightScore.Text = "Hight score: " + Environment.NewLine + highscore;
                lblHightScore.ForeColor = Color.Maroon;
                lblHightScore.TextAlign = ContentAlignment.MiddleCenter;
            }
        }
    }
}
