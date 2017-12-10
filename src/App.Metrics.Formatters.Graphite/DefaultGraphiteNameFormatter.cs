// <copyright file="DefaultGraphiteNameFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Metrics.Formatters.Graphite
{
    public class DefaultGraphiteNameFormatter : IGraphiteNameFormatter
    {
        private static readonly HashSet<string> ExcludeTags = new HashSet<string> { "app", "env", "server", "mtype", "unit", "unit_rate", "unit_dur" };
        private readonly string _prefix;

        public DefaultGraphiteNameFormatter()
            : this(null)
        {
        }

        public DefaultGraphiteNameFormatter(string prefix)
        {
            _prefix = prefix;
        }

        /// <inheritdoc />
        public IEnumerable<string> Format(GraphitePoint point)
        {
            var sb = new StringBuilder();

            var tagsDictionary = point.Tags.ToDictionary(GraphiteSyntax.EscapeName);

            if (!string.IsNullOrEmpty(_prefix))
            {
                sb.Append($"{_prefix}");
            }

            if (tagsDictionary.TryGetValue("app", out var appValue))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append("app.");
                sb.Append(appValue);
            }

            if (tagsDictionary.TryGetValue("env", out var envValue))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append("env.");
                sb.Append(envValue);
            }

            if (tagsDictionary.TryGetValue("server", out var serverValue))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append("server.");
                sb.Append(serverValue);
            }

            if (tagsDictionary.TryGetValue("mtype", out var metricType) && !string.IsNullOrWhiteSpace(metricType))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append(metricType);
                sb.Append(".");
            }

            if (!point.Context.IsMissing())
            {
                sb.Append(GraphiteSyntax.EscapeName(point.Context, true));
                sb.Append(".");
            }

            sb.Append(GraphiteSyntax.EscapeName(point.Name, true));

            sb.Append('.');
            var prefix = sb.ToString();

            foreach (var f in point.Fields)
            {
                var fieldStringBuilder = new StringBuilder(prefix);
                fieldStringBuilder.Append(GraphiteSyntax.EscapeName(f.Key));

            var tags = tagsDictionary.Where(tag => !ExcludeTags.Contains(tag.Key));

            foreach (var tag in tags)
            {
                sb.Append(';');
                sb.Append(GraphiteSyntax.EscapeName(tag.Key));
                sb.Append('=');
                sb.Append(tag.Value);
            }

                fieldStringBuilder.Append(' ');
                fieldStringBuilder.Append(GraphiteSyntax.FormatValue(f.Value));

                fieldStringBuilder.Append(' ');
                fieldStringBuilder.Append(GraphiteSyntax.FormatTimestamp(point.UtcTimestamp));

                yield return fieldStringBuilder.ToString();
            }
        }
    }
}
