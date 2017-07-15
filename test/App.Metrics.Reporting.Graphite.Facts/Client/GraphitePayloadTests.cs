// <copyright file="GraphitePayloadTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using App.Metrics.Formatting.Graphite;
using App.Metrics.Formatting.Graphite.Extensions;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts.Client
{
    public class GraphitePayloadTests
    {
        [Fact]
        public void Can_format_payload()
        {
            var nameFormatter = new DefaultGraphiteNameFormatter();
            var payload = new GraphitePayload();
            var fieldsOne = new Dictionary<string, object> { { "key", "value" } };
            var timestampOne = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var pointOne = new GraphitePoint(null, "measurement", fieldsOne, MetricTags.Empty, timestampOne);

            var fieldsTwo = new Dictionary<string, object>
                            {
                                { "field1key", "field1value" },
                                { "field2key", 2 },
                                { "field3key", false }
                            };
            var timestampTwo = new DateTime(2017, 1, 2, 1, 1, 1, DateTimeKind.Utc);
            var pointTwo = new GraphitePoint(null, "measurement", fieldsTwo, MetricTags.Empty, timestampTwo);

            payload.Add(pointOne);
            payload.Add(pointTwo);

            payload.Format(nameFormatter).Should()
                      .Be(
                          "measurement.key value 1483232461\nmeasurement.field1key field1value 1483318861\nmeasurement.field2key 2 1483318861\nmeasurement.field3key f 1483318861\n");
        }

        [Fact]
        public void When_null_text_writer_ignore_and_dont_throw()
        {
            var payload = new GraphitePayload();
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var pointOne = new GraphitePoint(null, "measurement", fields, MetricTags.Empty);

            Action action = () =>
            {
                payload.Add(pointOne);
                payload.Format(null);
            };

            action.ShouldNotThrow();
        }
    }
}