using Newtonsoft.Json;
using System.Collections.Generic;

namespace FormBot.Common.Models
{
    public class BotEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotEntity"/> class.
        /// </summary>
        public BotEntity()
        {
            this.IsValidated = false;
            this.Value = null;
        }

        /// <summary>
        /// Gets or sets the Name
        /// Name of Bot entity (UniqueName in expectedEntities of BotIntent)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the DisplayName
        /// Name which is shown on cards
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the Value
        /// Value which needs to be captured
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the EntityId
        /// Unique entity Id in BotIntent.ExpectedEntities
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsValidated
        /// Once IsValidated is true it won't ask the same question until the value is changed to false
        /// </summary>
        public bool IsValidated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether PoseInitialQuestion
        /// If PoseInitialQuestion is true, for Cards type when the 'Value' is empty
        /// The bot will first ask the initial question
        /// If the 'Value' already contains details, then bot will ask the questions or displays cards accordingly
        /// </summary>
        public bool PoseInitialQuestion { get; set; }

        /// <summary>
        /// Gets or sets the EntityType
        /// This can take the following values
        /// Date, Cards, Confirmation(Yes/No), string
        /// If anything else is specified, we consider it as string
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Gets or sets the DependentChildEntityIds
        /// This is mainly used in modification/edit cases to reset values
        /// This list will have the IDs of the entities that are dependent on the current entity
        /// </summary>
        public List<int> DependentChildEntityIds { get; set; }

        /// <summary>
        /// Gets or sets the InitialQuestion
        /// This is the question that is asked for the first time when nothing is provided
        /// AND**** the values in the database after filtering are less than the threshold
        /// Gets or sets the InitialQuestion
        /// </summary>
        [JsonProperty("InitalQuestion")]
        public string InitialQuestion { get; set; }

        /// <summary>
        /// Gets or sets the MoreThanOneRecordQuestion
        /// This question will be prompted when you search the provided value and get more than one result/record
        /// </summary>
        public string MoreThanOneRecordQuestion { get; set; }

        /// <summary>
        /// Gets or sets the RecordsExceededLimitQuestion
        /// number of records matching the criteria are exceeding the limit
        /// This question will be prompted to the user when we know that the 
        /// </summary>
        public string RecordsExceededLimitQuestion { get; set; }

        /// <summary>
        /// Gets or sets the NoMatchQuestion
        /// This is the question that will be asked when the user has already provided the details but there is no match with the value provided in the database
        /// </summary>
        public string NoMatchQuestion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsActiveEntity
        /// True if the entity or field needs to be prompted as question in the bot
        /// If false, this entity details will not be asked in the bot
        /// </summary>
        public bool IsActiveEntity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ShowInSummary
        /// Irrespective of whether the entity is active or not,
        /// if this value is true, the details are shown in summary (but not for editing)
        /// </summary>
        public bool ShowInSummary { get; set; }
    }
}
