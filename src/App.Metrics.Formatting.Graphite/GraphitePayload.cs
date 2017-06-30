// <copyright file="GraphitePayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;

namespace App.Metrics.Formatting.Graphite
{
    public class GraphitePayload : List<GraphitePoint>
    {
        public GraphitePayload() { }

        private GraphitePayload(IEnumerable<GraphitePoint> other)
            : base(other) { }

        public IEnumerable<GraphitePayload> ToBatches(int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentException("must be greater than zero", nameof(batchSize));
            }

            for (var i = 0; i < this.Count; i += batchSize)
            {
                yield return new GraphitePayload(this.GetRange(i, Math.Min(this.Count - i, batchSize)));
            }
        }
    }
}