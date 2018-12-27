using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOTFoodLUIS.Dialogs
{
    public class Calculation
    {
        public static int TotalAmount, NewPrice;

        public static int Calculate(List<Items> itemlist)

        {

            TotalAmount = 0;

            for (int i = 0; i < itemlist.Count; i++)

            {

                TotalAmount = TotalAmount + itemlist[i].Price * itemlist[i].Quantity;

            }

            return TotalAmount;

        }
    }
}