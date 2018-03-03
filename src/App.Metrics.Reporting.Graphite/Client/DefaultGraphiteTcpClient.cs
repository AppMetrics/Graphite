// <copyright file="DefaultGraphiteTcpClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Logging;

namespace App.Metrics.Reporting.Graphite.Client
{
    public class DefaultGraphiteTcpClient : IGraphiteClient
    {
        private static readonly ILog Logger = LogProvider.For<DefaultGraphiteTcpClient>();

        private static TimeSpan _backOffPeriod;
        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private readonly TcpClient _client;
        private readonly GraphiteOptions _options;

        public DefaultGraphiteTcpClient(
            TcpClient client,
            GraphiteOptions options,
            ClientPolicy clientPolicy)
        {
            _client = client;
            _options = options;
            _backOffPeriod = clientPolicy.BackoffPeriod;
            _failuresBeforeBackoff = clientPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
        }

        /// <inheritdoc />
        public async Task<GraphiteWriteResult> WriteAsync(string payload, CancellationToken cancellationToken = default)
        {
            if (payload == null)
            {
                return GraphiteWriteResult.SuccessResult;
            }

            if (NeedToBackoff())
            {
                return new GraphiteWriteResult(false, "Too many failures in writing to Graphite, Circuit Opened - TCP");
            }

            try
            {
                if (!_client.Connected)
                {
                    Logger.Trace("Opening TCP Connection for Graphite");
                    await _client.ConnectAsync(_options.BaseUri.Host, _options.BaseUri.Port);
                }

                using (var stream = _client.GetStream())
                {
                    using (var writer = new StreamWriter(stream) { NewLine = "\n" })
                    {
                        await writer.WriteLineAsync(payload);

                        await writer.FlushAsync();
                    }

                    await stream.FlushAsync(cancellationToken);

                    Logger.Trace("Successful write to Graphite - TCP");

                    return new GraphiteWriteResult(true);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failureAttempts);
                Logger.Error(ex, "Failed to write to Graphite - TCP");
                return new GraphiteWriteResult(false, ex.ToString());
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
