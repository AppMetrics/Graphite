// <copyright file="MetricsGraphiteDocumentFormattingOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;

namespace App.Metrics.Formatters.Graphite
{
    /// <summary>
    ///     Provides programmatic configuration for Graphite document formatting options in the App Metrics framework.
    /// </summary>
    public class MetricsGraphiteDocumentFormattingOptions
    {
        public MetricsGraphiteDocumentFormattingOptions()
        {
            MetricNameMapping = new GeneratedMetricNameMapping(
                histogram: GraphiteFormatterConstants.GraphiteDefaults.CustomHistogramDataKeys,
                meter: GraphiteFormatterConstants.GraphiteDefaults.CustomMeterDataKeys);
            MetricTagFormatter = GraphiteFormatterConstants.GraphiteDefaults.MetricTagValueFormatter;
            MetricNameFormatter = GraphiteFormatterConstants.GraphiteDefaults.MetricNameFormatter;
        }

        public DefaultGraphiteNameFormatter MetricNameFormatter { get; set; }

        public Func<string, string> MetricTagFormatter { get; set; }

        public GeneratedMetricNameMapping MetricNameMapping { get; set; }
    }
}
