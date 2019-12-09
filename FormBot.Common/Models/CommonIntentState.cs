using System;
using System.Collections.Generic;
using System.Text;

namespace FormBot.Common.Models
{
    public class CommonIntentState
    {
        /// <summary>
        /// Gets or sets the IntentName
        /// </summary>
        public string IntentName { get; set; }

        /// <summary>
        /// Gets or sets the IntentDetails
        /// </summary>
        public BotIntent IntentDetails { get; set; }
    }
}
