using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Mysqlx;

namespace searchengine123
{
    public partial class Form3 : Form
    {
        public Form3(double barcode, Form form)
        {
            barcode_ = barcode;
            InitializeComponent();
           this.UseWaitCursor = true;
            if (barcode > 0)
            {
                tbOpretStregkode.Text = barcode.ToString();
            }
           
           
            this.WindowState = FormWindowState.Maximized;
            tbOpretVareNavn.Focus();
            form_ = form;
        }
        double barcode_;
        Form form_;





        #region BUTTONS
        private void btnClose_Click(object sender, EventArgs e)
        {
            form_.Show();
            this.Close();
        }
        private void btnOpretVare_Click(object sender, EventArgs e)
        {
           if(tbOpretVareNavn.Text!=null && tbOpretStregkode.Text != null && tbPris.Text != null && tbOpretKategori.Text != null)
            {
                try
                {
                    int type = 0;
                    if (radioOpretKgPris.Checked)
                    {
                        type = 1;


                    }
                    else if (radioOpretStykPris.Checked)
                    {
                        type = 2;
                    }

                   

                    SQL_Product.Create(tbOpretVareNavn.Text, tbOpretStregkode.Text, tbPris.Text, tbOpretKategori.Text, type);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK);
                }
                form_.Show();
                this.Close();

            }
           

        }
        private void btnRedigerSøg_Click(object sender, EventArgs e)
        {
            if (tbRedigerStregkodeSøg != null)
            {
                try
                {
                    Product product = SQL_Product.Read(tbRedigerStregkodeSøg.Text);
                    if (product != null)
                    {
                        tbRedigerStregkode.Text = product.Stregkode.ToString();
                        tbVareNavn.Text = product.Vare.ToString();
                        tbRedigerKategori.Text = product.Kategori.ToString();
                        tbRedigerPris.Text = product.Pris.ToString();

                        if (product.Ingen_stregkodemærkning == "1")
                        {
                            radioRedigerStykPris.Checked = true;

                        }
                        else if (product.Ingen_stregkodemærkning == "2")
                        {
                            radioRedigerKgPris.Checked = true;

                        }
                        else if (product.Ingen_stregkodemærkning == "0" || product.Ingen_stregkodemærkning == null)
                        {
                            radioRedigerStregkode.Checked = true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("The barcode doesnt exist");
                        tbRedigerStregkodeSøg.Clear();
                    }
                }
                catch (Exception ex) { MessageBox.Show("error"); };
                
            }
        }
        private void btnRedigerUpdate_Click(object sender, EventArgs e)
        {
            int type = 0;
            if(radioRedigerStykPris.Checked) { type = 1; }
            else if (radioRedigerKgPris.Checked) { type = 2; }
            try
            {
                SQL_Product.Update(tbVareNavn.Text, tbRedigerStregkode.Text, tbRedigerPris.Text, tbRedigerKategori.Text,type);
                MessageBox.Show("Produktet er opdateret");
            }
            catch
            {
                MessageBox.Show("error");
            }
            
        }
        private void btnRedigerDelete_Click(object sender, EventArgs e)
        {
            if (tbOpretVareNavn.Text != null && tbRedigerStregkode.Text != null && tbPris.Text != null && tbOpretKategori.Text != null)
            {
                try
                {
                    SQL_Product.Delete(tbRedigerStregkode.Text.ToString());
                    MessageBox.Show("product deleted");
                    tbVareNavn.Clear();
                    tbRedigerStregkode.Clear();
                    tbRedigerPris.Clear();
                    tbRedigerKategori.Clear();
                    
                }
                catch { MessageBox.Show("error in deleting"); };
            }
            else
            {
                MessageBox.Show("hello");
            }
        }
        #endregion

        private void Form3_Load(object sender, EventArgs e)
        {
            this.UseWaitCursor = true;
            if (barcode_>0)
            {
                tbOpretVareNavn.Focus();
            }
            else
            {
                tbOpretStregkode.Focus();
            }
          
        }
    }
}

