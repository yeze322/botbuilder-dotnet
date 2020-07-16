using System;
using System.Collections.Generic;
using AdaptiveExpressions.BuiltinFunctions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;

namespace Microsoft.BotBuilderSamples
{
    public class RootDialog1 : ComponentDialog
    {
        public RootDialog1()
            : base(nameof(RootDialog1))
        {
            var outderDialog = new AdaptiveDialog("outer")
            {
                AutoEndDialog = false,
                Generator = new TemplateEngineLanguageGenerator(),
                Recognizer = new RegexRecognizer()
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern()
                        {
                            Intent = "start",
                            Pattern = "start"
                        },
                        new IntentPattern()
                        {
                            Intent = "where",
                            Pattern = "where"
                        }
                    }
                },
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Say 'start' to get started")
                        }
                    },

                    // joke is always available if it is an interruption.
                    new OnIntent()
                    {
                        Intent = "start",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Starting child dialog"),
                            new BeginDialog()
                            {
                                Dialog = "root"
                            },
                            new SendActivity("child dialog has ended and returned back")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "where",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("outer dialog..")
                        }
                    }
                }
            };

            var rootDialog = new AdaptiveDialog("root")
            {
                AutoEndDialog = false,
                Generator = new TemplateEngineLanguageGenerator(),
                Recognizer = new RegexRecognizer()
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern()
                        {
                            Intent = "replace",
                            Pattern = "replace"
                        },
                        new IntentPattern()
                        {
                            Intent = "where",
                            Pattern = "where"
                        }
                    }
                },
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Say 'replace' to get started")
                        }
                    },

                    // joke is always available if it is an interruption.
                    new OnIntent()
                    {
                        Intent = "replace",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Replacing this dialog with a child"),
                            new ReplaceDialog()
                            {
                                Dialog = "newDialog"
                            },
                            new SendActivity("You should not see these actions since this dialog has been replaced!")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "where",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("root dialog..")
                        }
                    }
                }
            };

            var newDialog = new AdaptiveDialog("newDialog")
            {
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("This dialog (newDialog) will end after this message")
                        }
                    }
                }
            };

            AddDialog(outderDialog);
            AddDialog(rootDialog);
            AddDialog(newDialog);

            InitialDialogId = "outer";
        }
    }
}
