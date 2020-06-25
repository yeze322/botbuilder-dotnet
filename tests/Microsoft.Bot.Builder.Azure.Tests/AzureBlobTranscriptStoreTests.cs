// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.Tests;
using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Xunit;
using Activity = Microsoft.Bot.Schema.Activity;

// These tests require Azure Storage Emulator v5.7
// The emulator must be installed at this path C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe
// More info: https://docs.microsoft.com/azure/storage/common/storage-use-emulator
namespace Microsoft.Bot.Builder.Azure.Tests
{
    public class AzureBlobTranscriptStoreTests : StorageBaseTests
    {
        private const string ConnectionString = @"AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
        private const string ChannelId = "test";

        private static readonly string[] LongId =
        {
            "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq1234567890Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq098765432112345678900987654321Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq123456789009876543211234567890Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq09876543211234567890098765432112345678900987654321",
        };

        private static readonly string[] ConversationIds =
        {
            "qaz", "wsx", "edc", "rfv", "tgb", "yhn", "ujm", "123", "456", "789",
            "ZAQ", "XSW", "CDE", "VFR", "BGT", "NHY", "NHY", "098", "765", "432",
            "zxc", "vbn", "mlk", "jhy", "yui", "kly", "asd", "asw", "aaa", "zzz",
        };

        private static readonly string[] ConversationSpecialIds = { "asd !&/#.'+:?\"", "ASD@123<>|}{][", "$%^;\\*()_" };
        private static readonly string[] ActivityIds =
        {
            "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
            "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        };

        public AzureBlobTranscriptStore TranscriptStore
        {
            get { return new AzureBlobTranscriptStore(ConnectionString, "TranscriptStore"); }
        }

        // These tests require Azure Storage Emulator v5.7
        public void ContainerInit(string name)
        {
            var container = CloudStorageAccount.Parse(ConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(name);
            container.DeleteIfExists();
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task TranscriptsEmptyTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("TranscriptsEmptyTest");

                var unusedChannelId = Guid.NewGuid().ToString();
                var transcripts = await TranscriptStore.ListTranscriptsAsync(unusedChannelId);
                Assert.Empty(transcripts.Items);
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task ActivityEmptyTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("ActivityEmptyTest");

                foreach (var convoId in ConversationSpecialIds)
                {
                    var activities = await TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, convoId);
                    Assert.Empty(activities.Items);
                }
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task ActivityAddTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("ActivityAddTest");

                var loggedActivities = new IActivity[5];
                var activities = new List<IActivity>();
                for (var i = 0; i < 5; i++)
                {
                    var a = CreateActivity(i, i, ConversationIds);
                    await TranscriptStore.LogActivityAsync(a);
                    activities.Add(a);
                    loggedActivities[i] = TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, ConversationIds[i]).Result.Items[0];
                }

                Assert.Equal(5, loggedActivities.Length);
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task TranscriptRemoveTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("TranscriptRemoveTest");

                for (var i = 0; i < 5; i++)
                {
                    var a = CreateActivity(i, i, ConversationIds);
                    await TranscriptStore.DeleteTranscriptAsync(a.ChannelId, a.Conversation.Id);

                    var loggedActivities =
                        await TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, ConversationIds[i]);
                    Assert.Empty(loggedActivities.Items);
                }
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task ActivityAddSpecialCharsTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("ActivityAddSpecialCharsTest");

                var loggedActivities = new IActivity[ConversationSpecialIds.Length];
                var activities = new List<IActivity>();
                for (var i = 0; i < ConversationSpecialIds.Length; i++)
                {
                    var a = CreateActivity(i, i, ConversationSpecialIds);
                    await TranscriptStore.LogActivityAsync(a);
                    activities.Add(a);
                    loggedActivities[i] = TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, ConversationSpecialIds[i]).Result.Items[0];
                }

                Assert.Equal(activities.Count, loggedActivities.Length);
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task TranscriptRemoveSpecialCharsTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("TranscriptRemoveSpecialCharsTest");

                for (var i = 0; i < ConversationSpecialIds.Length; i++)
                {
                    var a = CreateActivity(i, i, ConversationSpecialIds);
                    await TranscriptStore.DeleteTranscriptAsync(a.ChannelId, a.Conversation.Id);

                    var loggedActivities =
                        await TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, ConversationSpecialIds[i]);
                    Assert.Empty(loggedActivities.Items);
                }
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task ActivityAddPagedResultTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("ActivityAddPagedResultTest");

                var cleanChanel = Guid.NewGuid().ToString();

                var loggedPagedResult = new PagedResult<IActivity>();
                var activities = new List<IActivity>();

                for (var i = 0; i < ConversationIds.Length; i++)
                {
                    var a = CreateActivity(0, i, ConversationIds);
                    a.ChannelId = cleanChanel;

                    await TranscriptStore.LogActivityAsync(a);
                    activities.Add(a);
                }

                loggedPagedResult = TranscriptStore.GetTranscriptActivitiesAsync(cleanChanel, ConversationIds[0]).Result;
                var ct = loggedPagedResult.ContinuationToken;
                Assert.Equal(20, loggedPagedResult.Items.Length);
                Assert.NotNull(ct);
                Assert.True(loggedPagedResult.ContinuationToken.Length > 0);
                loggedPagedResult = TranscriptStore.GetTranscriptActivitiesAsync(cleanChanel, ConversationIds[0], ct).Result;
                ct = loggedPagedResult.ContinuationToken;
                Assert.Equal(10, loggedPagedResult.Items.Length);
                Assert.Null(ct);
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task TranscriptRemovePagedTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("TranscriptRemovePagedTest");

                var loggedActivities = new PagedResult<IActivity>();
                int i;
                for (i = 0; i < ConversationSpecialIds.Length; i++)
                {
                    var a = CreateActivity(i, i, ConversationIds);
                    await TranscriptStore.DeleteTranscriptAsync(a.ChannelId, a.Conversation.Id);
                }

                loggedActivities =
                    await TranscriptStore.GetTranscriptActivitiesAsync(ChannelId, ConversationIds[i]);
                Assert.Empty(loggedActivities.Items);
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task LongIdAddTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("LongIdAddTest");
                var a = CreateActivity(0, 0, LongId);
                await Assert.ThrowsAsync<StorageException>(() => TranscriptStore.LogActivityAsync(a));
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public void BlobTranscriptParamTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("BlobTranscriptParamTest");
                Assert.Throws<FormatException>(() => new AzureBlobTranscriptStore("123", "BlobTranscriptParamTest"));

                Assert.Throws<ArgumentNullException>(() =>
                    new AzureBlobTranscriptStore((CloudStorageAccount)null, ContainerName("BlobTranscriptParamTest")));

                Assert.Throws<ArgumentNullException>(() =>
                    new AzureBlobTranscriptStore((string)null, ContainerName("BlobTranscriptParamTest")));

                Assert.Throws<ArgumentNullException>(() =>
                    new AzureBlobTranscriptStore((CloudStorageAccount)null, null));

                Assert.Throws<ArgumentNullException>(() => new AzureBlobTranscriptStore((string)null, null));
            }
        }

        // These tests require Azure Storage Emulator v5.7
        [Fact]
        public async Task NullBlobTest()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("NullBlobTest");
                AzureBlobTranscriptStore store = null;

                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                    await store.LogActivityAsync(CreateActivity(0, 0, ConversationIds)));
                await Assert.ThrowsAsync<NullReferenceException>(async () =>
                    await store.GetTranscriptActivitiesAsync(ChannelId, ConversationIds[0]));
            }
        }

        [Fact]
        public async Task LogActivities()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("LogActivities");

                var conversation = TestAdapter.CreateConversation(Guid.NewGuid().ToString("n"));
                TestAdapter adapter = new TestAdapter(conversation)
                    .Use(new TranscriptLoggerMiddleware(TranscriptStore));

                await new TestFlow(adapter, async (context, cancellationToken) =>
                    {
                        var typingActivity = new Activity
                        {
                            Type = ActivityTypes.Typing,
                            RelatesTo = context.Activity.RelatesTo
                        };
                        await context.SendActivityAsync(typingActivity);
                        await Task.Delay(500);
                        await context.SendActivityAsync("echo:" + context.Activity.Text);
                    })
                    .Send("foo")
                        .AssertReply((activity) => Assert.Equal(activity.Type, ActivityTypes.Typing))
                        .AssertReply("echo:foo")
                    .Send("bar")
                        .AssertReply((activity) => Assert.Equal(activity.Type, ActivityTypes.Typing))
                        .AssertReply("echo:bar")
                    .StartTestAsync();

                await Task.Delay(1000);

                var pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id);
                Assert.Equal(6, pagedResult.Items.Length);
                Assert.Equal("foo", pagedResult.Items[0].AsMessageActivity().Text);
                Assert.NotNull(pagedResult.Items[1].AsTypingActivity());
                Assert.Equal("echo:foo", pagedResult.Items[2].AsMessageActivity().Text);
                Assert.Equal("bar", pagedResult.Items[3].AsMessageActivity().Text);
                Assert.NotNull(pagedResult.Items[4].AsTypingActivity());
                Assert.Equal("echo:bar", pagedResult.Items[5].AsMessageActivity().Text);
                foreach (var activity in pagedResult.Items)
                {
                    Assert.True(!string.IsNullOrWhiteSpace(activity.Id));
                    Assert.True(activity.Timestamp > default(DateTimeOffset));
                }
            }
        }

        [Fact]
        public async Task LogUpdateActivities()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("LogUpdateActivities");

                var conversation = TestAdapter.CreateConversation(Guid.NewGuid().ToString("n"));
                TestAdapter adapter = new TestAdapter(conversation)
                    .Use(new TranscriptLoggerMiddleware(TranscriptStore));
                Activity activityToUpdate = null;
                await new TestFlow(adapter, async (context, cancellationToken) =>
                {
                    if (context.Activity.Text == "update")
                    {
                        activityToUpdate.Text = "new response";
                        await context.UpdateActivityAsync(activityToUpdate);
                    }
                    else
                    {
                        var activity = context.Activity.CreateReply("response");
                        var response = await context.SendActivityAsync(activity);
                        activity.Id = response.Id;

                        // clone the activity, so we can use it to do an update
                        activityToUpdate = JsonConvert.DeserializeObject<Activity>(JsonConvert.SerializeObject(activity));
                    }
                })
                    .Send("foo")
                    .Send("update")
                        .AssertReply("new response")
                    .StartTestAsync();

                await Task.Delay(1000);

                var pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id);
                Assert.Equal(3, pagedResult.Items.Length);
                Assert.Equal("foo", pagedResult.Items[0].AsMessageActivity().Text);
                Assert.Equal("new response", pagedResult.Items[1].AsMessageActivity().Text);
                Assert.Equal("update", pagedResult.Items[2].AsMessageActivity().Text);
            }
        }

        [Fact]
        public async Task TestDateLogUpdateActivities()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("TestDateLogUpdateActivities");

                var dateTimeStartOffset1 = new DateTimeOffset(DateTime.Now);
                var dateTimeStartOffset2 = new DateTimeOffset(DateTime.UtcNow);
                var conversation = TestAdapter.CreateConversation(Guid.NewGuid().ToString("n"));
                TestAdapter adapter = new TestAdapter(conversation)
                    .Use(new TranscriptLoggerMiddleware(TranscriptStore));
                Activity activityToUpdate = null;
                await new TestFlow(adapter, async (context, cancellationToken) =>
                {
                    if (context.Activity.Text == "update")
                    {
                        activityToUpdate.Text = "new response";
                        await context.UpdateActivityAsync(activityToUpdate);
                    }
                    else
                    {
                        var activity = context.Activity.CreateReply("response");

                        var response = await context.SendActivityAsync(activity);
                        activity.Id = response.Id;

                        // clone the activity, so we can use it to do an update
                        activityToUpdate = JsonConvert.DeserializeObject<Activity>(JsonConvert.SerializeObject(activity));
                    }
                })
                    .Send("foo")
                    .Send("update")
                        .AssertReply("new response")
                    .StartTestAsync();

                await Task.Delay(5000);

                // Perform some queries
                var pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id, null, dateTimeStartOffset1.DateTime);
                Assert.Equal(3, pagedResult.Items.Length);
                Assert.Equal("foo", pagedResult.Items[0].AsMessageActivity().Text);
                Assert.Equal("new response", pagedResult.Items[1].AsMessageActivity().Text);
                Assert.Equal("update", pagedResult.Items[2].AsMessageActivity().Text);

                // Perform some queries
                pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id, null, DateTimeOffset.MinValue);
                Assert.Equal(3, pagedResult.Items.Length);
                Assert.Equal("foo", pagedResult.Items[0].AsMessageActivity().Text);
                Assert.Equal("new response", pagedResult.Items[1].AsMessageActivity().Text);
                Assert.Equal("update", pagedResult.Items[2].AsMessageActivity().Text);

                // Perform some queries
                pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id, null, DateTimeOffset.MaxValue);
                Assert.Empty(pagedResult.Items);
            }
        }

        [Fact]
        public async Task LogDeleteActivities()
        {
            if (StorageEmulatorHelper.CheckEmulator())
            {
                ContainerInit("LogDeleteActivities");

                var conversation = TestAdapter.CreateConversation(Guid.NewGuid().ToString("n"));
                TestAdapter adapter = new TestAdapter(conversation)
                    .Use(new TranscriptLoggerMiddleware(TranscriptStore));
                string activityId = null;
                await new TestFlow(adapter, async (context, cancellationToken) =>
                {
                    if (context.Activity.Text == "deleteIt")
                    {
                        await context.DeleteActivityAsync(activityId);
                    }
                    else
                    {
                        var activity = context.Activity.CreateReply("response");
                        var response = await context.SendActivityAsync(activity);
                        activityId = response.Id;
                    }
                })
                    .Send("foo")
                        .AssertReply("response")
                    .Send("deleteIt")
                    .StartTestAsync();

                await Task.Delay(1000);

                var pagedResult = await TranscriptStore.GetTranscriptActivitiesAsync(conversation.ChannelId, conversation.Conversation.Id);
                Assert.Equal(3, pagedResult.Items.Length);
                Assert.Equal("foo", pagedResult.Items[0].AsMessageActivity().Text);
                Assert.NotNull(pagedResult.Items[1].AsMessageDeleteActivity());
                Assert.Equal(ActivityTypes.MessageDelete, pagedResult.Items[1].Type);
                Assert.Equal("deleteIt", pagedResult.Items[2].AsMessageActivity().Text);
            }
        }

        private static Activity CreateActivity(int i, int j, string[] conversationIds)
        {
            return new Activity
            {
                Id = ActivityIds[j],
                ChannelId = "test",
                Text = "test",
                Type = ActivityTypes.Message,
                Conversation = new ConversationAccount(id: conversationIds[i]),
                Timestamp = DateTime.Now,
                From = new ChannelAccount("testUser"),
                Recipient = new ChannelAccount("testBot"),
            };
        }
    }
}
