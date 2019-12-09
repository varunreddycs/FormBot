using AdaptiveCards;
using FormBot.Common.Constants;
using FormBot.Common.Models;
using FormBot.Extensions;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBot.Helpers
{
    public class PromptsHelper
    {
        internal static UserResponse GetUserResponse(ITurnContext turnContext)
        {
            UserResponse userResponse = new UserResponse();

            // Get the active answer provided by the user
            if (!string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                userResponse.ResponseText = turnContext.Activity.Text.Trim();
            }
            else if (turnContext.Activity.Value != null)
            {
                // Try parsing the value in to a dictionary
                var valuesDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(turnContext.Activity.Value.ToString());

                // See if the key contains dictionary (for the DATE type objects)
                foreach (string key in valuesDictionary.Keys)
                {
                    //var newDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(key);
                    Dictionary<string, string> newDict = new Dictionary<string, string>();
                    if (key.TryParseJson(out newDict))
                    {
                        if (newDict.ContainsKey(CardActivityReferences.EntityName))
                        {
                            userResponse.CurrentEntityName = newDict[CardActivityReferences.EntityName];
                        }

                        if (newDict.ContainsKey(CardActivityReferences.FlowIDReference))
                        {
                            userResponse.FlowID = newDict[CardActivityReferences.FlowIDReference];
                        }

                        userResponse.ResponseText = valuesDictionary[key];
                    }
                }

                if (valuesDictionary.ContainsKey(CardActivityReferences.EntityValue))
                {
                    userResponse.ResponseText = valuesDictionary[CardActivityReferences.EntityValue];
                }

                if (valuesDictionary.ContainsKey(CardActivityReferences.EntityName))
                {
                    userResponse.CurrentEntityName = valuesDictionary[CardActivityReferences.EntityName];
                }

                if (valuesDictionary.ContainsKey(CardActivityReferences.FlowIDReference))
                {
                    userResponse.FlowID = valuesDictionary[CardActivityReferences.FlowIDReference];
                }

                if (valuesDictionary.ContainsKey(CardActivityReferences.Phase))
                {
                    userResponse.Phase = valuesDictionary[CardActivityReferences.Phase];

                    //// If there is a phase, then we need to know which entity has been selected for modification
                    //userResponse.ResponseText = valuesDictionary[CardActivityReferences.EntityName];
                }

                if (valuesDictionary.ContainsKey(CardActivityReferences.EntityId))
                {
                    userResponse.EntityId = valuesDictionary[CardActivityReferences.EntityId];
                }
            }

            return userResponse;
        }


        internal static async void PromptForConfirmationWithCards(DialogContext dialogContext, string questionToAsk, string entityName, string quoteReference)
        {
            // Adaptive cards Boolean Prompt Style
            // Instead of using the Botframework prompts these are handled using additional parameters specific to the flow(Quote,FirmOrder ..etc).
            await dialogContext.Context.SendActivityAsync(questionToAsk).ConfigureAwait(true);

            var activity = dialogContext.Context.Activity.CreateReply();
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var attachment = PromptsHelper.CreateAdaptiveCardAttachment(new List<string>() { "Yes", "No" }, entityName, quoteReference);
            activity.Attachments.Add(attachment);

            await dialogContext.Context.SendActivityAsync(activity).ConfigureAwait(true);
        }

        public static string ConstructSummaryContent(BotIntent intentDetails)
        {
            // Summary string is generated based on BotIntent Provided.
            List<BotEntity> currentFlowEntities = intentDetails.ExpectedEntities;
            StringBuilder summaryText = new StringBuilder();
            string heading = "The Summary of details provided by you are:";

            summaryText.AppendFormat("**{0}**", heading);
            summaryText.AppendLine();
            summaryText.AppendLine();

            foreach (BotEntity botEntity in currentFlowEntities)
            {
                if ((botEntity.IsActiveEntity || botEntity.ShowInSummary) && !string.IsNullOrEmpty(botEntity.Value))
                {
                    summaryText.AppendFormat("**{0}** : {1} \n\n", botEntity.DisplayName, botEntity.Value);
                    summaryText.AppendLine();
                }
            }

            return summaryText.ToString();
        }
        private static Attachment CreateAdaptiveCardAttachment(List<string> cardDisplayNames, string entityName, string quoteReference)
        {
            //Create Adaptive cards using AdaptiveCard Class .
            //Refer https://adaptivecards.io/ For more information
            // We are scoping down each button value to its purpose by providing entityName 
            // You can also scope up or down by restricting the button values but you have to handle the additional fields you provide or remove them.
            AdaptiveCard adaptiveCard = new AdaptiveCard();
            foreach (string cardName in cardDisplayNames)
            {
                var dict = new Dictionary<string, string>();
                dict.Add(CardActivityReferences.EntityValue, cardName);
                dict.Add(CardActivityReferences.EntityName, entityName);
                dict.Add(CardActivityReferences.FlowIDReference, quoteReference);

                adaptiveCard.Actions.Add(
                    new AdaptiveSubmitAction()
                    {
                        Title = cardName,
                        Type = "Action.Submit",
                        DataJson = JsonConvert.SerializeObject(dict).ToString()
                    });
            }

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCard.ToJson())
            };
        }


        internal static Activity ConstructAdaptiveCardActivity(DialogContext dialogContext, List<string> entityValues, string currentEntityName, string quoteReference)
        {
            var activity = dialogContext.Context.Activity.CreateReply();
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            // We need to break the list of values in to chunks of 5 or 6 and add each set as an attachement to activity
            // This enables pagination of results (showing 5 results per page)
            List<List<string>> validValuesChunkSet = new List<List<string>>();
            validValuesChunkSet = ListExtensions.ChunkBy<string>(entityValues, 5);

            foreach (List<string> itemSet in validValuesChunkSet)
            {
                var attachment = PromptsHelper.CreateAdaptiveCardAttachment(itemSet, currentEntityName, quoteReference);
                activity.Attachments.Add(attachment);
            }

            return activity;
        }

        internal static Activity ConstructDateActivity(DialogContext dialogContext, List<BotEntity> currentEntities, BotEntity currentEntity, string quoteReference)
        {
            // Date type Activity with Adaptive cards as Attachment
            var activity = dialogContext.Context.Activity.CreateReply();
            activity.Attachments.Add(new Attachment()
            {
                Content = CreateDateTimeAdaptiveCard(currentEntities, currentEntity, quoteReference),
                ContentType = AdaptiveCards.AdaptiveCard.ContentType
            });

            return activity;
        }
        private static AdaptiveCard CreateDateTimeAdaptiveCard(List<BotEntity> currentEntities, BotEntity currentEntity, string quoteReference)
        {
            // DateTime Adaptive card to generate Date.. Customizable as per Adaptivecards
            // Refer https://adaptivecards.io/ For more information
            AdaptiveCard adaptiveCard = new AdaptiveCard();
            DateTime valueToUse = DateTime.Now;
            string dateValueString = string.Empty;
            var dict = new Dictionary<string, string>();
            dict.Add(CardActivityReferences.EntityName, currentEntity.Name);
            dict.Add(CardActivityReferences.FlowIDReference, quoteReference);

            dateValueString = valueToUse.ToString("yyyy-MM-dd");

            adaptiveCard.Body = new List<AdaptiveElement>
            {
                new AdaptiveTextBlock()
                {
                    Text = currentEntity.Name
                },
                new AdaptiveDateInput()
                {
                    Type = "Input.Date",
                    Id = JsonConvert.SerializeObject(dict).ToString(CultureInfo.InvariantCulture),
                    Value = dateValueString
                }
            };
            adaptiveCard.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "OK",
            });
            return adaptiveCard;
        }

        internal static Activity ConstructAdaptiveCardActivityForModification(DialogContext dialogContext, List<BotEntity> expectedEntities, string quoteReference)
        {
            var activity = dialogContext.Context.Activity.CreateReply();
            activity.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            List<string> entityFormattedStrings = new List<string>();

            // First construct a list of string with ID:DisplayName as the format
            foreach (BotEntity entity in expectedEntities)
            {
                if (entity.IsActiveEntity.Equals(true))
                {
                    entityFormattedStrings.Add(string.Format("{0}:{1}", entity.EntityId, entity.Name));
                }
            }

            // We need to break the list of values in to chunks of 5 or 6 and add each set as an attachement to activity
            // This enables pagination of results (showing 5 results per page)
            List<List<string>> validValuesChunkSet = new List<List<string>>();
            validValuesChunkSet = ListExtensions.ChunkBy<string>(entityFormattedStrings, 5);

            //Create AdaptiveCard Attachment and add all the cards to attachment.
            foreach (List<string> itemSet in validValuesChunkSet)
            {
                var attachment = PromptsHelper.CreateAdaptiveCardAttachmentForModification(itemSet, quoteReference);
                activity.Attachments.Add(attachment);
            }

            return activity;
        }

        private static Attachment CreateAdaptiveCardAttachmentForModification(List<string> entityIDandDisplayNames, string quoteReference)
        {
            AdaptiveCard adaptiveCard = new AdaptiveCard();

            foreach (string entity in entityIDandDisplayNames)
            {
                string[] entityDetails = entity.Split(":");

                var dict = new Dictionary<string, string>();
                dict.Add(CardActivityReferences.Phase, "Modification");
                dict.Add(CardActivityReferences.EntityId, entityDetails[0]);
                dict.Add(CardActivityReferences.EntityName, entityDetails[1]);
                dict.Add(CardActivityReferences.FlowIDReference, quoteReference);

                adaptiveCard.Actions.Add(
                    new AdaptiveSubmitAction()
                    {
                        Title = entityDetails[1],
                        Type = "Action.Submit",
                        DataJson = JsonConvert.SerializeObject(dict).ToString()
                    });
            }

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = JsonConvert.DeserializeObject(adaptiveCard.ToJson())
            };
        }
    }
}
