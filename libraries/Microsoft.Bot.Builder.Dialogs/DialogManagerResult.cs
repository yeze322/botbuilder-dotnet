// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public class DialogManagerResult
    {
        /// <summary>
        /// Gets or sets the turn result. 
        /// </summary>
        /// <value>A <see cref="DialogTurnResult"/>.</value>
        public DialogTurnResult TurnResult { get; set; }

        /// <summary>
        /// Gets or sets a list of activities.
        /// </summary>
        /// <value>A list <see cref="Activity"/>.</value>
#pragma warning disable CA1819 // Properties should not return arrays (we can't change this without breaking binary compat)
        public Activity[] Activities { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the PersistedState.
        /// </summary>
        /// <value>A <see cref="PersistedState"/>.</value>
        public PersistedState NewState { get; set; }
    }
}
