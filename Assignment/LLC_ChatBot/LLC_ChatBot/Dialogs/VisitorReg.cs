using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LLC_ChatBot.Dialogs
{
    [Serializable]
    public  class VisitorReg: IDialog<object>
    {
        public const string ConfirmOption = "Confirm";
        public const string RejectOption = "Reject";
        ITTicket IT = new ITTicket();

        public async Task StartAsync(IDialogContext context)
        {
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(1);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            PromptDialog.Choice(context, this.OptionSelected, new List<string>() { ConfirmOption, RejectOption }, $"Sure {UserData.UserName}! {RootDialog.BotResponse}\n {UserData.UserID}\n{UserData.UserName}", "Not a valid options", 3);
        
        }
        
        private async Task OptionSelected(IDialogContext context, IAwaitable<string> result)

        {


            try

            {

                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected.ToString();


                switch (optionSelected)

                {

                    case ConfirmOption:

                        context.Call(new ConfirmDialog(), this.ResumeAfterOptionDialog);

                        break;

                    case RejectOption:

                        context.Call(new RejectDialog(), this.ResumeAfterOptionDialog);

                        break;

                }

            }

            catch (Exception e)

            {
                SQLManager.StoreExceptionData(e.GetType().ToString(), e.Message, e.StackTrace, e.Data.ToString());

                //await context.PostAsync("enter a valid option");



            }

        }

        private  async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            IT.StartAsync(context);
        }
    }
}