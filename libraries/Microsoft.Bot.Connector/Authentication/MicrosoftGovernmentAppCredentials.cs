// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Microsoft.Bot.Connector.Authentication
{
    /// <summary>
    /// MicrosoftGovernmentAppCredentials auth implementation.
    /// </summary>
    public class MicrosoftGovernmentAppCredentials : MicrosoftAppCredentials
    {
        private readonly string _oauthEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftGovernmentAppCredentials"/> class.
        /// </summary>
        /// <param name="appId">The Microsoft app ID.</param>
        /// <param name="password">The Microsoft app password.</param>
        /// <param name="customHttpClient">Optional <see cref="HttpClient"/> to be used when acquiring tokens.</param>
        public MicrosoftGovernmentAppCredentials(string appId, string password, HttpClient customHttpClient = null)
            : this(appId, password, customHttpClient, null, GovernmentAuthenticationConstants.ToChannelFromBotOAuthScope)
        {
        }

        //public MicrosoftGovernmentAppCredentials(string appId, string password, HttpClient customHttpClient, ILogger logger)
        //    : this(appId, password, customHttpClient, logger, GovernmentAuthenticationConstants.ToChannelFromBotOAuthScope)
        //{
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftGovernmentAppCredentials"/> class.
        /// </summary>
        /// <param name="appId">The Microsoft app ID.</param>
        /// <param name="password">The Microsoft app password.</param>
        /// <param name="customHttpClient">Optional <see cref="HttpClient"/> to be used when acquiring tokens.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to gather telemetry data while acquiring and managing credentials.</param>
        /// <param name="oAuthScope">The scope for the token (defaults to <see cref="GovernmentAuthenticationConstants.ToChannelFromBotOAuthScope"/> if null).</param>
        public MicrosoftGovernmentAppCredentials(string appId, string password, HttpClient customHttpClient, ILogger logger, string oAuthScope = null)
            : this(appId, password, customHttpClient, logger, CloudEnvironment.UsGovernment.ChannelService, oAuthScope)
        {
        }

        //public MicrosoftGovernmentAppCredentials(string appId, string password, HttpClient customHttpClient, ILogger logger, string channelService, string oAuthScope = null)
        //    : base(appId, password, customHttpClient, logger, oAuthScope ?? CloudEnvironment.GetCloudEnvironment(channelService).ToChannelFromBotOAuthScope)
        //{
        //    _oauthEndpoint = CloudEnvironment.GetCloudEnvironment(channelService).ToChannelFromBotLoginUrl;
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrosoftGovernmentAppCredentials"/> class.
        /// </summary>
        /// <param name="appId">The Microsoft app ID.</param>
        /// <param name="password">The Microsoft app password.</param>
        /// <param name="customHttpClient">Optional <see cref="HttpClient"/> to be used when acquiring tokens.</param>
        /// <param name="logger">Optional <see cref="ILogger"/> to gather telemetry data while acquiring and managing credentials.</param>
        /// <param name="oAuthScope">The scope for the token.</param>
        /// <param name="oauthEndpoint">The oauth endpoint to use.</param>
        public MicrosoftGovernmentAppCredentials(string appId, string password, HttpClient customHttpClient, ILogger logger, string oAuthScope, string oauthEndpoint)
            : base(appId, password, customHttpClient, logger, oAuthScope)
        {
            _oauthEndpoint = oauthEndpoint;
        }

        /// <summary>
        /// Gets the OAuth endpoint to use.
        /// </summary>
        /// <value>
        /// The OAuth endpoint to use.
        /// </value>
        public override string OAuthEndpoint
        {
            get { return _oauthEndpoint; }
        }
    }
}
