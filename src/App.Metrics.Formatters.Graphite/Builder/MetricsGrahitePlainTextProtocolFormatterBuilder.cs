// <copyright file="MetricsGrahitePlainTextProtocolFormatterBuilder.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Graphite;

// ReSharper disable CheckNamespace
namespace App.Metrics
    // ReSharper restore CheckNamespace
{
    public static class MetricsGrahitePlainTextProtocolFormatterBuilder
    {
        /// <summary>
        ///     Add the <see cref="MetricsGraphitePlainTextProtocolOutputFormatter" /> allowing metrics to optionally be reported to
        ///     Graphite using the Plain Text Protocol.
        /// </summary>
        /// <param name="metricFormattingBuilder">s
        ///     The <see cref="IMetricsOutputFormattingBuilder" /> used to configure InfluxDB Lineprotocol formatting
        ///     options.
        /// </param>
        /// <param name="setupAction">The InfluxDB LineProtocol formatting options to use.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder AsGraphitePlainTextProtocol(
            this IMetricsOutputFormattingBuilder metricFormattingBuilder,
            Action<MetricsGraphitePlainTextProtocolOptions> setupAction)
        {
            if (metricFormattingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricFormattingBuilder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            var options = new MetricsGraphitePlainTextProtocolOptions();

            setupAction.Invoke(options);

            var formatter = new MetricsGraphitePlainTextProtocolOutputFormatter(options);

            return metricFormattingBuilder.Using(formatter, false);
        }

        /// <summary>
        ///     Add the <see cref="MetricsGraphitePlainTextProtocolOutputFormatter" /> allowing metrics to optionally be reported to
        ///     Graphite using the Plain Text Protocol.
        /// </summary>
        /// <param name="metricFormattingBuilder">s
        ///     The <see cref="IMetricsOutputFormattingBuilder" /> used to configure InfluxDB Lineprotocol formatting
        ///     options.
        /// </param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder AsGraphitePlainTextProtocol(this IMetricsOutputFormattingBuilder metricFormattingBuilder)
        {
            if (metricFormattingBuilder == null)
            {
                throw new ArgumentNullException(nameof(metricFormattingBuilder));
            }

            var formatter = new MetricsGraphitePlainTextProtocolOutputFormatter();

            return metricFormattingBuilder.Using(formatter, false);
        }
    }
}