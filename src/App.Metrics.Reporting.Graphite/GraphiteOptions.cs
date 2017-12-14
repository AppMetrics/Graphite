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
        public GraphiteOptions(Uri address, string indexName)
        {
            BaseAddress = address ?? throw new ArgumentNullException(nameof(address));
            Index = indexName ?? throw new ArgumentNullException(nameof(indexName));

            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentException("Cannot be empty", nameof(indexName));
            }
        }

        internal GraphiteOptions()
        {
        }

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

        public GraphiteAuthorizationSchemes AuthorizationSchema { get; set; }

        public string Index { get; set; }

        public Protocol Protocol { get; set; }

        public IGraphiteNameFormatter MetricNameFormatter { get; set; }
    }
}
