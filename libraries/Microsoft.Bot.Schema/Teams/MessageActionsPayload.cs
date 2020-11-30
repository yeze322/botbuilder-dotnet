﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Schema.Teams
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the individual message within a chat or channel where a
    /// message actions is taken.
    /// </summary>
    public partial class MessageActionsPayload
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActionsPayload"/> class.
        /// </summary>
        public MessageActionsPayload()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageActionsPayload"/> class.
        /// </summary>
        /// <param name="id">Unique id of the message.</param>
        /// <param name="replyToId">Id of the parent/root message of the
        /// thread.</param>
        /// <param name="messageType">Type of message - automatically set to
        /// message. Possible values include: 'message'.</param>
        /// <param name="createdDateTime">Timestamp of when the message was
        /// created.</param>
        /// <param name="lastModifiedDateTime">Timestamp of when the message
        /// was edited or updated.</param>
        /// <param name="deleted">Indicates whether a message has been soft
        /// deleted.</param>
        /// <param name="subject">Subject line of the message.</param>
        /// <param name="summary">Summary text of the message that could be
        /// used for notifications.</param>
        /// <param name="importance">The importance of the message. Possible
        /// values include: 'normal', 'high', 'urgent'.</param>
        /// <param name="locale">Locale of the message set by the
        /// client.</param>
        /// <param name="from">Sender of the message.</param>
        /// <param name="body">Plaintext/HTML representation of the content of
        /// the message.</param>
        /// <param name="attachmentLayout">How the attachment(s) are displayed
        /// in the message.</param>
        /// <param name="attachments">Attachments in the message - card, image,
        /// file, etc.</param>
        /// <param name="mentions">List of entities mentioned in the
        /// message.</param>
        /// <param name="reactions">Reactions for the message.</param>
        public MessageActionsPayload(string id = default(string), string replyToId = default(string), string messageType = default(string), string createdDateTime = default(string), string lastModifiedDateTime = default(string), bool? deleted = default(bool?), string subject = default(string), string summary = default(string), string importance = default(string), string locale = default(string), MessageActionsPayloadFrom from = default(MessageActionsPayloadFrom), MessageActionsPayloadBody body = default(MessageActionsPayloadBody), string attachmentLayout = default(string), IList<MessageActionsPayloadAttachment> attachments = default(IList<MessageActionsPayloadAttachment>), IList<MessageActionsPayloadMention> mentions = default(IList<MessageActionsPayloadMention>), IList<MessageActionsPayloadReaction> reactions = default(IList<MessageActionsPayloadReaction>))
        {
            Id = id;
            ReplyToId = replyToId;
            MessageType = messageType;
            CreatedDateTime = createdDateTime;
            LastModifiedDateTime = lastModifiedDateTime;
            Deleted = deleted;
            Subject = subject;
            Summary = summary;
            Importance = importance;
            Locale = locale;
            From = from;
            Body = body;
            AttachmentLayout = attachmentLayout;
            Attachments = attachments;
            Mentions = mentions;
            Reactions = reactions;
            CustomInit();
        }

        /// <summary>
        /// Gets or sets unique id of the message.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets id of the parent/root message of the thread.
        /// </summary>
        [JsonProperty(PropertyName = "replyToId")]
        public string ReplyToId { get; set; }

        /// <summary>
        /// Gets or sets type of message - automatically set to message.
        /// Possible values include: 'message'.
        /// </summary>
        [JsonProperty(PropertyName = "messageType")]
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets timestamp of when the message was created.
        /// </summary>
        [JsonProperty(PropertyName = "createdDateTime")]
        public string CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets timestamp of when the message was edited or updated.
        /// </summary>
        [JsonProperty(PropertyName = "lastModifiedDateTime")]
        public string LastModifiedDateTime { get; set; }

        /// <summary>
        /// Gets or sets indicates whether a message has been soft deleted.
        /// </summary>
        [JsonProperty(PropertyName = "deleted")]
        public bool? Deleted { get; set; }

        /// <summary>
        /// Gets or sets subject line of the message.
        /// </summary>
        [JsonProperty(PropertyName = "subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets summary text of the message that could be used for
        /// notifications.
        /// </summary>
        [JsonProperty(PropertyName = "summary")]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the importance of the message. Possible values
        /// include: 'normal', 'high', 'urgent'.
        /// </summary>
        [JsonProperty(PropertyName = "importance")]
        public string Importance { get; set; }

        /// <summary>
        /// Gets or sets locale of the message set by the client.
        /// </summary>
        [JsonProperty(PropertyName = "locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets sender of the message.
        /// </summary>
        [JsonProperty(PropertyName = "from")]
        public MessageActionsPayloadFrom From { get; set; }

        /// <summary>
        /// Gets or sets plaintext/HTML representation of the content of the
        /// message.
        /// </summary>
        [JsonProperty(PropertyName = "body")]
        public MessageActionsPayloadBody Body { get; set; }

        /// <summary>
        /// Gets or sets how the attachment(s) are displayed in the message.
        /// </summary>
        [JsonProperty(PropertyName = "attachmentLayout")]
        public string AttachmentLayout { get; set; }

        /// <summary>
        /// Gets or sets attachments in the message - card, image, file, etc.
        /// </summary>
        [JsonProperty(PropertyName = "attachments")]
        public IList<MessageActionsPayloadAttachment> Attachments { get; set; }

        /// <summary>
        /// Gets or sets list of entities mentioned in the message.
        /// </summary>
        [JsonProperty(PropertyName = "mentions")]
        public IList<MessageActionsPayloadMention> Mentions { get; set; }

        /// <summary>
        /// Gets or sets reactions for the message.
        /// </summary>
        [JsonProperty(PropertyName = "reactions")]
        public IList<MessageActionsPayloadReaction> Reactions { get; set; }

        /// <summary>
        /// Gets or sets the link back to the message.
        /// </summary>
        [JsonProperty(PropertyName = "linkToMessage")]
        public Uri LinkToMessage { get; set; }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults.
        /// </summary>
        partial void CustomInit();
    }
}
