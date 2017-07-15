// <copyright file="IGraphiteNameFormatter.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Collections.Generic;

namespace App.Metrics.Formatting.Graphite
{
    public interface IGraphiteNameFormatter
    {
        IEnumerable<string> Format(GraphitePoint point);
    }
}