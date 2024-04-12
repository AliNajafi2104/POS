using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using searchengine123;
using searchengine123.Models;

namespace searchengine123.Back_end
{
    public static class SQL_Sales
    {

        internal static int QuantitySoldProduct(string Stregkode)
        {
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT AntalSolgt FROM solgte_varer WHERE Stregkode = @Value1 AND date=CURDATE()", cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@Value1", Stregkode);
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        // Handle case when no records were found for the provided Stregkode
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error", ex);
            }
        }




        public static double ReadDailySale()
        {
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT Total FROM total_for_dagen WHERE date = CURDATE()", cn))
                {
                    cn.Open();
                    object totalValue = cmd.ExecuteScalar();
                    return (totalValue != null && totalValue != DBNull.Value) ? Convert.ToDouble(totalValue) : 0;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error getting total.", ex);
            }
        }
        public static void UpdateDailySale(double currentBasket)
        {
            try
            {
                double totalSqlDate = ReadDailySale();
                string query = (totalSqlDate == 0)
                    ? "INSERT INTO total_for_dagen (Total, date) VALUES (@Value1, CURRENT_DATE())"
                    : "UPDATE total_for_dagen SET Total = @Value1 WHERE date = CURRENT_DATE()";
                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, cn))
                {
                    cn.Open();
                    cmd.Parameters.AddWithValue("@Value1", currentBasket + totalSqlDate);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error saving daily total.", ex);
            }
        }

        public static List<DateSale> ReadAllDailySales()
        {
            List<DateSale> weeklySale = new List<DateSale>();
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM total_for_dagen WHERE date <= CURDATE() ;", cn))
                {
                    cn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateSale datesale = new DateSale
                            {
                                TotalSale = reader.GetDouble("Total"),
                                Date = reader.GetDateTime("date")

                            };
                            weeklySale.Add(datesale);

                        }
                    }
                }
                return weeklySale;

            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error getting total.", ex);
            }
        }

        public static void RegisterProductSale(List<Product> productsSold)
        {
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                {
                    cn.Open();
                    for (int i = 0; i < productsSold.Count(); i++)
                    {
                        if (productsSold[i].Stregkode == null)
                        { }
                        else
                        {
                            if (SQL_Sales.QuantitySoldProduct(productsSold[i].Stregkode) > 0)
                            {
                                using (MySqlCommand cmd = new MySqlCommand("UPDATE solgte_varer SET AntalSolgt = AntalSolgt + 1 WHERE Stregkode = @Value1 AND DATE(date) = CURDATE()", cn))
                                {
                                    cmd.Parameters.AddWithValue("@Value1", productsSold[i].Stregkode);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                using (MySqlCommand cmd = new MySqlCommand("INSERT INTO solgte_varer (Stregkode, date) VALUES (@Value1, CURDATE())", cn))
                                {
                                    cmd.Parameters.AddWithValue("@Value1", productsSold[i].Stregkode);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error", ex);
            }

        }

        public static List<Product> ReadLatestSold()
        {
            List<Product> LatestSold = new List<Product>();
            try
            {

                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT sv.*\r\nFROM solgte_varer sv\r\nJOIN (\r\n    SELECT DISTINCT date \r\n    FROM solgte_varer \r\n    WHERE date >= DATE_SUB(CURDATE(), INTERVAL 7 DAY) \r\n    ORDER BY date DESC\r\n    LIMIT 7\r\n) recent_dates ON sv.date = recent_dates.date\r\nORDER BY sv.AntalSolgt DESC\r\nLIMIT 20;\r\n", cn))
                {
                    cn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string stregkode = reader.GetString("Stregkode");
                            Product existingProduct = LatestSold.FirstOrDefault(p => p.Stregkode == stregkode);
                            if (existingProduct != null)
                            {
                                // Increment the quantity if the product is already in the list
                                existingProduct.Antal += reader.GetInt32("AntalSolgt");
                            }
                            else
                            {

                                // If the product is not in the list, add it
                                Product newProduct = new Product
                                {
                                    Vare = SQL_Product.Read(stregkode).Vare,
                                    Stregkode = stregkode,
                                    Antal = reader.GetInt32("AntalSolgt")
                                };
                                LatestSold.Add(newProduct);
                            }
                        }

                    }
                    LatestSold = LatestSold.OrderByDescending(p => p.Antal).ToList();

                }

                return LatestSold;
            }

            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error", ex);
            }
        }

        public static List<Udgift> ReadExpenses()
        {
            List<Udgift> Expenses = new List<Udgift>();
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                using (MySqlCommand cmd = new MySqlCommand("SELECT * FROM udgifter;", cn))
                {
                    cn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Udgift datesale = new Udgift
                            {
                                name = reader.GetString("Navn på udgift"),
                                expense = reader.GetDouble("Udgift i kr"),
                                year = reader.GetInt32("år")

                            };
                            Expenses.Add(datesale);

                        }
                    }
                }
                return Expenses;

            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error", ex);
            }
        }


        public static void CreateSoldBasket(Basket basket)
        {
            try
            {
                using (MySqlConnection cn = SQL_Product.GetConnection())
                {
                    cn.Open();

                    foreach(Dictionary<string,double> dictionary in basket.keyValuePairs)
                    {
                        using (MySqlCommand cmd = new MySqlCommand("INSERT INTO solgte_kurve (Kategori,Total,date) VALUES (@Value1,@Value2,@Value3)", cn))
                        {
                            cmd.Parameters.AddWithValue("@Value1", dictionary.Keys);
                            cmd.Parameters.AddWithValue("@Value2", dictionary.Values);
                            cmd.Parameters.AddWithValue("@Value3", basket.time);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    

                 
                }
            }
            catch (Exception ex)
            {
                // Handle the exception or rethrow it as needed
                throw new Exception("Error", ex);
            }

        }


    }
}
