using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Bridg
{
    public partial class ChooceQuantityPlayers : Form
    {
        public int quantity { get; set; }
        public ChooceQuantityPlayers()
        {
            InitializeComponent();
        }

        private void ChooceQuantityPlayers_Load(object sender, EventArgs e)
        {
            quantity = 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
            quantity = trackBar1.Value;
        }
    }
}
