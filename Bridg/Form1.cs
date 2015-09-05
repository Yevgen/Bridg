using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Runtime.InteropServices;

namespace Bridg
{
    public partial class Form1 : Form
    {
        Table _table;
        Bitmap[] bitmaps;

        Bitmap canvas = new Bitmap(600, 600);
        Bitmap back = new Bitmap(600, 600);
        Graphics gr1;


        public Form1()
        {
            InitializeComponent();
            back = Properties.Resources.BackGround_1;
            gr1 = Graphics.FromImage(canvas);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadI();

            _table = new Table(600);
            _table.ShowMessage += ShowMessage;
            _table.GetCountPlayers += GetCountPlayers;
            _table.GetSuid += GetSuid;
            _table.ShowCard += showCard;
            _table.ShowScoresOfPlayers += ShowScoreOfPlayer;
            
            timer1.Enabled = true;
        }

        private int GetSuid()
        {
            Chooce chooce = new Chooce();
            chooce.Enabled = true;
            chooce.ShowDialog();

            return chooce.Suid;
        }

        private int GetCountPlayers()
        {
            ChooceQuantityPlayers newCh = new ChooceQuantityPlayers();
            newCh.Enabled = true;
            newCh.ShowDialog();
            return newCh.quantity;
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }

        //private bool OnDraw = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            _table.Referi();

            draw();
            _table.ShowAllCards();
            pictureBox2.Image = canvas;

            _table.ShowAllScores();
            if (_table.OpenDeck == null)
                return;

            if (_table.SelectedPlayerNumber == 1 && !_table.IsDistributionOver)
            {
                if (!_table.IsToCard || _table.OpenDeck.Priority() == 6)
                {
                    button1.Enabled = true;
                    button2.Enabled = false;
                }
                else
                {
                    button1.Enabled = false;
                    button2.Enabled = true;
                }
            }
            else
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
            if (_table.OpenDeck.Priority() == 11)
            {
                ShowSuid();
            }
            else pictureBox1.Image = null;

            label5.Text = "Очки множаться на " + _table.Multiplier;
        }
        private void ShowScoreOfPlayer(string[] names, string[] scores, int numberOfSelectPlayer)
        {
            Label[] arrLabel = new Label[4] { labelPlayer1, labelPlayer2, labelPlayer3, labelPlayer4 };
            Label[] arrLabelScores = new Label[4] { labelScore1, labelScore2, labelScore3, labelScore4 };

            for (int w = 0; w < 4; w++)
            {
                arrLabel[w].Text = names[w];
                arrLabelScores[w].Text = scores[w];
                arrLabel[w].ForeColor = Color.Black;
                arrLabelScores[w].ForeColor = Color.Black;
            }

            arrLabel[numberOfSelectPlayer].ForeColor = Color.Green;
            arrLabelScores[numberOfSelectPlayer].ForeColor = Color.Green;
        }
        private void ShowSuid()
        {
            switch (_table.SuitOfJeck)
            {
                case 0:
                    pictureBox1.Image = Properties.Resources._000;
                    break;
                case 1:
                    pictureBox1.Image = Properties.Resources._001;
                    break;
                case 2:
                    pictureBox1.Image = Properties.Resources._002;
                    break;
                case 3:
                    pictureBox1.Image = Properties.Resources._003;
                    break;
            }
        }
        public void LoadI()
        {
            bitmaps = BackArrBm();
        }

        private Bitmap[] BackArrBm()
        {
            Bitmap[] res = new Bitmap[40];
            res[0] = Properties.Resources._0;

            res[1] = Properties.Resources._1;
            res[2] = Properties.Resources._6;
            res[3] = Properties.Resources._7;
            res[4] = Properties.Resources._8;
            res[5] = Properties.Resources._9;
            res[6] = Properties.Resources._10;
            res[7] = Properties.Resources._11;
            res[8] = Properties.Resources._12;
            res[9] = Properties.Resources._13;

            res[10] = Properties.Resources._14;
            res[11] = Properties.Resources._19;
            res[12] = Properties.Resources._20;
            res[13] = Properties.Resources._21;
            res[14] = Properties.Resources._22;
            res[15] = Properties.Resources._23;
            res[16] = Properties.Resources._24;
            res[17] = Properties.Resources._25;
            res[18] = Properties.Resources._26;

            res[19] = Properties.Resources._27;
            res[20] = Properties.Resources._32;
            res[21] = Properties.Resources._33;
            res[22] = Properties.Resources._34;
            res[23] = Properties.Resources._35;
            res[24] = Properties.Resources._36;
            res[25] = Properties.Resources._37;
            res[26] = Properties.Resources._38;
            res[27] = Properties.Resources._39;

            res[28] = Properties.Resources._40;
            res[29] = Properties.Resources._45;
            res[30] = Properties.Resources._46;
            res[31] = Properties.Resources._47;
            res[32] = Properties.Resources._48;
            res[33] = Properties.Resources._49;
            res[34] = Properties.Resources._50;
            res[35] = Properties.Resources._51;
            res[36] = Properties.Resources._52;
            res[37] = Properties.Resources._53;

            return res;
        }
        private void showCard(int number, int x, int y)
        {
            //Оригінальна ширина і висота карти
            int widthC = 181, heightC = 255;
            //Ширина та висота карти на полі
            int widthCards = _table.CardWidth, heightCards = _table.CardHeight;

            Bitmap bitmapCard = bitmaps[number];

            Rectangle rec2 = new Rectangle(x, y, widthCards, heightCards);

            gr1.DrawImage(bitmapCard, rec2, 0, 0, widthC, heightC, GraphicsUnit.Pixel);

            pictureBox2.Image = canvas;
        }
        private void draw()
        {
            if (_table == null)
                return;

            Rectangle newR = new Rectangle(0, 0, 600, 600);
            gr1.DrawImage(back , newR, 0, 0, 600, 600, GraphicsUnit.Pixel);

        }

        #region Події мишки
        private void Dis_MouseMove(object sender, MouseEventArgs e)
        {
            if (_table != null)
                _table.MousMove(e.X, e.Y );
        }

        private void Dis_MouseUp(object sender, MouseEventArgs e)
        {
            _table.MousUp(e.X, e.Y);
        }

        private void Dis_MouseDown(object sender, MouseEventArgs e)
        {
            _table.MousDown(e.X , e.Y);
        }
        #endregion
        private void button1_Click(object sender, EventArgs e)
        {
            if (_table != null)
                _table.ToCards();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_table.ActiveChoice)
                _table.ChooiseSuidJack();
            Thread B = new Thread(Thr);
            B.Start();
        }
        public void Thr()
        {
            _table.NextPlayer();
        }

        private void новаГраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _table.NewGame();
        }

        private void вихідToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void проПрограмуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 newH = new AboutBox1();
            newH.Enabled = true;
            newH.ShowDialog();
        }
    }
}
