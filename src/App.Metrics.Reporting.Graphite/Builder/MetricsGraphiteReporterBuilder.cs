// <copyright file="MetricsGraphiteReporterBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Builder;
using App.Metrics.Reporting.Graphite;
using App.Metrics.Reporting.Graphite.Client;

// ReSharper disable CheckNamespace
namespace App.Metrics
    // ReSharper restore CheckNamespace
{
    public static class MetricsGraphiteReporterBuilder
    {
        public static IMetricsBuilder ToGraphite(
            this IMetricsReportingBuilder metricsReportingBuilder,
            MetricsReportingGraphiteOptions options)
        {
            if (metricsReportingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricsReportingBuilder));
            }

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