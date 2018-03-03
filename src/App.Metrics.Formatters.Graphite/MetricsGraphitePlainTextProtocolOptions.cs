// <copyright file="MetricsGraphitePlainTextProtocolOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Formatters.Graphite.Internal;

namespace App.Metrics.Formatters.Graphite
{
    /// <summary>
    ///     Provides programmatic configuration for Graphite document formatting options in the App Metrics framework.
    /// </summary>
    public class MetricsGraphitePlainTextProtocolOptions
    {
        public MetricsGraphitePlainTextProtocolOptions()
        {
            MetricNameMapping = new GeneratedMetricNameMapping(
                GraphiteFormatterConstants.GraphiteDefaults.CustomHistogramDataKeys,
                GraphiteFormatterConstants.GraphiteDefaults.CustomMeterDataKeys,
                GraphiteFormatterConstants.GraphiteDefaults.CustomApdexKeys,
                GraphiteFormatterConstants.GraphiteDefaults.CustomCounterDataKeys);
            MetricPointTextWriter = GraphiteFormatterConstants.GraphiteDefaults.MetricPointTextWriter;
        }

        public IGraphitePointTextWriter MetricPointTextWriter { get; set; }

        public GeneratedMetricNameMapping MetricNameMapping { get; set; }
    }
}
