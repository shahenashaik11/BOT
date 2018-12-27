using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BOTFoodLUIS.Dialogs
{
    public class Sorting
    {
        public static List<Items> itemlist = new List<Items>();

        public static int TotalAmount;

        public static int Amount;



        public static void InsertIntoCart(LuisResponse Data, IDialogContext context)

        {

            Data.entities = Data.entities.OrderBy(o => o.startIndex).ToArray();



            for (int i = 0; i < Data.entities.Length; i++)

            {

                Items item = new Items();

                if (i == 0 && !(Data.entities[i].type.Equals("builtin.number")))

                {

                    item.Quantity = 1;

                }

                else if (i != 0 && !(Data.entities[i].type.Equals("builtin.number")))

                {

                    item.Quantity = 1;

                }

                else

                {

                    item.Quantity = Convert.ToInt32(Data.entities[i].entity.ToString());

                    i++;

                }



                item.FoodItem = Data.entities[i].entity.ToString();

                item.Price = Convert.ToInt32(SQLManager.GetItems(item.FoodItem));

                itemlist.Add(item);

            }

            for (int i = 0; i < itemlist.Count; i++)

            {

                context.PostAsync("Food Item: " + itemlist[i].FoodItem + " Price : " + itemlist[i].Price + " Quantity : " + itemlist[i].Quantity);

            }



            Calculation.TotalAmount = Calculation.Calculate(itemlist);

            context.PostAsync($"Your Total Cost till now is : {Calculation.TotalAmount}");

            Modify(context);

        }



        public static void Modify(IDialogContext context)

        {

            PromptDialog.Choice(context,SelectOption, new List<string>() { "Add More", "Remove", "Confirm" }, "What Do you want to do Now ?", "Please select a valid Option");

        }



        private async static Task SelectOption(IDialogContext context, IAwaitable<string> result)

        {

            string choice = await result;



            switch (choice)

            {

                case "Add More":
                    RootDialog.Category(context);

                    break;

                case "Remove":
                    await context.PostAsync("What do you want to remove from the existing Cart?");

                    context.Wait(RemoveItem);

                    break;

                case "Confirm":

                    AddToCart(context);

                    PromptDialog.Text(context, Address, "Enter your address", "Enter Valid option");

                    break;

            }

        }



        public async static Task RemoveItem(IDialogContext context, IAwaitable<object> result)

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



                if (intent == "SelectItems" || intent == "Remove" && score > 0.8)

                {

                    foreach (var entity in Data.entities)

                    {

                        itemName += entity.entity;

                    }



                    string message = null;

                    List<Items> RemoveCart = new List<Items>();

                    List<Items> DuplicateCart = new List<Items>();

                    DuplicateCart = itemlist;

                    Data.entities = Data.entities.OrderBy(o => o.startIndex).ToArray();



                    for (int i = 0; i < Data.entities.Length; i++)

                    {

                        Items items = new Items();

                        if (i == 0 && !(Data.entities[i].type.Equals("builtin.number")))

                        {

                            items.Quantity = 1;

                        }

                        else if (i != 0 && !(Data.entities[i].type.Equals("builtin.number")))

                        {

                            items.Quantity = 1;

                        }

                        else

                        {

                            items.Quantity = Convert.ToInt32(Data.entities[i].entity.ToString());

                            i++;

                        }

                        items.FoodItem = Data.entities[i].entity.ToString();

                        RemoveCart.Add(items);

                    }



                    foreach (var j in itemlist.ToList())

                    {

                        foreach (var k in RemoveCart.ToList())

                        {

                            if (j.FoodItem.Equals(k.FoodItem))

                            {

                                if (j.Quantity == k.Quantity)

                                {

                                    DuplicateCart.Remove(j);

                                    message = "Item removed successfully";

                                }

                                else if (j.Quantity > k.Quantity)

                                {

                                    j.Quantity = j.Quantity - k.Quantity;

                                    message = "Item removed successfully";

                                }

                                else

                                {

                                    message = "Few items cannot be removed. Please check the cart and try again";

                                }

                            }

                        }

                    }

                    itemlist = DuplicateCart;

                    Calculation.TotalAmount = Calculation.Calculate(itemlist);

                    await context.PostAsync($"Your Total Cost till now is : {Calculation.TotalAmount}");

                    Modify(context);

                }

            }

        }



        public static void AddToCart(IDialogContext context)

        {

            SQLManager.AddToDB(itemlist);

        }



        private async static Task Address(IDialogContext context, IAwaitable<string> result)

        {

            string optionSelected = await result;

            await context.PostAsync("Your order has been placed and food will be delivered shortly at given address : " + optionSelected);

            await context.PostAsync("Thank You For coming. \n Please Visit again");

        }

    }
}
