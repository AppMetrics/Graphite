// Copyright (c) Allan Hardy. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using FluentAssertions;
using Xunit;

namespace App.Metrics.Extensions.Reporting.Graphite.Facts.Client
{
    // ReSharper disable InconsistentNaming
    public class GraphiteSettingsTests
        // ReSharper restore InconsistentNaming
    {
        [Fact]
        public void base_address_cannot_be_null()
        {
            Action action = () =>
            {
                var settings = new GraphiteSettings(null);
            };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData("net.tcp://localhost", Protocol.Tcp)]
        [InlineData("net.udp://localhost", Protocol.Udp)]
        [InlineData("net.pickled://localhost", Protocol.Pickled)]
        public void can_determine_protocol(string address, Protocol expected)
        {
            var settings = new GraphiteSettings(new Uri(address));

            settings.Protocol.Should().Be(expected);
        }
    }
}