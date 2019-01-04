using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LLC_ChatBot.Dialogs
{
    
    public class RejectDialog : IDialog<object>
    {
        RootDialog root = new RootDialog();
        public Task StartAsync(IDialogContext context)
        {
            context.PostAsync("Please enter your name");
            root.StartAsync(context);
            //context.PostAsync("Please enter your name, email id");
            //context.Wait(MessageReceivedAsync);
            


            return Task.CompletedTask;
        }
        //public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{
        //    root.EnterEmail(context, result);

        //}

    }
}