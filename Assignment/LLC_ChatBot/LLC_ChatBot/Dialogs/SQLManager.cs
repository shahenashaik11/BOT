using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace LLC_ChatBot.Dialogs
{
    public class SQLManager
    {
       // public static string VisitorID;

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static string GetName(string UserID)
        {

            //tring UserName;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("GetName", connection);

                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", UserID);

                    SqlParameter param1 = new SqlParameter();
                    param1.ParameterName = "@UserName";
                    param1.Direction = ParameterDirection.Output;
                    param1.SqlDbType = SqlDbType.NVarChar;
                    param1.Size = 50;

                    SqlParameter param2 = new SqlParameter();
                    param2.ParameterName = "@UserLocationID";
                    param2.Direction = ParameterDirection.Output;
                    param2.SqlDbType = SqlDbType.Int;

                    SqlParameter param3 = new SqlParameter();
                    param3.ParameterName = "@Department";
                    param3.Direction = ParameterDirection.Output;
                    param3.SqlDbType = SqlDbType.NVarChar;
                    param3.Size = 50;

                    command.Parameters.Add(param1);
                    command.Parameters.Add(param2);
                    command.Parameters.Add(param3);
                    command.ExecuteNonQuery();
                    UserData.UserName = command.Parameters["@UserName"].Value.ToString();
                    UserData.UserLocation = Convert.ToInt32(command.Parameters["@UserLocationID"].Value.ToString());
                    UserData.Department = command.Parameters["@Department"].Value.ToString();
                    connection.Close();

                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return UserData.UserName;
        }
        public static string GetResponse(string IntentName)
        {
            string Response;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("GetResponse", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@IntentName", IntentName);
                    SqlParameter param = new SqlParameter();
                    //  SqlParameter param1 = new SqlParameter();
                    param.ParameterName = "@Response";
                    //  param1.ParameterName = "@Hyperlink";

                    param.Direction = ParameterDirection.Output;
                    // param1.Direction = ParameterDirection.Output;
                    param.SqlDbType = SqlDbType.NVarChar;
                    //param1.SqlDbType = SqlDbType.NVarChar;
                    param.Size = 4000;
                    //param1.Size = 500;

                    command.Parameters.Add(param);
                    //command.Parameters.Add(param1);


                    command.ExecuteNonQuery();
                    Response = command.Parameters["@Response"].Value.ToString();
                    //Hyperlink= command.Parameters["@Hyperlink"].Value.ToString();
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return Response;
            // return Hyperlink;

        }

        internal static string GetDynamicResponse(string intent)

        {

            try

            {

                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {

                    connection.Open();

                    //UserData.UserLocation= 

                    SqlCommand command = new SqlCommand("GetDynamicResponse", connection);

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@IntentName", intent);
                    command.Parameters.AddWithValue("@CountryID", UserData.UserLocation);



                    SqlParameter paramResponse = new SqlParameter();

                    paramResponse.ParameterName = "@Response";

                    paramResponse.Direction = ParameterDirection.Output;

                    paramResponse.SqlDbType = SqlDbType.NVarChar;

                    paramResponse.Size = 4000;




                    //SqlParameter param = new SqlParameter();

                    //param.ParameterName = "@CountryID";

                    //param.Direction = ParameterDirection.Output;

                    //param.SqlDbType = SqlDbType.Int;



                    command.Parameters.Add(paramResponse);
                    //command.Parameters.Add(param);



                    // if (UserInfo.UserLocation.Equals("uk"))

                    //  {


                    //  }

                    //else

                    //{

                    //    command.Parameters.AddWithValue("@CountryID", 2);

                    //}

                    command.ExecuteNonQuery();

                    string response = command.Parameters["@Response"].Value.ToString();
                    //int CountryID = Convert.ToInt32(command.Parameters["@CountryID"]);

                    connection.Close();

                    return response;

                }

            }

            catch (Exception e)

            {

                throw e;

            }

        }
        public static List<string> DisplayBuilding()
        {
            string Query = "select BuildingName from BuildingTable";
            DataTable data = new DataTable();
            List<string> buildings = new List<string>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);
                adapter.Fill(data);

                foreach (DataRow dr in data.Rows)
                {
                    string BuildingName = dr["BuildingName"].ToString();
                    buildings.Add(BuildingName);
                }
            }

            return buildings;

        }
        public static List<string> ChooseIssueCategory()
        {
            string Query = "select CategoryName from IssueCategory";
            DataTable data = new DataTable();
            List<string> category = new List<string>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);
                adapter.Fill(data);

                foreach (DataRow dr in data.Rows)
                {
                    string CategoryName = dr["CategoryName"].ToString();
                    category.Add(CategoryName);
                }
            }

            return category;

        }

        public static List<string> GetIssueName(string Response)
        {
            string Issue;
            string Query;

            List<string> Issues = new List<string>();
            Query = "select IssueName from IssueTable IT join IssueCategory IC on IT.CategoryID=IC.CategoryID WHERE CategoryName='" + Response + "'";
            DataTable data = new DataTable();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);

                adapter.Fill(data);
                foreach (DataRow dr in data.Rows)
                {
                    Issue = dr[0].ToString();
                    Issues.Add(Issue);
                }
            }
            return Issues;
        }


        public static string GetITQuestions(int ID)
        {
            string Response;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetITQuestions", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", ID);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Responses";
                param.Direction = ParameterDirection.Output;
                param.SqlDbType = SqlDbType.NVarChar;
                param.Size = 4000;

                command.Parameters.Add(param);

                command.ExecuteNonQuery();
                Response = command.Parameters["@Responses"].Value.ToString();
            }
            return Response;
        }

        public static string GetVisitorBadgeQuestions(int ID)
        {
            string Response;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetVisitorQuestions", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ID", ID);

                SqlParameter param = new SqlParameter();
                param.ParameterName = "@Responses";
                param.Direction = ParameterDirection.Output;
                param.SqlDbType = SqlDbType.NVarChar;
                param.Size = 4000;

                command.Parameters.Add(param);

                command.ExecuteNonQuery();
                Response = command.Parameters["@Responses"].Value.ToString();
            }
            return Response;
        }

        public static void GetConversationData(string UserID,string UserResponse, string BotResponse)
        {
            string Response;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("GetConversation", connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@UserID", UserID);
                command.Parameters.AddWithValue("@UserResponse", UserResponse);
                command.Parameters.AddWithValue("@BotResponse", BotResponse);

                command.ExecuteNonQuery();
            }
        }
        public static string InsertVisitorData(string UserID,string ContactName,List<string> VisitorName,List<string> CompanyName,bool NeedParking,int NoOfParkingTicket,string BuildingName,string StartDate,string EndDate,string Comment)
        {
            try
            {
                for (int i = 0; i < VisitorName.Count; i++)
                {
                    using (SqlConnection connection = new SqlConnection(ConnectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand("InsertVisitorData", connection);

                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@ContactName", ContactName);

                        SqlParameter param1 = new SqlParameter();
                        param1.ParameterName = "@VisitorID";
                        param1.Direction = ParameterDirection.Output;
                        param1.SqlDbType = SqlDbType.Int;

                        command.Parameters.AddWithValue("@VisitorName", VisitorName[i]);
                        command.Parameters.AddWithValue("@Company", CompanyName[i]);
                        command.Parameters.AddWithValue("@NeedParking", NeedParking);
                        command.Parameters.AddWithValue("@NoOfParkingTicket", NoOfParkingTicket);
                        command.Parameters.AddWithValue("@BuildingName", BuildingName);
                        command.Parameters.AddWithValue("@StartDate", StartDate);
                        command.Parameters.AddWithValue("@EndDate", EndDate);
                        command.Parameters.AddWithValue("@Comment", Comment);

                        command.Parameters.Add(param1);

                        command.ExecuteNonQuery();

                        
                        VisitorData.VisitorID += command.Parameters["@VisitorID"].Value.ToString() + ",";
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return VisitorData.VisitorID;
        }
    }
}