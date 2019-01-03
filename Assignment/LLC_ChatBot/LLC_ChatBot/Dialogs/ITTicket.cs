using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace LLC_ChatBot.Dialogs
{
    [Serializable]
    public class ITTicket : IDialog<object>
    {
         
        public async Task StartAsync(IDialogContext context)
        {
             context.PostAsync("what are you looking for?");
            //this.MessageReceivedAsync(context, result);
            context.Wait(MessageReceivedAsync);

           // return Task.CompletedTask;
            //context.PostAsync("this is IT start ASYNC");
        }
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            string issue = message.Text;
            RootDialog.UserResponse = issue;

            using (HttpClient httpClient = new HttpClient())
            {
                LuisResponse Data = new LuisResponse();
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="
                    + System.Uri.EscapeDataString(issue));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                    //choice = null;
                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;
                    Data.entities.OrderBy(o => o.startIndex);
                    //string UserName;

                    if (IntentName == "RaiseITTicket" && score > 0.8)
                    {
                        await IssueCategory(context, result);
                    }
                    else
                    {
                        await context.PostAsync("invalid question which is not related");
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

            public async Task IssueCategory(IDialogContext context, IAwaitable<object> result)
            {
                List<string> category = new List<string>();
                category = SQLManager.ChooseIssueCategory();
            RootDialog.BotResponse = SQLManager.GetITQuestions(1);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            //await context.PostAsync(RootDialog.BotResponse);
            PromptDialog.Choice(context, this.ChooseIssue, category, RootDialog.BotResponse);
            }
        public async Task ChooseIssue(IDialogContext context, IAwaitable<string> result)
        {

            try

            {

                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected;

                List<string> Issues = new List<string>();
                Issues = SQLManager.GetIssueName(optionSelected.ToString());

                switch (optionSelected)

                {

                    case "Software":
                        RootDialog.BotResponse = SQLManager.GetITQuestions(2);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        PromptDialog.Choice(context, this.SoftwareIssue, Issues, RootDialog.BotResponse);
                        


                        break;

                    case "Hardware":
                        RootDialog.BotResponse = SQLManager.GetITQuestions(3);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        PromptDialog.Choice(context, this.HardwareIssue, Issues, RootDialog.BotResponse);
                      

                        break;

                }

            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }
      
        }

        public async Task SoftwareIssue(IDialogContext context, IAwaitable<string> result)
        {

            try

            {
                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected;
                RootDialog.BotResponse = SQLManager.GetITQuestions(3);
                await context.PostAsync(RootDialog.BotResponse);
                SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                context.Wait(IssueDescription);
                //
                this.IssueDescription(context, result);




            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }
        }
        public async Task HardwareIssue(IDialogContext context, IAwaitable<string> result)
        {

            try

            {

                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected;
                RootDialog.BotResponse = SQLManager.GetITQuestions(3);
                await context.PostAsync(RootDialog.BotResponse);
                SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                context.Wait(IssueDescription);




            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }

        }
        public async Task IssueDescription(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string description = activity.Text;
            RootDialog.UserResponse = description;
            RootDialog.BotResponse = SQLManager.GetITQuestions(4);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            PromptDialog.Choice(context, this.RaiseIssue, new List<string>() { "Yes", "No" }, RootDialog.BotResponse);

            //await context.PostAsync("end flow");

        }
        public async Task RaiseIssue(IDialogContext context, IAwaitable<string> result)
        {
            RootDialog root = new RootDialog();

            try

            {

                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected;
                switch (optionSelected)

                {

                    case "Yes":
                        RootDialog.BotResponse = SQLManager.GetITQuestions(5);
                        await context.PostAsync(RootDialog.BotResponse);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        root.StartAsync(context);


                        break;

                    case "No":
                       root.StartAsync(context);

                        break;
                  
                }

            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }
        }


    }
}