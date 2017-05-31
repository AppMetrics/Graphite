// <copyright file="GraphitePayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace App.Metrics.Formatting.Graphite
{
    public class GraphitePayload
    {
        internal List<GraphitePoint> Points { get; } = new List<GraphitePoint>();

        public void Add(GraphitePoint point)
        {
            if (point == null)
            {
                return;
            }

            Points.Add(point);
        }

        public void Format(TextWriter textWriter)
        {
            if (textWriter == null)
            {
                return;
            }

            var points = Points.ToList();

            foreach (var point in points)
            {
                point.Format(textWriter);
            }
        }
    }
}