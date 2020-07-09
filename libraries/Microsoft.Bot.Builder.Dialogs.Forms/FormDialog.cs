using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Dialogs.Forms
{
    public class FormDialog : AdaptiveDialog
    {
        protected override void EnsureDependenciesInstalled()
        {
            base.EnsureDependenciesInstalled();

            var existingTriggers = this.Triggers.ToList();

            // Add default Triggers
            foreach (var property in Schema.Properties())
            {
                // set the form OnMissingProperty(property.Name)
                AssignForm<OnSetProperty>(existingTriggers, property);
                AssignForm<OnClearProperty>(existingTriggers, property);
                AssignForm<OnMissingProperty>(existingTriggers, property);
                AssignForm<OnChooseEntity>(existingTriggers, property);
                AssignForm<OnAddToCollectionProperty>(existingTriggers, property);
                AssignForm<OnRemoveFromCollectionProperty>(existingTriggers, property);

                this.Triggers.Add(new OnAddToCollectionProperty() { Form = this, Property = property.Name, Priority = 500 });
                this.Triggers.Add(new OnSetProperty() { Form = this, Property = property.Name, Priority = 500 });
                this.Triggers.Add(new OnClearProperty() { Form = this, Property = property.Name, Priority = 500 });
                this.Triggers.Add(new OnRemoveFromCollectionProperty() { Form = this, Property = property.Name, Priority = 500 });
                this.Triggers.Add(new OnRemoveFromCollectionProperty() { Form = this, Property = property.Name, Priority = 500 });

                // .... ADD MORE DEFAULTS ....
            }

            this.Triggers.Add(new OnChooseProperty() { Priority = 500 });
            this.Triggers.Add(new OnSubmitForm() { Priority = 500 });
            this.Triggers.Add(new OnShowForm() { Form = this, Priority = 500 });
            this.Triggers.Add(new OnCancelForm() { Form = this, Priority = 500 });
        }

        private void AssignForm<T>(List<OnCondition> existingTriggers, JProperty property)
        {
            // This is a hack that needs to be rethought...
            foreach (var trigger in existingTriggers)
            {
                ObjectPath.HasValue(trigger, property.Name);
                dynamic trig = trigger;
                if (trigger is T && trig.Property == property.Name)
                {
                    trig.Form = this;
                }
            }
        }
    }
}
