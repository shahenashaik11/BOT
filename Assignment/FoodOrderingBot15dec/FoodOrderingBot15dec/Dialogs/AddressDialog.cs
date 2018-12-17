using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FoodOrderingBot15dec.Dialogs
{
    [Serializable]
    public class AddressDialog
    {


       // RootDialog root = new RootDialog();
        public Task StartAsync(IDialogContext context)
        {
            
           context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {

            // var activity = await result;
            await context.PostAsync(String.Format($"final price is:{RootDialog.finalprice}"));
            this.DisplayAddress(context);
            //this.FinalPrice(context);


            //context.Wait(MessageReceivedAsync);
        }
        //private async Task FinalPrice(IDialogContext context)
        //{
        //    finalprice += this.Price;
        //    await context.PostAsync(String.Format($"final price is:{finalprice}"));
            

        //}
        private void DisplayAddress(IDialogContext context)
        {
            
            PromptDialog.Text(context, this.EnterAddress, @"Enter Your Address");



        }
        private async Task EnterAddress(IDialogContext context, IAwaitable<string> result)
        {
            string address = await result;
          

            await context.PostAsync(String.Format("your order is placed\n THANKYOU"));
            
            context.Call(new RootDialog(), this.ResumeAfterOptionDialog);

            // return Task.CompletedTask;




        }
        
        


        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(ResumeAfterOptionDialog);
        }
    }
}