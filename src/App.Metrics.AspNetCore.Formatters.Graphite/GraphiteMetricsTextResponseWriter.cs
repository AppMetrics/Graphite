// <copyright file="GraphiteMetricsTextResponseWriter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace App.Metrics.Formatters.Graphite
{
    public class GraphiteMetricsTextResponseWriter : IMetricsTextResponseWriter
    {
        public string ContentType => "text/plain";

        /// <inheritdoc />
        public Task WriteAsync(HttpContext context, MetricsDataValueSource metricsData, CancellationToken token = default)
        {
            var payloadBuilder = new GraphitePayloadBuilder();

            var builder = new MetricDataValueSourceFormatter();
            builder.Build(metricsData, payloadBuilder);

            return context.Response.WriteAsync(payloadBuilder.PayloadFormatted(), token);
        }
    }
}