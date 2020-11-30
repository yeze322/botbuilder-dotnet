﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Bot.Schema
{
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Object representing error information.
    /// </summary>
#pragma warning disable CA1716 // Identifiers should not match keywords (Cannot change without breaking backwards compatibility.)
    public partial class Error
#pragma warning restore CA1716 // Identifiers should not match keywords
    {
        /// <summary>Initializes a new instance of the <see cref="Error"/> class.</summary>
        public Error()
        {
            CustomInit();
        }

        /// <summary>Initializes a new instance of the <see cref="Error"/> class.</summary>
        /// <param name="code">Error code.</param>
        /// <param name="message">Error message.</param>
        /// <param name="innerHttpError">Error from inner http call.</param>
        public Error(string code = default(string), string message = default(string), InnerHttpError innerHttpError = default(InnerHttpError))
        {
            Code = code;
            Message = message;
            InnerHttpError = innerHttpError;
            CustomInit();
        }

        /// <summary>Gets or sets error code.</summary>
        [JsonProperty(PropertyName = "code")]
        public string Code { get; set; }

        /// <summary>Gets or sets error message.</summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>Gets or sets error from inner http call.</summary>
        [JsonProperty(PropertyName = "innerHttpError")]
        public InnerHttpError InnerHttpError { get; set; }

        /// <summary>An initialization method that performs custom operations like setting defaults.</summary>
        partial void CustomInit();
    }
}
