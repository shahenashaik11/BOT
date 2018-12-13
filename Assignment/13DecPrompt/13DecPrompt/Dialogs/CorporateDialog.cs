using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace _13DecPrompt.Dialogs
{


    [Serializable]

    internal class CorporateDialog : IDialog<object>

    {

        public Task StartAsync(IDialogContext context)

        {

            context.PostAsync("Enter Course Name");

            



            return Task.CompletedTask;

        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<string> result)
        {
            throw new NotImplementedException();
        }



       
    }
}