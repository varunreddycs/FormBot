// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.6.2

using FormBot.Accessors;
using FormBot.Dialogs;
using FormBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.LuisV3;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using FormBot.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace FormBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var botConfig = new BotConfiguration();
            Configuration.Bind("botconfig", botConfig);
            var secretKey = Configuration.GetSection("botFileSecret")?.Value;
            var luisServices = Configuration.GetSection("botconfig")?.Value;
            // These are all the Bot configs defined for external services.Luis,QnaMaker
            //var botConfig = BotConfiguration.Load(botFilePath ?? @".\Hyperion.Bot.bot", secretKey);


            services.AddSingleton<IFormBotServices, FormBotServices>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddSingleton<RootDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, FormBot<RootDialog>>();






            // Create and register state accessors.
            // Accessors created here are passed into the IBot-derived class on every turn.
            services.AddSingleton<FormBotAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if (options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the state accessors");
                }
                var storage = new MemoryStorage();

                var conversationState = new ConversationState(storage);
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                // Create the custom state accessor.
                // State accessors enable other components to read and write individual properties of state.
                var accessors = new FormBotAccessors(conversationState)
                {
                   DialogStateAccessor = conversationState.CreateProperty<DialogState>(FormBotAccessors.DialogStateAccessorName),                   
                   FormEntitiesAccessor = conversationState.CreateProperty<List<BotEntity>>(FormBotAccessors.FormEntitiesAccessorName),                   
                   CommonIntentStateAccessor = conversationState.CreateProperty<CommonIntentState>(FormBotAccessors.CommonIntentStateName),                   
                   CurrentFlowStateAccessor = conversationState.CreateProperty<CurrentFlowState>(FormBotAccessors.CurrentFlowAccessorName),                   
                };

                return accessors;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            app.UseMvc();
        }



    }
}
