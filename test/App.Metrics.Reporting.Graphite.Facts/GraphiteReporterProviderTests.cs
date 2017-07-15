// <copyright file="GraphiteReporterProviderTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using App.Metrics.Core.Filtering;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts
{
    public class GraphiteReporterProviderTests
    {
        [Fact]
        public void Can_create_metric_reporter()
        {
            var provider = new GraphiteReporterProvider(new GraphiteReporterSettings(), new DefaultMetricsFilter());

            var reporter = provider.CreateMetricReporter("graphite", new LoggerFactory());

            reporter.Should().NotBeNull();
        }

        [Fact]
        public void Defaults_filter_to_no_op()
        {
            var provider = new GraphiteReporterProvider(new GraphiteReporterSettings());

            provider.Filter.Should().BeOfType<NoOpMetricsFilter>();
        }

        [Fact]
        public void Filter_is_not_required()
        {
            Action action = () =>
            {
                var provider = new GraphiteReporterProvider(new GraphiteReporterSettings(), null);
                provider.Filter.Should().BeOfType<NoOpMetricsFilter>();
            };

            action.ShouldNotThrow();
        }

        [Fact]
        public void Settings_are_required()
        {
            Action action = () =>
            {
                var provider = new GraphiteReporterProvider(null);
            };

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}