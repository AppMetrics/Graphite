// <copyright file="GraphiteSettings.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    // ReSharper disable InconsistentNaming
    public class GraphiteSettings
        // ReSharper restore InconsistentNaming
    {
        public GraphiteSettings(Uri baseAddress) { BaseAddress = baseAddress ?? throw new ArgumentNullException(nameof(baseAddress)); }

        internal GraphiteSettings() { }

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
                if (BaseAddress.Scheme.ToLowerInvariant() == "net.tcp")
                {
                    return Protocol.Tcp;
                }

                if (BaseAddress.Scheme.ToLowerInvariant() == "net.udp")
                {
                    return Protocol.Udp;
                }

                if (BaseAddress.Scheme.ToLowerInvariant() == "net.pickled")
                {
                    return Protocol.Pickled;
                }

                throw new ArgumentException("Graphite URI scheme must be either net.tcp or net.udp or net.pickled", nameof(BaseAddress));
            }
        }
    }
}