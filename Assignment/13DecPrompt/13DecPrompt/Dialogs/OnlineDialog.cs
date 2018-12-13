using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace _13DecPrompt.Dialogs
{
    [Serializable]
    
        internal class OnlineDialog : IDialog<object>

        {

            public Task StartAsync(IDialogContext context)

            {

                context.Wait(MessageReceivedAsync);

                return Task.CompletedTask;

            }



            private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)

            {

                throw new NotImplementedException();

            }

        }
}
