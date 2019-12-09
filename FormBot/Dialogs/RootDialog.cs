using FormBot.Accessors;
using FormBot.Common.Constants;
using FormBot.Helpers;
using FormBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Dialogs
{
    public class RootDialog: ComponentDialog
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
        public RootDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(RootDialog))
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _servicesHelper = new ServicesHelper(services);
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));



            // Initialize a Dialog Set and add MainDialog to the stack
            // The main dialog will take care of passing on to corresponding child dialogs
            // based on the information in the state


            // Add the dialogs which are to be added      
            this.AddDialog(new LuisDeciderDialog(services,accessors));
            this.AddDialog(new FormDialog(services, accessors));
            this.AddDialog(new FlowManagerDialog(services, accessors));
            this.AddDialog(new SummaryDialog(services, accessors));
            this.AddDialog(new ModifyDetailsDialog(services, accessors));

            // Root step is the firest step which will run every time user triggers the flow if the other dialogs are not running
            this.AddDialog(new WaterfallDialog(DialogIds.RootWaterFallDialogID, new WaterfallStep[]
            {
                            InitialStepAsync,
                            FinalStepAsync
            }));
            this.InitialDialogId = DialogIds.RootWaterFallDialogID;
        }


        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.BeginDialogAsync(nameof(LuisDeciderDialog)).ConfigureAwait(true);
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.ContinueDialogAsync().ConfigureAwait(true);
        }
    }
}
