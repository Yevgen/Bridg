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
    public partial class Chooce : Form
    {
        public int Suid { set; get; }
        public Chooce()
        {
            InitializeComponent();
        }

        private void Chooce_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Suid = 0;
            Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Suid = 3;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Suid = 1;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Suid = 2;
            Close();
        }
    }
}
