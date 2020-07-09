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
    /// Triggered to when request to clear the form.
    /// </summary>
    public class OnClearForm : OnAssignEntity
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnClearForm";

        [JsonConstructor]
        public OnClearForm(string property = null, string entity = null, List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(property: property, entity: entity, operation: "Clear", actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
        }

        [JsonIgnore]
        public FormDialog Form { get; set; }

        public override Expression GetExpression()
        {
            lock (Actions)
            {
                if (!Actions.Any())
                {
                    Actions.Add(new SendActivity() { Activity = new ActivityTemplate($"{Form.Id}.{Property}.getClearPropertyText") });
                    Actions.Add(new DeleteProperty(property: $"dialog.{Property}"));
                }
            }

            return base.GetExpression();
        }
    }
}
