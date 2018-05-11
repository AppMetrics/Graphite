// <copyright file="GraphiteFormatterConstants.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

namespace App.Metrics.Formatters.Graphite.Internal
{
    public static class GraphiteFormatterConstants
    {
        public static class GraphiteDefaults
        {
            public static readonly IGraphitePointTextWriter MetricPointTextWriter = new DefaultGraphitePointTextWriter();
        }
    }
}