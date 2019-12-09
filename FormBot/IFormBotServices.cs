using Microsoft.Bot.Builder.AI.LuisV3;
using Microsoft.Bot.Builder.AI.QnA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBot
{
    public interface IFormBotServices
    {
        public Dictionary<string, LuisRecognizer> LuisServices { get; }
        public Dictionary<string, QnAMaker> QnaServices { get; }
    }
}
