using FormBot.Accessors;
using FormBot.Common.Constants;
using FormBot.Common.Models;
using FormBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Dialogs
{
    public class FlowManagerDialog: ComponentDialog
    {
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
        /// 

        private readonly ServicesHelper _servicesHelper;
        public FlowManagerDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(FlowManagerDialog))
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _servicesHelper = new ServicesHelper(services);
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            this.AddDialog(new WaterfallDialog(DialogIds.FlowManagerWaterFallDialogID, new WaterfallStep[]
            {
                            InitialStepAsync,
            }));
            this.InitialDialogId = DialogIds.FlowManagerWaterFallDialogID ;
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            // This is the Main Dialog Instantiate All the dialogs from here.

            // This Flow Decider Dialog is key dialog which is intended to handle interruptions
            // If user asks the question which is irrelevant to the form . Pause the flow and answer the question and resume the flow 
            if (stepContext == null)
            {
                throw new ArgumentNullException(nameof(stepContext));
            }

            // Don't do anything for non-message activities.
            if (stepContext.Context.Activity.Type != ActivityTypes.Message)
            {
                return await stepContext.EndDialogAsync(new Dictionary<string, object>()).ConfigureAwait(true);
            }

            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, null, cancellationToken).ConfigureAwait(true);

            switch (_currentFlowState.NameOfCurrentFlow)
            {

                case IntentNames.CreateForm:
                    return await stepContext.BeginDialogAsync(DialogIds.FormDialogId).ConfigureAwait(true);
                case IntentNames.None:
                    return await stepContext.ContinueDialogAsync().ConfigureAwait(true);

                default:
                    await stepContext.Context.SendActivityAsync(StringConstants.NotImplemented).ConfigureAwait(true);

                    return await stepContext.ContinueDialogAsync().ConfigureAwait(true);
            }
        }
    }
}
