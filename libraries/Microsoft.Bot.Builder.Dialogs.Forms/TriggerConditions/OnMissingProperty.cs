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
    /// Triggered because a property is missing a value.
    /// </summary>
    /// <remarks>
    /// This fires because the one of the following:
    /// * property is required and missing a value  (Example: Age is required and not set so we ask "What is your age?"
    /// * Someone said they wanted to change the property and didn't provide a value. (Example: "I want to change my age" has no value, so we ask "What is your age?").
    /// </remarks>
    public class OnMissingProperty : OnEndOfActions
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnMissingProperty";

        [JsonConstructor]
        public OnMissingProperty(string property = null, string entity = null, string operation = null, List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
            Property = property;
        }

        [JsonIgnore]
        public FormDialog Form { get; set; }

        /// <summary>
        /// Gets or sets the property to be assigned for filtering events.
        /// </summary>
        /// <value>Property name.</value>
        [JsonProperty("property")]
        public string Property { get; set; }

        public override string GetIdentity() => $"{this.GetType().Name}({this.Property})";

        public override Expression GetExpression()
        {
            if (Priority == null)
            {
                Priority = $"indexOf(dialog.requiredProperties, '{Property}')";
            }

            lock (Actions)
            {
                if (!Actions.Any())
                {
                    // Default actions is to ask PROPERTY.getPromptText
                    Actions.Add(
                        new Ask()
                        {
                            Activity = new ActivityTemplate($"{Form.Id}.{Property}.getPromptText"),
                            ExpectedProperties = new List<string>() { Property }
                        });
                }
            }

            return Expression.AndExpression(
                base.GetExpression(),
                Expression.Parse($"!${Property} || $PropertyToChange == '{Property}'"));
        }
    }
}
