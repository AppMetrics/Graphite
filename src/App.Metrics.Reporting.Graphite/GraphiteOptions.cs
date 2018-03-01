// <copyright file="GraphiteOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Formatters.Graphite;
using App.Metrics.Reporting.Graphite.Client;

namespace App.Metrics.Reporting.Graphite
{
    public class GraphiteOptions
    {
        public GraphiteOptions(Uri address)
        {
            BaseUri = address ?? throw new ArgumentNullException(nameof(address));
        }

        public GraphiteOptions()
        {
        }

        public Uri BaseUri { get; set; }

        // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
        public int BatchSize { get; set; } = Constants.DefaultBatchSize;
        // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

        public GraphiteAuthorizationSchemes AuthorizationSchema { get; set; }

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
                switch (BaseUri.Scheme.ToLowerInvariant())
                {
                    case "net.tcp":
                        return Protocol.Tcp;
                    case "net.udp":
                        return Protocol.Udp;
                    case "net.pickled":
                        return Protocol.Pickled;
                    default:
                        throw new ArgumentException("Graphite URI scheme must be either net.tcp or net.udp or net.pickled", nameof(BaseUri));
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
        public IGraphiteNameFormatter MetricNameFormatter { get; set; }
    }
}
