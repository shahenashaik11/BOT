using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FoodOrderingBotLUIS.Dialogs
{
    public class SQLManager
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static string Query;

        public static float GetItems(string Item)
        {
            float Price = 0;
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
                    param.SqlDbType = SqlDbType.Float;
                    command.Parameters.Add(param);
                    command.ExecuteNonQuery();
                    Price = float.Parse(command.Parameters["@Price"].Value.ToString());
                    connection.Close();
                }
                return Price;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}