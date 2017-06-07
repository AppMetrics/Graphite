// <copyright file="GraphiteMetricsTextResponseWriter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Core;
using App.Metrics.Extensions.Middleware.Abstractions;
using App.Metrics.Formatting;
using App.Metrics.Formatting.Graphite;
using Microsoft.AspNetCore.Http;

namespace App.Metrics.Formatters.Graphite
{
    public class GraphiteMetricsTextResponseWriter : IMetricsTextResponseWriter
    {
        /// <inheritdoc />
        public string ContentType => "text/plain; app.metrics=vnd.app.metrics.v1.metrics.graphite; graphite=plain-text-0.10.0;";

        /// <inheritdoc />
        public Task WriteAsync(HttpContext context, MetricsDataValueSource metricsData, CancellationToken token = default(CancellationToken))
        {
            var payloadBuilder = new GraphitePayloadBuilder();

            var builder = new MetricDataValueSourceFormatter();
            builder.Build(metricsData, payloadBuilder);

            return context.Response.WriteAsync(payloadBuilder.PayloadFormatted(), token);
        }
    }
}