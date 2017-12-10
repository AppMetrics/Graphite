// <copyright file="GraphitePoint.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Metrics.Formatters.Graphite
{
    public struct GraphitePoint
    {
        public GraphitePoint(
            string context,
            string name,
            IReadOnlyDictionary<string, object> fields,
            MetricTags tags,
            DateTime? utcTimestamp = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("A name must be specified", nameof(name));
            }

            if (fields == null || fields.Count == 0)
            {
                throw new ArgumentException("At least one field must be specified", nameof(fields));
            }

            if (fields.Any(f => string.IsNullOrEmpty(f.Key)))
            {
                throw new ArgumentException("Fields must have non-empty names", nameof(fields));
            }

            if (utcTimestamp != null && utcTimestamp.Value.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Timestamps must be specified as UTC", nameof(utcTimestamp));
            }

            Context = context;
            Name = name;
            Fields = fields;
            Tags = tags;
            UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
        }

        public string Context { get; }

        public IReadOnlyDictionary<string, object> Fields { get; }

        public string Name { get; }

        public MetricTags Tags { get; }

        public DateTime UtcTimestamp { get; }
    }
}
