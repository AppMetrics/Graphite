// <copyright file="MetricsReportingGraphiteOptions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Filters;
using App.Metrics.Formatters;

namespace App.Metrics.Reporting.Graphite
{
    public class MetricsReportingGraphiteOptions
    {
        public MetricsReportingGraphiteOptions()
        {
            Graphite = new GraphiteOptions();
            HttpPolicy = new HttpPolicy();
            FlushInterval = TimeSpan.FromSeconds(5);
        }

        public TimeSpan FlushInterval { get; set; }

        public GraphiteOptions Graphite { get; set; }

        public IMetricsOutputFormatter MetricsOutputFormatter { get; set; }

        public IFilterMetrics Filter { get; set; }

        public HttpPolicy HttpPolicy { get; set; }
    }
}