﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FoodOrderingBot15dec.Dialogs
{
    [Serializable]
    internal class VegDialog : IDialog<object>
    {
        AddressDialog add = new AddressDialog();
        RootDialog root = new RootDialog();
        
        private const string YesOption = "Yes";

        private const string NoOption = "No";

        public static float Price, quantity;
        //public static float k;
       

        
        public static float n;
        public Task StartAsync(IDialogContext context)
        {
            string Query = "select ProductName,Price from FoodTable where CategoryID=1";
            DataTable dt = new DataTable();

           
            using (SqlConnection conn= new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, conn);
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {

                   string Name = dr["ProductName"].ToString();
                    Price = float.Parse(dr["Price"].ToString());
                    string dish = Name + " Cost: " + Price.ToString();
                    root.dishes.Add(dish);

                    

                }
                conn.Close();
            }
            PromptDialog.Choice(context, MessageReceivedAsync, root.dishes, "Please choose one dish  from the Menu", "Invalid Menu type. Please try again");
            return Task.CompletedTask;
        }
        

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            
            string choice = await result;
            RootDialog.newdishes.Add(choice);
               



            
            //  context.ConversationData.SetValue<List<string>>("dishesname", root.newdishes);
            //float k = choice.Length;
            string number = String.Empty;
            foreach (char str in choice)

            {

                if (char.IsDigit(str))

                    number += str.ToString();
                



            }

           
             n = float.Parse(number);
            
            await context.PostAsync($"You've selected {await result}");
            
            await context.PostAsync($"Please enter the quantity in integer only");
            context.Wait(this.TotalCost);


        }

        private async Task TotalCost(IDialogContext context, IAwaitable<object> result)
        {
            
            var qty = await result  as Activity;
            string qtys = qty.Text;
            quantity=float.Parse(qtys.ToString());
            //quantity = Convert.ToUInt32(qtys);
            Price = quantity * n;
            RootDialog.finalprice += Price;
            await context.PostAsync($"Your total Bill is {Price}");
            //context.ConversationData.SetValue<List<string>>("dishesname", root.dishes);
            //await context.PostAsync($"Enter Ok For Confirmation ");

            //context.Call(new AddressDialog(), this.ResumeAfterOptionDialog);

            this.GoBack(context);
            
        }
        public void GoBack(IDialogContext context)
        {
            PromptDialog.Choice(context, this.Redirect, new List<string>() { YesOption, NoOption }, "Do you want go back to the Menu?", "Not a valid options", 3);
        }
        private async Task Redirect(IDialogContext context, IAwaitable<string> result)

        {

            try

            {

                string optionSelected = await result;

                switch (optionSelected)

                {

                    case YesOption:

                        //context.Call(new RootDialog(), this.ResumeAfterOptionDialog);
                        root.ShowOptions(context);

                        break;

                    case NoOption:

                        //context.Call(new RootDialog(), this.ResumeAfterOptionDialog);
                        add.StartAsync(context);

                        break;

                }

            }

            catch (Exception e)

            {

                await context.PostAsync("Thanks");

                this.MessageReceivedAsync(context, result);

            }

        }
     

            //return itemName;

        }

        private Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}
