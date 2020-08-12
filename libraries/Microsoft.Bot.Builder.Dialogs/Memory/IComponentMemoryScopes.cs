// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs.Memory.Scopes;

namespace Microsoft.Bot.Builder.Dialogs.Memory
{
    /// <summary>
    /// 
    /// </summary>
    public interface IComponentMemoryScopes
    {
        /// <summary>
        /// Gets an enumerable of the memory scopes.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> of type <see cref="MemoryScope"/>.</returns>
        IEnumerable<MemoryScope> GetMemoryScopes();
    }
}
