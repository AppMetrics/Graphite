// <copyright file="GraphitePoint.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using App.Metrics.Tagging;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public class GraphitePoint
    {
        private readonly object _syncLock = new object();

        public GraphitePoint(
            string measurement,
            IReadOnlyDictionary<string, object> fields,
            MetricTags tags,
            DateTime? utcTimestamp = null)
        {
            if (string.IsNullOrEmpty(measurement))
            {
                throw new ArgumentException("A measurement name must be specified");
            }

            if (fields == null || fields.Count == 0)
            {
                throw new ArgumentException("At least one field must be specified");
            }

            if (fields.Any(f => string.IsNullOrEmpty(f.Key)))
            {
                throw new ArgumentException("Fields must have non-empty names");
            }

            if (utcTimestamp != null && utcTimestamp.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Timestamps must be specified as UTC");
            }

            Measurement = measurement;
            Fields = fields;
            Tags = tags;
            UtcTimestamp = utcTimestamp;
        }

        public IReadOnlyDictionary<string, object> Fields { get; }

        public string Measurement { get; }

        public MetricTags Tags { get; }

        public DateTime? UtcTimestamp { get; private set; }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                throw new ArgumentNullException(nameof(textWriter));
            }

            var sb = new StringBuilder();

            var tagsDictionary = Tags.ToDictionary(GraphiteSyntax.EscapeTagValue);

            if (tagsDictionary.ContainsKey("app"))
            {
                sb.Append("app.");
                sb.Append(tagsDictionary["app"]);
            }

            if (tagsDictionary.ContainsKey("env"))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append("env.");
                sb.Append(tagsDictionary["env"]);
            }

            if (tagsDictionary.ContainsKey("server"))
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append("server.");
                sb.Append(tagsDictionary["server"]);
            }

            var metricType = string.Empty;

            if (tagsDictionary.ContainsKey("mtype"))
            {
                metricType = tagsDictionary["mtype"];
            }

            if (metricType.IsPresent())
            {
                if (sb.Length > 0)
                {
                    sb.Append('.');
                }

                sb.Append($"{metricType}.");
            }

            sb.Append(GraphiteSyntax.EscapeName(Measurement));

            var tags = Tags.ToDictionary(GraphiteSyntax.EscapeTagValue).Where(tag => tag.Key != "app" && tag.Key != "env" && tag.Key != "server" && tag.Key != "mtype");

            foreach (var tag in tags)
            {
                sb.Append('.');
                sb.Append(GraphiteSyntax.EscapeName(tag.Key));
                sb.Append('.');
                sb.Append(GraphiteSyntax.EscapeTagValue(tag.Value));
            }

            foreach (var f in Fields)
            {
                textWriter.Write(sb.ToString());
                textWriter.Write('.');
                textWriter.Write(GraphiteSyntax.EscapeName(f.Key).Replace('.', '_'));
                textWriter.Write(' ');
                textWriter.Write(GraphiteSyntax.FormatValue(f.Value));

                if (UtcTimestamp == null)
                {
                    UtcTimestamp = DateTime.UtcNow;
                }

                textWriter.Write(' ');
                textWriter.Write(GraphiteSyntax.FormatTimestamp(UtcTimestamp.Value));
                textWriter.Write('\n');
            }
        }
    }
}