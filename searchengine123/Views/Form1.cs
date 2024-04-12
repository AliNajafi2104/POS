using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Google.Protobuf;
using MySql.Data.MySqlClient;
using searchengine123.Back_end;
using searchengine123.Views;
using System.Linq;


using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using searchengine123.Properties;


namespace searchengine123
{
        public partial class Form1 : Form
        {
            public Form1()
            {
                InitializeComponent();
                this.KeyPreview = true;
                this.KeyDown += Form1_KeyDown;



           

            
                dataInitialization();
                stylingInitialization();
            tbBarcode.Enabled = false;

           
            this.WindowState = FormWindowState.Maximized;


            panel1.Hide();


            categoryTotalPrices = new Dictionary<string, double>();


            dataGridView1.BackgroundColor = Color.LightGray;
            dataGridView1.RowTemplate.Height = 50;

            deleteOne.Image = Properties.Resources.back;


        }



        #region INITIALIZATION
        private void stylingInitialization()
            {
                KGpris.Columns["Kategori"].Visible = false;
                KGpris.Columns["Stregkode"].Visible = false;
                KGpris.Columns["Ingen_stregkodemærkning"].Visible = false;
                KGpris.Columns["Antal"].Visible = false;
                KGpris.ScrollBars = ScrollBars.None;
                KGpris.Columns["Pris"].HeaderText = "KG-pris";
                KGpris.RowTemplate.Height = 30;
                KGpris.ForeColor = Color.Black;
                dataGridViewNoBarcode.Columns["Kategori"].Visible = false;
                dataGridViewNoBarcode.RowHeadersVisible = false;
                KGpris.RowHeadersVisible = false;
            KGpris.DefaultCellStyle.SelectionBackColor = Color.MintCream;
            KGpris.DefaultCellStyle.SelectionForeColor = Color.Black;


            dataGridViewNoBarcode.Columns["Ingen_stregkodemærkning"].Visible = false;
                dataGridViewNoBarcode.Columns["Antal"].Visible = false;
                dataGridViewNoBarcode.ScrollBars = ScrollBars.None;
                dataGridViewNoBarcode.RowTemplate.Height = 30;
                dataGridViewNoBarcode.ForeColor= Color.Black;
            dataGridViewNoBarcode.DefaultCellStyle.SelectionBackColor = Color.MintCream;
            dataGridViewNoBarcode.DefaultCellStyle.SelectionForeColor = Color.Black;


            basketGridStyling();
            }
            private void dataInitialization()
            {
                List<Product> productsKgPrice = new List<Product>();
                List<Product> pricePrPiece = new List<Product>();
                foreach(Product product in SQL_Product.FetchProducts())
                {
                    if(product.Ingen_stregkodemærkning=="1")
                    {
                        pricePrPiece.Add(product);
                    }
                    if(product.Ingen_stregkodemærkning=="2")
                    {
                        productsKgPrice.Add(product);
                    }
                }
                dataGridViewNoBarcode.DataSource = pricePrPiece;
                KGpris.DataSource = productsKgPrice;

                totalSumForDagenDisplay = SQL_Sales.ReadDailySale();
                UpdateProgressBar();
                dataGridViewBasket.DataSource = scannedProducts;
            }
        #endregion




            #region BUTTONS
            private void btnAddToBasket_Click_1(object sender, EventArgs e)
            {
            try
            {
                panel1.Hide();
                if(reset)
                {
                    totalSum_CurrentBasket = 0;
                    reset = false;
                }
                Product addedProduct = SQL_Product.Read(tbBarcode.Text);

                for (int i = 0; i < multiplier; i++)
                {


                    if (addedProduct != null)
                    {
                        bool productExists = false;
                        foreach (Product product in scannedProducts)
                        {
                            if (product.Stregkode == addedProduct.Stregkode)
                            {
                                product.Antal++;
                                productExists = true;
                            }
                        }
                        if (!productExists)
                        {
                            scannedProducts.Add(addedProduct);
                        }

                        totalSum_CurrentBasket += addedProduct.Pris;
                        dataGridViewBasketRefresh();
                        tbBarcode.Clear();
                       
                    }

                }
                if (addedProduct == null)
                {
                    if (tbBarcode.Text != "")
                    {
                        double.TryParse(tbBarcode.Text, out double unknownBarcode);
                        opretVare(unknownBarcode);
                    }
                }
                multiplier = 1;
            }
            catch (Exception ex)
            {
                // Handle the exception
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
            public void btnOrderConfirmed_Click(object sender, EventArgs e)
            {
                if (totalSum_CurrentBasket> 0)
                {
                    DialogResult = MessageBox.Show("Tryk OK hvis betalingen er godkendt!", "Vil du godkende ordren?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (DialogResult == DialogResult.OK)
                    {
                        SQL_Sales.UpdateDailySale(totalSum_CurrentBasket);
                        totalSumForDagenDisplay += rundOp(totalSum_CurrentBasket);
                        label4.Text = $"Total salg i dag: {totalSumForDagenDisplay:C}";
                        UpdateProgressBar();
                        SQL_Sales.RegisterProductSale(scannedProducts);
                    categoryTotalPrices.Clear();
                       CalculateCategoryTotals();
                    scannedProducts.Clear();
                    dataGridViewBasketRefresh();
                    btnAddToBasket.Focus();
                    panel1.Show();
                    reset = true;
                    }
                }

            }
        bool reset;
            private void btnResetBasket_Click(object sender, EventArgs e)
            {
            if (!reset)
            {
                clearBasket();
            }
            }
            private void btnOpretVare_Click(object sender, EventArgs e)

            {
                opretVare(0);
            }
            #region GENERIC BUTTONS
            private void ManuelPrice_Click(object sender, EventArgs e)
            {
            if (reset)
            {
                totalSum_CurrentBasket = 0;
                reset = false;
            }
            panel1.Hide();
            Control control = sender as Control;
                string controlName = control.Name;
                if (double.TryParse(tbManuelPrice.Text, out double manuelPrice))
                {
                    scannedProducts.Add(new Product
                    {
                        Vare = control.Text,
                        Stregkode = null,
                        Pris = manuelPrice,
                        Kategori = control.Text
                    });
                    totalSum_CurrentBasket += manuelPrice;
                    dataGridViewBasketRefresh();
                    tbManuelPrice.Clear();
                btnAddToBasket.Focus();
                }

            }
            private void click(object sender, EventArgs e)
            {
                Control control = sender as Control;

                if (control.Text == ",")
                {
                    tbManuelPrice.Text += ",";
                }
                else if (control.Text == "c")
                {
                    tbManuelPrice.Clear();
                }
                else if (control.Text == "<")
                {
                    if (tbManuelPrice.Text.Length > 0)
                    {
                        tbManuelPrice.Text = tbManuelPrice.Text.Substring(0, tbManuelPrice.Text.Length - 1);
                    }
                }
               

            else if (multiply)
            {
               
                int.TryParse(control.Text, out multiplier);
                multiply = false;
                
                this.ActiveControl = null;
            }
            else
                {
               
                    tbManuelPrice.Text += control.Text;
                multiplier = 1;
                this.ActiveControl = null;

            }
            
        }
            #endregion
            #endregion






            #region BASKET_GRIDVIEW
            private void basketGridStyling()
            {
                dataGridViewBasket.Columns["Kategori"].Visible = false;
                dataGridViewBasket.Columns["Ingen_stregkodemærkning"].Visible = false;
                dataGridViewBasket.Columns["Vare"].Width = 200;
                dataGridViewBasket.Columns["Stregkode"].Width = 250;
                dataGridViewBasket.Columns["Pris"].Width = 130;
                dataGridViewBasket.Columns["Antal"].Width = 90;
                dataGridViewBasket.RowTemplate.Height = 50;
            dataGridViewBasket.RowHeadersVisible= false;
                dataGridViewBasket.ScrollBars = ScrollBars.None;

            }
            public void dataGridViewBasketRefresh()
            {
                dataGridViewBasket.DataSource = null;
                dataGridViewBasket.DataSource = scannedProducts;
                dataGridViewBasket.ClearSelection();
                label2.Text = $"Total: {rundOp(totalSum_CurrentBasket):C}";
                basketGridStyling();

            }       
            private void dataGridViewBasket_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                // Check if the click is on a valid cell
                if (e.RowIndex >= 0 && e.RowIndex < scannedProducts.Count)
                {
                    Product selectedProduct = scannedProducts[e.RowIndex];
                    totalSum_CurrentBasket -= selectedProduct.Pris;
                    scannedProducts.RemoveAt(e.RowIndex);
                    dataGridViewBasketRefresh();

                }
            }
            #endregion







            #region VARIABLES
            List<Product> scannedProducts = new List<Product>();
            double TotalSqlDate;
            double totalSum_CurrentBasket;
            double totalSumForDagenDisplay;
        bool multiply;
        int multiplier = 1;
        int test;
              Dictionary<string, double> categoryTotalPrices;
        #endregion






        private void Form1_KeyDown(object sender, KeyEventArgs e)
            {
                if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9))
                {
                    tbBarcode.AppendText(e.KeyCode.ToString().Substring(1)); // Append the numeric key to the barcode TextBox
                }
                if (e.KeyCode == Keys.Enter)
                {
                // Perform the desired action (e.g., trigger button click event)
                    btnAddToBasket.PerformClick();
                }
            if (e.KeyCode == Keys.Back && tbBarcode.Text.Length > 0)
            {
                // Delete the last character from the barcode input
                tbBarcode.Text = tbBarcode.Text.Remove(tbBarcode.Text.Length - 1);

                // Move the cursor to the end of the text
                tbBarcode.SelectionStart = tbBarcode.Text.Length;
            }

            if (!char.IsDigit((char)e.KeyCode) && !char.IsControl((char)e.KeyCode))
                {
                    // If not a digit or control key, ignore the key press event
                    e.Handled = true;
                }
            }
            private void UpdateProgressBar()
            {
                // Convert double progress to an integer
                int integerProgress = (int)Math.Round(totalSumForDagenDisplay);
                label4.Text = $"Total salg i dag: {totalSumForDagenDisplay:C}";
                // Update the ProgressBar value
                if (integerProgress <= progressBar1.Maximum)
                {
                    progressBar1.Value = integerProgress;
                }
                else
                {
                    // Handle the case when totalsumForDagen exceeds the ProgressBar's maximum
                    progressBar1.Value = progressBar1.Maximum;
                }
            }
            private void clearBasket()
            {
                scannedProducts.Clear();
                totalSum_CurrentBasket = 0;
                dataGridViewBasketRefresh();
            btnAddToBasket.Focus();
            }
            private void opretVare(double barcode)
            {
                Form3 form3 = new Form3(barcode, this);
            this.Hide();
                form3.ShowDialog(); 
            }
            public static double rundOp(double tal)
            {
                int intTal = (int)tal;
                double doubleTal = (double)intTal;
                double differens = tal - doubleTal;
                if (differens > 0.7500)
                {
                    differens = 1;
                    return doubleTal + differens;
                }
                else if (0.7500 > differens && differens > 0.2500)
                {
                    differens = 0.5;
                    return doubleTal + differens;
                }
                else
                {
                    return doubleTal;
                }



            }

            private void button10_Click(object sender, EventArgs e)
            {
                Form4 form4 = new Form4(this);
            
            form4.Show();
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(this);

            form5.Show();   
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            multiply = true;

        }

        private void dataGridViewBasket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            Galleri galleri = new Galleri(this);
            galleri.Show();
            this.Hide();
        }

        
        private void CalculateCategoryTotals()
        {
            foreach (Product product in scannedProducts)
            {
                if (!categoryTotalPrices.ContainsKey(product.Kategori))
                {
                    categoryTotalPrices[product.Kategori] = 0.0;
                }

                categoryTotalPrices[product.Kategori] += product.Pris * product.Antal;
            }


            // Filter out categories with a total of 0
            var nonZeroCategories = categoryTotalPrices.Where(kv => kv.Value != 0.0)
                                                      .ToDictionary(kv => kv.Key, kv => kv.Value);

            // Bind the filtered data to the DataGridView
            dataGridView1.DataSource = nonZeroCategories.Select(kv => new { Kategori = kv.Key, Pris = kv.Value }).ToList();

            // Calculate and display the overall total
            overallTotal = nonZeroCategories.Values.Sum();
           
            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        double overallTotal;

        private void button15_Click(object sender, EventArgs e)
        {
            double inputNumber;
            if (double.TryParse(tbManuelPrice.Text, out inputNumber))
            {
                double result = inputNumber - totalSum_CurrentBasket;
                label3.Text = "Kunden skal have "+result.ToString() +" Kr. tilbage";
                tbManuelPrice.Clear();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tbBarcode_TextChanged(object sender, EventArgs e)
        {

        }
        
    }
}