// <copyright file="GraphiteReporterExtensionsTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Core.Filtering;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts.Extensions
{
    public class GraphiteReporterExtensionsTests
    {
        [Fact]
        public void Can_add_graphite_provider_with_custom_settings()
        {
            var factory = SetupReportFactory();
            var settings = new GraphiteReporterSettings
                           {
                               HttpPolicy = new HttpPolicy
                                            {
                                                BackoffPeriod = TimeSpan.FromMinutes(1)
                                            }
                           };
            Action action = () => { factory.AddGraphite(settings); };

            action.ShouldNotThrow();
        }

        [Fact]
        public void Can_add_graphite_provider_with_custom_settings_and_filter()
        {
            var factory = SetupReportFactory();

            var settings = new GraphiteReporterSettings
            {
                               HttpPolicy = new HttpPolicy
                                            {
                                                BackoffPeriod = TimeSpan.FromMinutes(1)
                                            }
                           };
            Action action = () => { factory.AddGraphite(settings, new DefaultMetricsFilter()); };

            action.ShouldNotThrow();
        }

        [Fact]
        public void Can_add_graphite_provider_with_filter()
        {
            var factory = SetupReportFactory();

            Action action = () => { factory.AddGraphite(new Uri("net.tcp://localhost"), new DefaultMetricsFilter()); };

            action.ShouldNotThrow();
        }

        [Fact]
        public void Can_add_graphite_provider_without_filter()
        {
            var factory = SetupReportFactory();

            Action action = () => { factory.AddGraphite(new Uri("net.tcp://localhost")); };

            action.ShouldNotThrow();
        }

        private static ReportFactory SetupReportFactory()
        {
            var metricsMock = new Mock<IMetrics>();
            return new ReportFactory(metricsMock.Object, new LoggerFactory());
        }
    }
}