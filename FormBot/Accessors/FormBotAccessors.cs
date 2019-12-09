using FormBot.Common.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBot.Accessors
{
    public class FormBotAccessors
    {
        public FormBotAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        #region

        // Define All the names of the Accessors Here.. 
        // Accessors are passed as the services to all the dialogs during the runtime initialization.
        // To initialize accessors the accessors you need to uniquely name them which are defined here.
        // Usually you use the Accessors to manipulate the data .
        public static string DialogStateAccessorName { get; } = "DialogStateAccessorName";

        public static string FormEntitiesAccessorName { get; } = "FormEntities";

        public static string CommonIntentStateName { get; } = "CommonIntentState";

        public static string CurrentFlowAccessorName { get; } = "CurrentFlow";

        #endregion

        #region Accessors

        // Accessors are usually initialized when the dialog is instantiated using the constructor(obviously) and used along the dialog.
        // There can be same type of Accessors Initialized by different names . 
        // To keep the confusion to minimal we keep the accessors created by their uniquenames . Just like key value pairs.
        public IStatePropertyAccessor<List<BotEntity>> FormEntitiesAccessor { get; set; }

        public IStatePropertyAccessor<CurrentFlowState> CurrentFlowStateAccessor { get; set; }

        public IStatePropertyAccessor<CommonIntentState> CommonIntentStateAccessor { get; set; }

        public ConversationState ConversationState { get; }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; } 
        #endregion

    }
}
