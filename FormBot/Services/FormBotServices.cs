using Microsoft.Bot.Builder.AI.LuisV3;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBot.Services
{
    public class FormBotServices : IFormBotServices
    {
        public FormBotServices(IConfiguration configuration)
        {
            //TelemetryClient = client ?? throw new ArgumentNullException(nameof(client));
            LuisServices = new Dictionary<string, LuisRecognizer>();
            QnaServices = new Dictionary<string, QnAMaker>();

            if (configuration==null)
            {
                throw new NullReferenceException("configuration is null");
            }

            // now You can add n number of services if you need.
            // You could also chain the luis services
            // Configure your Luis Application creds here
            LuisServices.Add("LuisServiceOne", new LuisRecognizer( new LuisApplication(
                $"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/da64882a-8d60-4c89-aaf9-47885bb41ea3?verbose=false&timezoneOffset=0&subscription-key=a24ea7170ee7480282919f5839b964ce&q=")
                ));
            // now You can add n number of services if you need.
            // You could also chain the Qna services

            //QnaServices.Add("QnaServiceOne", new QnAMaker(new QnAMakerEndpoint
            //{
            //    KnowledgeBaseId = configuration.GetSection("QnaAppOne:QnAKnowledgebaseId").Value,
            //    EndpointKey = configuration.GetSection("QnaAppOne:QnAEndpointKey").Value,
            //    Host = configuration.GetSection("QnaAppOne:QnAEndpointHostName").Value
            //})
            //    );

            //}));
        }

        public Dictionary<string, LuisRecognizer> LuisServices { get; } = new Dictionary<string, LuisRecognizer>();

        public Dictionary<string, QnAMaker> QnaServices { get; } = new Dictionary<string, QnAMaker>();

    }
}
