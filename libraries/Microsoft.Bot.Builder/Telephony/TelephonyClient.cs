using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Telephony
{
    /// <summary>
    /// TelephonyClient.
    /// </summary>
    public static class TelephonyClient
    {
        /// <summary>
        /// Gets the details for the given meeting participant. This only works in teams meeting scoped conversations. 
        /// </summary>
        /// <param name="turnContext">Turn context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>InvalidOperationException will be thrown if meetingId, participantId or tenantId have not been
        /// provided, and also cannot be retrieved from turnContext.Activity.</remarks>
        /// <returns>Team participant channel account.</returns>
        public static async Task<RecordingResult> StartRecordingAsync(
            this ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var convId = turnContext.Activity.Conversation.Id;
            var activityId = turnContext.Activity.Id;

            var startRecordingActivity = new Activity(ActivityTypes.Invoke) 
            { 
                Name = "start.recording"
            };

            var client = turnContext.TurnState.Get<IConnectorClient>() as ConnectorClient;
            var conversationObject = client.Conversations as Conversations;

            return await conversationObject.ReplyToActivityAndGetResponseAsync<RecordingResult>(
                convId, activityId, startRecordingActivity, cancellationToken).ConfigureAwait(false);
        }
    }
}
