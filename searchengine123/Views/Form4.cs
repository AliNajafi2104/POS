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
using System.Windows.Forms.DataVisualization.Charting;
using Google.Protobuf.WellKnownTypes;

namespace searchengine123.Views
{
    public partial class Form4 : Form
    {
        public Form4(Form form)
        {
            InitializeComponent();
            weeklyTotal();
            chartStyling();
            dataGridView2.DataSource = SQL_Sales.ReadLatestSold();
            MostSoldStyling();
            // Assuming your DataGridView is named dataGridView1
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            
            form_ = form;
        }

        Form form_;

        #region STYLING
        private void chartStyling()
        {
            chart1.ChartAreas[0].BackColor = Color.Transparent; // Make chart area background transparent
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray; // Customize grid lines
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chart1.ChartAreas[0].AxisX.Title = "Date";
            chart1.ChartAreas[0].AxisY.Title = "Total Sales";
            chart1.ChartAreas[0].AxisX.LabelStyle.Font = new Font("Segoe UI", 9); // Change font
            chart1.ChartAreas[0].AxisY.LabelStyle.Font = new Font("Segoe UI", 9);
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.DimGray; // Change label color
            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.DimGray;
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "MM/dd";
            chart1.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
            chart1.BackColor = Color.FromArgb(240, 240, 240); // Set background color
                                                              // Assuming you have a reference to your chart object
           

        }
        private void MostSoldStyling()
        {
            dataGridView2.Columns["Ingen_stregkodemærkning"].Visible = false;
            dataGridView2.Columns["Pris"].Visible = false;
            dataGridView2.Columns["Kategori"].Visible = false;
            // Assuming your DataGridView is named dataGridView1
            dataGridView2.RowHeadersVisible = false;
            dataGridView2.DefaultCellStyle.SelectionBackColor = Color.White;
            dataGridView2.DefaultCellStyle.SelectionForeColor = Color.Black;
        }


        #endregion
        private void weeklyTotal()
        {
            List<DateSale> reading = SQL_Sales.ReadAllDailySales();
            dataGridView1.DataSource = reading;
            double[] totalArr = new double[reading.Count()];
            DateTime[] date = new DateTime[reading.Count()];
            for (int i = 0; i < reading.Count(); i++)
            {
                totalArr[i] = reading[i].TotalSale;
                date[i] = reading[i].Date;
            }
            Series series = chart1.Series.Add("Weekly Sales");
            series.ChartType = SeriesChartType.Column;

            for (int i = 0; i < totalArr.Length; i++)
            {
                // Add data point with date as X value and total sale as Y value
                series.Points.AddXY(date[i], totalArr[i]);
            }
        }
       


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            form_.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            form_.Show();
            this.Close();
        }
    }
}
