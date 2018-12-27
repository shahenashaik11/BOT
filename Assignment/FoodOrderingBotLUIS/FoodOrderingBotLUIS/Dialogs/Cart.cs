using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodOrderingBotLUIS.Dialogs
{
    [Serializable]
    public class Cart
    {
        public string ProductName;
        public float Price;
        public int Quantity;
        public string URL;
    }
}