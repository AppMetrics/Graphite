// <copyright file="GraphiteReporterSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using App.Metrics.Reporting;

namespace App.Metrics.Extensions.Reporting.Graphite
{
    // ReSharper disable InconsistentNaming
    public class GraphiteReporterSettings : IReporterSettings
        // ReSharper restore InconsistentNaming
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GraphiteReporterSettings" /> class.
        /// </summary>
        public GraphiteReporterSettings()
        {
            GraphiteSettings = new GraphiteSettings();

            var customCounterDataKeys = new Dictionary<CounterValueDataKeys, string>
                                        {
                                            { CounterValueDataKeys.Total, "Total" },
                                            { CounterValueDataKeys.MetricSetItemSuffix, "-SetItem" },
                                            { CounterValueDataKeys.SetItemPercent, "Percent" }
                                        };

            var customHistogramDataKeys = new Dictionary<HistogramValueDataKeys, string>
                                          {
                                              { HistogramValueDataKeys.Count, "Count" },
                                              { HistogramValueDataKeys.UserLastValue, "User-Last" },
                                              { HistogramValueDataKeys.UserMinValue, "User-Min" },
                                              { HistogramValueDataKeys.UserMaxValue, "User-Max" },
                                              { HistogramValueDataKeys.LastValue, "Last" },
                                              { HistogramValueDataKeys.Min, "Min" },
                                              { HistogramValueDataKeys.Max, "Max" },
                                              { HistogramValueDataKeys.Mean, "Mean" },
                                              { HistogramValueDataKeys.Median, "Median" },
                                              { HistogramValueDataKeys.P75, "Percentile-75" },
                                              { HistogramValueDataKeys.P95, "Percentile-95" },
                                              { HistogramValueDataKeys.P98, "Percentile-98" },
                                              { HistogramValueDataKeys.P99, "Percentile-99" },
                                              { HistogramValueDataKeys.P999, "Percentile-999" },
                                              { HistogramValueDataKeys.Samples, "Samples" },
                                              { HistogramValueDataKeys.StdDev, "StdDev" },
                                              { HistogramValueDataKeys.Sum, "Sum" }
                                          };

            var customMeterDataKeys = new Dictionary<MeterValueDataKeys, string>
                                      {
                                          { MeterValueDataKeys.Count, "Total" },
                                          { MeterValueDataKeys.RateMean, "Rate-Mean" },
                                          { MeterValueDataKeys.SetItemPercent, "Percent" },
                                          { MeterValueDataKeys.MetricSetItemSuffix, "-SetItem" },
                                          { MeterValueDataKeys.Rate1M, "Rate-1-Min" },
                                          { MeterValueDataKeys.Rate5M, "Rate-5-Min" },
                                          { MeterValueDataKeys.Rate15M, "Rate-15-Min" }
                                      };

            var customApdexKeys = new Dictionary<ApdexValueDataKeys, string>
                                  {
                                      { ApdexValueDataKeys.Frustrating, "Frustrating" },
                                      { ApdexValueDataKeys.Samples, "Samples" },
                                      { ApdexValueDataKeys.Satisfied, "Satisfied" },
                                      { ApdexValueDataKeys.Score, "Score" },
                                      { ApdexValueDataKeys.Tolerating, "Tolerating" }
                                  };

            DataKeys = new MetricValueDataKeys(histogram: customHistogramDataKeys, meter: customMeterDataKeys, counter: customCounterDataKeys, apdex: customApdexKeys);

            HttpPolicy = new HttpPolicy
                         {
                             FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                             BackoffPeriod = Constants.DefaultBackoffPeriod,
                             Timeout = Constants.DefaultTimeout
                         };
            ReportInterval = TimeSpan.FromSeconds(5);
            MetricNameFormatter = (metricContext, metricName) => metricContext.IsMissing()
                ? $"{metricName}".Replace(' ', '_').Replace('.', '_')
                : $"{metricContext}.{metricName.Replace(' ', '_').Replace('.', '_')}".Replace(' ', '_');
        }

        /// <inheritdoc />
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public MetricValueDataKeys DataKeys { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        /// <summary>
        ///     Gets or sets the influx database settings.
        /// </summary>
        /// <value>
        ///     The influx database settings.
        /// </value>
        public GraphiteSettings GraphiteSettings { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP policy settings which allows circuit breaker configuration to be adjusted
        /// </summary>
        /// <value>
        ///     The HTTP policy.
        /// </value>
        public HttpPolicy HttpPolicy { get; set; }

        /// <summary>
        ///     Gets or sets the metric name formatter func which takes the metric context and name and returns a formatted string
        ///     which will be reported to influx as the measurement
        /// </summary>
        /// <value>
        ///     The metric name formatter.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public Func<string, string, string> MetricNameFormatter { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        /// <summary>
        ///     Gets or sets the report interval for which to flush metrics to Graphite.
        /// </summary>
        /// <value>
        ///     The report interval.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public TimeSpan ReportInterval { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
    }
}