using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;

namespace Microsoft.BotBuilderSamples
{ 
    public class TestRootDialog : ComponentDialog
    {
        public TestRootDialog()
            : base(nameof(TestRootDialog))
        {
            var testRootDialog = new AdaptiveDialog("mainDialog")
            {
                Generator = new TemplateEngineLanguageGenerator(),
                Recognizer = new RegexRecognizer()
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern()
                        {
                            Intent = "profile",
                            Pattern = "profile"
                        },
                        new IntentPattern()
                        {
                            Intent = "joke",
                            Pattern = "joke"
                        },
                        new IntentPattern()
                        {
                            Intent = "start",
                            Pattern = "start"
                        }
                    }
                },
                Triggers = new List<OnCondition>()
                {
                    // Ask for joke will be queued up if in the middle of user age prompt
                    new OnIntent()
                    {
                        Intent = "joke",
                        Condition = "contains(dialogContext.stack, 'askForAge')",
                        Actions = new List<Dialog>()
                        {
                            new EditActions()
                            {
                                ChangeType = ActionChangeType.AppendActions,
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("Sure. I will need us to finish our conversation before we can get to that. Don't worry I will remember to get back to it...")
                                }
                            },
                            new EditActions()
                            {
                                ChangeType = ActionChangeType.AppendActions,
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("Here is the joke you asked for -- 'Joke' :)")
                                }
                            }
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "joke",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Sure. 'Joke' :)")
                        }
                    },

                    // Only possible if in turn.0. Not possible if this is an interruption
                    new OnIntent()
                    {
                        Intent = "start",
                        Condition = "count(dialogContext.stack) == 2",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("In start .. this is not possible as an interruption...")
                        }
                    },

                    // Do not do this if childDialog is already in the stack => child cannot self interrupt itself
                    new OnIntent()
                    {
                        Intent = "profile",
                        Condition = "!contains(dialogContext.stack, 'childDialog')", 
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("In profile .. this is always possible and I handle it immediately."),
                            new BeginDialog()
                            {
                                Dialog = "TestDialog"
                            }
                        }
                    }
                }
            };

            AddDialog(new TestDialog());
            AddDialog(testRootDialog);

            InitialDialogId = "mainDialog";
        }
    }
}
