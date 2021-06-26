using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Zmeyka
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Font font1 = new Font("Arial", 24, FontStyle.Bold);
        Font font2 = new Font("Times New Roman", 18, FontStyle.Italic);
        bool[,] pole;
        Point [] hiro;

        int kletka = 30,
            tableI,
            tableJ,
            poleWidth,
            poleHeight,
            posI,
            posJ,
            posX = 0,
            posY = 0,
            points = 0;
      
        private void Form1_KeyDown(object sender, KeyEventArgs e) //привязка клавиш управления
        {
            e.SuppressKeyPress = true; // убирает БАГ звук восклицания при нажатии на клавиши
            //запрет на нажатие других клавиш
            if (e.KeyCode != Keys.D && e.KeyCode != Keys.A && e.KeyCode
                != Keys.S && e.KeyCode != Keys.W) return;
            // запрет на обратное движение
            if ((posX == 1 && e.KeyCode == Keys.A) ||
                (posX == -1 && e.KeyCode == Keys.D) ||
                (posY == 1 && e.KeyCode == Keys.W) ||
                (posY == -1 && e.KeyCode == Keys.S)) return;

            posX = 0; posY = 0;
            
            if (e.KeyCode == Keys.D) posX = 1;
            if (e.KeyCode == Keys.A) posX = -1;
            if (e.KeyCode == Keys.S) posY = 1;
            if (e.KeyCode == Keys.W) posY = -1;
            
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            panelGameOver.Visible = false;
            drawSetup();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            drawPole();
            timer1.Start();
            posX = 1;
            buttonStart.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e) // таймер движения и отрисовки
        {
            proverkaNaPopadanieVCel();
           proverkaNaVyhodZaPredelyMassiva();
            zmey();
            drawPole();
            proverkaNaNaezdNaHvost();
        }

       void proverkaNaPopadanieVCel()
        {
            if (hiro[0].X + posX == posI && hiro[0].Y + posY == posJ)
            {
                Array.Resize(ref hiro, hiro.Length + 1);
                points++;
                point.Text = "Очки: " + points;
                pole[posI, posJ] = false;
                randomCell();
            }
        }

        void proverkaNaVyhodZaPredelyMassiva()
        {
            if (hiro[0].X == -1)
            {
                hiro[0].X = tableI;
                return;
            }
                
            if (hiro[0].X == tableI)
            {
                hiro[0].X = 0;
                return;
            }
            if (hiro[0].Y == -1)
            {
                hiro[0].Y = tableJ;
                return;
            }

            if (hiro[0].Y == tableJ)
            {
                hiro[0].Y = 0;
                return;
            }

        }

        void proverkaNaNaezdNaHvost()
        {
            for(int i = 1; i < hiro.Length; i++)
            {
                if(hiro[0].X == hiro[i].X && hiro[0].Y == hiro[i].Y)
                {
                    timer1.Stop();
                    labelGameOver.Text = "Очки: " + points;
                    panelGameOver.Visible = true;
                }
            }
        }
        public Form1() // инициализация и первичная отрисовка
        {
            InitializeComponent();
            this.KeyPreview = true; //подключаем кнопки клавиатуры
            drawSetup();
        }

        void drawSetup() // начальные установки и отрисовка исходного поля
        {
            poleWidth = poleGame.Width - (poleGame.Width%kletka);
            poleHeight = poleGame.Height - (poleGame.Width % kletka);
            poleGame.Width = poleWidth;
            poleGame.Height = poleHeight;
            tableI = poleWidth / kletka;
            tableJ = poleHeight / kletka;
            pole = new bool[tableI, tableJ];
            hiro = new Point[1];
            posX = 0;
            posY = 0;
            points = 0;
            point.Text = "Очки: 0";

            randomCell();
            poleGame.Image = new Bitmap(poleWidth, poleHeight);
            graphics = Graphics.FromImage(poleGame.Image);
            string str = "Игра ЗМЕЙКА";
            SizeF size = graphics.MeasureString(str, font1);
            graphics.DrawString(str, font1, Brushes.Black, poleGame.Width / 2 - Convert.ToInt32(size.Width) / 2, poleGame.Height / 2 - Convert.ToInt32(size.Height) / 2);
            str = "© StudioFant 2021";
            size = graphics.MeasureString(str, font2);
            graphics.DrawString(str, font2, Brushes.Black, poleGame.Width / 2 - Convert.ToInt32(size.Width) / 2, poleGame.Height / 2 + Convert.ToInt32(size.Height));
            buttonStart.Enabled = true;
        }

        void zmey () // создание змея. построение массива значений
        {
            int x = hiro[0].X + posX; int y = hiro[0].Y + posY;
            for (int i = hiro.Length - 1; i >= 1; i--)
            {
                hiro[i].X = hiro[i - 1].X; hiro[i].Y = hiro[i - 1].Y;
            }
            hiro[0].X = x; hiro[0].Y = y;
        }

        void randomCell () // генератор случайного расположения цнли для змейки
        {
            Random random = new Random();
            bool flag = true;
            while (flag)
            { flag = false;
                posI = random.Next(0, tableI - 1);
                posJ = random.Next(0, tableJ - 1);
                for (int i = 0; i < hiro.Length; i++)
                {
                    if (posI == hiro[i].X && posJ == hiro[i].Y)
                    {
                        flag = true;
                        break;  
                    }
                }
                
            }

            pole[posI, posJ] = true;
        }

        void drawPole() // отрисовка игрового поля по тику таймера
        {
            graphics.Clear(Color.LightCoral);
            for(int i = 0; i < tableI; i++)
            {
                for(int j = 0; j < tableJ; j++)
                {
                    graphics.DrawRectangle(Pens.Black, i*kletka, j*kletka, kletka, kletka);
                    if (pole[i,j]) graphics.FillEllipse(Brushes.Yellow, i * kletka, j * kletka, kletka, kletka);
                }
            }
            for(int i = 0; i < hiro.Length; i++)
            {
                {
                    graphics.FillRectangle(Brushes.Blue, hiro[i].X * kletka, hiro[i].Y * kletka, kletka, kletka);
                }
            }
            poleGame.Refresh();
            //GC.Collect(); // сборщик мусора принудительное включение
        }

        
    }
}
