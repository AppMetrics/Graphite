// <copyright file="GraphiteReporterExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Abstractions.Filtering;
using App.Metrics.Extensions.Reporting.Graphite;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using App.Metrics.Reporting.Abstractions;

// ReSharper disable CheckNamespace
namespace App.Metrics.Reporting.Interfaces
{
    // ReSharper restore CheckNamespace
    public static class GraphiteReporterExtensions
    {
        public static IReportFactory AddGraphite(
            this IReportFactory factory,
            GraphiteReporterSettings settings,
            IFilterMetrics filter = null)
        {
            factory.AddProvider(new GraphiteReporterProvider(settings, filter));
            return factory;
        }

        public static IReportFactory AddGraphite(
            this IReportFactory factory,
            Uri baseAddress,
            IFilterMetrics filter = null)
        {
            var settings = new GraphiteReporterSettings
                           {
                               GraphiteSettings = new GraphiteSettings(baseAddress)
                           };

            factory.AddGraphite(settings, filter);
            return factory;
        }
    }
}