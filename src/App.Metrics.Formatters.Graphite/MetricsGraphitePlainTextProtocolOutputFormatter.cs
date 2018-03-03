// <copyright file="MetricsGraphitePlainTextProtocolOutputFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if !NETSTANDARD1_6
using App.Metrics.Internal;
#endif
using App.Metrics.Serialization;

namespace App.Metrics.Formatters.Graphite
{
    public class MetricsGraphitePlainTextProtocolOutputFormatter : IMetricsOutputFormatter
    {
        private readonly MetricsGraphitePlainTextProtocolOptions _options;

        public MetricsGraphitePlainTextProtocolOutputFormatter()
        {
            _options = new MetricsGraphitePlainTextProtocolOptions();
        }

        public MetricsGraphitePlainTextProtocolOutputFormatter(MetricsGraphitePlainTextProtocolOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public MetricsMediaTypeValue MediaType => new MetricsMediaTypeValue("text", "vnd.appmetrics.metrics.graphite", "v1", "plain");

        /// <inheritdoc />
        public Task WriteAsync(
            Stream output,
            MetricsDataValueSource metricsData,
            CancellationToken cancellationToken = default)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var serializer = new MetricSnapshotSerializer();

            using (var streamWriter = new StreamWriter(output))
            {
                using (var textWriter = new MetricSnapshotGraphitePlainTextProtocolWriter(
                    streamWriter,
                    _options.MetricPointTextWriter,
                    _options.MetricNameMapping))
                {
                    serializer.Serialize(textWriter, metricsData);
                }
            }

#if !NETSTANDARD1_6
            return AppMetricsTaskHelper.CompletedTask();
#else
            return Task.CompletedTask;
#endif
        }
    }
}
