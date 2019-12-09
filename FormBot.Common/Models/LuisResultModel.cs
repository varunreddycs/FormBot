using System.Collections.Generic;

namespace FormBot.Common.Models
{
    public class LuisResultModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LuisResultModel"/> class.
        /// </summary>
        public LuisResultModel()
        {
            ExtractedEntities = new List<BotEntity>();
        }

        /// <summary>
        /// Gets or sets the TopScoringIntentName
        /// </summary>
        public string TopScoringIntentName { get; set; }

        /// <summary>
        /// Gets or sets the ExtractedEntities
        /// </summary>
        public List<BotEntity> ExtractedEntities { get; set; }
    }
}
