// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Bot.Builder.Dialogs.Memory.PathResolvers
{
    /// <summary>
    /// Maps aliasXXX -> path.xxx ($foo => dialog.foo).
    /// </summary>
    public class AliasPathResolver : IPathResolver
    {
        private readonly string _postfix;
        private readonly string _prefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="AliasPathResolver"/> class.
        /// </summary>
        /// <param name="alias">The alias.</param>
        /// <param name="prefix">The prefix for the alias.</param>
        /// <param name="postfix">The postfix for the alias.</param>
        public AliasPathResolver(string alias, string prefix, string postfix = null)
        {
            Alias = alias?.Trim() ?? throw new ArgumentNullException(nameof(alias));
            _prefix = prefix?.Trim() ?? throw new ArgumentNullException(nameof(prefix));
            _postfix = postfix ?? string.Empty;
        }

        /// <summary>
        /// Gets the alias name.
        /// </summary>
        /// <value>
        /// The alias name.
        /// </value>
        public string Alias { get; }

        /// <summary>
        /// Transforms the path parameter if it has an alias mapping.
        /// </summary>
        /// <param name="path">The path to parse.</param>
        /// <returns>A string representing the path.</returns>
        public virtual string TransformPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            path = path.Trim();
            if (path.StartsWith(Alias, StringComparison.Ordinal) && path.Length > Alias.Length && IsPathChar(path[Alias.Length]))
            {
                // here we only deals with trailing alias, alias in middle be handled in further breakdown
                // $xxx -> path.xxx
                return $"{_prefix}{path.Substring(Alias.Length)}{_postfix}".TrimEnd('.');
            }

            return path;
        }

#pragma warning disable CA1822 // Mark members as static (we can't change this without breaking binary compat)
        /// <summary>
        /// Checks if the ch parameter is a character or an underscore.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>A bool.</returns>
        protected bool IsPathChar(char ch)
#pragma warning restore CA1822 // Mark members as static
        {
            return char.IsLetter(ch) || ch == '_';
        }
    }
}
