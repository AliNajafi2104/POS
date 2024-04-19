﻿using System;
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
                this.KeyPress += Form1_KeyPress;


            dataGridViewBasket.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewBasket.MultiSelect = false;




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

            dataGridViewBasket.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewBasket.MultiSelect = false;


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
            //dataGridViewBasket.DataSource = scannedProducts;

            dataGridViewBasket.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewBasket.MultiSelect = false;
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
                        dataGridViewBasket.RowTemplate.Height = 70;
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
                basketGridStyling();
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
                   
                        SQL_Sales.UpdateDailySale(totalSum_CurrentBasket);
                        totalSumForDagenDisplay += rundOp(totalSum_CurrentBasket);
                        label4.Text = $"Total i dag:    {totalSumForDagenDisplay:C}";
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
            dataGridViewBasket.RowTemplate.Height = 70;
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
                else if (control.Text == "")
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
                

               
            }
            else
                {
               
                    tbManuelPrice.Text += control.Text;
                multiplier = 1;
             

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
                dataGridViewBasket.RowTemplate.Height = 70;
            dataGridViewBasket.RowHeadersVisible= false;
                dataGridViewBasket.ScrollBars = ScrollBars.None;
            dataGridViewBasket.SelectionMode = DataGridViewSelectionMode.CellSelect;

            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Transparent; // Or any color matching cell's background
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black; // Or any color matching cell's text

        }
        public void dataGridViewBasketRefresh()
            {
                dataGridViewBasket.DataSource = null;
                dataGridViewBasket.DataSource = scannedProducts;
                dataGridViewBasket.ClearSelection();
                label2.Text = $"Total:   {rundOp(totalSum_CurrentBasket):C}";
                basketGridStyling();

            dataGridViewBasket.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dataGridViewBasket.MultiSelect = false;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Transparent; // Or any color matching cell's background
            dataGridView1.DefaultCellStyle.SelectionForeColor = dataGridView1.DefaultCellStyle.ForeColor; // Or any color matching cell's text

        }
        private void dataGridViewBasket_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                // Check if the click is on a valid cell
                if (e.RowIndex >= 0 && e.RowIndex < scannedProducts.Count)
                {



                if (scannedProducts[e.RowIndex].Antal > 1)
                {
                    totalSum_CurrentBasket -= scannedProducts[e.RowIndex].Pris;
                    scannedProducts[e.RowIndex].Antal--;
                    dataGridViewBasketRefresh();
                }
                else
                {
                    Product selectedProduct = scannedProducts[e.RowIndex];
                    totalSum_CurrentBasket -= selectedProduct.Pris;
                    scannedProducts.RemoveAt(e.RowIndex);
                    dataGridViewBasketRefresh();
                }
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






        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar >= '0' && e.KeyChar <= '9')
            {
                tbBarcode.AppendText(e.KeyChar.ToString()); 
            }
            else if (e.KeyChar == (char)Keys.Enter)
            {
                
                btnAddToBasket.PerformClick();
                e.Handled = true; 
            }
            else if (e.KeyChar == '\b' && tbBarcode.Text.Length > 0)
            {
                tbBarcode.Text = tbBarcode.Text.Remove(tbBarcode.Text.Length - 1);

              
                tbBarcode.SelectionStart = tbBarcode.Text.Length;
                e.Handled = true; 
            }
            else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
               
                e.Handled = true;
            }
        }

        private void UpdateProgressBar()
            {
              
                int integerProgress = (int)Math.Round(totalSumForDagenDisplay);
                label4.Text = $"Total i dag:    {totalSumForDagenDisplay:C}";
               
                if (integerProgress <= progressBar1.Maximum)
                {
                    progressBar1.Value = integerProgress;
                }
                else
                {
                    
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
            this.ActiveControl = null;
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
                Form4 form4 = new Form4(this,btnAddToBasket);
            
            form4.Show();
            this.Hide();
            this.ActiveControl = null;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5(this);

            form5.Show();   
            this.Hide();
            this.ActiveControl = null;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            multiply = true;
            this.ActiveControl = null;
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
            this.ActiveControl = null;
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

            dataGridView1.DataSource = categoryTotalPrices.ToList();
            Basket basket = new Basket
            {
                keyValuePairs =categoryTotalPrices,
               
            };
           
            overallTotal = categoryTotalPrices.Values.Sum();
            SQL_Sales.CreateSoldBasket(basket);
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
            this.ActiveControl = null;


        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tbBarcode_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridViewBasket_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridViewBasket_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {

        }

        private void button24_Click(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {

        }

        private void tbManuelPrice_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void btnOrderConfirmed_KeyUp(object sender, KeyEventArgs e)
        {
            this.ActiveControl = null;
        }
    }
}