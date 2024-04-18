using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace searchengine123.Views
{
    public partial class Galleri : Form
    {
        public Galleri(Form form)
        {
            InitializeComponent();
            pictureBox1.Image = Properties.Resources.glaskål;
            this.WindowState = FormWindowState.Maximized;
            form_ = form;
            pictureBox2.Image = Properties.Resources.persille;
            label2.Text = "Persille";
        }
        Form form_;
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            form_.Show();
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
    }
}
