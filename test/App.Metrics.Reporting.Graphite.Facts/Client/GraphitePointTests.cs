// <copyright file="GraphitePointTests.cs" company="Allan Hardy">
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
    public class GraphitePointTests
    {
        [Fact]
        public void At_least_one_field_is_required()
        {
            var fields = new Dictionary<string, object>();
            Action action = () =>
            {
                var point = new GraphitePoint(null, "measurement", fields, MetricTags.Empty);
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Can_format_payload_correctly()
        {
            var nameFormatter = new DefaultGraphiteNameFormatter();
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var point = new GraphitePoint(null, "measurement", fields, MetricTags.Empty, timestamp);

            var paload = new GraphitePayload { point };

            paload.Format(nameFormatter).Should().Be("measurement.key value 1483232461\n");
        }

        [Fact]
        public void Can_format_payload_with_multiple_fields_correctly()
        {
            var nameFormatter = new DefaultGraphiteNameFormatter();
            var fields = new Dictionary<string, object>
                         {
                             { "field1key", "field1value" },
                             { "field2key", 2 },
                             { "field3key", false }
                         };
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var point = new GraphitePoint(null, "measurement", fields, MetricTags.Empty, timestamp);

            var paload = new GraphitePayload { point };

            paload.Format(nameFormatter).Should()
                      .Be("measurement.field1key field1value 1483232461\nmeasurement.field2key 2 1483232461\nmeasurement.field3key f 1483232461\n");
        }

        [Fact]
        public void Can_format_payload_with_tags_correctly()
        {
            var nameFormatter = new DefaultGraphiteNameFormatter();
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var tags = new MetricTags("tagkey", "tagvalue");
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, DateTimeKind.Utc);
            var point = new GraphitePoint(null, "measurement", fields, tags, timestamp);

            var paload = new GraphitePayload { point };

            paload.Format(nameFormatter).Should().Be("measurement.tagkey.tagvalue.key value 1483232461\n");
        }

        [Fact]
        public void Field_key_cannot_be_empty()
        {
            var fields = new Dictionary<string, object> { { string.Empty, "value" } };
            Action action = () =>
            {
                var point = new GraphitePoint(null, "measurement", fields, MetricTags.Empty);
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Measurement_is_required()
        {
            var fields = new Dictionary<string, object> { { "key", "value" } };
            Action action = () =>
            {
                var point = new GraphitePoint(null, string.Empty, fields, MetricTags.Empty);
            };

            action.ShouldThrow<ArgumentException>();
        }

        [Theory]
        [InlineData(DateTimeKind.Unspecified, false)]
        [InlineData(DateTimeKind.Local, false)]
        [InlineData(DateTimeKind.Utc, true)]
        public void Time_stamp_should_be_utc(DateTimeKind dateTimeKind, bool expected)
        {
            var fields = new Dictionary<string, object> { { "key", "value" } };
            var timestamp = new DateTime(2017, 1, 1, 1, 1, 1, dateTimeKind);

            Action action = () =>
            {
                var point = new GraphitePoint(null, "measurement", fields, MetricTags.Empty, timestamp);
            };

            if (!expected)
            {
                action.ShouldThrow<ArgumentException>();
            }
            else
            {
                action.ShouldNotThrow();
            }
        }
    }
}