using FormBot.Accessors;
using FormBot.Common.Constants;
using FormBot.Common.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FormBot.Helpers
{
    public class DialogHelper
    {
        private CurrentFlowState _currentFlowState;
        private CommonIntentState _commonIntentState;
        private FormBotAccessors _accessors;

        public DialogHelper(FormBotAccessors accessors, CurrentFlowState currentFlowState)
        {
            _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
            _currentFlowState = currentFlowState ?? throw new System.ArgumentNullException(nameof(currentFlowState));
        }


        internal async Task<DialogTurnResult> ProcessDialogFlow(DialogContext outerDc, UserResponse userResponse, CancellationToken cancellationToken)
        {
            _currentFlowState = await _accessors.CurrentFlowStateAccessor.GetAsync(outerDc.Context, () => null, cancellationToken).ConfigureAwait(true);
            _commonIntentState = await _accessors.CommonIntentStateAccessor.GetAsync(outerDc.Context, () => null, cancellationToken).ConfigureAwait(true);

            // Check what entities should be asked for and validated for first
            bool isSummaryToBeShown = await this.ProcessFlowState(outerDc, _commonIntentState.IntentDetails.ExpectedEntities).ConfigureAwait(true);

            if (isSummaryToBeShown)
            {
                return await outerDc.BeginDialogAsync(DialogIds.SummaryDialogId).ConfigureAwait(true);
            }

            // Make sure that modify options/summary is reset as we entered the create flow
            _currentFlowState.IsModifyOptionShown = false;
            _currentFlowState.IsSubmitOptionShown = false;
            await _accessors.CurrentFlowStateAccessor.SetAsync(outerDc.Context, _currentFlowState).ConfigureAwait(true);
            await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);




            if (userResponse.CurrentEntityName == StringConstants.Submit)
            {
                return await outerDc.BeginDialogAsync(DialogIds.SummaryDialogId, cancellationToken).ConfigureAwait(true);
            }
            if (!string.IsNullOrEmpty(_currentFlowState.NameOfCurrentFlow)
                //&& _currentFlowState.NameOfCurrentFlow.Equals(IntentNames.CreateQuote)
                && _commonIntentState != null)
            {
                int currentEntityId = _currentFlowState.CurrentEntityId;
                string currentEntityName = _currentFlowState.CurrentEntityName;
                string temporaryQuoteReference = _currentFlowState.FlowID;

                // When Summary and modification options are shown, if the user clicks on fields after selecting a field already,
                // ignore the click and do nothing (i.e. The modification cards will have phase as "Modification"
                if (userResponse.Phase.Equals(StringConstants.Modification, StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(userResponse.ResponseText))
                {
                    // Implies user is not providing answer but clicking on the modification options
                    if (_currentFlowState.IsModificationEntitySelected == true)
                    {
                        // Implies the user has already clicked on which entity to modify
                        // Hence do not respond to another click
                    }
                    else
                    {
                        // User has selected the modification option for the first time
                        // Set the details and update the state
                        _currentFlowState.IsModificationEntitySelected = true;
                        await _accessors.CurrentFlowStateAccessor.SetAsync(outerDc.Context, _currentFlowState).ConfigureAwait(true);
                        await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);
                    }
                }

                BotEntity currentEntity = _commonIntentState.IntentDetails.ExpectedEntities
                                                    .Find(x => x.Name.Equals(currentEntityName, StringComparison.OrdinalIgnoreCase));


                // Check if the response type is cards and the user has selected a valid card
                if (currentEntity.EntityType == EntityTypes.Cards)
                {
                    if (!string.IsNullOrEmpty(userResponse.ResponseText)
                        && !string.IsNullOrEmpty(userResponse.CurrentEntityName)
                        && !string.IsNullOrEmpty(userResponse.FlowID))
                    {
                        if (currentEntityName.Equals(userResponse.CurrentEntityName, StringComparison.OrdinalIgnoreCase)
                            && temporaryQuoteReference.Equals(userResponse.FlowID, StringComparison.OrdinalIgnoreCase))
                        {


                            // This implies the user has selected a valid card from the selection
                            // No need to re-validate the value.
                            // Just assign and proceed to next question
                            return await this.AssignValueAndAskNextQuestion(outerDc, currentEntity, userResponse.ResponseText).ConfigureAwait(true);
                        }
                        else
                        {
                            // This implies user is taking action on some other cards or previous items
                            // Just ignore
                            return await outerDc.EndDialogAsync();
                        }
                    }
                }



                if (currentEntity.EntityType == EntityTypes.Text && _currentFlowState.IsFlowInitialization == false)
                {
                    if (!string.IsNullOrEmpty(userResponse.ResponseText)
                        && !string.IsNullOrEmpty(userResponse.CurrentEntityName)
                        && !string.IsNullOrEmpty(userResponse.FlowID))
                    {
                        if (currentEntityName.Equals(userResponse.CurrentEntityName, StringComparison.OrdinalIgnoreCase)
                            && temporaryQuoteReference.Equals(userResponse.FlowID, StringComparison.OrdinalIgnoreCase))
                        {
                            // This implies the user has selected a valid card from the selection
                            // No need to re-validate the value.
                            // Just assign and proceed to next question
                            return await this.AssignValueAndAskNextQuestion(outerDc, currentEntity, userResponse.ResponseText).ConfigureAwait(true);
                        }
                        else
                        {
                            // This implies user is taking action on some other cards or previous items
                            // Just ignore
                        }
                    }
                }
                // Validate the response of the user against the current entity Id
                // NOTE: Do not validate if this is the first statement of flow initiation 
                //       (unless there is a value in the entity already provided in the initial statement itself)
                if (_currentFlowState.IsFlowInitialization)
                {
                    // Ensure that the IsFlowInitiation is reset to false
                    _currentFlowState.IsFlowInitialization = false;
                    await outerDc.Context.SendActivityAsync(currentEntity.InitialQuestion).ConfigureAwait(false);

                    await _accessors.CurrentFlowStateAccessor.SetAsync(outerDc.Context, _currentFlowState).ConfigureAwait(true);
                    await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);

                    //return await this.ProcessEntitiesAndValidation(outerDc, currentEntity, currentEntity.Value,
                    //                            _currentFlowState.FlowID, _commonIntentState.IntentDetails.ExpectedEntities, userResponse).ConfigureAwait(true);
                    return await outerDc.ContinueDialogAsync(cancellationToken).ConfigureAwait(true);
                    
                }
                else
                {
                    // This implies we need to validate the value provided by the user either by typing or by selecting the card
                    // Proceed with validation against current entity name and then ask the next question
                    return await this.ProcessEntitiesAndValidation(outerDc, currentEntity, userResponse.ResponseText,
                                                _currentFlowState.FlowID, _commonIntentState.IntentDetails.ExpectedEntities, userResponse).ConfigureAwait(true);
                }
            }
            else
            {
                return await outerDc.EndDialogAsync();
            }
        }

        internal async Task<bool> ProcessFlowState(DialogContext innerDc, List<BotEntity> expectedEntities)
        {
            bool areAllFieldsValidated = false;

            if (null == expectedEntities)
            {
                return false;
            }

            bool isCurrentEntityFound = false;

            // During modification cases, first process the entity that has been intended to modify
            // Then follow the order for remaining child entities

            foreach (var entity in expectedEntities.OrderBy(x => x.EntityId).ToList())
            {
                // Step 1 : Check if there are is any entity that has been set for modification
                if (entity.IsActiveEntity && entity.IsValidated == false && entity.EntityId.ToString().Equals(_currentFlowState.ModificationStartEntityId))
                {
                    // Implies we got the entity to check for
                    isCurrentEntityFound = true;

                    _currentFlowState.CurrentEntityId = entity.EntityId;
                    _currentFlowState.CurrentEntityName = entity.Name;

                    await _accessors.CurrentFlowStateAccessor.SetAsync(innerDc.Context, _currentFlowState).ConfigureAwait(true);
                    await _accessors.ConversationState.SaveChangesAsync(innerDc.Context).ConfigureAwait(true);

                    return areAllFieldsValidated;
                }
            }

            foreach (var entity in expectedEntities.OrderBy(x => x.EntityId).ToList())
            {
                // Step 2 : Check if there are any entities that already have values
                // which are provided already in the initial statement. If so, validate them first
                if (entity.IsActiveEntity && entity.IsValidated == false && !string.IsNullOrEmpty(entity.Value))
                {
                    // Implies we got the entity to check for
                    isCurrentEntityFound = true;

                    _currentFlowState.CurrentEntityId = entity.EntityId;
                    _currentFlowState.CurrentEntityName = entity.Name;

                    await _accessors.CurrentFlowStateAccessor.SetAsync(innerDc.Context, _currentFlowState).ConfigureAwait(true);
                    await _accessors.ConversationState.SaveChangesAsync(innerDc.Context).ConfigureAwait(true);

                    return areAllFieldsValidated;
                }
            }

            // If all the entities which are valid and have values are validated, then the value will be false
            if (isCurrentEntityFound == false)
            {
                foreach (var entity in expectedEntities.OrderBy(x => x.EntityId).ToList())
                {
                    // Check for the first hit of entity Id in the order which is not yet valid
                    if (entity.IsActiveEntity && entity.IsValidated == false)
                    {
                        // Implies we got the entity to check for
                        _currentFlowState.CurrentEntityId = entity.EntityId;
                        _currentFlowState.CurrentEntityName = entity.Name;

                        await _accessors.CurrentFlowStateAccessor.SetAsync(innerDc.Context, _currentFlowState).ConfigureAwait(true);
                        await _accessors.ConversationState.SaveChangesAsync(innerDc.Context).ConfigureAwait(true);

                        break;
                    }
                }
            }

            if (!isCurrentEntityFound)
            {
                // Check if all active entities are validated
                // If so, go to summary and submission dialog
                foreach (var entity in expectedEntities)
                {
                    if (entity.IsActiveEntity)
                    {
                        if (entity.IsValidated == true)
                        {
                            areAllFieldsValidated = true;
                        }
                        else
                        {
                            areAllFieldsValidated = false;
                            break;
                        }
                    }
                }
            }

            return areAllFieldsValidated;
        }


        public async Task<DialogTurnResult> ProcessEntitiesAndValidation(DialogContext outerDc, BotEntity currentEntity,
                    string valueToValidate, string quoteReference, List<BotEntity> currentEntities, UserResponse userResponse)
        {
            // Only for Cards type, allow free text.
            // For all other entity types, user should only select the appropriate options
            if (!string.IsNullOrEmpty(userResponse.ResponseText)
                       && !string.IsNullOrEmpty(userResponse.CurrentEntityName)
                       && !string.IsNullOrEmpty(userResponse.FlowID))
            {
                if (!(currentEntity.Name.Equals(userResponse.CurrentEntityName, StringComparison.OrdinalIgnoreCase)
                    && quoteReference.Equals(userResponse.FlowID, StringComparison.OrdinalIgnoreCase)))
                {
                    return await outerDc.EndDialogAsync();
                }
            }

            // This implies user is taking action on expected entity and is valid
            if (currentEntity.EntityType == EntityTypes.Confirmation)
            {
                if (string.IsNullOrEmpty(valueToValidate))
                {
                    // Ask the confirmation prompt question
                    string initialQuesion = string.Format(currentEntity.InitialQuestion, currentEntity.DisplayName);
                    PromptsHelper.PromptForConfirmationWithCards(outerDc, initialQuesion, currentEntity.Name, quoteReference);
                    return await outerDc.EndDialogAsync();
                }
                else
                {
                    // Only for Cards type, allow free text.
                    // For all other entity types, user should only select the approprate options
                    if (!string.IsNullOrEmpty(userResponse.ResponseText)
                               && !string.IsNullOrEmpty(userResponse.CurrentEntityName)
                               && !string.IsNullOrEmpty(userResponse.FlowID))
                    {
                        if ((currentEntity.Name.Equals(userResponse.CurrentEntityName, StringComparison.OrdinalIgnoreCase)
                            && quoteReference.Equals(userResponse.FlowID, StringComparison.OrdinalIgnoreCase)))
                        {
                            await AssignValueAndAskNextQuestion(outerDc, currentEntity, valueToValidate).ConfigureAwait(true);
                            return await outerDc.EndDialogAsync();
                        }
                    }
                }
            }
            // Process the dateTypes and validating them
            if (currentEntity.EntityType == EntityTypes.Date || currentEntity.EntityType == EntityTypes.PastDate || currentEntity.EntityType == EntityTypes.FutureDate)
            {
                if (string.IsNullOrEmpty(valueToValidate))
                {
                    await outerDc.Context.SendActivityAsync(string.Format(currentEntity.InitialQuestion, currentEntity.DisplayName)).ConfigureAwait(true);

                    // Ask the date for input
                    Activity dateActivity = PromptsHelper.ConstructDateActivity(outerDc, currentEntities, currentEntity, quoteReference);
                    await outerDc.Context.SendActivityAsync(dateActivity).ConfigureAwait(true);
                    return await outerDc.EndDialogAsync();
                }
                else
                {
                    if (!string.IsNullOrEmpty(userResponse.ResponseText)
                       && !string.IsNullOrEmpty(userResponse.CurrentEntityName)
                       && !string.IsNullOrEmpty(userResponse.FlowID))
                    {
                        if ((currentEntity.Name.Equals(userResponse.CurrentEntityName, StringComparison.OrdinalIgnoreCase)
                            && quoteReference.Equals(userResponse.FlowID, StringComparison.OrdinalIgnoreCase)))
                        {
                            return await AssignValueAndAskNextQuestion(outerDc, currentEntity, valueToValidate).ConfigureAwait(true);

                        }
                    }
                    return await outerDc.EndDialogAsync();
                }
            }
            // Handle if user inputs are from Buttons.
            if (currentEntity.EntityType == EntityTypes.Cards)
            {
    
                // Uncomment below code for direct client
                string refinedValueToValidate = string.Empty;

                if (!string.IsNullOrEmpty(valueToValidate))
                {
                    // If the valueToValidate has colon ( to separate name and code)
                    // Take only the first part of the value
                    refinedValueToValidate = valueToValidate.Split(":")[0];
                }

                // Valid Results are the types which are got from Database or whatever
                // Valid results are the type of List<string>

                //  List<string> validatedResults = Get Your Validation Values here

                // For now sample values are provided 
                List<string> validatedResults = new List<string>() { "Hello", "Click Me" };
                if (string.IsNullOrEmpty(refinedValueToValidate) && string.IsNullOrEmpty(userResponse.ResponseText) )
                    
                {
                    await outerDc.Context.SendActivityAsync(string.Format(currentEntity.InitialQuestion, currentEntity.DisplayName)).ConfigureAwait(true);
                }

                else if (validatedResults.Count >= _currentFlowState.NumberOfCardsToShow)
                {
                    //check if there is a value for the entity
                    // If there is a value, then show records exceeded question
                    // else, show inital question

                    if (string.IsNullOrEmpty(valueToValidate)) // CHECK AGAIN
                    {
                        await outerDc.Context.SendActivityAsync(string.Format(currentEntity.InitialQuestion, currentEntity.DisplayName)).ConfigureAwait(true);

                    }
                    else
                    {
                        // No need to show the results in cards. Just show the records exceeded text message to refine
                        await outerDc.Context.SendActivityAsync(string.Format(currentEntity.RecordsExceededLimitQuestion, currentEntity.DisplayName, valueToValidate)).ConfigureAwait(true);
                    }
                    return await outerDc.EndDialogAsync();
                }
                else if (validatedResults.Count == 0)
                {
                    if (string.IsNullOrEmpty(valueToValidate)) // CHECK AGAIN
                    {
                        await outerDc.Context.SendActivityAsync(string.Format(currentEntity.InitialQuestion, currentEntity.DisplayName)).ConfigureAwait(true);
                        return await outerDc.EndDialogAsync();
                    }
                    else
                    {
                        // for assured and client if there are no matched ask if the user wants to add new client

                        // Implies the results are none
                        // So show the cards as well the question
                        await outerDc.Context.SendActivityAsync(string.Format(currentEntity.NoMatchQuestion, userResponse.ResponseText, currentEntity.DisplayName)).ConfigureAwait(true);
                        return await outerDc.EndDialogAsync();
                    }
                }
                else if (validatedResults.Count == 1)
                {
                    return await AssignValueAndAskNextQuestion(outerDc, currentEntity, validatedResults[0]).ConfigureAwait(true);
                }
                else
                {
                    // Implies the results are with in limit
                    // So show the cards as well the question

                    // Here we will store the results which are 6 .
                    // If the results are greater than 6 then add the
                    // we will show first 6 and save the rest in the state
                    //if (validatedResults.Count > 6)
                    //{
                    //    // save the rest of them in state
                    //    int countTobeSplitted = 6;
                    //    List<string> resultsWithShowmore = await SplitAndSaveValidatedResultsAsync(outerDc, validatedResults, currentEntity, quoteReference, countTobeSplitted).ConfigureAwait(true);
                    //    await outerDc.Context.SendActivityAsync(string.Format(currentEntity.MoreThanOneRecordQuestion, currentEntity.DisplayName)).ConfigureAwait(true);
                    //    Activity activity = PromptsHelper.ConstructAdaptiveCardActivity(outerDc, resultsWithShowmore, currentEntity.Name, quoteReference);
                    //    await outerDc.Context.SendActivityAsync(activity).ConfigureAwait(true);
                    //}
                    //else
                    //{
                    //    await outerDc.Context.SendActivityAsync(string.Format(currentEntity.MoreThanOneRecordQuestion, currentEntity.DisplayName)).ConfigureAwait(true);
                    //    Activity activity = PromptsHelper.ConstructAdaptiveCardActivity(outerDc, validatedResults, currentEntity.Name, quoteReference);
                    //    await outerDc.Context.SendActivityAsync(activity).ConfigureAwait(true);
                    //}
                    await outerDc.Context.SendActivityAsync(string.Format(currentEntity.MoreThanOneRecordQuestion, currentEntity.DisplayName)).ConfigureAwait(true);
                    Activity activity = PromptsHelper.ConstructAdaptiveCardActivity(outerDc, validatedResults, currentEntity.Name, quoteReference);
                    await outerDc.Context.SendActivityAsync(activity).ConfigureAwait(true);
                    return await outerDc.EndDialogAsync();
                }
            }

            // Handle if user types in
            if (currentEntity.EntityType == EntityTypes.Text)
            {
                // Perform no validations
                // Just assign and ask next question
                if (!string.IsNullOrEmpty(userResponse.ResponseText))
                {
                    return await AssignValueAndAskNextQuestion(outerDc, currentEntity, valueToValidate).ConfigureAwait(true);
                }
                else
                {
                    // Ask the current question
                    await outerDc.Context.SendActivityAsync(currentEntity.InitialQuestion).ConfigureAwait(true);
                    return await outerDc.EndDialogAsync();
                }
            }
            return await outerDc.EndDialogAsync();
        }

        public async Task<DialogTurnResult> AssignValueAndAskNextQuestion(DialogContext outerDc, BotEntity currentEntity, string valueToPopulate)
        {

            CommonIntentState commonIntentState = await _accessors.CommonIntentStateAccessor.GetAsync(outerDc.Context, () => null).ConfigureAwait(true); ;
            List<BotEntity> newBotEntities = await _accessors.FormEntitiesAccessor.GetAsync(outerDc.Context, () => new List<BotEntity>()).ConfigureAwait(true); ;
            string presentDialogId = string.Empty;
            bool isSummaryToBeShown = false;
            string actualClientNameWithoutCode;
            #region temp

            if (!string.IsNullOrEmpty(valueToPopulate))
            {
                foreach (var entity in commonIntentState.IntentDetails.ExpectedEntities)
                {
                    if (entity.Name.Equals(currentEntity.Name, StringComparison.OrdinalIgnoreCase) && entity.EntityId.Equals(currentEntity.EntityId))
                    {
                        if (entity.EntityType == EntityTypes.Text)
                        {
                            await outerDc.Context.SendActivityAsync(string.Format(StringConstants.UserEnteredMessage, valueToPopulate)).ConfigureAwait(true);
                        }
                        else
                        {
                            await outerDc.Context.SendActivityAsync(string.Format(StringConstants.UserSelectionMessage, valueToPopulate)).ConfigureAwait(true);
                        }
                        entity.Value = valueToPopulate;
                        entity.IsValidated = true;
                        break;
                    }
                }

                await _accessors.CommonIntentStateAccessor.SetAsync(outerDc.Context, commonIntentState).ConfigureAwait(true);
                await _accessors.FormEntitiesAccessor.SetAsync(outerDc.Context, newBotEntities).ConfigureAwait(true);
                await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);
                isSummaryToBeShown = await this.ProcessFlowState(outerDc, commonIntentState.IntentDetails.ExpectedEntities).ConfigureAwait(true);
            }
            else
            {
                commonIntentState.IntentDetails.ExpectedEntities.FirstOrDefault(a => a.Name.Equals(currentEntity.Name)).Value = valueToPopulate;
                commonIntentState.IntentDetails.ExpectedEntities.FirstOrDefault(a => a.Name.Equals(currentEntity.Name)).IsValidated = false;
                await _accessors.CommonIntentStateAccessor.SetAsync(outerDc.Context, commonIntentState).ConfigureAwait(true);
                await _accessors.ConversationState.SaveChangesAsync(outerDc.Context).ConfigureAwait(true);
                await _accessors.FormEntitiesAccessor.SetAsync(outerDc.Context, newBotEntities).ConfigureAwait(true);
                isSummaryToBeShown = await this.ProcessFlowState(outerDc, commonIntentState.IntentDetails.ExpectedEntities).ConfigureAwait(true);

            }
            #endregion  

            #region Old Switch Case

            switch (_currentFlowState.NameOfCurrentFlow)
            {
                case IntentNames.CreateForm:
                    presentDialogId = DialogIds.FormDialogId;
                    break;
                default:
                    break;
            }

            #endregion

            if (!isSummaryToBeShown)
            {
                // Ensure the previous user selection is made to null
                outerDc.Context.Activity.Text = null;
                outerDc.Context.Activity.Value = null;

                // This will re-start the dialog and asks the next question depending on validation
                return await outerDc.ReplaceDialogAsync(presentDialogId).ConfigureAwait(true);
            }
            else
            {
                return await outerDc.BeginDialogAsync(DialogIds.SummaryDialogId).ConfigureAwait(true);
            }
        }

    }
}
