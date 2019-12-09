// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using FormBot.Common.Models;
using FormBot.Common.Constants;
using FormBot.Helpers;
using FormBot.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Builder.Dialogs;
using FormBot.Accessors;

namespace FormBot
{
    public class FormBot<T> : ActivityHandler where T: Dialog
    {

        protected readonly Dialog Dialog;
        private readonly ServicesHelper _servicesHelper;
        private readonly IFormBotServices _services;
        private readonly FormBotAccessors _accessors;


        public FormBot(IFormBotServices services, FormBotAccessors accessors, T dialog, ILoggerFactory loggerFactory)
        {
            this._services = services ?? throw new System.ArgumentNullException(nameof(services));
            this._servicesHelper = new ServicesHelper(services);
            this.Dialog = dialog;

            if (loggerFactory == null)
            {
                throw new System.ArgumentNullException(nameof(loggerFactory));
            }
            this._accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
        }

        /// <summary>
        /// The OnTurnAsync
        /// </summary>
        /// <param name="turnContext">The turnContext<see cref="ITurnContext"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>


        /// <summary>
        /// The OnMessageActivityAsync
        /// </summary>
        /// <param name="turnContext">The turnContext<see cref="ITurnContext{IMessageActivity}"/></param>
        /// <param name="cancellationToken">The cancellationToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, _accessors.ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken).ConfigureAwait(true);
        }
    }
}
