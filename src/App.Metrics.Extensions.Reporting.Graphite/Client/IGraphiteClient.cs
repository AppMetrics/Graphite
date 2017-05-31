// <copyright file="IGraphiteClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Formatting.Graphite;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public interface IGraphiteClient
    {
        Task<GraphiteWriteResult> WriteAsync(
            GraphitePayload payload,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}