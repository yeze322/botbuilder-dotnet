﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Schema
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Conversation and its members.
    /// </summary>
    public partial class ConversationMembers
    {
        /// <summary>Initializes a new instance of the <see cref="ConversationMembers"/> class.</summary>
        public ConversationMembers()
        {
            CustomInit();
        }

        /// <summary>Initializes a new instance of the <see cref="ConversationMembers"/> class.</summary>
        /// <param name="id">Conversation ID.</param>
        /// <param name="members">List of members in this conversation.</param>
        public ConversationMembers(string id = default(string), IList<ChannelAccount> members = default(IList<ChannelAccount>))
        {
            Id = id;
            Members = members;
            CustomInit();
        }

        /// <summary>Gets or sets conversation ID.</summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>Gets or sets list of members in this conversation.</summary>
        [JsonProperty(PropertyName = "members")]
        public IList<ChannelAccount> Members { get; set; }

        /// <summary>An initialization method that performs custom operations like setting defaults.</summary>
        partial void CustomInit();
    }
}
