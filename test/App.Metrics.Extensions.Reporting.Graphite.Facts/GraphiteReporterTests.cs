// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;
using App.Metrics.Abstractions.Reporting;
using App.Metrics.Abstractions.ReservoirSampling;
using App.Metrics.Apdex;
using App.Metrics.Core;
using App.Metrics.Counter;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Infrastructure;
using App.Metrics.Meter;
using App.Metrics.Reporting;
using App.Metrics.Reporting.Abstractions;
using App.Metrics.ReservoirSampling.ExponentialDecay;
using App.Metrics.Tagging;
using App.Metrics.Timer;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts
{
    public class GraphiteReporterTests
    {
        private const string MultidimensionalMetricNameSuffix = "|host:server1,env:staging";
        private readonly IReservoir _defaultReservoir = new DefaultForwardDecayingReservoir();
        private readonly MetricTags _tags = new MetricTags(new[] { "host", "env" }, new[] { "server1", "staging" });
        private static readonly DateTime Origin = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly GraphiteReporterSettings _settings = new GraphiteReporterSettings();

        [Fact]
        public void can_clear_payload()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var meter = new DefaultMeterMetric(clock);
            meter.Mark(new MetricSetItem("item1", "value1"), 1);
            meter.Mark(new MetricSetItem("item2", "value2"), 1);
            var meterValueSource = new MeterValueSource(
                "test meter",
                ConstantValue.Provider(meter.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys);
            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", meterValueSource);

            var sr = new StringWriter();
            payloadBuilder.Payload().Format(sr);
            sr.ToString().Should().NotBeNullOrWhiteSpace();

            payloadBuilder.Clear();

            payloadBuilder.Payload().Should().BeNull();
        }

        [Fact]
        public void can_report_apdex()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var gauge = new DefaultApdexMetric(_defaultReservoir, clock, false);
            var apdexValueSource = new ApdexValueSource(
                "test apdex",
                ConstantValue.Provider(gauge.Value),
                MetricTags.Empty,
                false);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);
            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", apdexValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("apdex.test.test_apdex.Samples 0 0\napdex.test.test_apdex.Score 0.00 0\napdex.test.test_apdex.Satisfied 0 0\napdex.test.test_apdex.Tolerating 0 0\napdex.test.test_apdex.Frustrating 0 0\n");
        }

        [Fact]
        public void can_report_apdex__when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var gauge = new DefaultApdexMetric(_defaultReservoir, clock, false);
            var apdexValueSource = new ApdexValueSource(
                "test apdex" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(gauge.Value),
                _tags,
                resetOnReporting: false);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", apdexValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be("env.staging.apdex.test.test_apdex.host.server1.Samples 0 0\nenv.staging.apdex.test.test_apdex.host.server1.Score 0.00 0\nenv.staging.apdex.test.test_apdex.host.server1.Satisfied 0 0\nenv.staging.apdex.test.test_apdex.host.server1.Tolerating 0 0\nenv.staging.apdex.test.test_apdex.host.server1.Frustrating 0 0\n");
        }

        [Fact]
        public void can_report_apdex_with_tags()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var gauge = new DefaultApdexMetric(_defaultReservoir, clock, false);
            var apdexValueSource = new ApdexValueSource(
                "test apdex",
                ConstantValue.Provider(gauge.Value),
                new MetricTags(new[] { "key1", "key2" }, new[] { "value1", "value2" }),
                false);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", apdexValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be("apdex.test.test_apdex.key1.value1.key2.value2.Samples 0 0\napdex.test.test_apdex.key1.value1.key2.value2.Score 0.00 0\napdex.test.test_apdex.key1.value1.key2.value2.Satisfied 0 0\napdex.test.test_apdex.key1.value1.key2.value2.Tolerating 0 0\napdex.test.test_apdex.key1.value1.key2.value2.Frustrating 0 0\n");
        }

        [Fact]
        public void can_report_apdex_with_tags_when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var gauge = new DefaultApdexMetric(_defaultReservoir, clock, false);
            var apdexValueSource = new ApdexValueSource(
                "test apdex" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(gauge.Value),
                MetricTags.Concat(_tags, new MetricTags("anothertag", "thevalue")),
                resetOnReporting: false);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", apdexValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "env.staging.apdex.test.test_apdex.host.server1.anothertag.thevalue.Samples 0 0\nenv.staging.apdex.test.test_apdex.host.server1.anothertag.thevalue.Score 0.00 0\nenv.staging.apdex.test.test_apdex.host.server1.anothertag.thevalue.Satisfied 0 0\nenv.staging.apdex.test.test_apdex.host.server1.anothertag.thevalue.Tolerating 0 0\nenv.staging.apdex.test.test_apdex.host.server1.anothertag.thevalue.Frustrating 0 0\n");
        }

        [Fact]
        public void can_report_counter_with_items()
        {
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(new MetricSetItem("item1", "value1"), 1);
            counter.Increment(new MetricSetItem("item2", "value2"), 1);
            var counterValueSource = new CounterValueSource(
                "test counter",
                ConstantValue.Provider(counter.Value),
                Unit.None,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "counter.test.test_counter-SetItem.item.item1_value1.Total 1 0\ncounter.test.test_counter-SetItem.item.item1_value1.Percent 50.00 0\ncounter.test.test_counter-SetItem.item.item2_value2.Total 1 0\ncounter.test.test_counter-SetItem.item.item2_value2.Percent 50.00 0\ncounter.test.test_counter.value 2 0\n");
        }

        [Fact]
        public void can_report_counter_with_items_and_tags()
        {
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(new MetricSetItem("item1", "value1"), 1);
            counter.Increment(new MetricSetItem("item2", "value2"), 1);
            var counterValueSource = new CounterValueSource(
                "test counter",
                ConstantValue.Provider(counter.Value),
                Unit.None,
                new MetricTags(new[] { "key1", "key2" }, new[] { "value1", "value2" }));
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "counter.test.test_counter-SetItem.key1.value1.key2.value2.item.item1_value1.Total 1 0\ncounter.test.test_counter-SetItem.key1.value1.key2.value2.item.item1_value1.Percent 50.00 0\ncounter.test.test_counter-SetItem.key1.value1.key2.value2.item.item2_value2.Total 1 0\ncounter.test.test_counter-SetItem.key1.value1.key2.value2.item.item2_value2.Percent 50.00 0\ncounter.test.test_counter.key1.value1.key2.value2.value 2 0\n");
        }

        [Fact]
        public void can_report_counter_with_items_tags_when_multidimensional()
        {
            var counterTags = new MetricTags(new[] { "key1", "key2" }, new[] { "value1", "value2" });
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(new MetricSetItem("item1", "value1"), 1);
            counter.Increment(new MetricSetItem("item2", "value2"), 1);
            var counterValueSource = new CounterValueSource(
                "test counter" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(counter.Value),
                Unit.None,
                MetricTags.Concat(_tags, counterTags));
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "env.staging.counter.test.test_counter-SetItem.host.server1.key1.value1.key2.value2.item.item1_value1.Total 1 0\nenv.staging.counter.test.test_counter-SetItem.host.server1.key1.value1.key2.value2.item.item1_value1.Percent 50.00 0\nenv.staging.counter.test.test_counter-SetItem.host.server1.key1.value1.key2.value2.item.item2_value2.Total 1 0\nenv.staging.counter.test.test_counter-SetItem.host.server1.key1.value1.key2.value2.item.item2_value2.Percent 50.00 0\nenv.staging.counter.test.test_counter.host.server1.key1.value1.key2.value2.value 2 0\n");
        }

        [Fact]
        public void can_report_counter_with_items_with_option_not_to_report_percentage()
        {
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(new MetricSetItem("item1", "value1"), 1);
            counter.Increment(new MetricSetItem("item2", "value2"), 1);
            var counterValueSource = new CounterValueSource(
                "test counter",
                ConstantValue.Provider(counter.Value),
                Unit.None,
                MetricTags.Empty,
                reportItemPercentages: false);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "counter.test.test_counter-SetItem.item.item1_value1.Total 1 0\ncounter.test.test_counter-SetItem.item.item2_value2.Total 1 0\ncounter.test.test_counter.value 2 0\n");
        }

        [Fact]
        public void can_report_counters()
        {
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(1);
            var counterValueSource = new CounterValueSource(
                "test counter",
                ConstantValue.Provider(counter.Value),
                Unit.None,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("counter.test.test_counter.value 1 0\n");
        }

        [Fact]
        public void can_report_counters__when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var counter = new DefaultCounterMetric();
            counter.Increment(1);
            var counterValueSource = new CounterValueSource(
                "test counter" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(counter.Value),
                Unit.None,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", counterValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("env.staging.counter.test.test_counter.host.server1.value 1 0\n");
        }

        [Fact]
        public void can_report_gauges()
        {
            var metricsMock = new Mock<IMetrics>();
            var gauge = new FunctionGauge(() => 1);
            var gaugeValueSource = new GaugeValueSource(
                "test gauge",
                ConstantValue.Provider(gauge.Value),
                Unit.None,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", gaugeValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("gauge.test.test_gauge.value 1.00 0\n");
        }

        [Fact]
        public void can_report_gauges__when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var gauge = new FunctionGauge(() => 1);
            var gaugeValueSource = new GaugeValueSource(
                "gauge-group" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(gauge.Value),
                Unit.None,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", gaugeValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("env.staging.gauge.test.gauge-group.host.server1.value 1.00 0\n");
        }

        [Fact]
        public void can_report_histograms()
        {
            var metricsMock = new Mock<IMetrics>();
            var histogram = new DefaultHistogramMetric(_defaultReservoir);
            histogram.Update(1000, "client1");
            var histogramValueSource = new HistogramValueSource(
                "test histogram",
                ConstantValue.Provider(histogram.Value),
                Unit.None,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", histogramValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "histogram.test.test_histogram.Samples 1 0\nhistogram.test.test_histogram.Last 1000.00 0\nhistogram.test.test_histogram.Count 1 0\nhistogram.test.test_histogram.Sum 1000.00 0\nhistogram.test.test_histogram.Min 1000.00 0\nhistogram.test.test_histogram.Max 1000.00 0\nhistogram.test.test_histogram.Mean 1000.00 0\nhistogram.test.test_histogram.Median 1000.00 0\nhistogram.test.test_histogram.StdDev 0.00 0\nhistogram.test.test_histogram.Percentile-999 1000.00 0\nhistogram.test.test_histogram.Percentile-99 1000.00 0\nhistogram.test.test_histogram.Percentile-98 1000.00 0\nhistogram.test.test_histogram.Percentile-95 1000.00 0\nhistogram.test.test_histogram.Percentile-75 1000.00 0\nhistogram.test.test_histogram.User-Last client1 0\nhistogram.test.test_histogram.User-Min client1 0\nhistogram.test.test_histogram.User-Max client1 0\n");
        }

        [Fact]
        public void can_report_histograms_when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var histogram = new DefaultHistogramMetric(_defaultReservoir);
            histogram.Update(1000, "client1");
            var histogramValueSource = new HistogramValueSource(
                "test histogram" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(histogram.Value),
                Unit.None,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", histogramValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "env.staging.histogram.test.test_histogram.host.server1.Samples 1 0\nenv.staging.histogram.test.test_histogram.host.server1.Last 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Count 1 0\nenv.staging.histogram.test.test_histogram.host.server1.Sum 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Min 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Max 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Mean 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Median 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.StdDev 0.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Percentile-999 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Percentile-99 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Percentile-98 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Percentile-95 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.Percentile-75 1000.00 0\nenv.staging.histogram.test.test_histogram.host.server1.User-Last client1 0\nenv.staging.histogram.test.test_histogram.host.server1.User-Min client1 0\nenv.staging.histogram.test.test_histogram.host.server1.User-Max client1 0\n");
        }

        [Fact]
        public void can_report_meters()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var meter = new DefaultMeterMetric(clock);
            meter.Mark(1);
            var meterValueSource = new MeterValueSource(
                "test meter",
                ConstantValue.Provider(meter.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", meterValueSource);

            payloadBuilder.PayloadFormatted().Should().Be("meter.test.test_meter.Total 1 0\nmeter.test.test_meter.Rate-1-Min 0.00 0\nmeter.test.test_meter.Rate-5-Min 0.00 0\nmeter.test.test_meter.Rate-15-Min 0.00 0\n");
        }

        [Fact]
        public void can_report_meters_when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var meter = new DefaultMeterMetric(clock);
            meter.Mark(1);
            var meterValueSource = new MeterValueSource(
                "test meter" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(meter.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", meterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be("env.staging.meter.test.test_meter.host.server1.Total 1 0\nenv.staging.meter.test.test_meter.host.server1.Rate-1-Min 0.00 0\nenv.staging.meter.test.test_meter.host.server1.Rate-5-Min 0.00 0\nenv.staging.meter.test.test_meter.host.server1.Rate-15-Min 0.00 0\n");
        }

        [Fact]
        public void can_report_meters_with_items()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var meter = new DefaultMeterMetric(clock);
            meter.Mark(new MetricSetItem("item1", "value1"), 1);
            meter.Mark(new MetricSetItem("item2", "value2"), 1);
            var meterValueSource = new MeterValueSource(
                "test meter",
                ConstantValue.Provider(meter.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", meterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "meter.test.test_meter-SetItem.item.item1_value1.Total 1 0\nmeter.test.test_meter-SetItem.item.item1_value1.Rate-1-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item1_value1.Rate-5-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item1_value1.Rate-15-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item1_value1.Percent 50.00 0\nmeter.test.test_meter-SetItem.item.item2_value2.Total 1 0\nmeter.test.test_meter-SetItem.item.item2_value2.Rate-1-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item2_value2.Rate-5-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item2_value2.Rate-15-Min 0.00 0\nmeter.test.test_meter-SetItem.item.item2_value2.Percent 50.00 0\nmeter.test.test_meter.Total 2 0\nmeter.test.test_meter.Rate-1-Min 0.00 0\nmeter.test.test_meter.Rate-5-Min 0.00 0\nmeter.test.test_meter.Rate-15-Min 0.00 0\n");
        }

        [Fact]
        public void can_report_meters_with_items_tags_when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var meter = new DefaultMeterMetric(clock);
            meter.Mark(new MetricSetItem("item1", "value1"), 1);
            meter.Mark(new MetricSetItem("item2", "value2"), 1);
            var meterValueSource = new MeterValueSource(
                "test meter" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(meter.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", meterValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "env.staging.meter.test.test_meter-SetItem.host.server1.item.item1_value1.Total 1 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item1_value1.Rate-1-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item1_value1.Rate-5-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item1_value1.Rate-15-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item1_value1.Percent 50.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item2_value2.Total 1 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item2_value2.Rate-1-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item2_value2.Rate-5-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item2_value2.Rate-15-Min 0.00 0\nenv.staging.meter.test.test_meter-SetItem.host.server1.item.item2_value2.Percent 50.00 0\nenv.staging.meter.test.test_meter.host.server1.Total 2 0\nenv.staging.meter.test.test_meter.host.server1.Rate-1-Min 0.00 0\nenv.staging.meter.test.test_meter.host.server1.Rate-5-Min 0.00 0\nenv.staging.meter.test.test_meter.host.server1.Rate-15-Min 0.00 0\n");
        }

        [Fact]
        public void can_report_timers()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var timer = new DefaultTimerMetric(_defaultReservoir, clock);
            timer.Record(1000, TimeUnit.Milliseconds, "client1");
            var timerValueSource = new TimerValueSource(
                "test timer",
                ConstantValue.Provider(timer.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                TimeUnit.Milliseconds,
                MetricTags.Empty);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", timerValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "timer.test.test_timer.Total 1 0\ntimer.test.test_timer.Rate-1-Min 0.00 0\ntimer.test.test_timer.Rate-5-Min 0.00 0\ntimer.test.test_timer.Rate-15-Min 0.00 0\ntimer.test.test_timer.Samples 1 0\ntimer.test.test_timer.Last 1000.00 0\ntimer.test.test_timer.Count 1 0\ntimer.test.test_timer.Sum 1000.00 0\ntimer.test.test_timer.Min 1000.00 0\ntimer.test.test_timer.Max 1000.00 0\ntimer.test.test_timer.Mean 1000.00 0\ntimer.test.test_timer.Median 1000.00 0\ntimer.test.test_timer.StdDev 0.00 0\ntimer.test.test_timer.Percentile-999 1000.00 0\ntimer.test.test_timer.Percentile-99 1000.00 0\ntimer.test.test_timer.Percentile-98 1000.00 0\ntimer.test.test_timer.Percentile-95 1000.00 0\ntimer.test.test_timer.Percentile-75 1000.00 0\ntimer.test.test_timer.User-Last client1 0\ntimer.test.test_timer.User-Min client1 0\ntimer.test.test_timer.User-Max client1 0\n");
        }

        [Fact]
        public void can_report_timers__when_multidimensional()
        {
            var metricsMock = new Mock<IMetrics>();
            var clock = new TestClock();
            var timer = new DefaultTimerMetric(_defaultReservoir, clock);
            timer.Record(1000, TimeUnit.Milliseconds, "client1");
            var timerValueSource = new TimerValueSource(
                "test timer" + MultidimensionalMetricNameSuffix,
                ConstantValue.Provider(timer.Value),
                Unit.None,
                TimeUnit.Milliseconds,
                TimeUnit.Milliseconds,
                _tags);
            var payloadBuilder = new GraphitePayloadBuilder(_settings.MetricNameFormatter, _settings.DataKeys, Origin);

            var reporter = CreateReporter(payloadBuilder);

            reporter.StartReportRun(metricsMock.Object);
            reporter.ReportMetric("test", timerValueSource);

            payloadBuilder.PayloadFormatted().
                           Should().
                           Be(
                               "env.staging.timer.test.test_timer.host.server1.Total 1 0\nenv.staging.timer.test.test_timer.host.server1.Rate-1-Min 0.00 0\nenv.staging.timer.test.test_timer.host.server1.Rate-5-Min 0.00 0\nenv.staging.timer.test.test_timer.host.server1.Rate-15-Min 0.00 0\nenv.staging.timer.test.test_timer.host.server1.Samples 1 0\nenv.staging.timer.test.test_timer.host.server1.Last 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Count 1 0\nenv.staging.timer.test.test_timer.host.server1.Sum 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Min 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Max 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Mean 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Median 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.StdDev 0.00 0\nenv.staging.timer.test.test_timer.host.server1.Percentile-999 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Percentile-99 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Percentile-98 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Percentile-95 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.Percentile-75 1000.00 0\nenv.staging.timer.test.test_timer.host.server1.User-Last client1 0\nenv.staging.timer.test.test_timer.host.server1.User-Min client1 0\nenv.staging.timer.test.test_timer.host.server1.User-Max client1 0\n");
        }

        [Fact]
        public async Task on_end_report_clears_playload()
        {
            var metricsMock = new Mock<IMetrics>();
            var payloadBuilderMock = new Mock<IMetricPayloadBuilder<GraphitePayload>>();
            payloadBuilderMock.Setup(p => p.Clear());
            var reporter = CreateReporter(payloadBuilderMock.Object);

            await reporter.EndAndFlushReportRunAsync(metricsMock.Object).ConfigureAwait(false);

            payloadBuilderMock.Verify(p => p.Clear(), Times.Once);
        }

        [Fact]
        public void when_disposed_clears_playload()
        {
            var payloadBuilderMock = new Mock<IMetricPayloadBuilder<GraphitePayload>>();
            payloadBuilderMock.Setup(p => p.Clear());
            var reporter = CreateReporter(payloadBuilderMock.Object);

            reporter.Dispose();

            payloadBuilderMock.Verify(p => p.Clear(), Times.Once);
        }

        private static IMetricReporter CreateReporter(IMetricPayloadBuilder<GraphitePayload> payloadBuilder)
        {
            var reportInterval = TimeSpan.FromSeconds(1);
            var loggerFactory = new LoggerFactory();

            return new ReportRunner<GraphitePayload>(
                p => Task.FromResult(true),
                payloadBuilder,
                reportInterval,
                "Graphite Reporter",
                loggerFactory);
        }
    }
}