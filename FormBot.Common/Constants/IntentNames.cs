using System;
using System.Collections.Generic;
using System.Text;

namespace FormBot.Common.Constants
{
    public static class IntentNames
    {
        // IntentNames are defined here as constants as this bot doesnot target answering the user questions
        // If there are many intents to answer then the optimal solution is to put them in a storage and query them.


        /// <summary>
        /// Defines the Cancel
        /// </summary>
        public const string Cancel = "Cancel(Exit)";

        /// <summary>
        /// Defines the None
        /// </summary>
        public const string None = "None";

        /// <summary>
        /// Defines the FAQ
        /// </summary>
        public const string FAQ = "FAQ";

        /// <summary>
        /// Defines the FAQ
        /// </summary>
        public const string CreateForm = "CreateForm";
    }
}
