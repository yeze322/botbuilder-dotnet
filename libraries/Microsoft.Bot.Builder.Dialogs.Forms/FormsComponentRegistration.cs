#pragma warning disable SA1300 // Element should begin with upper-case letter
using System;
using System.Collections.Generic;
using System.Text;
using AdaptiveExpressions;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace Microsoft.Bot.Builder.Dialogs.Forms
{
    public class FormsComponentRegistration : ComponentRegistration, IComponentDeclarativeTypes
    {
        public FormsComponentRegistration()
        {
            Expression.Functions.Add(nameof(missingProperty), (args) => missingProperty(args[0], args[1]));
            Expression.Functions.Add(nameof(isEntityValid), (args) => isEntityValid(args[0], args[1]));
        }

        /// <summary>
        /// Gets declarative type registrations.
        /// </summary>
        /// <param name="resourceExplorer">resourceExplorer to use for resolving references.</param>
        /// <returns>enumeration of DeclarativeTypes.</returns>
        public IEnumerable<DeclarativeType> GetDeclarativeTypes(ResourceExplorer resourceExplorer)
        {
            yield return new DeclarativeType<OnCancelForm>(OnCancelForm.Kind);
            yield return new DeclarativeType<OnClearForm>(OnClearForm.Kind);
            yield return new DeclarativeType<OnSubmitForm>(OnSubmitForm.Kind);
            yield return new DeclarativeType<OnShowForm>(OnShowForm.Kind);

            yield return new DeclarativeType<OnAddToCollectionProperty>(OnAddToCollectionProperty.Kind);
            yield return new DeclarativeType<OnSetProperty>(OnCancelForm.Kind);
            yield return new DeclarativeType<OnClearProperty>(OnCancelForm.Kind);
            yield return new DeclarativeType<OnShowProperty>(OnCancelForm.Kind);

            yield return new DeclarativeType<OnAddToCollectionProperty>(OnCancelForm.Kind);
            yield return new DeclarativeType<OnRemoveFromCollectionProperty>(OnCancelForm.Kind);
        }

        /// <summary>
        /// Gets JsonConverters for DeclarativeTypes.
        /// </summary>
        /// <param name="resourceExplorer">resourceExplorer to use for resolving references.</param>
        /// <param name="sourceContext">SourceContext to build debugger source map.</param>
        /// <returns>enumeration of json converters.</returns>
        public IEnumerable<JsonConverter> GetConverters(ResourceExplorer resourceExplorer, SourceContext sourceContext)
        {
            yield break;
        }

        private static bool isEntityValid(dynamic schema, dynamic value)
        {
            // look at schema and validate value
            switch (schema.type)
            {
                case "number":
                    return schema.minimum >= value && schema.maximum <= value;

                    // TODO other schema validate.
            }

            return false;
        }

        private static bool missingProperty(object value, string name)
        {
            return true; // !$name || $PropertyToChange == 'name'
        }
    }
}
