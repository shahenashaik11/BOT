﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FoodOrderingBot15dec.Dialogs
{
    [Serializable]
    internal class VegDialog : IDialog<object>
    {
        //public static string Query= ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static float Price;
        
        public  float quantity;
        public static float n;
        public Task StartAsync(IDialogContext context)
        {
            string Query = "select * from FoodTable where CategoryID=1";
            DataTable dt = new DataTable();

            List<string> dishes = new List<string>();
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {

                    string Name = dr["ProductName"].ToString();
                    Price = float.Parse(dr["Price"].ToString());
                    string dish = Name + " Cost: " + Price.ToString();
                   
                    dishes.Add(dish);
                }
                connection.Close();
            }
            PromptDialog.Choice(context, MessageReceivedAsync, dishes, "Please choose one dish  from the Menu", "Invalid Menu type. Please try again");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            string choice = await result;
           float number = 0.0f;
            foreach (float s in choice)
            {
                number += s;
                
            }
            n = number;
            await context.PostAsync($"You've selected {await result}");
            await context.PostAsync($"Please enter the quantity in integer only");
           
            context.Wait(this.TotalCost);

        }

        private async Task TotalCost(IDialogContext context, IAwaitable<object> result)
        {
            //var activity = await result as Activity;
            //string choice = activity.Text;
            var qty = await result as Activity;
            string qtys = qty.Text;
            quantity = Convert.ToUInt32(qtys);
            Price = quantity * n;
            await context.PostAsync($"Your total Bill is {Price}");
            
           
        }
    }
}