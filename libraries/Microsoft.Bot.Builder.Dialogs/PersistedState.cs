// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    public class PersistedState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedState"/> class.
        /// </summary>
        public PersistedState()
        {
            UserState = new Dictionary<string, object>();
            ConversationState = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedState"/> class.
        /// </summary>
        /// <param name="keys">The keys for the state.</param>
        /// <param name="data">The dictionary of data.</param>
        public PersistedState(PersistedStateKeys keys, IDictionary<string, object> data)
        {
            UserState = data.ContainsKey(keys.UserState) ? (IDictionary<string, object>)data[keys.UserState] : new ConcurrentDictionary<string, object>();
            ConversationState = data.ContainsKey(keys.ConversationState) ? (IDictionary<string, object>)data[keys.ConversationState] : new ConcurrentDictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the user state.
        /// </summary>
        /// <value>
        /// An <see cref="IDictionary{TKey, TValue}"/>.
        /// </value>
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
        public IDictionary<string, object> UserState { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets the conversation state. 
        /// </summary>
        /// <value>A <see cref="IDictionary{TKey, TValue}"/>.</value>
#pragma warning disable CA2227 // Collection properties should be read only (we can't change this without breaking binary compat)
        public IDictionary<string, object> ConversationState { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
