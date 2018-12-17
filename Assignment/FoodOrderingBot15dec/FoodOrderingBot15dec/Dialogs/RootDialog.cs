using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace FoodOrderingBot15dec.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string VegOption = "Veg";

        private const string NonVegOption = "NonVeg";
        public static float finalprice;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            //var activity = await result as Activity;
            //this.DisplayName(context);
            var message = await result;
            var userName = String.Empty;
            var IsNameAvailable = false;
            context.UserData.TryGetValue("Name", out userName);
            context.UserData.TryGetValue("GetName", out IsNameAvailable);
            if (IsNameAvailable)
            {
                userName = message.Text;
                context.UserData.SetValue("Name", userName);
                context.UserData.SetValue("GetName", false);
            }
            if (string.IsNullOrEmpty(userName))
            {
                await context.PostAsync("What is your name??");

                context.UserData.SetValue("GetName", true);


            }
            else
            {
                await context.PostAsync(String.Format("Hi {0}..Welcome to NewFriends Food Ordering", userName));
                //await context.PostAsync(String.Format("{0} Please select one of these stone,paper,scissors ", userName));
                //this.DisplayName(context);
                this.ShowOptions(context);


            }
        }
        //private void DisplayName(IDialogContext context)
        //{
        //    PromptDialog.Text(context, this.EnterName, @"what is your Name?");

        //}
        //private async Task EnterName(IDialogContext context, IAwaitable<string> result)
        //{
        //    string Name = await result;
        //    await context.PostAsync(String.Format("Hi {0}.Welcome to NewFriends Food Ordering", Name));

        //    this.ShowOptions(context);
        //}
        public void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OptionSelected, new List<string>() { VegOption, NonVegOption }, "What Would you like to have?", "Not a valid options", 3);
        }
        private async Task OptionSelected(IDialogContext context, IAwaitable<string> result)

        {

            try

            {

                string optionSelected = await result;

                switch (optionSelected)

                {

                    case VegOption:

                        context.Call(new VegDialog(), this.ResumeAfterOptionDialog);

                        break;

                    case NonVegOption:

                        context.Call(new NonVegDialog(), this.ResumeAfterOptionDialog);

                        break;

                }

            }

            catch (Exception e)

            {

                await context.PostAsync("Thanks");

                context.Wait(this.MessageReceivedAsync);

            }

        }
        public async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)

        {


            context.Wait(this.MessageReceivedAsync);
        }
       

   
    }

}
