// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Bot.Builder.BotFramework
{
    /// <summary>
    /// Channel provider which uses <see cref="Microsoft.Extensions.Configuration.IConfiguration"/> to lookup the channel service property.
    /// </summary>
    /// <remarks>
    /// This will populate the <see cref="SimpleChannelProvider.ChannelService"/> from a configuration entry with the key of <see cref="ChannelServiceKey"/>.
    ///
    /// NOTE: if the keys are not present, a <c>null</c> value will be used.
    /// </remarks>
    public sealed class ConfigurationChannelProvider : SimpleChannelProvider, ICloudEnvironmentProvider
    {
        /// <summary>
        /// The key for ChannelService.
        /// </summary>
        public const string ChannelServiceKey = "ChannelService";

        /// <summary>
        /// The key for IsGovernment.
        /// </summary>
        public const string IsGovernmentKey = "IsGovernment";

        /// <summary>
        /// The key for ToChannelFromBotLoginUrl.
        /// </summary>
        public const string ToChannelFromBotLoginUrlKey = "ToChannelFromBotLoginUrl";

        /// <summary>
        /// The key for ToChannelFromBotOAuthScope.
        /// </summary>
        public const string ToChannelFromBotOAuthScopeKey = "ToChannelFromBotOAuthScope";

        /// <summary>
        /// The key for ToBotFromChannelTokenIssuer.
        /// </summary>
        public const string ToBotFromChannelTokenIssuerKey = "ToBotFromChannelTokenIssuer";

        /// <summary>
        /// The key for OAuthUrl.
        /// </summary>
        public const string OAuthUrlKey = "OAuthUrl";

        /// <summary>
        /// The key for ToBotFromChannelOpenIdMetadataUrl.
        /// </summary>
        public const string ToBotFromChannelOpenIdMetadataUrlKey = "ToBotFromChannelOpenIdMetadataUrl";

        /// <summary>
        /// The key for ToBotFromEmulatorOpenIdMetadataUrl.
        /// </summary>
        public const string ToBotFromEmulatorOpenIdMetadataUrlKey = "ToBotFromEmulatorOpenIdMetadataUrl";

        /// <summary>
        /// The key for CallerId.
        /// </summary>
        public const string CallerIdKey = "CallerId";

        private bool _isGovernment;
        private string _toChannelFromBotLoginUrl;
        private string _toChannelFromBotOAuthScope;
        private string _toBotFromChannelTokenIssuer;
        private string _oAuthUrl;
        private string _toBotFromChannelOpenIdMetadataUrl;
        private string _toBotFromEmulatorOpenIdMetadataUrl;
        private string _callerId;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationChannelProvider"/> class.
        /// </summary>
        /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
        public ConfigurationChannelProvider(IConfiguration configuration)
        {
            this.ChannelService = configuration.GetSection(ChannelServiceKey)?.Value;
            _isGovernment = configuration.GetSection(IsGovernmentKey)?.Get<bool>() ?? false;
            _toChannelFromBotLoginUrl = configuration.GetSection(ToChannelFromBotLoginUrlKey)?.Value;
            _toChannelFromBotOAuthScope = configuration.GetSection(ToChannelFromBotOAuthScopeKey)?.Value;
            _toBotFromChannelTokenIssuer = configuration.GetSection(ToBotFromChannelTokenIssuerKey)?.Value;
            _oAuthUrl = configuration.GetSection(OAuthUrlKey)?.Value;
            _toBotFromChannelOpenIdMetadataUrl = configuration.GetSection(ToBotFromChannelOpenIdMetadataUrlKey)?.Value;
            _toBotFromEmulatorOpenIdMetadataUrl = configuration.GetSection(ToBotFromEmulatorOpenIdMetadataUrlKey)?.Value;
            _callerId = configuration.GetSection(CallerIdKey)?.Value;
        }

        /// <summary>
        /// Gets the cloud environment to be used.
        /// </summary>
        /// <returns>The cloud environment property.</returns>
        public Task<CloudEnvironment> GetCloudEnvironmentAsync()
        {
            var newCloudEnvironment = new CloudEnvironment(
                ChannelService,
                _isGovernment,
                _toChannelFromBotLoginUrl,
                _toChannelFromBotOAuthScope,
                _toBotFromChannelTokenIssuer,
                _oAuthUrl,
                _toBotFromChannelOpenIdMetadataUrl,
                _toBotFromEmulatorOpenIdMetadataUrl,
                _callerId);

            return Task.FromResult(newCloudEnvironment);
        }
    }
}
