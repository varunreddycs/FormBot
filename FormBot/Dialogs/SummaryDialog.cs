using FormBot.Accessors;
using FormBot.Common.Constants;
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
    public class SummaryDialog : ComponentDialog
    {
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

        private CurrentFlowState _currentFlowState;

        private CommonIntentState _commonIntentState;


        public SummaryDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(SummaryDialog))
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _servicesHelper = new ServicesHelper(services);
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                            InitialStepAsync,
                            FinalStepAsync
            }));
            this.InitialDialogId = nameof(WaterfallDialog);
        }



        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string summaryText = string.Empty;
            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, () => null, cancellationToken).ConfigureAwait(true);

            // Check if summary + modify option is already shown to the user
            if (_currentFlowState.IsModifyOptionShown)
            {
                // Send to modify flow (because already summary and modify options are shown)
                // The modify flow will figure out the response and details
                return await stepContext.BeginDialogAsync(DialogIds.ModifyDetailsDialogId).ConfigureAwait(true);
            }
            else
            {
                // Get the Additional entities if added . For now only Assureds are added.

                // Before showing the summary dialog, set the state accordingly
                // Since we reached summary, ensure current entity name and value is set to null
                _currentFlowState.CurrentEntityName = string.Empty;
                _currentFlowState.CurrentEntityId = -1;

                _currentFlowState.IsModifyOptionShown = true;
                _currentFlowState.ModifyOptionAnswer = string.Empty;
                _currentFlowState.IsModificationEntitySelected = false;

                await _accessors.CurrentFlowStateAccessor.SetAsync(stepContext.Context, _currentFlowState).ConfigureAwait(true);
                await _accessors.ConversationState.SaveChangesAsync(stepContext.Context).ConfigureAwait(true);


                _commonIntentState = await _accessors.CommonIntentStateAccessor.GetAsync(stepContext.Context, () => null, cancellationToken).ConfigureAwait(true);
                summaryText = PromptsHelper.ConstructSummaryContent(_commonIntentState.IntentDetails);

                await stepContext.Context.SendActivityAsync(summaryText).ConfigureAwait(true);


                PromptsHelper.PromptForConfirmationWithCards(stepContext, _commonIntentState.IntentDetails.ModificationQuestion,
                            StringConstants.Modify, _currentFlowState.FlowID);

            }
            return await base.BeginDialogAsync(stepContext).ConfigureAwait(true);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }
    }
}
