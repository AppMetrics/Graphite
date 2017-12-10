// <copyright file="MetricsGraphiteOutputFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Internal;
using App.Metrics.Serialization;

namespace App.Metrics.Formatters.Graphite
{
    public class MetricsGraphiteOutputFormatter : IMetricsOutputFormatter
    {
        private readonly string _graphiteIndex;
        private readonly MetricsGraphiteDocumentFormattingOptions _options;

        public MetricsGraphiteOutputFormatter(string graphiteIndex)
        {
            if (string.IsNullOrEmpty(graphiteIndex))
            {
                throw new ArgumentNullException(nameof(graphiteIndex));
            }

            _graphiteIndex = graphiteIndex;
            _options = new MetricsGraphiteDocumentFormattingOptions();
        }

        public MetricsGraphiteOutputFormatter(
            string graphiteIndex,
            MetricsGraphiteDocumentFormattingOptions options)
        {
            if (string.IsNullOrEmpty(graphiteIndex))
            {
                throw new ArgumentNullException(nameof(graphiteIndex));
            }

            _graphiteIndex = graphiteIndex;
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public MetricsMediaTypeValue MediaType => default;

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
                using (var textWriter = new MetricSnapshotGraphiteWriter(
                    streamWriter,
                    _graphiteIndex,
                    _options.MetricNameFormatter,
                    _options.MetricTagFormatter,
                    _options.MetricNameMapping))
                {
                    serializer.Serialize(textWriter, metricsData);
                }
            }

            return AppMetricsTaskHelper.CompletedTask();
        }
    }
}
