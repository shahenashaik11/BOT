using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Herocard.Dialogs
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
            var welcomeMessage = context.MakeMessage();
            welcomeMessage.Text = "Welcome to bot Hero Card Demo";

            await context.PostAsync(welcomeMessage);

            await this.DisplayHeroCard(context);
            

            //context.Wait(MessageReceivedAsync);
        }
        public async Task DisplayHeroCard(IDialogContext context)
        {

            var replyMessage = context.MakeMessage();
            Attachment attachment = GetProfileHeroCard(); ;
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }
        private static Attachment GetProfileHeroCard()
        {
            var heroCard =new HeroCard
            {
                // title of the card  
                Title = "beautiful image",
                //subtitle of the card  
                Subtitle = "wallpaper",
                // navigate to page , while tab on card  
                Tap = new CardAction(ActionTypes.OpenUrl, "Learn More", value: "https://www.w3schools.com/"),
                //Detail Text  
                Text = "best ",
                // list of  Large Image  
                Images = new List<CardImage> { new CardImage("D:/beautiful-blur-bright-326055.jpg") },
                // list of buttons   
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "back", value: " "), new CardAction(ActionTypes.OpenUrl, "new", value: ""), new CardAction(ActionTypes.OpenUrl, "MSDN", value: "") }
            };

            return heroCard.ToAttachment();
        }

      
    }
}