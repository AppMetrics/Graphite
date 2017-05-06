// <copyright file="GraphitePayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public class GraphitePayloadBuilder : IMetricPayloadBuilder<GraphitePayload>
    {
        private readonly DateTime? _timestamp;
        private GraphitePayload _payload;

        public GraphitePayloadBuilder() { _timestamp = null; }

        public GraphitePayloadBuilder(DateTime timestamp) { _timestamp = timestamp; }

        public void Clear() { _payload = null; }

        public void Init()
        {
            _payload = new GraphitePayload();
        }

        public void Pack(string name, object value, MetricTags tags)
        {
            _payload?.Add(new GraphitePoint(name, new Dictionary<string, object> { { "value", value } }, tags, _timestamp));
        }

        public void Pack(
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data })
                                .ToDictionary(pair => pair.column, pair => pair.data);

            _payload?.Add(new GraphitePoint(name, fields, tags, _timestamp));
        }

        public GraphitePayload Payload() { return _payload; }

        public string PayloadFormatted()
        {
            var result = new StringWriter();
            _payload.Format(result);
            return result.ToString();
        }
    }
}