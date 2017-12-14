// <copyright file="GraphiteClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Formatters.Graphite;

namespace App.Metrics.Reporting.Graphite.Client
{
    public class DefaultGraphiteClient : IGraphiteClient
    {
        private static TimeSpan _backOffPeriod;
        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private readonly GraphiteOptions _graphiteOptions;
        private readonly HttpPolicy _httpPolicy;

        public DefaultGraphiteClient(
            GraphiteOptions graphiteOptions,
            HttpPolicy httpPolicy)
        {
            _graphiteOptions = graphiteOptions ?? throw new ArgumentNullException(nameof(graphiteOptions));
            _httpPolicy = httpPolicy ?? throw new ArgumentNullException(nameof(httpPolicy));
            _backOffPeriod = httpPolicy.BackoffPeriod;
            _failuresBeforeBackoff = httpPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
        }

        public async Task<GraphiteWriteResult> WriteAsync(string payload, CancellationToken cancellationToken)
        {
            if (payload == null)
            {
                return new GraphiteWriteResult(true);
            }

            if (NeedToBackoff())
            {
                return new GraphiteWriteResult(false, "Too many failures in writing to Graphite, Circuit Opened");
            }

            try
            {
                switch (_graphiteOptions.Protocol)
                {
                    case Protocol.Tcp:
                        await GraphiteTcpClient.WriteAsync(
                            _graphiteOptions,
                            _httpPolicy,
                            cancellationToken,
                            payload);
                        break;
                    case Protocol.Udp:
                        await GraphiteUdpClient.WriteAsync(
                            _graphiteOptions,
                            _httpPolicy,
                            cancellationToken,
                            payload);
                        break;
                    case Protocol.Pickled:
                        return new GraphiteWriteResult(false, "Picked protocol not implemented, use UDP or TCP");
                    default:
                        return new GraphiteWriteResult(
                            false,
                            "Unsupported protocol, UDP, TCP and Pickled protocols are accepted");
                }

                return GraphiteWriteResult.SuccessResult;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failureAttempts);
                return new GraphiteWriteResult(false, ex.ToString());
            }
        }

        private class GraphiteUdpClient
        {
            private static UdpClient _client;

            public static async Task WriteAsync(
                GraphiteOptions graphiteOptions,
                HttpPolicy httpPolicy,
                CancellationToken cancellationToken,
                string text)
            {
                await CreateClient(graphiteOptions, httpPolicy);

                var datagram = Encoding.UTF8.GetBytes(text);

                await _client.Client.SendAsync(new ArraySegment<byte>(datagram), SocketFlags.None);
            }

            private static async Task CreateClient(
                GraphiteOptions graphiteOptions,
                HttpPolicy httpPolicy)
            {
                // FIXME ED Not thread-safe
                if (_client == null)
                {
                    _client = new UdpClient { Client = { SendTimeout = httpPolicy.Timeout.Milliseconds } };
                }

                await _client.Client.ConnectAsync(graphiteOptions.BaseAddress.Host, graphiteOptions.BaseAddress.Port);
            }
        }

        private class GraphiteTcpClient
        {
            public static async Task WriteAsync(
                GraphiteOptions graphiteOptions,
                HttpPolicy httpPolicy,
                CancellationToken cancellationToken,
                string text)
            {
                using (var client = await CreateClient(graphiteOptions, httpPolicy))
                {
                    using (var stream = client.GetStream())
                    {
                        using (var writer = new StreamWriter(stream) { NewLine = "\n" })
                        {
                            await writer.WriteLineAsync(text);

                            await writer.FlushAsync();
                        }

                        await stream.FlushAsync(cancellationToken);
                    }
                }
            }

            private static async Task<TcpClient> CreateClient(
                GraphiteOptions graphiteOptions,
                HttpPolicy httpPolicy)
            {
                var client = new TcpClient { SendTimeout = httpPolicy.Timeout.Milliseconds };
                await client.ConnectAsync(graphiteOptions.BaseAddress.Host, graphiteOptions.BaseAddress.Port);

                return client;
            }
        }

        private bool NeedToBackoff()
        {
            if (Interlocked.Read(ref _failureAttempts) < _failuresBeforeBackoff)
            {
                return false;
            }

            if (Interlocked.Read(ref _backOffTicks) == 0)
            {
                Interlocked.Exchange(ref _backOffTicks, DateTime.UtcNow.Add(_backOffPeriod).Ticks);
            }

            if (DateTime.UtcNow.Ticks <= Interlocked.Read(ref _backOffTicks))
            {
                return true;
            }

            Interlocked.Exchange(ref _failureAttempts, 0);
            Interlocked.Exchange(ref _backOffTicks, 0);

            return false;
        }
    }
}