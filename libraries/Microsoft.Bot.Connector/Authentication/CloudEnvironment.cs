// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.using System;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Bot.Connector.Authentication
{
    /// <summary>
    /// Represents an Azure Cloud Environment
    /// Used to help determine which set of Bot and Channel service to service authentication constants to use.
    /// </summary>
    public class CloudEnvironment
    {
        /// <summary>
        /// Public Azure Cloud.
        /// </summary>
        public static readonly CloudEnvironment PublicCloud = new CloudEnvironment(
            string.Empty,
            false,
            AuthenticationConstants.ToChannelFromBotLoginUrl,
            AuthenticationConstants.ToChannelFromBotOAuthScope,
            AuthenticationConstants.ToBotFromChannelTokenIssuer,
            AuthenticationConstants.OAuthUrl,
            AuthenticationConstants.ToBotFromChannelOpenIdMetadataUrl,
            AuthenticationConstants.ToBotFromEmulatorOpenIdMetadataUrl,
            CallerIdConstants.PublicAzureChannel);

        /// <summary>
        /// US Governement Cloud.
        /// </summary>
        public static readonly CloudEnvironment UsGovernment = new CloudEnvironment(
            GovernmentAuthenticationConstants.ChannelService,
            true,
            GovernmentAuthenticationConstants.ToChannelFromBotLoginUrl,
            GovernmentAuthenticationConstants.ToChannelFromBotOAuthScope,
            GovernmentAuthenticationConstants.ToBotFromChannelTokenIssuer,
            GovernmentAuthenticationConstants.OAuthUrlGov,
            GovernmentAuthenticationConstants.ToBotFromChannelOpenIdMetadataUrl,
            GovernmentAuthenticationConstants.ToBotFromEmulatorOpenIdMetadataUrl,
            CallerIdConstants.USGovChannel);

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEnvironment"/> class.
        /// </summary>
        /// <param name="channelService">The channelService identifier.</param>
        /// <param name="isGovernment">Flag indicating whether this is a government cloud.</param>
        /// <param name="toChannelFromBotLoginUrl">The toChannelFromBotLoginUrl.</param>
        /// <param name="toChannelFromBotOAuthScope">The toChannelFromBotOAuthScope.</param>
        /// <param name="toBotFromChannelTokenIssuer">The toBotFromChannelTokenIssuer.</param>
        /// <param name="oAuthUrl">The oAuthUrl.</param>
        /// <param name="toBotFromChannelOpenIdMetadataUrl">The toBotFromChannelOpenIdMetadataUrl.</param>
        /// <param name="toBotFromEmulatorOpenIdMetadataUrl">The toBotFromEmulatorOpenIdMetadataUrl.</param>
        /// <param name="callerId">The callerId.</param>
        public CloudEnvironment(
            string channelService,
            bool isGovernment,
            string toChannelFromBotLoginUrl,
            string toChannelFromBotOAuthScope,
            string toBotFromChannelTokenIssuer,
            string oAuthUrl,
            string toBotFromChannelOpenIdMetadataUrl,
            string toBotFromEmulatorOpenIdMetadataUrl,
            string callerId)
        {
            ChannelService = channelService;
            IsGovernment = isGovernment;
            ToChannelFromBotLoginUrl = toChannelFromBotLoginUrl;
            ToChannelFromBotOAuthScope = toChannelFromBotOAuthScope;
            ToBotFromChannelTokenIssuer = toBotFromChannelTokenIssuer;
            OAuthUrl = oAuthUrl;
            ToBotFromChannelOpenIdMetadataUrl = toBotFromChannelOpenIdMetadataUrl;
            ToBotFromEmulatorOpenIdMetadataUrl = toBotFromEmulatorOpenIdMetadataUrl;
            CallerId = callerId;
        }

        /// <summary>
        /// Gets the channel service for the cloud.
        /// </summary>
        /// <value>ChannelService.</value>
        public string ChannelService { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this cloud environment is a government cloud.
        /// </summary>
        /// <value>IsGovernment.</value>
        public bool IsGovernment { get; private set; }

        /// <summary>
        /// Gets the channel service for the cloud.
        /// </summary>
        /// <value>ToChannelFromBotLoginUrl.</value>
#pragma warning disable CA1056 // Uri properties should not be strings (we can't change this without breaking binary compat)
        public string ToChannelFromBotLoginUrl { get; private set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Gets the OAuth scope to request.
        /// </summary>
        /// <value>ToChannelFromBotOAuthScope.</value>
        public string ToChannelFromBotOAuthScope { get; private set; }

        /// <summary>
        /// Gets the Token issuer.
        /// </summary>
        /// <value>ToBotFromChannelTokenIssuer.</value>
        public string ToBotFromChannelTokenIssuer { get; private set; }

        /// <summary>
        /// Gets the OAuth Url used to get a token from OAuthApiClient.
        /// </summary>
        /// <value>OAuthUrl.</value>
#pragma warning disable CA1056 // Uri properties should not be strings (we can't change this without breaking binary compat)
        public string OAuthUrl { get; private set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Gets the OpenID metadata document for tokens coming from MSA.
        /// </summary>
        /// <value>ToBotFromChannelOpenIdMetadataUrl.</value>
#pragma warning disable CA1056 // Uri properties should not be strings (we can't change this without breaking binary compat)
        public string ToBotFromChannelOpenIdMetadataUrl { get; private set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        ///  Gets the OpenID metadata document for tokens coming from MSA.
        /// </summary>
        /// <value>ToBotFromEmulatorOpenIdMetadataUrl.</value>
#pragma warning disable CA1056 // Uri properties should not be strings (we can't change this without breaking binary compat)
        public string ToBotFromEmulatorOpenIdMetadataUrl { get; private set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        /// <summary>
        /// Gets a caller ID constant for this cloud environment.
        /// </summary>
        /// <value>CallerId.</value>
        public string CallerId { get; private set; }

        /// <summary>
        /// Gets a CloudEnvironment for a particular channel provider.
        /// </summary>
        /// <param name="channelProvider">The channel provider.</param>
        /// <returns>The CloudEnvironment.</returns>
        public static async Task<CloudEnvironment> GetCloudEnvironmentAsync(IChannelProvider channelProvider)
        {
            if (channelProvider == null)
            {
                return CloudEnvironment.PublicCloud;
            }

            var channelService = await channelProvider.GetChannelServiceAsync().ConfigureAwait(false);

            if (channelService == GovernmentAuthenticationConstants.ChannelService)
            {
                return CloudEnvironment.UsGovernment;
            }
            else
            {
                var cloudEnvironmentProvider = channelProvider as ICloudEnvironmentProvider;
                if (cloudEnvironmentProvider != null)
                {
                    return await cloudEnvironmentProvider.GetCloudEnvironmentAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new Exception("Unable to create CloudEnvironment");
                }
            }
        }

        /// <summary>
        /// Gets an empty MicrosoftAppCredential for a particular channel service constant.
        /// </summary>
        /// <param name="channelProvider">The channel provider.</param>
        /// <returns>The empty credential.</returns>
        public static async Task<MicrosoftAppCredentials> GetEmptyCredentialAsync(IChannelProvider channelProvider)
        {
            var cloudEnvironment = await GetCloudEnvironmentAsync(channelProvider).ConfigureAwait(false);
            if (cloudEnvironment == CloudEnvironment.PublicCloud)
            {
                return MicrosoftAppCredentials.Empty;
            }
            else
            {
                return new MicrosoftGovernmentAppCredentials(null, null, null, null, cloudEnvironment.ToChannelFromBotOAuthScope, cloudEnvironment.ToChannelFromBotLoginUrl);
            }
        }

        /// <summary>
        /// Get appropriate TokenValidationParameters for JwtTokenExtractor.
        /// </summary>
        /// <returns>TokenValidationParameters.</returns>
        public TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { ToBotFromChannelTokenIssuer },

                // Audience validation takes place in JwtTokenExtractor
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireSignedTokens = true,
                ValidateIssuerSigningKey = true,
            };
        }
    }
}
