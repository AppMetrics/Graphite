// <copyright file="GraphiteClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Formatting.Graphite;
using Microsoft.Extensions.Logging;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public class GraphiteClient : IGraphiteClient
    {
        private static TimeSpan _backOffPeriod;
        private static long _backOffTicks;
        private static long _failureAttempts;
        private static long _failuresBeforeBackoff;
        private readonly GraphiteSettings _graphiteSettings;
        private readonly HttpPolicy _httpPolicy;
        private readonly ILogger<GraphiteClient> _logger;

        // ReSharper disable UnusedMember.Global
        public GraphiteClient(ILoggerFactory loggerFactory, GraphiteSettings graphiteSettings)
            // ReSharper restore UnusedMember.Global
            : this(
                loggerFactory,
                graphiteSettings,
#pragma warning disable SA1118
                new HttpPolicy
                {
                    FailuresBeforeBackoff = Constants.DefaultFailuresBeforeBackoff,
                    BackoffPeriod = Constants.DefaultBackoffPeriod,
                    Timeout = Constants.DefaultTimeout
                })
        {
        }

#pragma warning disable SA1118

        public GraphiteClient(
            ILoggerFactory loggerFactory,
            GraphiteSettings graphiteSettings,
            HttpPolicy httpPolicy)
        {
            _graphiteSettings = graphiteSettings ?? throw new ArgumentNullException(nameof(graphiteSettings));
            _httpPolicy = httpPolicy ?? throw new ArgumentNullException(nameof(httpPolicy));
            _backOffPeriod = httpPolicy.BackoffPeriod;
            _failuresBeforeBackoff = httpPolicy.FailuresBeforeBackoff;
            _failureAttempts = 0;
            _logger = loggerFactory.CreateLogger<GraphiteClient>();
        }

        public async Task<GraphiteWriteResult> WriteAsync(
            GraphitePayload payload,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (NeedToBackoff())
            {
                return new GraphiteWriteResult(false, "Too many failures in writing to Graphite, Circuit Opened");
            }

            try
            {
                var batches = payload.ToBatches(_graphiteSettings.BatchSize).ToList();

                switch (_graphiteSettings.Protocol)
                {
                    case Protocol.Tcp:
                        await batches.TcpWriteAsync(_graphiteSettings, _httpPolicy, _logger, cancellationToken);
                        break;
                    case Protocol.Udp:
                        await batches.UdpWriteAsync(_graphiteSettings, _httpPolicy, _logger, cancellationToken);
                        break;
                    case Protocol.Pickled:
                        throw new NotImplementedException("Picked protocol not implemented, use UDP or TCP");
                    default:
                        throw new InvalidOperationException("Unsupported protocol, UDP, TCP and Pickled protocols are accepted");
                }

                return GraphiteWriteResult.SuccessResult;
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref _failureAttempts);
                _logger.LogError(LoggingEvents.GraphiteWriteError, "Failed to write to Graphite", ex);
                return new GraphiteWriteResult(false, ex.ToString());
            }
        }

        private bool NeedToBackoff()
        {
            if (Interlocked.Read(ref _failureAttempts) < _failuresBeforeBackoff)
            {
                return false;
            }

            _logger.LogError($"Graphite write backoff for {_backOffPeriod.Seconds} secs");

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