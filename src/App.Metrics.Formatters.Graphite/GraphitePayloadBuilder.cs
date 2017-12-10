// <copyright file="GraphitePayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using App.Metrics.Formatters.Graphite.Extensions;

namespace App.Metrics.Formatters.Graphite
{
    public class GraphitePayloadBuilder
    {
        private readonly IGraphiteNameFormatter _metricNameFormatter;
        private readonly DateTime? _timestamp;
        private GraphitePayload _payload;

        public GraphitePayloadBuilder(IGraphiteNameFormatter metricNameFormatter = null, GeneratedMetricNameMapping dataKeys = null, DateTime? timestamp = null)
        {
            _timestamp = timestamp;
            _payload = new GraphitePayload();
            _metricNameFormatter = metricNameFormatter ?? GraphiteFormatterConstants.GraphiteDefaults.MetricNameFormatter;
            DataKeys = dataKeys ?? new GeneratedMetricNameMapping(
                           GraphiteFormatterConstants.GraphiteDefaults.CustomHistogramDataKeys,
                           GraphiteFormatterConstants.GraphiteDefaults.CustomMeterDataKeys,
                           GraphiteFormatterConstants.GraphiteDefaults.CustomApdexKeys,
                           GraphiteFormatterConstants.GraphiteDefaults.CustomCounterDataKeys);
        }

        public GeneratedMetricNameMapping DataKeys { get; }

        public void Clear() { _payload = null; }

        public void Init() { _payload = new GraphitePayload(); }

        public void Pack(string context, string name, object value, MetricTags tags)
        {
            _payload?.Add(new GraphitePoint(context, name, new Dictionary<string, object> { { "value", value } }, tags, _timestamp));
        }

        public void Pack(
            string context,
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);

            _payload?.Add(new GraphitePoint(context, name, fields, tags, _timestamp));
        }

        public string PayloadFormatted()
        {
            return _payload.Format(_metricNameFormatter);
        }
    }
}