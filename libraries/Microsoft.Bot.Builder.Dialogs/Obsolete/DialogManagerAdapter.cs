using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("This class is not used anymore", error: true)]
    public class DialogManagerAdapter : BotAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DialogManagerAdapter"/> class.
        /// </summary>
        public DialogManagerAdapter()
        {
        }

        /// <summary>
        /// Gets the list of <see cref="Activity"/>s.
        /// </summary>
        /// <value>
        /// The list of <see cref="Activity"/>s.
        /// </value>
        public List<Activity> Activities { get; private set; } = new List<Activity>();

        /// <summary>
        /// Sends mulitple activities.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="activities">The list of <see cref="Activity"/>s.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> of type <see cref="ResourceResponse"/>.</returns>
        public override Task<ResourceResponse[]> SendActivitiesAsync(ITurnContext turnContext, Activity[] activities, CancellationToken cancellationToken)
        {
            this.Activities.AddRange(activities);
            return Task.FromResult(activities.Select(a => new ResourceResponse(a.Id)).ToArray());
        }

        /// <summary>
        /// Updates an activity.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="activity">The activity.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> of type <see cref="ResourceResponse"/>.</returns>
        public override Task<ResourceResponse> UpdateActivityAsync(ITurnContext turnContext, Activity activity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an activiy based on the conversation reference.
        /// </summary>
        /// <param name="turnContext">The turn context.</param>
        /// <param name="reference">The conversation reference.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public override Task DeleteActivityAsync(ITurnContext turnContext, ConversationReference reference, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
