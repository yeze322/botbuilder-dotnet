using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Azure.Queues
{
    /// <summary>
    /// Defines the interface for a queue storage layer.
    /// </summary>
    public interface IQueuesStorage
    {
        /// <summary>
        /// Queues an Activity. The visibility timeout specifies how long the message should be invisible.
        /// </summary>
        /// <param name="activity"><see cref="Activity"/> retrieved from a call to 
        /// activity.GetConversationReference().GetContinuationActivity(). Used to restart the conversation
        /// using BotAdapter.ContinueConversationAsync.</param>
        /// <param name="visibilityTimeout">Default value of 0. Cannot be larger than 7 days.</param>
        /// <param name="timeToLive">Specifies the time-to-live interval for the message.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns><see cref="SendReceipt"/> as a Json string, from the QueueClient SendMessageAsync operation.</returns>
        Task<string> QueueActivityAsync(Activity activity, TimeSpan? visibilityTimeout = null, TimeSpan? timeToLive = null,  CancellationToken cancellationToken = default);
    }
}
