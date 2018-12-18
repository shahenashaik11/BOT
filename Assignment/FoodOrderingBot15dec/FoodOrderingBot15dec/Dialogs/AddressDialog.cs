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
        public  static string display = "";
        RootDialog root = new RootDialog();
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
           
        }
       
        private void DisplayAddress(IDialogContext context)
        {
            context.ConversationData.SetValue<List<string>>("dishesname", RootDialog.newdishes);
            PromptDialog.Text(context, this.EnterAddress, @"Enter Your Address");
            



        }
        private async Task EnterAddress(IDialogContext context, IAwaitable<string> result)
        {
            string address = await result;

            context.ConversationData.TryGetValue<List<string>>("dishesname", out RootDialog.selecteddishes);
            

                for(int i=0;i< RootDialog.selecteddishes.Count; i++)
                {
                    context.PostAsync($"selected items are:\n {RootDialog.selecteddishes[i]} ");
                }
                   




            await context.PostAsync(String.Format("your order is placed\n THANKYOU"));
            
            context.Call(new RootDialog(), this.ResumeAfterOptionDialog);

            




        }
        
        


        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(ResumeAfterOptionDialog);
        }
    }
}