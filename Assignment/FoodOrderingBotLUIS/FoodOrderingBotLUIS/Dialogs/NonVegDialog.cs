using Microsoft.Bot.Builder.Dialogs;
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
    internal class NonVegDialog : IDialog<object>
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
            //context.Wait(MessageReceivedAsync);
            //PromptDialog.Choice(context, MessageReceivedAsync, root.dishes, "Please choose one dish  from the Menu", "Invalid Menu type. Please try again");

        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var option = await result as Activity;
            string choice = option.Text;
           
            RootDialog.newdishes.Add(choice);
            //  context.ConversationData.SetValue<List<string>>("dishesname", root.newdishes);
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
                    if (intent == "SelectNonVegDishes" && score > 0.8)
                    {
                        foreach (var entity in Data.entities)
                        {
                            choice += entity.entity;

                        }
                        LUIS.InsertIntoCart(Data,context);
                    }
                }
                catch (Exception e)
                {

                    context.PostAsync("NonVegException");
                }

                await this.NonVegy(context, choice);
            }
            string number = String.Empty;
            foreach (char str in choice)

            {

                if (char.IsDigit(str))

                    number += str.ToString();



            }

            //foreach (float s in choice)
            //{
            //    number += s;


            //}

            n = float.Parse(number);
            await context.PostAsync($"You've selected {await result}");
            await context.PostAsync($"Please enter the quantity in integer only");
            context.Wait(this.TotalCost);

        }

        private async Task TotalCost(IDialogContext context, IAwaitable<object> result)
        {
            var qty = await result as Activity;
            string qtys = qty.Text;
            // quantity = Convert.ToUInt32(qtys);
            quantity = float.Parse(qtys.ToString());
            Price = quantity * n;
            RootDialog.finalprice += Price;
            await context.PostAsync($"Your total Bill is {Price}");

            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            //{
            //    string query = "INSERT INTO OrderDetails VALUES(@OrderID,@ProductName,@Quantity, @Price)";
            //    SqlCommand cmd = new SqlCommand(query);

            //    cmd.Parameters.AddWithValue("@OrderID", RootDialog.OrderID);
            //    cmd.Parameters.AddWithValue("@Quantity", quantity);
            //    cmd.Parameters.AddWithValue("@Price", Price);
            //    cmd.Parameters.AddWithValue("@ProductName", Name);

            //    cmd.Connection = con;
            //    con.Open();
            //    cmd.ExecuteNonQuery();
            //    con.Close();

            //}
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

        private Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
        private async Task NonVegy(IDialogContext context, string result)
        {
            var Item = result;

            string Entity = Item;

            await context.PostAsync($"{Entity}");
        }
    }
}