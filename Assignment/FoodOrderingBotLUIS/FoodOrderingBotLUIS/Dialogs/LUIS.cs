using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace FoodOrderingBotLUIS.Dialogs
{

    [Serializable]
    public class LUIS  
    {
        public static float TotalAmount;
        public static List<Cart> CartList = new List<Cart>();
         VegDialog veg = new VegDialog();

        public   static void InsertIntoCart(LuisResponse Data,IDialogContext context)
        {
            
            try
            {
                for (int i = 0; i < Data.entities.Length; i++)
                {
                   // dish dish1 = new dish();
                    Cart cart = new Cart();

                    if (i == 0 && !(Data.entities[i].type.Equals("builtin.number")))
                    {
                        cart.Quantity = 1;
                    }
                    else if (i != 0 && !(Data.entities[i].type.Equals("builtin.number")))
                    {

                        cart.Quantity = 1;

                        //i++
                    }
                    else
                    {

                        cart.Quantity = Convert.ToInt32(Data.entities[i].entity.ToString());
                        i++;
                    }
                    cart.ProductName= Data.entities[i].entity.ToString();
                    cart.Price = Convert.ToInt32(SQLManager.GetItems(cart.ProductName));
                    CartList.Add(cart);
                    
                    //cart.ProductName = Data.entities[i].entity.ToString();
                    //CartList.Add(cart);
                }
                //for (int i = 0; i <CartList.Count; i++)
                //{
                //    context.PostAsync("Food Item: " + CartList[i].ProductName + " Price : " +CartList[i].Price + " Quantity : " + CartList[i].Quantity);
                //}
                for (int i = 0; i < CartList.Count; i++)
                {
                    TotalAmount = CartList[i].Price * CartList[i].Quantity;

                }

                //TotalAmount = Calculation.Calculate(CartList);

                //Modify(context);
                context.PostAsync($"Your Total Cost till now is : {TotalAmount}");
                

            }
            
            catch (Exception e)
            {
                throw e;
            }
            
        }
      

    }


}