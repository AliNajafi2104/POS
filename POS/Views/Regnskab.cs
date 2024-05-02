using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using searchengine123.Back_end;
using searchengine123.Models;

namespace searchengine123.Views
{
    public partial class Regnskab : Form
    {
        public Regnskab(Form form)
        {
            InitializeComponent();
            List<DateSale> AllDailySales = SQL_Sales.ReadAllDailySales();
            foreach (DateSale sales in AllDailySales)
            {
                total += sales.TotalSale;
            }
            label12.Text = total.ToString() ;


            List<Udgift> expenses = SQL_Sales.ReadExpenses();
            for (int i = expenses.Count - 1; i >= 0; i--)
            {
                Udgift expense = expenses[i];
                if (expense.name == "vareforbrug")
                {
                    vareforbrug = expense.expense;
                    expenses.RemoveAt(i);
                }
            }
            label13.Text=vareforbrug.ToString() ;
            label11.Text=(total-vareforbrug).ToString() ;

            
            dataGridView2.DataSource = expenses;

            foreach(Udgift udgift in expenses)
            {
                totalExpenses += udgift.expense;
            }

            label15.Text =
                (totalExpenses).ToString();

            this.WindowState = FormWindowState.Maximized;
            label7.Text = (total - vareforbrug-totalExpenses).ToString() ;
            form_ = form;
        }
        Form form_;
        private void label7_Click(object sender, EventArgs e)
        {

        }
        double total = 0;
        double vareforbrug = 0;
        double totalExpenses= 0;
        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click_1(object sender, EventArgs e)
        {

        }

        private void label26_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label21_Click(object sender, EventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }

        private void label24_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            form_.Show();
            this.Close();
           
        }
    }
}
