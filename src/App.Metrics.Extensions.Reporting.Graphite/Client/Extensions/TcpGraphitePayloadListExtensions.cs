// <copyright file="TcpGraphitePayloadListExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Extensions.Reporting.Graphite;
using App.Metrics.Extensions.Reporting.Graphite.Client;
using Microsoft.Extensions.Logging;

// ReSharper disable CheckNamespace
namespace System.Collections.Generic
    // ReSharper restore CheckNamespace
{
    public static class TcpGraphitePayloadListExtensions
    {
        public static async Task TcpWriteAsync(
            this List<GraphitePayload> batches,
            GraphiteSettings graphiteSettings,
            HttpPolicy httpPolicy,
            ILogger<GraphiteClient> logger,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var client = await CreateClient(graphiteSettings, httpPolicy))
            {
                using (var stream = client.GetStream())
                {
                    using (var writer = new StreamWriter(stream) { NewLine = "\n" })
                    {
                        var currentBatch = 1;

                        foreach (var batch in batches)
                        {
                            var payloadText = new StringWriter();
                            batch.Format(payloadText);
                            var text = payloadText.ToString();

                            logger.LogDebug(text);

                            await writer.WriteLineAsync(text);

                            logger.LogTrace($"Successful batch {currentBatch} / {batches.Count} write to Graphite (TCP)");

                            currentBatch++;
                        }

                        await writer.FlushAsync();
                    }

                    await stream.FlushAsync(cancellationToken);
                }
            }
        }

        private static async Task<TcpClient> CreateClient(
            GraphiteSettings graphiteSettings,
            HttpPolicy httpPolicy)
        {
            var client = new TcpClient { SendTimeout = httpPolicy.Timeout.Milliseconds };
            await client.ConnectAsync(graphiteSettings.BaseAddress.Host, graphiteSettings.BaseAddress.Port);

            return client;
        }
    }
}