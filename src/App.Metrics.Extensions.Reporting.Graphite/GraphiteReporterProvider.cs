// <copyright file="GraphiteReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using App.Metrics.Internal;
using App.Metrics.Reporting;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.Graphite
{
    public class GraphiteReporterProvider : IReporterProvider
    {
        private readonly GraphiteReporterSettings _settings;

        public GraphiteReporterProvider(GraphiteReporterSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = new NoOpMetricsFilter();
        }

        public GraphiteReporterProvider(GraphiteReporterSettings settings, IFilterMetrics fitler)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Filter = fitler ?? new NoOpMetricsFilter();
        }

        public IFilterMetrics Filter { get; }

        public IMetricReporter CreateMetricReporter(string name, ILoggerFactory loggerFactory)
        {
            var graphtieClient = new GraphiteClient(
                loggerFactory,
                _settings.GraphiteSettings,
                _settings.HttpPolicy);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys);

            return new ReportRunner<GraphitePayload>(
                async p =>
                {
                    var result = await graphtieClient.WriteAsync(p.Payload());
                    return result.Success;
                },
                payloadBuilder,
                _settings.ReportInterval,
                name,
                loggerFactory);
        }
    }
}