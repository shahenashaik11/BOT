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

namespace FoodOrderingBotLUIS.Dialogs
{
    [Serializable]
    internal class NonVegDialog : IDialog
    {
        AddressDialog add = new AddressDialog();
        RootDialog root = new RootDialog();

        private const string YesOption = "Yes";
        static List<Cart> cartlist = new List<Cart>();
        private const string NoOption = "No";
        static string Food;
        static int ItemPrice;
        static string url;

        public static float Price, quantity;
        public static float n;
        public async Task StartAsync(IDialogContext context)
        {
            string Query = "select ProductName,Price,URL from ShahenaFoodTable where CategoryID=2";
            DataTable dt = new DataTable();


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(Query, conn);
                adapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    Cart cart = new Cart();
                    Food = dr[0].ToString();
                    ItemPrice = Convert.ToInt16(dr[1]);
                    url = dr[2].ToString();
                    cart.ProductName = Food.ToString();
                    cart.Price = Convert.ToInt16(ItemPrice);
                    cart.URL = url.ToString();
                    cartlist.Add(cart);


                    //Cart cart = new Cart();
                    //cart.ProductName= dr["ProductName"].ToString();
                    //cart.Price = float.Parse(dr["Price"].ToString());
                    //cart.URL = url.ToString(); ;


                    //root.dishes.Add(dish);
                }

                var resultMessage = context.MakeMessage();
                resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();

                foreach (var i in cartlist)
                {
                    HeroCard hero = new HeroCard()
                    {
                        Title = "Item: " + i.ProductName,
                        Subtitle = "Price: Rs. " + i.Price.ToString(),
                        Images = new List<CardImage>()
                    {
                        new CardImage() {Url=i.URL }
                    },
                        Buttons = new List<CardAction>()
                    {
                        new CardAction()
                        {
                            Title="Add To Cart",
                            Type=ActionTypes.ImBack,
                            Value=i.ProductName + i.Price
                        }
                    }
                    };
                    resultMessage.Attachments.Add(hero.ToAttachment());
                }

                await context.PostAsync(resultMessage);
                context.Wait(MessageReceivedAsync);

            }


        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var option = await result as Activity;
            string choice = option.Text;
            //string choice2 = await choice.Text;
            RootDialog.newdishes.Add(choice);
            using (HttpClient httpClient = new HttpClient())
            {
                LuisResponse Data = new LuisResponse();
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/684384d6-f50b-4f66-b628-270bf84b2980?staging=true&verbose=true&timezoneOffset=-360&subscription-key=59e472415afd44a595605a762b8c81ab&q="
                    + System.Uri.EscapeDataString(choice));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                    choice = null;
                    var intent = Data.topScoringIntent.intent;
                    var score = Data.topScoringIntent.score;
                    Data.entities.OrderBy(o => o.startIndex);
                    if (intent == "SelectNonVegDishes" && score > 0.8)
                    {
                        foreach (var entity in Data.entities)
                        {
                            choice += entity.entity;

                            List<Cart> CartData = new List<Cart>();

                            context.ConversationData.TryGetValue("UserCart", out CartData);

                            CartData = LUIS.CartList;

                            context.ConversationData.SetValue("UserCart", CartData);


                        }
                        LUIS.InsertIntoCart(Data, context);
                        this.GoBack(context);
                    }
                    else

                    {


                    }
                }
                catch (Exception e)
                {
                    context.PostAsync("NonVegException");

                }


            }







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


                        root.ShowOptions(context);

                        break;

                    case NoOption:


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


        private Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

    }
}
