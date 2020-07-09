// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AdaptiveExpressions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Forms
{
    /// <summary>
    /// Triggered because the required constraints for the form have been met and there are no more pending actions or operations.
    /// </summary>
    public class OnSubmitForm : OnEndOfActions
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnSubmitForm";

        [JsonConstructor]
        public OnSubmitForm(List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
        }

        public FormDialog Form { get; set; }

        public override Expression GetExpression()
        {
            lock (Actions)
            {
                if (!Actions.Any())
                {
                    Actions.Add(new ConfirmInput()
                    {
                        Prompt = new ActivityTemplate($"{Form.Id}.getFormReadyConfirmation"),
                        ConfirmChoices = new ChoiceSet()
                        {
                            new Choice("Yes"),
                            new Choice("No")
                        }
                    });
                    Actions.Add(new IfCondition()
                    {
                        Condition = "dialog.lastResult == true",
                        Actions = new List<Dialog>()
                        {
                            new EndDialog()
                        },
                        ElseActions = new List<Dialog>()
                        {
                            // ??? 
                        }
                    });
                }
            }

            // HOW DO WE CHECK THAT A FORM IS READY AS AN EXPRESSION?  We AND it with base expression.
            return base.GetExpression();
        }
    }
}
