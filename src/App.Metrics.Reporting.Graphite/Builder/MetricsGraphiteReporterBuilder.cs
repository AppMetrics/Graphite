// <copyright file="GraphiteReporterProvider.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Net.Http;
using App.Metrics.Builder;
using App.Metrics.Reporting.Graphite;
using App.Metrics.Reporting.Graphite.Client;

namespace App.Metrics
{
    public static class MetricsGraphiteReporterBuilder
    {
        public static IMetricsBuilder ToGraphite(
            this IMetricsReportingBuilder metricsReportingBuilder,
            Action<MetricsReportingGraphiteOptions> setupAction)
        {
            if (metricsReportingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricsReportingBuilder));
            }

            var options = new MetricsReportingGraphiteOptions();

            setupAction?.Invoke(options);

            var httpClient = CreateClient(options.Graphite, options.HttpPolicy);
            var reporter = new GraphiteReporter(options, httpClient);

            return metricsReportingBuilder.Using(reporter);
        }

        internal static IGraphiteClient CreateClient(
            GraphiteOptions options,
            HttpPolicy httpPolicy)
        {
            return new DefaultGraphiteClient(options, httpPolicy);
        }
    }
}