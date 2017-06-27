// <copyright file="GraphiteSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatting.Graphite;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    // ReSharper disable InconsistentNaming
    public class GraphiteSettings
        // ReSharper restore InconsistentNaming
    {
        public GraphiteSettings(Uri baseAddress)
            : this()
        {
            BaseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress));
        }

        internal GraphiteSettings(IGraphiteNameFormatter nameFormatter = null) { MetricNameFormatter = nameFormatter ?? new DefaultGraphiteNameFormatter(); }

        /// <summary>
        ///     Gets the Graphite host.
        /// </summary>
        /// <value>
        ///     The Graphite host.
        /// </value>
        public Uri BaseAddress { get; }

        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public int BatchSize { get; set; } = Constants.DefaultBatchSize;
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        /// <summary>
        ///     Gets the number of Graphite notes that must confirm the write
        /// </summary>
        /// <value>
        ///     The Graphite node write consistency.
        /// </value>
        /// <exception cref="System.ArgumentException">
        ///     Graphite URI scheme must be either net.tcp or net.udp or net.pickled - BaseAddress
        /// </exception>
        // ReSharper disable UnusedMember.Global
        public Protocol Protocol
            // ReSharper restore UnusedMember.Global
        {
            get
            {
                switch (BaseAddress.Scheme.ToLowerInvariant())
                {
                    case "net.tcp":
                        return Protocol.Tcp;
                    case "net.udp":
                        return Protocol.Udp;
                    case "net.pickled":
                        return Protocol.Pickled;
                    default:
                        throw new ArgumentException("Graphite URI scheme must be either net.tcp or net.udp or net.pickled", nameof(BaseAddress));
                }
            }
        }

        /// <summary>
        ///     Gets or sets the metric name formatter func which takes the metric context and name and returns a formatted string
        ///     which will be reported to influx as the measurement
        /// </summary>
        /// <value>
        ///     The metric name formatter.
        /// </value>
        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        // ReSharper disable MemberCanBePrivate.Global
        public IGraphiteNameFormatter MetricNameFormatter { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global
    }
}