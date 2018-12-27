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

namespace BOTFoodLUIS.Dialogs
{
    [Serializable]

    internal class Category : IDialog<object>

    {

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        List<Cart> cartlist = new List<Cart>();

        static string Food;

        static int ItemPrice;

        static string url;

        public static string menuSelected;



        public Task StartAsync(IDialogContext context)

        {

            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;

        }



        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)

        {

            var optionSelected = await result;



            string Query = "select ft.ProductName,ft.Price,ft.URL from ShahenaFoodTable ft join ShahenaCategory c ON c.CategoryId=ft.CategoryId where c.CategoryName='" + optionSelected + "'";

            DataTable data = new DataTable();



            using (SqlConnection connection = new SqlConnection(ConnectionString))

            {

                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(Query, connection);

                adapter.Fill(data);

                foreach (DataRow dr in data.Rows)

                {

                    Cart cart = new Cart();

                    Food = dr[0].ToString();

                    ItemPrice = Convert.ToInt16(dr[1]);

                    url = dr[2].ToString();

                    cart.FoodItems = Food.ToString();

                    cart.Price = Convert.ToInt16(ItemPrice);

                    cart.URL = url.ToString();

                    cartlist.Add(cart);

                }

            }



            var resultMessage = context.MakeMessage();

            resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            resultMessage.Attachments = new List<Attachment>();



            foreach (var i in cartlist)

            {

                HeroCard hero = new HeroCard()

                {

                    Title = "Item: " + i.FoodItems,

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

                            Value=i.FoodItems

                        }

                    }

                };

                resultMessage.Attachments.Add(hero.ToAttachment());

            }



            await context.PostAsync(resultMessage);

            context.Wait(GetPrice);

        }



        public async Task GetPrice(IDialogContext context, IAwaitable<object> result)

        {

            var item = await result as Activity;

            string itemName = item.Text;



            using (HttpClient httpClient = new HttpClient())

            {

                LuisResponse Data = new LuisResponse();



                var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/9ca96fe0-b35d-4acf-a4c9-a30dd4159afe?staging=true&verbose=true&timezoneOffset=-360&subscription-key=6889ab41b2314eb7b27eb01dff3fe161&q="

                + System.Uri.EscapeDataString(itemName));

                Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                itemName = null;

                var intent = Data.topScoringIntent.intent;

                var score = Data.topScoringIntent.score;

                //Data.entities.OrderBy(o => o.startIndex);



                if (intent == "SelectItems" && score > 0.8)

                {

                    foreach (var entity in Data.entities)

                    {

                        itemName += entity.entity;

                    }

                    Sorting.InsertIntoCart(Data, context);

                }

            }

        }

    }
}