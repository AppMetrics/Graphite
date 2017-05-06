// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Filtering;
using App.Metrics.Internal;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts
{
    public class GraphiteReporterProviderTests
    {
        [Fact]
        public void can_create_metric_reporter()
        {
            var provider = new GraphiteReporterProvider(new GraphiteReporterSettings(), new DefaultMetricsFilter());

            var reporter = provider.CreateMetricReporter("graphite", new LoggerFactory());

            reporter.Should().NotBeNull();
        }

        [Fact]
        public void defaults_filter_to_no_op()
        {
            var provider = new GraphiteReporterProvider(new GraphiteReporterSettings());

            provider.Filter.Should().BeOfType<NoOpMetricsFilter>();
        }

        [Fact]
        public void filter_is_not_required()
        {
            Action action = () =>
            {
                var provider = new GraphiteReporterProvider(new GraphiteReporterSettings(), null);
                provider.Filter.Should().BeOfType<NoOpMetricsFilter>();
            };

            action.ShouldNotThrow();
        }

        [Fact]
        public void settings_are_required()
        {
            Action action = () =>
            {
                var provider = new GraphiteReporterProvider(null);
            };

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}