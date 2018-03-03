// <copyright file="IGraphiteClient.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Threading;
using System.Threading.Tasks;

namespace App.Metrics.Reporting.Graphite.Client
{
    public interface IGraphiteClient
    {
        Task<GraphiteWriteResult> WriteAsync(string payload, CancellationToken cancellationToken = default);
    }
}