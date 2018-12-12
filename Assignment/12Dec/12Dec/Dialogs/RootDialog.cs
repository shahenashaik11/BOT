using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace _12Dec.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            if (activity.Text.Contains("Csharp") || activity.Text.Contains("C#"))
            {
                await context.PostAsync($"Are you looking for Csharp?");
            }
            else if (activity.Text.Contains("AI") || activity.Text.Contains("Artificial Intelligence"))
            {
                await context.PostAsync($"Are you looking for AI?");
            }
            else if (activity.Text.Contains("BOT") || activity.Text.Contains("Chat Bot"))
            {
                await context.PostAsync($"Are you looking for BOT?");
            }
            else
            {
                await context.PostAsync($"Course Not Found!");
            }


            //switch (activity.Text)
            //{
            //    case "C#":
            //        await context.PostAsync($"Are you looking for C#?");
            //        break;
            //    case "CSharp":
            //        await context.PostAsync($"Are you looking for C#?");
            //        break;
            //    case "AI":
            //        await context.PostAsync($"Are you looking for AI?");
            //        break;
            //    case "ArtificialIntelligence":
            //        await context.PostAsync($"Are you looking for AI?");
            //        break;
            //    default:
            //        await context.PostAsync($"Course not found!");
            //        break;
            //}


            // Calculate something for us to return
            // int length = (activity.Text ?? string.Empty).Length;

            // Return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}