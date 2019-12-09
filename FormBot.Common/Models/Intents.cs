using System.Collections.Generic;

namespace FormBot.Common.Models
{
    public class Intents
    {
        /// <summary>
        /// Gets or sets the BotIntents
        /// </summary>
        public List<BotIntent> BotIntents { get; set; }

        /// <summary>
        /// Gets or sets the NumberOfCardsToShow
        /// </summary>
        public int NumberOfCardsToShow { get; set; }

        /// <summary>
        /// Gets or sets the SubmissionFailureResponse
        /// </summary>
        public string SubmissionFailureResponse { get; set; }

        /// <summary>
        /// Gets or sets the FlowAbortedMessage
        /// </summary>
        public string FlowAbortedMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Intents"/> class.
        /// </summary>
        public Intents()
        {
            this.BotIntents = new List<BotIntent>();
        }
    }

}
