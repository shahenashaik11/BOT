using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;

namespace _12DECEMBER.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //int count = 3;
        //int BotScore = 0;
        //int UserScore = 0;
        private string message;

        public  async Task StartAsync(IDialogContext context)
        {

            //var message = await result;

           // context.Wait(IdentifyCourseUsingLuis);
            context.Wait(MessageReceivedAsync);

            //return Task.CompletedTask;
        }

        //public Task StartAsync(IDialogContext context)
        //{
        //    throw new NotImplementedException();
        //}


        //public Task StartAsync(IDialogContext context)
        //{
        //    throw new NotImplementedException();
        //}



        //public static async Task<object> IdentifyCourseUsingLuis(string message)
        //{
        //    //string course = "";
        //using (HttpClient httpClient = new HttpClient())
        //{
        //    try
        //    {
        //        var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/684384d6-f50b-4f66-b628-270bf84b2980?staging=true&verbose=true&timezoneOffset=-360&subscription-key=59e472415afd44a595605a762b8c81ab&q=veg"
        //        + System.Uri.EscapeDataString(message));
        //        dynamic response = JObject.Parse(responseInString);
        //        dynamic intent =response.intents?.First?.intent;
        //        if (intent == "SelectCategory")
        //        {
        //            foreach (var entity in response.entities)
        //            {
        //                course = entity.entity;
        //                break;
        //            }
        //        }
        //    }






        //    catch (Exception e)
        //    {
        //        return "Failed to process";



        //    }
        //    return course;
        //}


        //}


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            // PromptDialog.Text(context, Name, @" Whats your name");
           
            //var message = await result;
            //var userName = String.Empty;
            //var IsNameAvailable = false;
            //context.UserData.TryGetValue("Name", out userName);
            //context.UserData.TryGetValue("GetName", out IsNameAvailable);
            //if (IsNameAvailable)
            //{
            //    userName = message.Text;
            //    context.UserData.SetValue("Name", userName);
            //    context.UserData.SetValue("GetName", false);
            //}
            //if (string.IsNullOrEmpty(userName))
            //{
            //    await context.PostAsync("What is your name??");

            //    context.UserData.SetValue("GetName", true);
                

            //}
            //else
            //{
            //    await context.PostAsync(String.Format("Hi {0}.lets Play a game  StonePaperScissors ?", userName));
            //    await context.PostAsync(String.Format("{0} Please select one of these stone,paper,scissors ",userName));
            //    context.Wait(Game);
            //}
            string course = "";
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var responseInString = await httpClient.GetStringAsync(@"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/684384d6-f50b-4f66-b628-270bf84b2980?staging=true&verbose=true&timezoneOffset=-360&subscription-key=59e472415afd44a595605a762b8c81ab&q=veg"
                    + System.Uri.EscapeDataString(message));
                    dynamic response = JObject.Parse(responseInString);
                    dynamic intent = response.intents?.First?.intent;
                    if (intent == "SelectCategory")
                    {
                        foreach (var entity in response.entities)
                        {
                            course = entity.entity;
                            break;
                        }
                    }
                }






                catch (Exception e)
                {
                    //return "Failed to process";



                }
               // return course;
            }







        }
        //private async Task Naming(IDialogContext context, IAwaitable<object> result)
        //{

        //    await context.PostAsync($"{Name} lets Play a game  StonePaperScissors");

        //    await context.PostAsync($"{await result} Please select one of these stone,paper,scissors ");
        //    context.Wait(Game);




        //}
        //private async Task Game(IDialogContext context, IAwaitable<object> result)
        //{
        //    var activity = await result as Activity;
        //    string UserChoice = activity.Text;
        //    Random r = new Random();
          
        //    int BotChoice = r.Next(1,4);




        //    if (count > 0)
        //    {
        //        if (BotChoice == 1)
        //        {
        //            if (UserChoice == "stone")
        //            {
        //                await context.PostAsync($"Bot chose stone");

        //                await context.PostAsync($"It is a tie ");
        //                count--;
        //            }
        //            else if (UserChoice == "paper")
        //            {
        //                await context.PostAsync($"Bot chose paper");
        //                await context.PostAsync($"It is a tie ");
        //                count--;

        //            }
        //            else if (UserChoice == "scissors")
        //            {
        //                await context.PostAsync($"Bot chose scissors");
        //                await context.PostAsync($"It is a tie ");
        //                count--;
        //            }
        //            else
        //            {
        //                await context.PostAsync($"You must choose stone,paper or scissors!");

        //            }

        //        }

        //        else if (BotChoice == 2)
        //        {
        //            if (UserChoice == "stone")
        //            {
        //                await context.PostAsync($"Bot chose paper");
        //                await context.PostAsync($"Sorry you lose,paper beat stone");
        //                count--;
        //                BotScore++;

        //            }
        //            else if (UserChoice == "paper")
        //            {
        //                await context.PostAsync($"Bot chose scissors");
        //                await context.PostAsync($"Sorry you lose,scissors beat paper ");
        //                count--;
        //                BotScore++;


        //            }
        //            else if (UserChoice == "scissors")
        //            {
        //                await context.PostAsync($"Bot chose rock");
        //                await context.PostAsync($"Sorry you lose,stone beats scissors");
        //                count--;
        //                BotScore++;
        //            }
        //            else
        //            {
        //                await context.PostAsync($"You must choose stone,paper or scissors!");
        //            }
                    
        //        }
        //        else if (BotChoice == 3)
        //        {
        //            if (UserChoice == "stone")
        //            {
        //                await context.PostAsync($"Bot chose scissors");
        //                await context.PostAsync($"You win,stone beats scissors");
        //                count--;
        //                UserScore++;
        //            }
        //            else if (UserChoice == "paper")
        //            {
        //                await context.PostAsync($"Bot chose stone");
        //                await context.PostAsync($"You win,paper beats stone");
        //                count--;
        //                UserScore++;
        //            }
        //            else if (UserChoice == "scissors")
        //            {
        //                await context.PostAsync($"Bot chose paper");
        //                await context.PostAsync($"You win,scissors beat paper");
        //                UserScore++;
        //                count--;
        //            }
        //            else
        //            {
        //                await context.PostAsync($"You must choose stone,paper or scissors!");

        //            }
                    
        //        }

        //    }

        //    else
        //    {
        //        await context.PostAsync($"Users score is: {UserScore}");
        //        await context.PostAsync($"Bot score is: {BotScore}");

        //        if (BotScore > UserScore)
        //        {
        //            await context.PostAsync($"Bot is the Winner");
        //        }
        //        else if (BotScore < UserScore)
        //        {
        //            await context.PostAsync($"User is the Winner");
        //        }
        //        else
        //        {
        //            await context.PostAsync($"It is a TIE match");
        //        }
        //    }
            


            

        //    // Calculate something for us to return
        //    //int length = (activity.Text ?? string.Empty).Length;

        //    // Return our reply to the user
        //    // await context.PostAsync($"You sent {activity.Text} which was {length} characters");

        //}
        
    }
}