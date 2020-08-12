// Licensed under the MIT License.
// Copyright (c) Microsoft Corporation. All rights reserved.

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// These are the keys which are persisted.
    /// </summary>
    public class PersistedStateKeys : IEnumerable<string>
    {
        /// <summary>
        /// Gets or sets the user state.
        /// </summary>
        /// <value>A string.</value>
        public string UserState { get; set; }

        /// <summary>
        /// Gets or sets the conversations state.
        /// </summary>
        /// <value>A string.</value>
        public string ConversationState { get; set; }

        /// <summary>
        /// Gets an enumerator of the <see cref="PersistedStateKeys"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> of type string.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            yield return UserState;
            yield return ConversationState;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return UserState;
            yield return ConversationState;
        }
    }
}
