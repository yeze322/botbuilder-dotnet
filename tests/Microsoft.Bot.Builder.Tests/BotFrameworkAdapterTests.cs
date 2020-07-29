// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.using System.Security.Claims;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;

namespace Microsoft.Bot.Builder.Tests
{
    [TestClass]
    public class BotFrameworkAdapterTests
    {
        private const string AppCredentialsCacheName = "_appCredentialMap";
        private const string ConnectorClientsCacheName = "_connectorClients";

        [TestMethod]
        public async Task TenantIdShouldBeSetInConversationForTeams()
        {
            var activity = await ProcessActivity(Channels.Msteams, "theTenantId", null);
            Assert.AreEqual("theTenantId", activity.Conversation.TenantId);
        }

        [TestMethod]
        public async Task TenantIdShouldNotChangeInConversationForTeamsIfPresent()
        {
            var activity = await ProcessActivity(Channels.Msteams, "theTenantId", "shouldNotBeReplaced");
            Assert.AreEqual("shouldNotBeReplaced", activity.Conversation.TenantId);
        }

        [TestMethod]
        public async Task TenantIdShouldNotBeSetInConversationIfNotTeams()
        {
            var activity = await ProcessActivity(Channels.Directline, "theTenantId", null);
            Assert.IsNull(activity.Conversation.TenantId);
        }

        [TestMethod]
        public async Task TenantIdShouldNotFailIfNoChannelData()
        {
            var activity = await ProcessActivity(Channels.Directline, null, null);
            Assert.IsNull(activity.Conversation.TenantId);
        }

        [TestMethod]
        public async Task CreateConversationOverloadProperlySetsTenantId()
        {
            // Arrange
            const string activityIdName = "ActivityId";
            const string activityIdValue = "SendActivityId";
            const string conversationIdName = "Id";
            const string conversationIdValue = "NewConversationId";
            const string tenantIdValue = "theTenantId";
            const string eventActivityName = "CreateConversation";

            Func<Task<HttpResponseMessage>> createResponseMessage = () =>
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                response.Content = new StringContent(new JObject { { activityIdName, activityIdValue }, { conversationIdName, conversationIdValue } }.ToString());
                return Task.FromResult(response);
            };

            var mockCredentialProvider = new Mock<ICredentialProvider>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) => createResponseMessage());

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            var adapter = new BotFrameworkAdapter(mockCredentialProvider.Object, customHttpClient: httpClient);

            var activity = new Activity("test")
            {
                ChannelId = Channels.Msteams,
                ServiceUrl = "https://fake.service.url",
                ChannelData = new JObject
                {
                    ["tenant"] = new JObject
                    { ["id"] = tenantIdValue },
                },
                Conversation = new ConversationAccount
                { TenantId = tenantIdValue },
            };

            var parameters = new ConversationParameters()
            {
                Activity = new Activity()
                {
                    ChannelData = activity.ChannelData,
                },
            };
            var reference = activity.GetConversationReference();
            var credentials = new MicrosoftAppCredentials(string.Empty, string.Empty, httpClient);

            Activity newActivity = null;

            Task UpdateParameters(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                newActivity = turnContext.Activity;
                return Task.CompletedTask;
            }

            // Act
            await adapter.CreateConversationAsync(activity.ChannelId, activity.ServiceUrl, credentials, parameters, UpdateParameters, reference, new CancellationToken());

            // Assert - all values set correctly
            Assert.AreEqual(tenantIdValue, JObject.FromObject(newActivity.ChannelData)["tenant"]["tenantId"]);
            Assert.AreEqual(activityIdValue, newActivity.Id);
            Assert.AreEqual(conversationIdValue, newActivity.Conversation.Id);
            Assert.AreEqual(tenantIdValue, newActivity.Conversation.TenantId);
            Assert.AreEqual(eventActivityName, newActivity.Name);
        }

        [TestMethod]
        public async Task OutgoingActivityIdsAreNotSent()
        {
            // Arrange
            var mockCredentialProvider = new Mock<ICredentialProvider>();
            var mockConnector = new MemoryConnectorClient();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var adapter = new BotFrameworkAdapter(mockCredentialProvider.Object, customHttpClient: httpClient);

            var incomingActivity = new Activity("test")
            {
                Id = "testid",
                ChannelId = Channels.Directline,
                ServiceUrl = "https://fake.service.url",
                Conversation = new ConversationAccount
                {
                    Id = "cid",
                }
            };

            var reply = MessageFactory.Text("test");
            reply.Id = "TestReplyId";
            
            // Act
            using (var turnContext = new TurnContext(adapter, incomingActivity))
            {
                turnContext.TurnState.Add<IConnectorClient>(mockConnector);

                var responseIds = await turnContext.SendActivityAsync(reply, default);
            }

            var sentActivity = mockConnector.MemoryConversations.SentActivities.FirstOrDefault(f => f.Type == ActivityTypes.Message);

            // Assert - assert the reply's id is not sent
            Assert.IsNull(sentActivity.Id); 
        }

        [TestMethod]
        public async Task ContinueConversationAsyncWithoutAudience()
        {
            // Arrange
            var mockCredentialProvider = new Mock<ICredentialProvider>();
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var adapter = new BotFrameworkAdapter(mockCredentialProvider.Object, customHttpClient: httpClient);

            // Create ClaimsIdentity that represents Skill2-to-Skill1 communication
            var skill2AppId = "00000000-0000-0000-0000-000000skill2";
            var skill1AppId = "00000000-0000-0000-0000-000000skill1";

            var skillClaims = new List<Claim>
            {
                new Claim(AuthenticationConstants.AudienceClaim, skill1AppId),
                new Claim(AuthenticationConstants.AppIdClaim, skill2AppId),
                new Claim(AuthenticationConstants.VersionClaim, "1.0")
            };
            var skillsIdentity = new ClaimsIdentity(skillClaims);
            var channelServiceUrl = "https://smba.trafficmanager.net/amer/";

            // Skill1 is calling ContinueSkillConversationAsync() to proactively send an Activity to the channel
            var callback = new BotCallbackHandler(async (turnContext, ct) =>
            {
                GetAppCredentialsAndAssertValues(turnContext, skill1AppId, AuthenticationConstants.ToChannelFromBotOAuthScope, 1);
                GetConnectorClientsAndAssertValues(
                    turnContext,
                    skill1AppId,
                    AuthenticationConstants.ToChannelFromBotOAuthScope,
                    new Uri(channelServiceUrl),
                    1);

                // Get "skill1-to-channel" ConnectorClient off of TurnState
                var adapter = turnContext.Adapter as BotFrameworkAdapter;
                var clientCache = GetCache<ConcurrentDictionary<string, ConnectorClient>>(adapter, ConnectorClientsCacheName);
                clientCache.TryGetValue($"{channelServiceUrl}{skill1AppId}:{AuthenticationConstants.ToChannelFromBotOAuthScope}", out var client);

                var turnStateClient = turnContext.TurnState.Get<IConnectorClient>();
                var clientCreds = turnStateClient.Credentials as AppCredentials;

                Assert.AreEqual(skill1AppId, clientCreds.MicrosoftAppId);
                Assert.AreEqual(AuthenticationConstants.ToChannelFromBotOAuthScope, clientCreds.OAuthScope);
                Assert.AreEqual(client.BaseUri, turnStateClient.BaseUri);

                // var scope = turnContext.TurnState.Get<string>(BotAdapter.OAuthScopeKey);
                // Assert.AreEqual(AuthenticationConstants.ToChannelFromBotOAuthScope, scope);
            });

            // Create ConversationReference to send a proactive message from Skill1 to a channel
            var refs = new ConversationReference(serviceUrl: channelServiceUrl);

            await adapter.ContinueConversationAsync(skillsIdentity, refs, callback, default);
        }

        private static void GetAppCredentialsAndAssertValues(ITurnContext turnContext, string expectedAppId, string expectedScope, int credsCount)
        {
            if (credsCount > 0)
            {
                var credsCache = GetCache<ConcurrentDictionary<string, AppCredentials>>((BotFrameworkAdapter)turnContext.Adapter, AppCredentialsCacheName);
                var cacheKey = $"{expectedAppId}{expectedScope}";
                credsCache.TryGetValue(cacheKey, out var creds);
                Assert.AreEqual(credsCount, credsCache.Count);

                Assert.AreEqual(expectedAppId, creds.MicrosoftAppId);
                Assert.AreEqual(expectedScope, creds.OAuthScope);
            }
        }

        private static void GetConnectorClientsAndAssertValues(ITurnContext turnContext, string expectedAppId, string expectedScope, Uri expectedUrl, int clientCount)
        {
            var clientCache = GetCache<ConcurrentDictionary<string, ConnectorClient>>((BotFrameworkAdapter)turnContext.Adapter, ConnectorClientsCacheName);
            var cacheKey = expectedAppId == null ? $"{expectedUrl}:" : $"{expectedUrl}{expectedAppId}:{expectedScope}";
            clientCache.TryGetValue(cacheKey, out var client);

            Assert.AreEqual(clientCount, clientCache.Count);
            var creds = (AppCredentials)client?.Credentials;
            Assert.AreEqual(expectedAppId, creds?.MicrosoftAppId);
            Assert.AreEqual(expectedScope, creds?.OAuthScope);
            Assert.AreEqual(expectedUrl, client?.BaseUri);
        }

        private static T GetCache<T>(BotFrameworkAdapter adapter, string fieldName)
        {
            var cacheField = typeof(BotFrameworkAdapter).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)cacheField.GetValue(adapter);
        }

        private static async Task<IActivity> ProcessActivity(string channelId, object channelData, string conversationTenantId)
        {
            IActivity activity = null;
            var mockClaims = new Mock<ClaimsIdentity>();
            var mockCredentialProvider = new Mock<ICredentialProvider>();

            var sut = new BotFrameworkAdapter(mockCredentialProvider.Object);
            await sut.ProcessActivityAsync(
                mockClaims.Object,
                new Activity("test")
                {
                    ChannelId = channelId,
                    ServiceUrl = "https://smba.trafficmanager.net/amer/",
                    ChannelData = channelData,
                    Conversation = new ConversationAccount
                        { TenantId = conversationTenantId },
                },
                (context, token) =>
                {
                    activity = context.Activity;
                    return Task.CompletedTask;
                },
                CancellationToken.None);
            return activity;
        }

        private static async Task<IActivity> ProcessActivity(string channelId, string channelDataTenantId, string conversationTenantId)
        {
            var channelData = new JObject
            {
                ["tenant"] = new JObject
                    { ["id"] = channelDataTenantId },
            };

            return await ProcessActivity(channelId, channelData, conversationTenantId);
        }
    }
}
