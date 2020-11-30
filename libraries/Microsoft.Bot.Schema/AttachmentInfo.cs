﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Schema
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>Metadata for an attachment.</summary>
    public partial class AttachmentInfo
    {
        /// <summary>Initializes a new instance of the <see cref="AttachmentInfo"/> class.</summary>
        public AttachmentInfo()
        {
            CustomInit();
        }

        /// <summary>Initializes a new instance of the <see cref="AttachmentInfo"/> class.</summary>
        /// <param name="name">Name of the attachment.</param>
        /// <param name="type">ContentType of the attachment.</param>
        /// <param name="views">attachment views.</param>
        public AttachmentInfo(string name = default(string), string type = default(string), IList<AttachmentView> views = default(IList<AttachmentView>))
        {
            Name = name;
            Type = type;
            Views = views;
            CustomInit();
        }

        /// <summary>Gets or sets name of the attachment.</summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>Gets or sets contentType of the attachment.</summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>Gets or sets attachment views.</summary>
        [JsonProperty(PropertyName = "views")]
        public IList<AttachmentView> Views { get; set; }

        /// <summary>An initialization method that performs custom operations like setting defaults.</summary>
        partial void CustomInit();
    }
}
