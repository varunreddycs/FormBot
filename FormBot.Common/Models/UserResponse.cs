namespace FormBot.Common.Models
{
    public class UserResponse
    {

        public string ResponseText { get; set; } = string.Empty;

        public string CurrentEntityName { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;

        public string Phase { get; set; } = string.Empty;

        public string FlowID { get; set; } = string.Empty;
    }
}
