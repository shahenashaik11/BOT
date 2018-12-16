using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace _13DecFood.Dialogs
{
    [Serializable]

    internal class NonVegDialog : IDialog<object>
    {
        private const string BiryaniOption = "Biryani";

        private const string NonVegCurryOption = "NonVegCurry";
        public int BiryaniCost;
        public int NonVegCurryCost;

        public Task StartAsync(IDialogContext context)
        {
            this.NonVegMenu(context);
            return Task.CompletedTask;
        }

        private void NonVegMenu(IDialogContext context)
        {
            PromptDialog.Choice(context, this.NonVegOption, new List<string>() { BiryaniOption, NonVegCurryOption }, "Select from the Menu", "Not a valid options", 3);
        }
        private async Task NonVegOption(IDialogContext context, IAwaitable<string> result)

        {

            try

            {

                string SelectedVegDish = await result;

                switch (SelectedVegDish)

                {

                    case BiryaniOption:

                        BiryaniCost = 100;
                        await context.PostAsync($"The Cost of  Biryani you have choosen is: {BiryaniCost} ");
                       await context.PostAsync($"Enter OK For Confirmation");
                        context.Call(new AddressDialog(), this.ResumeAfterOptionDialog);


                        break;

                    case NonVegCurryOption:
                        NonVegCurryCost = 150;

                        await context.PostAsync($"The Cost  of NonVegCurry you have choosen is: {NonVegCurryCost} ");
                        await context.PostAsync($"Enter OK For Confirmation");
                        context.Call(new AddressDialog(), this.ResumeAfterOptionDialog);
                        break;

                }

            }
            catch (Exception e)

            {

                await context.PostAsync("Thanks");

                context.Wait(this.MessageReceivedAsync);

            }
        }
        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)

        {

            context.Wait(this.MessageReceivedAsync);

        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }

    }
}