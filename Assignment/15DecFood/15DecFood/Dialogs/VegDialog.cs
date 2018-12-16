using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.IdentityModel.Protocols;

namespace _13DecFood.Dialogs
{
    internal class VegDialog : IDialog<object>
    {
        RootDialog rootdialog = new RootDialog();
        static float Price;
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;


        public Task StartAsync(IDialogContext context)
        {
            string Query = "select * from FoodTable where CategoryID=1";
            DataTable data = new DataTable();
            List<string> dishes = new List<string>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);
                adapter.Fill(data);
                foreach (DataRow dr in data.Rows)
                {
                    string dish = dr[1].ToString() + " Rs. " + dr[2].ToString();
                    dishes.Add(dish);
                }
            }
            PromptDialog.Choice(context, MessageReceivedAsync, dishes, "Please choose one dish from the Menu", "Invalid Menu type. Please try again",3);
            return Task.CompletedTask;

        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result ;
                string number = string.Empty;
                foreach (char str in optionSelected)
                {
                    if (char.IsDigit(str))
                        number += str.ToString();
                }
                Price = float.Parse(number);
                 await this.EnterQuantity(context);
            }
            catch (Exception e)
            {
                await context.PostAsync("Thanks");
                this.MessageReceivedAsync(context,result);

                
            }
            //context.Wait(this.MessageReceivedAsync);
        }
        private async Task EnterQuantity(IDialogContext context)
        {
            PromptDialog.Text(context, Calculation, "Enter The Quantity of Food You Selected", "Enter Valid option");
        }
        private async Task Calculation(IDialogContext context, IAwaitable<string> result)
        {
            var Quantity = await result;
            float Amount = Price * Convert.ToInt32(Quantity);
            await context.PostAsync($"Total amount to be paid: " + Amount);
            rootdialog.ShowOptions(context);
            context.Wait(AddMore);
        }

        private async Task AddMore(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Choice(context, Answer, new List<string>() { "Yes", "No" }, "Do you want to add more?", "Invalid Menu type. Please try again");

        }
        private async Task Answer(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string choice = await result;
                switch (choice)
                {
                    case "Yes":
                        rootdialog.ShowOptions(context);
                        break;
                    case "No":
                        PromptDialog.Choice(context, Confirmation, new List<string>() { "Yes", "No" }, "Do you want to confirm your order?", "Invalid Menu type. Please try again");
                        break;
                }
            }
            catch (Exception e)
            {
                await context.PostAsync("Thanks");
                this.MessageReceivedAsync(context, result);

                // context.Wait(this.MessageReceivedAsync);
            }
            //context.Wait(this.MessageReceivedAsync);
        }
        private async Task Confirmation(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string choice = await result;
                switch (choice)
                {
                    case "Yes":

                        break;
                    case "No":
                        rootdialog.ShowOptions(context);
                        break;
                }
            }
            catch ( Exception e)
            {
                await context.PostAsync("Thanks");

                this.MessageReceivedAsync(context,result);

            }
        }










    }
}