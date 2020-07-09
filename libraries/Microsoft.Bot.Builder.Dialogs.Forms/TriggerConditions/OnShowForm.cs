// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AdaptiveExpressions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Templates;
using Newtonsoft.Json;

namespace Microsoft.Bot.Builder.Dialogs.Forms
{
    /// <summary>
    /// Triggered to show the user the current value of the form.
    /// </summary>
    public class OnShowForm : OnAssignEntity
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnShowForm";

        [JsonConstructor]
        public OnShowForm(string property = null, string entity = null, List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(property: property, entity: entity, operation: "show", actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
        }

        public FormDialog Form { get; set; }

        public override Expression GetExpression()
        {
            lock (Actions)
            {
                if (!Actions.Any())
                {
                    Actions.Add(new SendActivity()
                    {
                        Activity = new ActivityTemplate($"{Form.Id}.getShowFormActivity"),
                    });
                }
            }

            return base.GetExpression();
        }
    }
}
