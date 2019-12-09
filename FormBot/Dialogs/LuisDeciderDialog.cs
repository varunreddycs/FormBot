using FormBot.Accessors;
using FormBot.Common.Constants;
using FormBot.Common.Helpers;
using FormBot.Common.Models;
using FormBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Dialogs
{
    public class LuisDeciderDialog : ComponentDialog
    {
        // This Dialog is key dialog which will decide which dialog need to be hit for the user request
        // Each Dialog has set of instructions to respond to user request if configured.


        /// <summary>
        /// Defines the _currentFlowState
        /// </summary>
        private CurrentFlowState _currentFlowState;

        /// <summary>
        /// Defines the _accessors
        /// Custom create your own accessors .These will let you interact with the ConversationState and UserState
        /// You can store and retrieve values as keyvalue pairs across.
        /// </summary>
        private readonly FormBotAccessors _accessors;

        /// <summary>
        /// Defines the _services
        /// Define your own external services and these can be used across the project.
        /// </summary>
        private readonly IFormBotServices _services;

        /// <summary>
        /// Defines the _dialogSet
        /// A set of dialogs to maintain.
        /// You can Add dialogs invoke them and also delete dialogs in the dialogstack
        /// </summary>
        private readonly DialogSet _dialogSet;

        /// <summary>
        /// Defines the _servicesHelper
        /// Define Your Own services in HyperionBotSerices
        /// </summary>
        private readonly ServicesHelper _servicesHelper;
        public LuisDeciderDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(LuisDeciderDialog))
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _servicesHelper = new ServicesHelper(services);
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            this.AddDialog(new WaterfallDialog(DialogIds.LuisDeciderWaterFallStepID, new WaterfallStep[]
            {
                            InitialStepAsync
                           
            }));
            this.InitialDialogId = DialogIds.LuisDeciderWaterFallStepID;
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await stepContext.Context.SendActivityAsync(" Hello").ConfigureAwait(false);
            LuisResultModel luisResult = await _servicesHelper.GetLuisIntentForMessage(stepContext.Context, ServicesHelper.LuisConfiguration).ConfigureAwait(true);

            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, () => new CurrentFlowState()).ConfigureAwait(true);

            if (string.IsNullOrEmpty(_currentFlowState.NameOfCurrentFlow))
            {
                string quoteTemporaryReference = DateTime.UtcNow.ToString("ddMMyyyyhhmmssfff");
                // Check what is the LUIS intent for the current message
                FileStorageHelper fileStorageHelper = new FileStorageHelper();
                //Get all the Intents from the Json File which is in Azure Storage Account
                //Intents intentsJson = fileStorageHelper.GetFileAsJsonTypeFromBlob<Intents>();
                Intents intentsJson = FileStorageHelper.GetIntent<Intents>();
                // comment below line and uncomment the above line to use the IntentsInfo from Blob storage 

                //Set the Ttemporary Quote Reference for every flow triggered.
                //Add the prefix to temporary quote reference as it is generated from this bot.
                _currentFlowState.FlowAbortedResponse = intentsJson.FlowAbortedMessage;
                _currentFlowState.NameOfCurrentFlow = luisResult.TopScoringIntentName;
                _currentFlowState.NumberOfCardsToShow = intentsJson.NumberOfCardsToShow;
                _currentFlowState.IsFlowInitialization = true;
                BotIntent quoteFlowCommonIntentFromJson = intentsJson.BotIntents.Find(x => x.Name.Equals(_currentFlowState.NameOfCurrentFlow, StringComparison.OrdinalIgnoreCase));

                switch (luisResult.TopScoringIntentName)
                {

                    case IntentNames.CreateForm:
                        // Set that the current flow is "CreateQuote" (Only once)

                        // Authorizing the User based on Azure Security Groups
                        // User is able to do specific tasks only if the user is present in the groups defined by Flows in IntentsInfo.Json
                        // User is Authorized only if the matchScore is greater than 1

                        _currentFlowState.FlowID = StringConstants.ReferenceIdPrefix + quoteTemporaryReference;

                        foreach (var entity in luisResult.ExtractedEntities)
                        {
                            quoteFlowCommonIntentFromJson.ExpectedEntities.Find(x => x.Name.Equals(entity.Name, StringComparison.OrdinalIgnoreCase)).Value = entity.Value;
                        }

                        await stepContext.Context.SendActivityAsync(quoteFlowCommonIntentFromJson.IntentResponse).ConfigureAwait(true);

                        // Set the State with appropriate values and forward to Main dialog for validation
                        CommonIntentState quoteFlowCommonIntentState = await _accessors.CommonIntentStateAccessor.GetAsync(stepContext.Context, () => new CommonIntentState(), cancellationToken).ConfigureAwait(true);
                        quoteFlowCommonIntentState.IntentDetails = quoteFlowCommonIntentFromJson;
                        quoteFlowCommonIntentState.IntentName = luisResult.TopScoringIntentName;
                        await _accessors.CommonIntentStateAccessor.SetAsync(stepContext.Context, quoteFlowCommonIntentState).ConfigureAwait(true);

                        await _accessors.CurrentFlowStateAccessor.SetAsync(stepContext.Context, _currentFlowState).ConfigureAwait(true);
                        await _accessors.ConversationState.SaveChangesAsync(stepContext.Context).ConfigureAwait(true);
                        return await stepContext.BeginDialogAsync(DialogIds.FlowManagerDialogID).ConfigureAwait(true);

                    case IntentNames.None:

                        // Check if QnA has an answer
                        // Qna Response
                        string answer = await _servicesHelper.GetAnswerFromQnA(stepContext.Context).ConfigureAwait(true);

                        await stepContext.Context.SendActivityAsync(StringConstants.NoneIntentResponse).ConfigureAwait(true);


                        await _accessors.ConversationState.ClearStateAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                        await _accessors.ConversationState.DeleteAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                        return await stepContext.EndDialogAsync(DialogIds.LuisDeciderDialogId).ConfigureAwait(true);

                    case IntentNames.Cancel:
                        // This implies, user wants to end the flow
                        await stepContext.Context.SendActivityAsync(StringConstants.GoodBye).ConfigureAwait(true);

                        // Ask for confirmation and clear the state
                        await _accessors.ConversationState.ClearStateAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                        await _accessors.ConversationState.DeleteAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);

                        return await stepContext.EndDialogAsync(DialogIds.LuisDeciderDialogId).ConfigureAwait(true);

                    default:

                        return await stepContext.EndDialogAsync(DialogIds.LuisDeciderDialogId).ConfigureAwait(true);

                }

            }
            else
            {
                // This implies, user wants to end the flow
                await stepContext.Context.SendActivityAsync(StringConstants.GoodBye).ConfigureAwait(true);

                // Ask for confirmation and clear the state
                await _accessors.ConversationState.ClearStateAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                await _accessors.ConversationState.DeleteAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);

                return await stepContext.EndDialogAsync(DialogIds.LuisDeciderDialogId).ConfigureAwait(true);

            }
        }


    }
}
