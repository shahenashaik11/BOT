using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;

namespace LLC_ChatBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        VisitorReg vreg = new VisitorReg();
        ITTicket IT = new ITTicket();
        public static string UserResponse;
        public static string BotResponse;

        public static string choice;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            await context.PostAsync("Enter your Email id");
            context.Wait(EnterEmail);


        }
        public async Task EnterEmail(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result as Activity;
            string UserID = message.Text;

            using (HttpClient httpClient = new HttpClient())
            {
                LuisResponse Data = new LuisResponse();
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="
                    + System.Uri.EscapeDataString(UserID));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                    //choice = null;
                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;
                    Data.entities.OrderBy(o => o.startIndex);
                    //string UserName;

                    if (IntentName == "UserInfo" && score > 0.8)
                    {
                        UserData.UserID = Data.entities[0].entity.ToString();

                        UserData.UserName = SQLManager.GetName(UserData.UserID);

                        if (UserData.UserName.Equals(""))

                        {

                            await context.PostAsync("Enter a valid email");



                        }
                        else
                        {
                            await context.PostAsync($"hi {UserData.UserName},How can I help you?");
                            context.Wait(StaticQuestion);


                        }
                    }
                    else
                    {
                        await context.PostAsync("UserName not found");
                    }
                }
                catch (Exception e)
                {
                    await context.PostAsync("Name Exception");
                }
            }
        }
        private async Task StaticQuestion(IDialogContext context, IAwaitable<object> result)
        {

            var option = await result as Activity;
            string choice = option.Text;
            string Response;
            UserResponse = choice;

            using (HttpClient httpClient = new HttpClient())
            {
                LuisResponse Data = new LuisResponse();
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="


                    + System.Uri.EscapeDataString(choice));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);

                    choice = null;
                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;
                    Data.entities.OrderBy(o => o.startIndex);
                    if (score > 0.8)
                    {

                        switch (IntentName)
                        {

                            case "FAQHrtAtkDefine":
                                Response = SQLManager.GetResponse(IntentName);
                                await context.PostAsync($"{Response}");

                                break;
                            case "FAQHrtAtkSymptoms":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "FAQHrtAtkMeasures":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "FAQHrtAtkCardiacArrest":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "FAQHrtAtkPrecaution":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "FinanceGlobalCorporateCardApply":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "FinanceOut-of-PocketExpensesClaim":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "EnterpriseCodeofConductFind":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "ITHelpdeskSupport":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            case "None":
                                Response = SQLManager.GetResponse(IntentName);

                                await context.PostAsync($"{Response}");
                                break;
                            default:
                               await this.DynamicQuestion(context,result);
                               
                                break;
                        }
                    }
                    else
                    {
                        await context.PostAsync("Sorry we couldn't found it");
                        await context.PostAsync("can you please mention the main topic of question?");

                    }
                }
                catch (Exception e)
                {
                    await context.PostAsync("StaticException");
                }

            }

        }
      
        private async Task DynamicQuestion(IDialogContext context, IAwaitable<object> result)
        {
            var option = await result as Activity;
            string choice2 = option.Text;
            string Response;
            //string choice2 = choice;
            using (HttpClient httpClient = new HttpClient())
            {
                LuisResponse Data = new LuisResponse();
                try
                {
                    var responseInString =  await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/41a6a9ad-77ae-474c-9cc7-f2ae5205c1ca?staging=true&verbose=true&timezoneOffset=-360&subscription-key=c17a9179a96c42a5b6ed8ce59d66edd2&q="
                    + System.Uri.EscapeDataString(choice2));

                    Data = JsonConvert.DeserializeObject<LuisResponse>(responseInString);
                    var intent = Data.topScoringIntent.intent;
                    string IntentName = intent;
                    var score = Data.topScoringIntent.score;
                    Data.entities.OrderBy(o => o.startIndex);
                   
                     
                   
                        switch (IntentName)
                        {
                            case "RealEstateMeetingRoomReservationBook":
                                Response = SQLManager.GetDynamicResponse(IntentName);
                                await context.PostAsync($" {Response}");

                                
                                break;
                            case "RealEstateReceptionRegister":
                                Response = SQLManager.GetDynamicResponse(IntentName);
                                await context.PostAsync($" {Response}");
                                
                                break;
                            case "RealEstateShuttleBusSchedule":
                                
                                Response = SQLManager.GetDynamicResponse(IntentName);
                                await context.PostAsync($" {Response}");
                                
                                break;
                            case "TravelBookITBook":
                            
                                Response = SQLManager.GetDynamicResponse(IntentName);
                                await context.PostAsync($" {Response}");
                               
                                break;
                        case "VisitorIntent":
                            vreg.StartAsync(context);

                            break;
                        default :
                            //vreg.StartAsync(context);
                           IT.StartAsync(context);
                            break;
                        }


                      
                    
                   

                }
                
                catch (Exception e)
                {
                    throw e;
                    //await context.PostAsync("Dynamic Exception");
                }
            }
        }
    }
}