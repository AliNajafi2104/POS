using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace searchengine123
{
    public partial class Form2 : Form
    {
        
        
        public Form2(List<Product> scannedProducts)
        {
            InitializeComponent();
            this.scannedProducts = new List<Product>(scannedProducts);
            categoryTotalPrices = new Dictionary<string, double>();
            CalculateCategoryTotals();
            this.KeyPreview = true;
            this.KeyDown += Form2_KeyDown;
            tbDifferens.Focus();
            labelDifferens.Visible = false;
            dataGridViewKategorier.ClearSelection();
            dataGridViewKategorier.Columns.RemoveAt(0);
            tbDifferens.Focus();
            dataGridViewKategorier.ClearSelection();

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
            dataGridViewKategorier.DataSource = nonZeroCategories.Select(kv => new { Kategori = kv.Key, Pris = kv.Value }).ToList();

            // Calculate and display the overall total
            overallTotal = nonZeroCategories.Values.Sum();
            labelTotal.Text = $"Total: {overallTotal:C}"; // Assuming you want to display the total as currency
            tbDifferens.Focus();
            dataGridViewKategorier.ClearSelection();
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                this.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                differens();
            }
            dataGridViewKategorier.ClearSelection();
        }
        private void differens()
        {
            if (double.TryParse(tbDifferens.Text, out double modtagetKontant))
            {
                labelDifferens.Text = $"Differens: {(modtagetKontant - overallTotal).ToString():C} kr"; ;
            }
            labelDifferens.Visible = true;
            dataGridViewKategorier.ClearSelection();
        }
        private void btnLuk_Click(object sender, EventArgs e)
        {
            this.Close();
        }




        #region VARIABLES
        List<Product> scannedProducts;
        Dictionary<string, double> categoryTotalPrices;
        double overallTotal;
        #endregion

    }
}
