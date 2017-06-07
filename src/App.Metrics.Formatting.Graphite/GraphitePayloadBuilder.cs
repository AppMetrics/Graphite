// <copyright file="GraphitePayloadBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.Tagging;
using FormattingConstants = App.Metrics.Formatting.Graphite.Constants;

namespace App.Metrics.Formatting.Graphite
{
    public class GraphitePayloadBuilder : IMetricPayloadBuilder<GraphitePayload>
    {
        private readonly Func<string, string, string> _metricNameFormatter;
        private readonly DateTime? _timestamp;
        private GraphitePayload _payload;

        public GraphitePayloadBuilder(
            Func<string, string, string> metricNameFormatter = null,
            MetricValueDataKeys dataKeys = null,
            DateTime? timestamp = null)
        {
            _timestamp = timestamp;
            _payload = new GraphitePayload();
            _metricNameFormatter = metricNameFormatter ?? FormattingConstants.GraphiteDefaults.MetricNameFormatter;
            DataKeys = dataKeys ?? new MetricValueDataKeys(
                           FormattingConstants.GraphiteDefaults.CustomHistogramDataKeys,
                           FormattingConstants.GraphiteDefaults.CustomMeterDataKeys,
                           FormattingConstants.GraphiteDefaults.CustomApdexKeys,
                           FormattingConstants.GraphiteDefaults.CustomCounterDataKeys);
        }

        /// <inheritdoc />
        public MetricValueDataKeys DataKeys { get; }

        public void Clear() { _payload = null; }

        public void Init() { _payload = new GraphitePayload(); }

        public void Pack(string context, string name, object value, MetricTags tags)
        {
            var measurement = _metricNameFormatter(context, name);
            _payload?.Add(new GraphitePoint(measurement, new Dictionary<string, object> { { "value", value } }, tags, _timestamp));
        }

        public void Pack(
            string context,
            string name,
            IEnumerable<string> columns,
            IEnumerable<object> values,
            MetricTags tags)
        {
            var fields = columns.Zip(values, (column, data) => new { column, data }).ToDictionary(pair => pair.column, pair => pair.data);

            var measurement = _metricNameFormatter(context, name);

            _payload?.Add(new GraphitePoint(measurement, fields, tags, _timestamp));
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