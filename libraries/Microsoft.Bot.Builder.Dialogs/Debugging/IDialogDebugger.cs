// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Bot.Builder.Dialogs.Debugging
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDialogDebugger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context">The dialog context.</param>
        /// <param name="item"></param>
        /// <param name="more"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task StepAsync(DialogContext context, object item, string more, CancellationToken cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IDebugger
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="item"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task OutputAsync(string text, object item, object value, CancellationToken cancellationToken);
    }
}
