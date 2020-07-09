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
    /// Triggered to remove a value from a property which is a collection.
    /// </summary>
    public class OnRemoveFromCollectionProperty : OnAssignEntity
    {
        [JsonProperty("$kind")]
        public new const string Kind = "Microsoft.OnRemoveFromCollectionProperty";

        [JsonConstructor]
        public OnRemoveFromCollectionProperty(string property = null, string entity = null, string operation = null, List<Dialog> actions = null, string condition = null, [CallerFilePath] string callerPath = "", [CallerLineNumber] int callerLine = 0)
            : base(property: property, entity: entity, operation: "Remove", actions: actions, condition: condition, callerPath: callerPath, callerLine: callerLine)
        {
            Property = property;
        }

        [JsonIgnore]
        public FormDialog Form { get; set; }

        public override Expression GetExpression()
        {
            lock (Actions)
            {
                if (!Actions.Any())
                {
                    //Actions.Add(
                    //    new Ask()
                    //    {
                    //        Activity = new ActivityTemplate($"{Form.Id}.{Property}.getPromptText"),
                    //        ExpectedProperties = new List<string>() { Property }
                    //    }
                    //);
                }
            }

            return Expression.AndExpression(
                base.GetExpression(),
                Expression.Parse($"${Property} == @{Entity}"));
        }
    }
}
