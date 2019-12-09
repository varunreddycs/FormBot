using System;
using System.Collections.Generic;
using System.Text;

namespace FormBot.Common.Models
{
    /// <summary>
    /// Defines the <see cref="CurrentFlowState" />
    /// </summary>
    public class CurrentFlowState
    {
        /// <summary>
        /// Gets or sets the NameOfCurrentFlow
        ///  Name of the current flow in triggered
        /// </summary>
        public string NameOfCurrentFlow { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the NumberOfCardsToShow
        /// Number of buttons to show if the user has to select from results
        /// </summary>
        public int NumberOfCardsToShow { get; set; }

        /// <summary>
        /// Gets or sets the CurrentEntityId
        /// </summary>
        public int CurrentEntityId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the CurrentEntityName
        /// </summary>
        public string CurrentEntityName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsFlowInitialization
        /// True if the flow is initialized
        /// </summary>
        public bool IsFlowInitialization { get; set; }

        /// <summary>
        /// Gets or sets the TemporaryQuoteReference
        /// </summary>
        public string FlowID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsModifyOptionShown
        /// True if the Modify option is given to the user
        /// </summary>
        public bool IsModifyOptionShown { get; set; }

        /// <summary>
        /// Gets or sets the ModifyOptionAnswer
        /// Value of the modify option
        /// </summary>
        public string ModifyOptionAnswer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ModificationStartEntityId
        /// </summary>
        public string ModificationStartEntityId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether IsSubmitOptionShown
        /// True if the submit option is shown
        /// </summary>
        public bool IsSubmitOptionShown { get; set; }

        /// <summary>
        /// Gets or sets the SubmitOptionAnswer
        /// Value of submit option
        /// </summary>
        public string SubmitOptionAnswer { get; set; } = "The form has been submitted";

        /// <summary>
        /// Gets or sets a value indicating whether IsModificationEntitySelected
        /// True if the modification value is selected
        /// </summary>
        public bool IsModificationEntitySelected { get; set; }

        /// <summary>
        /// Gets or sets the SubmissionFailureResponse
        /// Response if the submission of the user request is failed
        /// </summary>
        public string SubmissionFailureResponse { get; set; }


        public string FlowAbortedResponse { get; set; }
    }
}
