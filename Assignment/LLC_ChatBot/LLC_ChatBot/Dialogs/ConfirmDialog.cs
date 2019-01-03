using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace LLC_ChatBot.Dialogs
{   [Serializable]
    public class ConfirmDialog : IDialog<object>
    {
        public static string UserStartDate, UserEndDate;
        public const string YesOption = "Yes";
        public const string NoOption = "No";
        public const string AddOption = "Yes";
        public const string RemoveOption = "No";
        public static bool Answer;
        public static string VisitorID;
        


        public async Task StartAsync(IDialogContext context)
        {
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(2);
            await context.PostAsync(RootDialog.BotResponse);
            SQLManager.GetConversationData(UserData.UserID,RootDialog.UserResponse,RootDialog.BotResponse);

            context.Wait(EnterDepartment);

        }
        //private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        //{

        //    RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(2);
        //    PromptDialog.Text(context, this.EnterDepartment, RootDialog.BotResponse);
        //}

        public async Task EnterDepartment(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string departmentchoice = activity.Text;
            
            try

            {
                using (HttpClient httpClient = new HttpClient())

                {

                    LuisResponse Data = new LuisResponse();

                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="

                    + System.Uri.EscapeDataString(departmentchoice));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);



                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;



                    if (IntentName == "DepartmentIntent" && score > 0.8)

                    {

                        UserData.Department = Data.entities[0].entity.ToString();
                        RootDialog.UserResponse = UserData.Department; 

                        SQLManager.GetName(UserData.UserID);

                        if (UserData.Department == departmentchoice)
                        {
                            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(5);
                            await context.PostAsync(RootDialog.BotResponse);
                            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                            context.Wait(EnterContactName);


                        }
                        else
                        {
                            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(6);
                            await context.PostAsync(RootDialog.BotResponse);
                            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                            context.Wait(EnterDepartment);
                        }

                    }


                    else

                    {
                        RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(7);
                        await context.PostAsync(RootDialog.BotResponse);

                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        context.Wait(EnterDepartment);



                    }

                }

            }

            catch (Exception e)

            {

                throw e;

            }

        }
        public async Task EnterContactName(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string contactname = activity.Text;
            try

            {
                using (HttpClient httpClient = new HttpClient())
                {

                    LuisResponse Data = new LuisResponse();

                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="

                    + System.Uri.EscapeDataString(contactname));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);



                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;

                    if (IntentName == "VisitorNameIntent" && score > 0.8)

                    {

                        contactname = Data.entities[0].entity.ToString();
                        RootDialog.UserResponse = contactname;
                        VisitorData.ContactName = contactname.ToString();
                        RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(8);
                        await context.PostAsync(RootDialog.BotResponse);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);


                        this.GetStartDate(context, result);


                    }


                    else

                    {
                        await context.PostAsync("please enter the  valid contact name");
                        context.Wait(EnterDepartment);

                    }

                }
            }


            catch (Exception e)
            {

                throw e;

            }

        }
        private async Task GetStartDate(IDialogContext context, IAwaitable<object> result)
        {

            var replyActivity = context.MakeMessage();
            replyActivity.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JObject.Parse($@"
                 {{
              ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
              ""type"": ""AdaptiveCard"",
              ""version"": ""1.0"",
                  ""body"": [
                   {{
                            ""type"": ""TextBlock"",
                            
                        }},
                        {{
                            ""type"": ""Input.Date"",
""id"": ""date"",
      ""placeholder"": ""Enter a date"",
                                }},
            ],
              
""actions"": [
    {{
                        ""type"": ""Action.Submit"",
                        ""title"": ""OK""
                    }}
  ]
}}")
            });

            context.PostAsync(replyActivity);
            context.Wait(ExtractStartDate);
        }

        private async Task ExtractStartDate(IDialogContext context, IAwaitable<object> result)
        {
            var Date = await result as Activity;
            string StartDate = Date.Value.ToString();
            RootDialog.UserResponse = StartDate;
            if (StartDate == null)
            {
                RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(9);
                await context.PostAsync(RootDialog.BotResponse);
                SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                //await context.PostAsync("Please input the date");
                context.Wait(GetStartDate);
            }
            else
            {
                DateTime currentDate = DateTime.Now.Date;

                string[] SplitDate = StartDate.ToString().Split('"', '{', '}', ':', 'd', 'a', 't', 'e', '\n');

                foreach (string i in SplitDate)
                {
                    if (i.Trim() != "")
                    {
                        UserStartDate += i;
                    }
                }
                RootDialog.UserResponse = UserStartDate;
                VisitorData.StartDate = UserStartDate;
                if (currentDate.Date <= DateTime.Parse(UserStartDate).Date)
                {
                    RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(11);
                    await context.PostAsync(RootDialog.BotResponse);
                    SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                    this.GetEndDate(context, result);
                }
                else
                {
                    RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(10);
                    await context.PostAsync(RootDialog.BotResponse);
                    SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                    //await context.PostAsync($"Sorry, the date you have entered is invalid. Please check and enter Valid Date again.");
                    this.GetStartDate(context, result);
                }
            }
        }
        private async Task GetEndDate(IDialogContext context, IAwaitable<object> result)
        {
            var replyActivity = context.MakeMessage();
            replyActivity.Attachments.Add(new Microsoft.Bot.Connector.Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JObject.Parse($@"
                 {{
                ""$schema"": ""http://adaptivecards.io/schemas/adaptive-card.json"",
              ""type"": ""AdaptiveCard"",
              ""version"": ""1.0"",
              ""body"": [
                        {{
                            ""type"": ""TextBlock"",
                            
                        }},
                        {{
                            ""type"": ""Input.Date"",
                            ""id"": ""date"",
                            ""placeholder"": ""Enter a date"",
                        }},
                        ],
            ""actions"": [
                    {{
                        ""type"": ""Action.Submit"",
                        ""title"": ""OK""
                    }}
                         ]
                    }}")
            });

            context.PostAsync(replyActivity);
            context.Wait(ExtractEndDate);


        }
        private async Task ExtractEndDate(IDialogContext context, IAwaitable<object> result)
        {
            var Date = await result as Activity;
            string EndDate = Date.Value.ToString();
            RootDialog.UserResponse = EndDate;


            if (EndDate == null)
            {
                RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(12);
                await context.PostAsync(RootDialog.BotResponse);
                SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
               // await context.PostAsync("Please input the date");
                context.Wait(GetStartDate);
            }
            else
            {
                DateTime currentDate = DateTime.Now.Date;

                string[] SplitDate = EndDate.ToString().Split('"', '{', '}', ':', 'd', 'a', 't', 'e', '\n');

                foreach (string i in SplitDate)
                {
                    if (i.Trim() != "")
                    {
                        UserEndDate += i;
                    }
                }
                RootDialog.UserResponse = UserEndDate;
                VisitorData.EndDate = UserEndDate;
                if (DateTime.Parse(UserStartDate).Date <= DateTime.Parse(UserEndDate).Date)  //condition False
                {
                    RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(14);
                    await context.PostAsync(RootDialog.BotResponse);

                    SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                    this.ConfirmParkingTicket(context, result);

                }
                else
                {
                    RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(13);
                    await context.PostAsync(RootDialog.BotResponse);

                    SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);


                    //await context.PostAsync($"Sorry, the date you have entered is invalid. Please check and enter Valid Date again.");
                    this.GetEndDate(context, result);
                }
            }
        }
        public async Task ConfirmParkingTicket(IDialogContext context, IAwaitable<object> result)
        {
            //RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(14);
            //await context.PostAsync(RootDialog.BotResponse);
            PromptDialog.Choice(context, this.OptionSelected, new List<string>() { YesOption, NoOption }," ");

        }
        public async Task OptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try

            {

                string optionSelected = await result;
                //RootDialog.UserResponse = optionSelected;
                //VisitorData.NeedParking = Convert.ToBoolean(optionSelected);
                switch (optionSelected)

                {

                    case YesOption:
                        VisitorData.NeedParking = true;
                        RootDialog.UserResponse = VisitorData.NeedParking.ToString();
                        RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(15);
                        await context.PostAsync(RootDialog.BotResponse);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                        context.Wait(NoOfParkingTickets);

                        break;

                    case NoOption:
                        VisitorData.NeedParking = false;
                        RootDialog.UserResponse = VisitorData.NeedParking.ToString();
                        //context.Call(new RejectDialog(), this.ResumeAfterOptionDialog);

                        break;

                }

            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }

        }
        public async Task NoOfParkingTickets(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result as Activity;
                int tickets = Convert.ToInt16(activity.Text);
                VisitorData.NoOfParkingTicket = tickets;
                RootDialog.UserResponse = activity.Text;
                RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(16);
                await context.PostAsync(RootDialog.BotResponse);
                SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);

                this.SelectBuilding(context, result);
            }
            catch (Exception e)
            {
                throw e;
            }

        }
        public async Task SelectBuilding(IDialogContext context, IAwaitable<object> result)
        {
            List<string> buildings = new List<string>();
            buildings = SQLManager.DisplayBuilding();
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(17);
            PromptDialog.Choice(context, this.VistorName, buildings, RootDialog.BotResponse,"Invalid input");

        }
        public async Task VistorName(IDialogContext context, IAwaitable<object> result)
        {
            var activity= await result;
            string buildingname = activity.ToString();
            RootDialog.UserResponse = buildingname;
            VisitorData.BuildingName = buildingname;
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(18);
            await context.PostAsync(RootDialog.BotResponse);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            context.Wait(EnterVisitorName);
            //this.EnterVisitorName(context, result);
        }
        public async Task EnterVisitorName(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string VisitorName = activity.Text;
            
            //VisitorData.VisitorName = VisitorName;
            RootDialog.UserResponse = VisitorName;
            try

            {
                using (HttpClient httpClient = new HttpClient())
                {

                    LuisResponse Data = new LuisResponse();

                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="

                    + System.Uri.EscapeDataString(VisitorName));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);



                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;

                    if (IntentName == "VisitorNameIntent" && score > 0.8)

                    {

                       VisitorName = Data.entities[0].entity.ToString();
                        VisitorData.VisitorName.Add(VisitorName);

                        //await context.PostAsync("visitor  name");
                        this.CompanyName(context, result);
                        //this.GetStartDate(context, result);


                    }


                    else

                    {
                        await context.PostAsync("please enter the  valid visitor  name");

                        context.Wait(EnterVisitorName);

                    }

                }
            }
            catch (Exception e)
            {

                throw e;

            }

        }
        public async Task CompanyName(IDialogContext context, IAwaitable<object> result)
        {
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(19);
            context.PostAsync(RootDialog.BotResponse);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            context.Wait(EnterCompanyName);
            
            //this.EnterCompanyNam


        }
        public async Task EnterCompanyName(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string CompanyName = activity.Text;
            VisitorData.CompanyName.Add(CompanyName);
            RootDialog.UserResponse = CompanyName;
            VisitorData.Company= CompanyName;
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            VisitorData.Company = CompanyName;
            await context.PostAsync($" company name is {CompanyName} ");
            this.AddVisitors(context, result);

            

        }
        public async Task AddVisitors(IDialogContext context, IAwaitable<object> result)
        {
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(21);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            PromptDialog.Choice(context, this.MoreVisitors, new List<string>() {AddOption,RemoveOption  }, RootDialog.BotResponse);
        }
        public async Task MoreVisitors(IDialogContext context, IAwaitable<string> result)
        {
            try

            {
                
                string optionSelected = await result;
                RootDialog.UserResponse = optionSelected;
                //RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(21);


                switch (optionSelected)

                {

                    case AddOption :
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        context.Wait(VistorName);
                       
                        break;

                    case RemoveOption :
                        RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(22);
                        SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
                        await context.PostAsync(RootDialog.BotResponse);
                        context.Wait(EnterAnyComments);


                        //context.Call(new RejectDialog(), this.ResumeAfterOptionDialog);

                        break;

                }

            }

            catch (Exception e)

            {

                await context.PostAsync("enter a valid option");



            }

        }
        public async Task EnterAnyComments(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string Comments = activity.Text;
            RootDialog.UserResponse = Comments;
            VisitorData.Comment = Comments;
            RootDialog.BotResponse = SQLManager.GetVisitorBadgeQuestions(23);
            await context.PostAsync(RootDialog.BotResponse);
            SQLManager.GetConversationData(UserData.UserID, RootDialog.UserResponse, RootDialog.BotResponse);
            VisitorID=SQLManager.InsertVisitorData(UserData.UserID, VisitorData.ContactName, VisitorData.VisitorName, VisitorData.CompanyName, VisitorData.NeedParking, VisitorData.NoOfParkingTicket, VisitorData.BuildingName, VisitorData.StartDate, VisitorData.EndDate, VisitorData.Comment);
            this.StartAsync(context);

        }
        //private async Task SendMail(IDialogContext context, IAwaitable<string> result)
        //{

        //    using (MailMessage mailmsg = new MailMessage("hitesh.garg@acuvate.com", "hitesh.garg@acuvate.com"))
        //    {
        //        mailmsg.Subject = "Visitor Badge request";
        //        mailmsg.Body = "Your visitor badge request has been raised and the reference number is <ref.no>";
        //        mailmsg.IsBodyHtml = false;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "smtp.office365.com";
        //        smtp.EnableSsl = true;
        //        NetworkCredential NetworkCred = new NetworkCredential("hitesh.garg@acuvate.com", "Intelligence@");
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = NetworkCred;
        //        smtp.Port = 587;
        //        smtp.Send(mailmsg);
        //    }
        //}

    }
}
