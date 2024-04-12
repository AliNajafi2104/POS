using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;

using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Math.EC.Multiplier;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace searchengine123
{
    public class SQL_Product
    {
        #region INTERNAL
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString);
        }
        public static Product CreateProduct(MySqlDataReader reader)
        {
            return new Product
            {
                Vare = reader["Vare"].ToString(),
                Stregkode = reader["Stregkode"].ToString(),
                Pris = Convert.ToDouble(reader["Pris"]),
                Kategori = reader["Kategori"].ToString()
            };
        }
        #endregion

        
        


        #region CRUD_operations
        public static void Create(string vare, string stregkode, string pris, string kategori, int type = 0)
        {
            try
            {
                using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
                {
                    cn.Open();
                    string selectQuery = "INSERT INTO alle_varer (Vare,Stregkode,Pris,Kategori, Ingen_stregkodemærkning) VALUES (@Value1,@Value2,@Value3,@Value4,@Value5)";
                    using (MySqlCommand command = new MySqlCommand(selectQuery, cn))
                    {
                        command.Parameters.AddWithValue("@Value1", vare);
                        command.Parameters.AddWithValue("@Value2", stregkode);
                        command.Parameters.AddWithValue("@Value3", pris);
                        command.Parameters.AddWithValue("@Value4", kategori);
                        command.Parameters.AddWithValue("@Value5", type);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error creating product.", ex);
            }
        }
        public static Product Read(string barcode)
        {
            try
            {
                using (MySqlConnection cn = GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM alle_varer WHERE Stregkode = @Barcode", cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@Barcode", barcode);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Check if there are any rows returned
                        if (reader.Read())
                        {

                            Product product = new Product {
                                Vare = reader["Vare"].ToString(),
                                Stregkode = reader["Stregkode"].ToString(),
                                Pris = Convert.ToDouble(reader["Pris"]),
                                Kategori = reader["Kategori"].ToString(),
                                Ingen_stregkodemærkning = reader["Ingen_stregkodemærkning"].ToString()

                            };
                            return product;
                        }
                        else
                        {
                            // No product found, return null
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error finding product.", ex);
            }
        }
        public static void Update(string vare, string stregkode, string pris, string kategori, int type = 0)
        {
            try
            {

                using (MySqlConnection cn = new MySqlConnection(ConfigurationManager.ConnectionStrings["cn"].ConnectionString))
                {
                    cn.Open();
                    string selectQuery = "UPDATE alle_varer SET Vare = @Value1, Pris = @Value3, Kategori = @Value4, Ingen_stregkodemærkning = @Value5 WHERE Stregkode = @Value2";
                    using (MySqlCommand command = new MySqlCommand(selectQuery, cn))
                    {
                        command.Parameters.AddWithValue("@Value1", vare);
                        command.Parameters.AddWithValue("@Value2", stregkode);
                        command.Parameters.AddWithValue("@Value3", pris);
                        command.Parameters.AddWithValue("@Value4", kategori);
                        command.Parameters.AddWithValue("@Value5", type);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error creating product.", ex);
            }
        }
        public static void Delete(String stregkode)
        {
            try
            {
                using (MySqlConnection cn = GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("DELETE FROM alle_varer WHERE Stregkode = @Barcode", cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@Barcode", stregkode);
                    cmd.ExecuteNonQuery();
                    
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error finding product.", ex);
            }
        }
        #endregion


        public static List<Product> FetchProducts()
        {
            try
            {
                List<Product> products = new List<Product>();
                using (MySqlConnection cn = GetConnection())
                using (MySqlDataAdapter da = new MySqlDataAdapter("SELECT * FROM alle_varer", cn))
                {
                    cn.Open();
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        Product product = new Product
                        {
                            Vare = row["Vare"].ToString(),
                            Pris = Convert.ToDouble(row["Pris"]),
                            Stregkode = row["Stregkode"].ToString(),
                            Kategori = row["Kategori"].ToString(),
                            Ingen_stregkodemærkning = row["Ingen_stregkodemærkning"].ToString()
                        };
                        products.Add(product);
                    }
                }
                return products;
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error fetching products.", ex);
            }
        }

      
  
       

        
    }
}