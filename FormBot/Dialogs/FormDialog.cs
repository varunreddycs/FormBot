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
    public class FormDialog: ComponentDialog
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
        private DialogHelper dialogHelper;
        private CurrentFlowState _currentFlowState;

        //public FormDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(FormDialog))
        //{
        //    _services = services ?? throw new System.ArgumentNullException(nameof(services));
        //    _servicesHelper = new ServicesHelper(services);
        //    _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
        //    //this.AddDialog(new WaterfallDialog(DialogIds.FormWaterFallDialog, new WaterfallStep[]
        //    //{
        //    //                InitialStepAsync,
        //    //                FinalStepAsync
        //    //}));
        //    //this.InitialDialogId = DialogIds.FormWaterFallDialog;
        //}

        //protected async override Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default)
        //{
        //    _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(innerDc.Context, () => new CurrentFlowState()).ConfigureAwait(true);

        //    this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);
        //    UserResponse userResponse = PromptsHelper.GetUserResponse(innerDc.Context);

        //    return await dialogHelper.ProcessDialogFlow(innerDc, userResponse, cancellationToken).ConfigureAwait(true);
        //}



        //private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, () => new CurrentFlowState()).ConfigureAwait(true);

        //    this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);
        //    UserResponse userResponse = PromptsHelper.GetUserResponse(stepContext.Context);

        //    return await this.dialogHelper.ProcessDialogFlow(stepContext, userResponse, cancellationToken).ConfigureAwait(true);
        //}
        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    return await stepContext.ContinueDialogAsync().ConfigureAwait(true);
        //}


        public FormDialog(IFormBotServices services, FormBotAccessors accessors) : base(nameof(FormDialog))
        {
            _services = services ?? throw new System.ArgumentNullException(nameof(services));
            _servicesHelper = new ServicesHelper(services);
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

            //this.AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            //{
            //                InitialStepAsync,
            //                FinalStepAsync
            //}));
            //this.InitialDialogId = nameof(WaterfallDialog);
        }


        //private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
      
        //    _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, () => new CurrentFlowState()).ConfigureAwait(true);

        //    this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);
        //    UserResponse userResponse = PromptsHelper.GetUserResponse(stepContext.Context);

        //    return await dialogHelper.ProcessDialogFlow(stepContext, userResponse, cancellationToken).ConfigureAwait(true);

        //}

        protected async override Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default)
        {
            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(innerDc.Context, () => new CurrentFlowState()).ConfigureAwait(true);

            this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);
            UserResponse userResponse = PromptsHelper.GetUserResponse(innerDc.Context);

            return await dialogHelper.ProcessDialogFlow(innerDc, userResponse, cancellationToken).ConfigureAwait(true);

        }

        protected  async override Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(innerDc.Context, () => new CurrentFlowState()).ConfigureAwait(true);

            this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);
            UserResponse userResponse = PromptsHelper.GetUserResponse(innerDc.Context);

            return await dialogHelper.ProcessDialogFlow(innerDc, userResponse, cancellationToken).ConfigureAwait(true);
        }

        //private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    return await stepContext.ContinueDialogAsync().ConfigureAwait(true);
        //}
    }
}
