﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Core.Extensions.Tests;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Ai.Tests
{
    class LanguageState { 
        public string Language { get; set; }
    }

    [TestClass]
    public class TranslatorMiddlewareTests
    {
        public string translatorKey = TestUtilities.GetKey("TRANSLATORKEY");

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task TranslatorMiddleware_DetectAndTranslateToEnglish()
        {
            
            TestAdapter adapter = new TestAdapter()
            .Use(new BatchOutputMiddleware()) 
            .Use(new TranslationMiddleware(new string[] { "en-us" }, translatorKey));

            await new TestFlow(adapter, (context) =>
            {
                if (!context.Responded)
                {
                    context.Batch().Reply(context.Request.AsMessageActivity().Text);
                }
                return Task.CompletedTask;
            })
            .Send("salut")
                .AssertReply("Hello")
            .Send("salut 10-20")
                .AssertReply("Hi 10-20")
                .StartTest();
        }

        [TestMethod]
        [TestCategory("AI")]
        [TestCategory("Translator")]
        public async Task TranslatorMiddleware_TranslateFrenchToEnglish()
        {

            TestAdapter adapter = new TestAdapter()
                .Use(new BatchOutputMiddleware())
                .Use(new UserState<LanguageState>(new MemoryStorage()))
                .Use(new TranslationMiddleware(new string[] { "en-us" }, translatorKey, new Dictionary<string, List<string>>(), GetActiveLanguage, SetActiveLanguage));

            await new TestFlow(adapter, (context) =>
            {
                if (!context.Responded)
                {
                    context.Batch().Reply(context.Request.AsMessageActivity().Text);  
                }
                return Task.CompletedTask;
            })
            .Send("set my language to fr")
                .AssertReply("Changing your language to fr")
            .Send("salut")
                .AssertReply("Hello")
                .StartTest();
        }

        private void SetLanguage(IBotContext context, string language) =>context.GetUserState<LanguageState>().Language = language ; 
       
        protected async Task<bool> SetActiveLanguage(IBotContext context)
        {
            bool changeLang = false;//logic implemented by developper to make a signal for language changing 
            //use a specific message from user to change language 
            var messageActivity = context.Request.AsMessageActivity();
            if (messageActivity.Text.ToLower().StartsWith("set my language to"))
            {
                changeLang = true;
            }
            if (changeLang)
            {
                var newLang = messageActivity.Text.ToLower().Replace("set my language to", "").Trim();
                if (!string.IsNullOrWhiteSpace(newLang))
                {
                    SetLanguage(context, newLang);
                    await context.SendActivity($@"Changing your language to {newLang}");
                }
                else
                {
                    await context.SendActivity($@"{newLang} is not a supported language.");
                }
                //intercepts message
                return true;
            }

            return false;
        }
        protected string GetActiveLanguage(IBotContext context)
        {
            if (context.Request.Type == ActivityTypes.Message
                && !string.IsNullOrEmpty(context.GetUserState<LanguageState>().Language))
            {
                return context.GetUserState<LanguageState>().Language;
            }

            return "en";
        }   
    }
}
