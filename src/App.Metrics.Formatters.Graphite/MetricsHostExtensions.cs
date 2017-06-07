// <copyright file="MetricsHostExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Extensions.Middleware.Abstractions;
using App.Metrics.Formatters.Graphite;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
    // ReSharper restore CheckNamespace
{
    public static class MetricsHostExtensions
    {
        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metric endpoint's response
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <returns>The metrics host builder</returns>
        public static IMetricsHostBuilder AddGraphiteMetricsSerialization(this IMetricsHostBuilder host)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricsResponseWriter, GraphiteMetricsResponseWriter>());

            return host;
        }

        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metric endpoint's response
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <returns>The metrics host builder</returns>
        public static IMetricsHostBuilder AddGraphiteMetricsTextSerialization(this IMetricsHostBuilder host)
        {
            host.Services.Replace(ServiceDescriptor.Transient<IMetricsTextResponseWriter, GraphiteMetricsTextResponseWriter>());

            return host;
        }

        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metrics and metrics-text responses
        /// </summary>
        /// <param name="host">The metrics host builder.</param>
        /// <returns>The metrics host builder</returns>
        public static IMetricsHostBuilder AddGraphiteSerialization(this IMetricsHostBuilder host)
        {
            host.AddGraphiteMetricsSerialization();
            host.AddGraphiteMetricsTextSerialization();
            return host;
        }
    }
}