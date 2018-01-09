using AlexaSkillsKit.Speechlet;
using Azure4Alexa.Alexa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Azure4Alexa.RockPaperScissors
{
    public class RockPaperScissors
    {
        // URL to GET random result of rock, paper, or scissors        
        // "https://mls-get-rock-paper-scissors.azurewebsites.net/api/HttpTriggerCSharp1?code=ztDs1UMgxTNJg/ZgNr69Nj/ZW6wiu8kDlRkXGlFj749ZNxBPByMgLg==";

        public static string getRockPaperScissorsUrl =
            "https://mls-get-rock-paper-scissors.azurewebsites.net/api/HttpTriggerCSharp1?code=ztDs1UMgxTNJg/ZgNr69Nj/ZW6wiu8kDlRkXGlFj749ZNxBPByMgLg==";

        // Call the remote web service.  Invoked from AlexaSpeechletAsync
        // Then, call another function with the raw JSON results to generate the spoken text and card text

        public static async Task<SpeechletResponse> GetResults(Session session, HttpClient httpClient)
        {
            string httpResultString = "";

            // Connect to RockPaperScissors API Endpoint

            httpClient.DefaultRequestHeaders.Clear();

            var httpResponseMessage = await httpClient.GetAsync(getRockPaperScissorsUrl);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                httpResultString = await httpResponseMessage.Content.ReadAsStringAsync();
            }
            else
            {
                httpResponseMessage.Dispose();
                return AlexaUtils.BuildSpeechletResponse(new AlexaUtils.SimpleIntentResponse() { cardText = AlexaConstants.AppErrorMessage }, true);
            }


            var simpleIntentResponse = ParseResults(httpResultString);
            httpResponseMessage.Dispose();
            return AlexaUtils.BuildSpeechletResponse(simpleIntentResponse, true);

        }

        private static AlexaUtils.SimpleIntentResponse ParseResults(string resultString)
        {
            string stringToRead = String.Empty;
            string stringForCard = String.Empty;
           
            JObject resultObject = JObject.Parse(resultString);

            foreach (KeyValuePair<string, JToken> property in resultObject)
                //only expecting 1
            {                
                //string s = property.Value.ToString();
                stringToRead += "<break time=\"0s\" /> One. Two. Three. ";
                stringToRead += $"{property.Value}";
                stringForCard += $"{property.Value}";
            }

            string extraString = " Amy, you are the most charming, beautiful human I've ever met! I hope you win the lottery on Saturday.";
            stringToRead = Alexa.AlexaUtils.AddSpeakTagsAndClean(stringToRead + extraString);

            // Build the response

            //stringToRead = $"One. Two. Three. {stringToRead}";            

            //return new AlexaUtils.SimpleIntentResponse() { cardText = stringForCard, ssmlString = stringToRead };

            // if you want to add images, you can include them in the reply
            // images should be placed into the ~/Images/ folder of this project
            // 

            // JPEG or PNG supported, no larger than 2MB
            // 720x480 - small size recommendation
            // 1200x800 - large size recommendation

            return new AlexaUtils.SimpleIntentResponse()
            {
                cardText = stringForCard,
                ssmlString = stringToRead,
                largeImage = "msft.png",
                smallImage = "msft.png",
            };

        }
    }
}
