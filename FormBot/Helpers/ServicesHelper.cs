using FormBot.Common.Constants;
using FormBot.Common.Models;
using FormBot.Services;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Helpers
{
    public class ServicesHelper
    {

        private IFormBotServices _services;

        /// <summary>
        /// Defines the LuisConfiguration
        /// </summary>
        /// 

        public static readonly string QnaConfiguration = "IQna";

        public static readonly string LuisConfiguration = "LuisServiceOne";

        public ServicesHelper(IFormBotServices services)
        {
            //Get services defined in the HyperionBotServices
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
        }
        public async Task<LuisResultModel> GetLuisIntentForMessage(ITurnContext turnContext, string ServiceName)
        {
            try
            {
                //Model class LuisResult provided by the BotBuilder SDK
                LuisResultModel tempCreateQuote = new LuisResultModel();
                if (turnContext == null) {
                    throw new  NullReferenceException("TurnContext Error");
                }
                // If the user clicks the buttons the values are populated onto Activity.Value .
                // We don't need to Hit Luis for Button clicks .
                if (turnContext.Activity.Value != null)
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(turnContext.Activity.Value.ToString());
                    tempCreateQuote.TopScoringIntentName = IntentNames.None;
                    return tempCreateQuote;
                }

                //Get the Luis results for the user input for specified luis config in LuisServices ..
                var luisResults = await _services.LuisServices[ServiceName].RecognizeAsync(turnContext, default(CancellationToken)).ConfigureAwait(true);

                tempCreateQuote.TopScoringIntentName = luisResults.Intents.FirstOrDefault().Key;

                // Gather entity values if available. Uses a const list of LUIS entity names.
                foreach (var entity in luisResults.Entities)
                {
                    var entityName = entity.Key;
                    var entityValues = entity.Value;

                    if (!entityName.Equals("$instance", StringComparison.OrdinalIgnoreCase) && null != entityValues && entityValues.Count() > 0)
                    {
                        tempCreateQuote.ExtractedEntities.Add(new BotEntity() { Name = entityName, Value = entityValues[0].ToString() });
                    }
                }

                return tempCreateQuote;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> GetAnswerFromQnA(ITurnContext turnContext)
        {
            // If the TopScoring intent is None Qna flow is triggered.
            // Results are matched based on topscoring question in QnaMaker

            // Call QnA Maker and get results
            // Refer https://www.qnamaker.ai/ 
            try
            {
                var qnaResult = await _services.QnaServices[QnaConfiguration].GetAnswersAsync(turnContext).ConfigureAwait(true);

                // Show the answer only if the answer count is 1 else omit.
                if (qnaResult != null || qnaResult.Count() > 0)
                {
                    return qnaResult[0].Answer;
                }
                else
                {
                    return StringConstants.NoMatchAnswer;
                }
            }
            catch (Exception)
            {
                return StringConstants.NoMatchAnswer;
            }
        }


    }
}
