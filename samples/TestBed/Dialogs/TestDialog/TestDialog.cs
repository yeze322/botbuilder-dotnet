using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.LanguageGeneration;

namespace Microsoft.BotBuilderSamples
{
    public class TestDialog : ComponentDialog
    {
        private Templates _lgFile;

        public TestDialog()
            : base(nameof(TestDialog))
        {
            var rootDialog = new AdaptiveDialog(nameof(AdaptiveDialog))
            {
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
                            Intent = "help",
                            Pattern = "help"
                        }
                    }
                },
                Triggers = new List<OnCondition>()
                {
                    new OnIntent()
                    {
                        Intent = "start",
                        Actions = new List<Dialog>()
                        {
                            new BeginDialog()
                            {
                                Dialog = "child1"
                            }
                        }
                    },
                    new OnIntent()
                    {
                        Intent = "help",
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("This is help in rootDialog")
                        }
                    },
                    new OnUnknownIntent()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("This is fallback text in rootDialog")
                        }
                    }
                }
            };

            var childDialog = new AdaptiveDialog("child1")
            {
                Generator = new TemplateEngineLanguageGenerator(),
                Triggers = new List<OnCondition>()
                {
                    new OnBeginDialog()
                    {
                        Actions = new List<Dialog>()
                        {
                            new SendActivity("say one of these - one | end | help"),
                            new EndTurn(),
                            new SwitchCondition()
                            {
                                Condition = "turn.activity.text",
                                Cases = new List<Case>()
                                {
                                    new Case()
                                    {
                                        Value = "one",
                                        Actions = new List<Dialog>()
                                        {
                                            new SendActivity("You said 'one' (local)")
                                        }
                                    },
                                    new Case()
                                    {
                                        Value = "end",
                                        Actions = new List<Dialog>()
                                        {
                                            new SendActivity("Ending child1"),
                                            new EndDialog()
                                        }
                                    }
                                },
                                Default = new List<Dialog>()
                                {
                                    new SendActivity("Child cannot handle this.. bubble up to parent"),
                                    new EmitEvent()
                                    {
                                        EventName = AdaptiveEvents.UnknownIntent,
                                        BubbleEvent = true
                                    }
                                }
                            },
                            new RepeatDialog()
                        }
                    }
                }
            };

            AddDialog(rootDialog);
            AddDialog(childDialog);
            InitialDialogId = nameof(AdaptiveDialog);
        }
    }
}
