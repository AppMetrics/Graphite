// <copyright file="AppMetricsMiddlewareGraphiteOptionsBuilderExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using App.Metrics.Builder;
using App.Metrics.Formatters.Graphite;
using App.Metrics.Middleware;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
// ReSharper restore CheckNamespace
{
    public static class AppMetricsMiddlewareGraphiteOptionsBuilderExtensions
    {
        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metric endpoint's response
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <returns>The metrics host builder</returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddMetricsGraphiteFormatters(this IAppMetricsMiddlewareOptionsBuilder options)
        {
            options.AppMetricsBuilder.Services.Replace(ServiceDescriptor.Transient<IMetricsResponseWriter, GraphiteMetricsResponseWriter>());

            return options;
        }

        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metric endpoint's response
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <returns>The metrics host builder</returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddMetricsTextGraphiteFormatters(this IAppMetricsMiddlewareOptionsBuilder options)
        {
            options.AppMetricsBuilder.Services.Replace(ServiceDescriptor.Transient<IMetricsTextResponseWriter, GraphiteMetricsTextResponseWriter>());

            return options;
        }

        /// <summary>
        /// Enables Graphites's Plain Text serialization on the metrics and metrics-text responses
        /// </summary>
        /// <param name="options">The metrics middleware options checksBuilder.</param>
        /// <returns>The metrics host builder</returns>
        public static IAppMetricsMiddlewareOptionsBuilder AddGrahpiteFormatters(this IAppMetricsMiddlewareOptionsBuilder options)
        {
            options.AddMetricsGraphiteFormatters();
            options.AddMetricsTextGraphiteFormatters();

            return options;
        }
    }
}