using System.Collections.Generic;

namespace FormBot.Common.Models
{
    public class BotIntent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotIntent"/> class.
        /// </summary>
        public BotIntent()
        {
            this.ExpectedEntities = new List<BotEntity>();
        }

        /// <summary>
        /// Gets or sets the Name
        /// Name of the intent . Unique in a luis application
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the AuthorisedGroups
        /// Azure Ad Groups that are allowred to access this intent(flow)
        /// </summary>
        public List<string> AuthorisedGroups { get; set; }

        /// <summary>
        /// Gets or sets the ExpectedEntities
        /// Expected entities the bot should ask user
        /// </summary>
        public List<BotEntity> ExpectedEntities { get; set; }

        /// <summary>
        /// Gets or sets the IntentResponse
        /// Response for the bot Intent
        /// </summary>
        public string IntentResponse { get; set; }

        /// <summary>
        /// Gets or sets the ModificationQuestion
        /// In case if user wishes to change the response given to the entity.
        /// </summary>
        public string ModificationQuestion { get; set; }

        /// <summary>
        /// Gets or sets the SubmissionQuestion
        /// Question raised when the submit the answers for this botintent
        /// </summary>
        public string SubmissionQuestion { get; set; }

        /// <summary>
        /// Gets or sets the SubmissionSucessfulResponse
        /// Send Response after complettion of Flow of BotIntent
        /// </summary>
        public string SubmissionSucessfulResponse { get; set; }
    }
}
