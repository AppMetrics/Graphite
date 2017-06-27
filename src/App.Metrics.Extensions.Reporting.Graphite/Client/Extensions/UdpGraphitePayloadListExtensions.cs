// <copyright file="UdpGraphitePayloadListExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Extensions.Reporting.Graphite;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using App.Metrics.Formatting.Graphite;
using App.Metrics.Formatting.Graphite.Extensions;
using Microsoft.Extensions.Logging;

// ReSharper disable CheckNamespace
namespace System.Collections.Generic
    // ReSharper restore CheckNamespace
{
    public static class UdpGraphitePayloadListExtensions
    {
        private static UdpClient _client;

        public static async Task UdpWriteAsync(
            this List<GraphitePayload> batches,
            GraphiteSettings graphiteSettings,
            HttpPolicy httpPolicy,
            ILogger<GraphiteClient> logger,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await CreateClient(graphiteSettings, httpPolicy);

            var currentBatch = 1;

            foreach (var batch in batches)
            {
                var text = batch.Format(graphiteSettings.MetricNameFormatter);

                logger.LogDebug(text);

                var datagram = Encoding.UTF8.GetBytes(text);

                await _client.Client.SendAsync(new ArraySegment<byte>(datagram), SocketFlags.None);

                logger.LogTrace($"Successful batch {currentBatch} / {batches.Count} write to Graphite (UDP)");

                currentBatch++;
            }
        }

        private static async Task CreateClient(
            GraphiteSettings graphiteSettings,
            HttpPolicy httpPolicy)
        {
            if (_client == null)
            {
                _client = new UdpClient { Client = { SendTimeout = httpPolicy.Timeout.Milliseconds } };
            }

            await _client.Client.ConnectAsync(graphiteSettings.BaseAddress.Host, graphiteSettings.BaseAddress.Port);
        }
    }
}