using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BOTFoodLUIS.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public string Name;
        public static int orderID;
        public const string Family = "👪";
        public const string X = "❌";
        public const string Ramen = "🍜";
        public const string Spaghetti = "🍝";
        public const string Fork_And_Knife = "🍴";

        public Task StartAsync(IDialogContext context)

        {

            context.Wait(MessageReceivedAsync);



            return Task.CompletedTask;

        }



        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)

        {

            PromptDialog.Text(context, NameEntered, @"May I know your Good Name, Please?");

        }



        public async Task NameEntered(IDialogContext context, IAwaitable<string> result)

        {

            Name = await result;



            var UserName = string.Empty;

            bool isUserNameAvailable = false;

            context.UserData.TryGetValue("NameKey",out UserName);

            if (string.IsNullOrEmpty(UserName))

            {

                UserName = Name;

                context.UserData.SetValue("NameKey", UserName);

                using (SqlConnection connection = new SqlConnection(ConnectionString))

                {

                    connection.Open();

                    SqlCommand command = new SqlCommand("GetName", connection);

                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@Name", UserName);

                    command.Connection = connection;

                    command.ExecuteNonQuery();

                }

            }

            if (isUserNameAvailable)

            {

                await context.PostAsync($"Hi {UserName}, Welcome" + Family + "Back! to SpeedyFood" + Spaghetti + ". Order and Eat great food." + Ramen);

            }

            else

            {

                await context.PostAsync($@"{await result}! Welcome to SpeedyFood." + Spaghetti + " Order and Eat great food." + Ramen);

            }

            await context.PostAsync($"What would you like to have?" + Fork_And_Knife);

            this.CaptureID(context);

            Category(context);

        }



        private void CaptureID(IDialogContext context)

        {

            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {

                connection.Open();

                SqlCommand command2 = new SqlCommand("Select OrderID from ShahenaUsers where UserID='" + Name + "'", connection);

                SqlDataReader reader = command2.ExecuteReader();

                while (reader.Read())

                {

                    orderID = Convert.ToInt32(reader[0]);

                }

            }

        }



        public static void Category(IDialogContext context)

        {

            string Query = "select CategoryName from Category";

            DataTable data = new DataTable();

            List<string> Items = new List<string>();



            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {

                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);

                adapter.Fill(data);

                foreach (DataRow dr in data.Rows)

                {

                    string Item = dr[0].ToString();

                    Items.Add(Item);

                }

            }

            Category category = new Category();

            PromptDialog.Choice(context, category.MessageReceivedAsync, Items, "Please Pick a Food Category!!", "Invalid Menu type. Please try again");

        }





    }

}
