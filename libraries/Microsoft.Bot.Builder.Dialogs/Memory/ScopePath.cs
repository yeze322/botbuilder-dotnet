// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable (we can't change this without breaking binary compat)
    public class ScopePath
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        /// <summary>
        /// User memory scope root path.
        /// </summary>
        public const string User = "user";

        /// <summary>
        /// Conversation memory scope root path.
        /// </summary>
        public const string Conversation = "conversation";

        /// <summary>
        /// Dialog memory scope root path.
        /// </summary>
        public const string Dialog = "dialog";

        /// <summary>
        /// DialogClass memory scope root path.
        /// </summary>
        public const string DialogClass = "dialogclass";

        /// <summary>
        /// DialogContext memory scope root path.
        /// </summary>
        public const string DialogContext = "dialogContext";

        /// <summary>
        /// This memory scope root path.
        /// </summary>
        public const string This = "this";

        /// <summary>
        /// Class memory scope root path.
        /// </summary>
        public const string Class = "class";

        /// <summary>
        /// Settings memory scope root path.
        /// </summary>
        public const string Settings = "settings";

        /// <summary>
        /// Turn memory scope root path.
        /// </summary>
        public const string Turn = "turn";

        /// <summary>
        /// User scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.User instead.")]
        public const string USER = "user";

        /// <summary>
        /// Conversation scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.Conversation instead.")]
        public const string CONVERSATION = "conversation";

        /// <summary>
        /// Dialog scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.Dialog instead.")]
        public const string DIALOG = "dialog";

        /// <summary>
        /// Dialog class scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.DialogClass instead.")]
        public const string DIALOGCLASS = "dialogclass";

        /// <summary>
        /// This scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.This instead.")]
        public const string THIS = "this";

        /// <summary>
        /// Class scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.Class instead.")]
        public const string CLASS = "class";

        /// <summary>
        /// Settings scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.Settings instead.")]
        public const string SETTINGS = "settings";

        /// <summary>
        /// Turn scope root path.
        /// </summary>
        [Obsolete("This property is deprecated, use ScopePath.Turn instead.")]
        public const string TURN = "turn";
    }
}
