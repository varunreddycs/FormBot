using FormBot.Accessors;
using FormBot.Common.Constants;
using FormBot.Common.Models;
using FormBot.Helpers;
using FormBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Dialogs
{
    public class ModifyDetailsDialog: ComponentDialog
    {
        private CurrentFlowState _currentFlowState;
        private CommonIntentState _commonIntentState;
        private FormBotAccessors _accessors;
        private DialogHelper dialogHelper;
        public ModifyDetailsDialog(IFormBotServices services,FormBotAccessors accessors)
          : base(DialogIds.ModifyDetailsDialogId)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            this.AddDialog(new WaterfallDialog(nameof(ModifyDetailsDialog), new WaterfallStep[]
            {
                 InitialStepAsync,
                 FinalStepAsync
            }));
            this.InitialDialogId = nameof(ModifyDetailsDialog);

        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(stepContext.Context, () => null, cancellationToken).ConfigureAwait(true);
            _commonIntentState = await _accessors.CommonIntentStateAccessor.GetAsync(stepContext.Context, () => null, cancellationToken).ConfigureAwait(true);

            // If modify option is already shown, see if user has selected a valid option
            UserResponse userResponse = PromptsHelper.GetUserResponse(stepContext.Context);
            this.dialogHelper = new DialogHelper(_accessors, _currentFlowState);

            switch (_currentFlowState.NameOfCurrentFlow)
            {

                case IntentNames.CreateForm:
                    #region Temp Region

                    List<BotEntity> newBotEntities;

                    if (userResponse.Phase.Equals(StringConstants.Modification, StringComparison.OrdinalIgnoreCase) && !_currentFlowState.IsModificationEntitySelected)
                    {
                        // The the user response entity id as the current modification ID
                        // NOTE: **Ensure that this is set before the entities are updated 
                        //       as the UpdateEntitiesInJson will change the user response object
                        _currentFlowState.ModificationStartEntityId = userResponse.EntityId;


                        // IF the user selects Assured Entitiies clear all the Entities 


                        // Implies the cards are already shown to the user and user has selected the field to modify
                        this.UpdateEntitiesInJson(userResponse, _commonIntentState.IntentDetails.ExpectedEntities);

                        await _accessors.CurrentFlowStateAccessor.SetAsync(stepContext.Context, _currentFlowState).ConfigureAwait(true);
                        await _accessors.CommonIntentStateAccessor.SetAsync(stepContext.Context, _commonIntentState).ConfigureAwait(true);
                        await _accessors.ConversationState.SaveChangesAsync(stepContext.Context).ConfigureAwait(true);

                        // This will re-start the dialog and asks the next question depending on validation
                        await stepContext.ReplaceDialogAsync(DialogIds.FormDialogId).ConfigureAwait(true);
                    }
                    else if (userResponse.CurrentEntityName.Equals(StringConstants.Submit, StringComparison.OrdinalIgnoreCase))
                    {
                        // User has Clicked the submit .. Build Json and Push to the queue
                        if (userResponse.ResponseText.Equals(StringConstants.Yes, StringComparison.OrdinalIgnoreCase))
                        {

                            // These are the additional entites which were been added during the flow.
                            // Essentially for now only the Assureds are being added into this . since this is multiple valued entity

                            //Common.Helpers.SharepointHelper.InsretIntoSPList();

                            //Proceed for submission

                            // Send the JSON for submission to WebApp

                            // Do submit where ever you want 

                            await stepContext.Context.SendActivityAsync(_currentFlowState.SubmitOptionAnswer).ConfigureAwait(true);


                        }
                        else
                        {
                            await stepContext.Context.SendActivityAsync(_currentFlowState.FlowAbortedResponse).ConfigureAwait(true);
                        }

                        // Ask for confirmation and clear the state
                        await _accessors.ConversationState.ClearStateAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                        await _accessors.ConversationState.DeleteAsync(stepContext.Context, cancellationToken).ConfigureAwait(true);
                    }
                    else
                    {
                        await ShowModificationEntityCards(stepContext, userResponse, _commonIntentState.IntentDetails.ExpectedEntities.Where(a => a.IsActiveEntity == true).ToList()).ConfigureAwait(true);
                    }

                    #endregion

                    break;



                    break;
                default:
                    break;
            }

            return await stepContext.EndDialogAsync().ConfigureAwait(true);
        }



        private void UpdateEntitiesInJson(UserResponse userResponse, List<BotEntity> currentEntities)
        {
            // If we have entity Id and entity name available in the response, 
            // implies the user has selected the entity to modify
            string entityid = userResponse.EntityId;
            List<int> childEntityIds = new List<int>();

            foreach (var entity in currentEntities)
            {
                if (userResponse.EntityId.Equals(entity.EntityId.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    entity.IsValidated = false;
                    entity.Value = string.Empty;
                    childEntityIds = entity.DependentChildEntityIds;
                    break;
                }
            }

            foreach (var entity in currentEntities)
            {
                if (childEntityIds.Contains(entity.EntityId) && entity.IsValidated)
                {
                    entity.IsValidated = false;
                    entity.Value = string.Empty;

                    userResponse.EntityId = entity.EntityId.ToString();

                    this.UpdateEntitiesInJson(userResponse, currentEntities);
                }
            }
        }


        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }

        private async Task ShowModificationEntityCards(DialogContext outerDc, UserResponse userResponse, List<BotEntity> currentEntities)
        {
            // Implies the user has clicked on "Yes" or "No" for question to modify the summary
            if (userResponse.CurrentEntityName.Equals(StringConstants.Modify, StringComparison.OrdinalIgnoreCase)
                && userResponse.ResponseText.Equals(StringConstants.Yes, StringComparison.OrdinalIgnoreCase)
                && userResponse.FlowID.Equals(_currentFlowState.FlowID, StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrEmpty(_currentFlowState.ModifyOptionAnswer))
            {
                // Since we show the modification options again, ensure that previous selections are set to false
                _currentFlowState.IsModificationEntitySelected = false;
                _currentFlowState.ModifyOptionAnswer = StringConstants.Yes;

                await _accessors.CurrentFlowStateAccessor.SetAsync(outerDc.Context, _currentFlowState).ConfigureAwait(true);
                await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);

                // Implies the user is not yet shown with modification options
                // Hence construct the options and let the user choose
                Activity activity = PromptsHelper.ConstructAdaptiveCardActivityForModification(outerDc,
                                                currentEntities, _currentFlowState.FlowID);

                await outerDc.Context.SendActivityAsync(activity).ConfigureAwait(true);
            }

            // Implies the user has clicked on "Yes" or "No" for question to modify the summary
            if (userResponse.CurrentEntityName.Equals(StringConstants.Modify, StringComparison.OrdinalIgnoreCase)
                && userResponse.ResponseText.Equals(StringConstants.No, StringComparison.OrdinalIgnoreCase)
                && userResponse.FlowID.Equals(_currentFlowState.FlowID)
                && string.IsNullOrEmpty(_currentFlowState.ModifyOptionAnswer))
            {
                // Since we show the modification options again, ensure that previous selections are set to false
                _currentFlowState.IsModificationEntitySelected = true;
                _currentFlowState.ModifyOptionAnswer = StringConstants.No;

                await _accessors.CurrentFlowStateAccessor.SetAsync(outerDc.Context, _currentFlowState).ConfigureAwait(true);
                await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);

                // Implies the user want to proceed for submission
                // Hence show the submission options
                PromptsHelper.PromptForConfirmationWithCards(outerDc, StringConstants.SubmitQuestion,
                    StringConstants.Submit, _currentFlowState.FlowID);
            }

            // Ignore any other clicks or selections
        }

    }
}
