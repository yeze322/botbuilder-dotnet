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
    /// Triggered to assign an entity value to a property.
    /// </summary>
    public class OnSetProperty : OnAssignEntity
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnSetProperty";

        [JsonConstructor]
        public OnSetProperty(string property = null, string entity = null, string operation = null, List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(property: property, entity: entity, operation: "set", actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
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
                    Actions = new List<Dialog>
                    {
                        new IfCondition()
                        {
                            Condition = $"isEntityValid('{Property}', @{Entity})",
                            Actions = new List<Dialog>
                            {
                                new SendActivity() { Activity = new ActivityTemplate($"${{{Form.Id}.{Property}.getSetPropertyText}}") },
                                new SetProperty() { Property = $"dialog.{Property}", Value = $"=@{Entity}" }
                            },
                            ElseActions = new List<Dialog>
                            {
                                new SendActivity() { Activity = new ActivityTemplate($"${{{Form.Id}.{Property}.getSetPropertyInvalidText(@{Entity})}}") },
                            }
                        }
                    };
                }
            }

            return base.GetExpression();
        }
    }
}
