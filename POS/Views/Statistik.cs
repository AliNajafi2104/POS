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
    public partial class Statistik : Form
    {
        public Statistik(Form form,Button btn)
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
            baskets = SQL_Sales.ReadSoldBaskets();
            form_ = form;


            foreach (Basket b in baskets)
            {
                List<Product> products = new List<Product>();
                foreach (KeyValuePair<String, double> kvp in b.keyValuePairs)
                {
                    totalPrBasket += kvp.Value;
                    Product product = new Product
                    {
                        Vare = kvp.Key,
                        Pris = kvp.Value,
                    };
                    products.Add(product);
                }

                DateSale datesale = new DateSale
                {
                    TotalSale = totalPrBasket,
                    Date = b.time
                };
                list.Add(datesale);
                totalPrBasket = 0;
                listOfLists.Add(products);
            }

            dataGridView3.DataSource = list;
            
            basketStyling();
            btn_ = btn;
        }

        List<List<Product>> listOfLists = new List<List<Product>>();

        Button btn_;

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


        private void basketStyling()
        {


            dataGridView3.Columns["TotalSale"].HeaderText = "Total for kurv";
            dataGridView3.Columns["date"].HeaderText = "Tidspunkt";
            dataGridView3.RowTemplate.Height = 30;

            dataGridView3.RowHeadersVisible = false;

            





        }

        #endregion
        private void weeklyTotal()
        {
            List<DateSale> reading = SQL_Sales.ReadAllDailySales();
            reading.Reverse();
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



        List<Basket> baskets = new List<Basket>();
        List<DateSale> list = new List<DateSale>();
        Form form_;
        double totalPrBasket = 0;


        private void button1_Click(object sender, EventArgs e)
        {
            btn_.Focus();
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

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < listOfLists.Count)
            {
                // Clear the current data source
                dataGridView4.DataSource = null;

                // Get the index of the clicked row
                int clickedRowIndex = e.RowIndex;

                // Set the selected list from listOfLists as the data source for dataGridView4
                dataGridView4.DataSource = listOfLists[clickedRowIndex];

                // Refresh the dataGridView4 to reflect the changes
                dataGridView4.Columns["Stregkode"].Visible = false;
                dataGridView4.Columns["Ingen_stregkodemærkning"].Visible = false;
                dataGridView4.Columns["Antal"].Visible = false;
                dataGridView4.Columns["Kategori"].Visible = false;
                dataGridView4.RowTemplate.Height = 30;
                dataGridView4.Refresh();

                // Assuming list is a list of DateSale objects and timestamp is a TextBox
                timestamp.Text = list[clickedRowIndex].Date.ToString("HH:mm:ss"); // Adjust format as needed


            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView4.DataSource = null;
            dataGridView4.DataSource = baskets[baskets.Count - 1];
            dataGridView4.Refresh();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
