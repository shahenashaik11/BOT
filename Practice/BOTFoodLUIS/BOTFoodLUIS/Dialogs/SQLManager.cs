using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BOTFoodLUIS.Dialogs
{
    [Serializable]

    public class SQLManager

    {

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static string Query;



        public static int GetItems(string Item)

        {

            int Price = 0;

            try

            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {

                    connection.Open();

                    SqlCommand command = new SqlCommand("GetItem", connection);

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Item", Item);

                    SqlParameter param = new SqlParameter();

                    param.ParameterName = "@Price";

                    param.Direction = ParameterDirection.Output;

                    param.SqlDbType = SqlDbType.Int;

                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();

                    Price = int.Parse(command.Parameters["@Price"].Value.ToString());

                    connection.Close();

                }

                return Price;

            }

            catch (Exception e)

            {

                throw e;

            }

        }



        public static void AddToDB(List<Items> itemlist)

        {

            for (var i = 0; i < itemlist.Count; i++)

            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {

                    using (SqlCommand command = new SqlCommand("InsertDetails"))

                    {

                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@ProductName", itemlist[i].FoodItem.ToString());

                        command.Parameters.AddWithValue("@Price", Convert.ToInt16(itemlist[i].Price.ToString()));

                        command.Parameters.AddWithValue("@OrderID", Convert.ToInt32(RootDialog.orderID.ToString()));

                        command.Parameters.AddWithValue("@Quantity", Convert.ToInt16(itemlist[i].Quantity.ToString()));

                        command.Connection = connection;

                        connection.Open();

                        command.ExecuteNonQuery();

                        connection.Close();

                    }

                }

                using (SqlConnection con = new SqlConnection(ConnectionString))

                {

                    using (SqlCommand cmd = new SqlCommand("GetTotalAmount"))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Connection = con;

                        con.Open();

                        cmd.ExecuteNonQuery();

                        con.Close();

                    }

                }

            }

        }

    }
   
}