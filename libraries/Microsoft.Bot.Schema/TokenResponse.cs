﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Schema
{
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Response schema sent back from Bot Framework Token Service, in response to a request to get or exchange a token for a user.
    /// </summary>
    public partial class TokenResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        public TokenResponse()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenResponse"/> class.
        /// </summary>
        public TokenResponse(string channelId = default(string), string connectionName = default(string), string token = default(string), string expiration = default(string))
        {
            ChannelId = channelId;
            ConnectionName = connectionName;
            Token = token;
            Expiration = expiration;
            CustomInit();
        }

        /// <summary>
        /// The channel ID.
        /// </summary>
        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        /// <summary>
        /// The connection name.
        /// </summary>
        [JsonProperty(PropertyName = "connectionName")]
        public string ConnectionName { get; set; }

        /// <summary>
        /// The token.
        /// </summary>
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        /// <summary>
        /// The expiration.
        /// </summary>
        [JsonProperty(PropertyName = "expiration")]
        public string Expiration { get; set; }
        
        /// <summary>
        /// Extra propreties.
        /// </summary>
        [JsonExtensionData(ReadData = true, WriteData = true)]
        public JObject Properties { get; set; } = new JObject();

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults.
        /// </summary>
        partial void CustomInit();
    }
}
