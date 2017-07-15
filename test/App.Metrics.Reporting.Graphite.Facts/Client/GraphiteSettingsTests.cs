// <copyright file="GraphiteSettingsTests.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

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
        public void Base_address_cannot_be_null()
        {
            Action action = () =>
            {
                var settings = new GraphiteSettings((Uri)null);
            };

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData("net.tcp://localhost", Protocol.Tcp)]
        [InlineData("net.udp://localhost", Protocol.Udp)]
        [InlineData("net.pickled://localhost", Protocol.Pickled)]
        public void Can_determine_protocol(string address, Protocol expected)
        {
            var settings = new GraphiteSettings(new Uri(address));

            settings.Protocol.Should().Be(expected);
        }
    }
}