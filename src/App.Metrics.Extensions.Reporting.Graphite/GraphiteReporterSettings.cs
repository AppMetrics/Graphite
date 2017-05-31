// <copyright file="GraphiteReporterSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
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
            HttpPolicy = new HttpPolicy();
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