using System;
using System.Collections.Generic;
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
                            new ReplaceDialog()
                            {
                                Dialog = "jokeDialog"
                            }
                        }
                    },

                    // queue up joke if we are in the middle of prompting for name or age. You can also do this via 
                    // contains(dialogContext.state, 'askForName') || contains(dialogContext.state, 'askForAge')
                    new OnIntent()
                    {
                        Intent = "joke",
                        Condition = "isDialogActive('askForName', 'askForAge')",
                        Actions = new List<Dialog>()
                        {
                            new EditActions()
                            {
                                ChangeType = ActionChangeType.InsertActions,
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("Sure. I will get you a joke after I get your name..")
                                }
                            },
                            new EditActions()
                            {
                                ChangeType = ActionChangeType.AppendActions,
                                Actions = new List<Dialog>()
                                {
                                    new SendActivity("Here is the joke you asked for.."),
                                    new BeginDialog()
                                    {
                                        Dialog = "jokeDialog"
                                    }
                                }
                            }
                        }
                    },

                    // Only possible if in turn.0. Not possible if this is an interruption
                    new OnIntent()
                    {
                        Intent = "start",
                        Condition = "!hasPendingActions()",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("In start .. this action is not possible as an interruption...")
                        }
                    },

                    // Do not do this if childDialog is already in the stack => child cannot self interrupt itself
                    new OnIntent()
                    {
                        Intent = "profile",
                        Condition = "!isDialogActive('userProfileDialog')",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("In profile .. this is always possible and I handle it immediately."),
                            new BeginDialog()
                            {
                                Dialog = "userProfileDialog"
                            }
                        }
                    }
                }
            };

            var jokeDialog = new AdaptiveDialog("jokeDialog")
            {
                Generator = new TemplateEngineLanguageGenerator(),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("Sure.. Here's the joke .. ha ha ha :) ")
                        }
                    }
                }
            };

            var userProfileDialog = new AdaptiveDialog("userProfileDialog")
            {
                AutoEndDialog = false,
                Generator = new TemplateEngineLanguageGenerator(),
                Recognizer = new RegexRecognizer()
                {
                    Intents = new List<IntentPattern>()
                    {
                        new IntentPattern()
                        {
                            Intent = "why",
                            Pattern = "why"
                        },
                        new IntentPattern()
                        {
                            Intent = "no",
                            Pattern = "no"
                        }
                    }
                },
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("In profile dialog..."),
                            new TextInput()
                            {
                                Id = "askForName",
                                Prompt = new ActivityTemplate("What is your name?"),
                                Property = "user.name"
                            },
                            new SendActivity("I have ${user.name}"),
                            new TextInput()
                            {
                                Id = "askForAge",
                                Prompt = new ActivityTemplate("What is your age?"),
                                Property = "user.age"
                            },
                            new SendActivity("I have ${user.age}")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "why",
                        Condition = "isDialogActive('askForName')",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("I need your name to address you correctly"),
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "why",
                        Condition = "isDialogActive('askForAge')",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("I need your age to provide relevant product recommendations")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "why",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("I need your information to complete the sample..")
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "no",
                        Actions = new List<Dialog>()
                        {
                            new SetProperties()
                            {
                                Assignments = new List<PropertyAssignment>()
                                {
                                    new PropertyAssignment()
                                    {
                                        Property = "user.name",
                                        Value = "Human"
                                    },
                                    new PropertyAssignment()
                                    {
                                        Property = "user.age",
                                        Value = "30"
                                    }
                                }
                            }
                        }
                    },
                    new OnIntent()
                    {
                       Intent = "no",
                       Condition = "isDialogActive('askForName')",
                       Actions = new List<Dialog>()
                       {
                           new SetProperty()
                           {
                               Property = "user.name",
                               Value = "Human"
                           }
                       }
                    },
                    new OnIntent()
                    {
                       Intent = "no",
                       Condition = "isDialogActive('askForAge')",
                       Actions = new List<Dialog>()
                       {
                           new SetProperty()
                           {
                               Property = "user.age",
                               Value = "30"
                           }
                       }
                    }
                }
            };

            AddDialog(rootDialog);
            AddDialog(jokeDialog);
            AddDialog(userProfileDialog);

            InitialDialogId = "root";
        }
    }
}
