using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace _13DecFood.Dialogs
{

    [Serializable]

    internal class VegDialog : IDialog<object>
    {
        private const string FriedRiceOption = "FriedRice";

        private const string VegCurryOption = "VegCurry";
        public int FriedRiceCost;
        public int VegCurryCost;

        public Task StartAsync(IDialogContext context)
        {
            this.VegMenu(context);
            return Task.CompletedTask;
        }
       
        private void VegMenu(IDialogContext context)
        {
            PromptDialog.Choice(context, this.VegOption, new List<string>() { FriedRiceOption,VegCurryOption}, "Select from the Menu", "Not a valid options", 3);
        }
        private async Task VegOption(IDialogContext context, IAwaitable<string> result)

        {

            try

            {

                string SelectedVegDish = await result;

                switch (SelectedVegDish)

                {

                    case FriedRiceOption:

                        FriedRiceCost = 100;
                        await context.PostAsync($"The Cost of FriedRice you have choosen is: {FriedRiceCost} ");
                        await context.PostAsync($"Enter OK For Confirmation");
                        context.Call(new AddressDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case VegCurryOption:
                        VegCurryCost = 150;

                        await context.PostAsync($"The Cost of  VegCurry you have choosen is: {VegCurryCost} ");
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