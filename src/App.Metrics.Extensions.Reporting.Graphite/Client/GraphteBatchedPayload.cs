// <copyright file="GraphteBatchedPayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public class GraphteBatchedPayload
    {
        private readonly GraphitePayload _payload;
        private readonly int _batchSize;

        public GraphteBatchedPayload(GraphitePayload payload, int batchSize)
        {
            if (batchSize <= 0)
            {
                throw new ArgumentException("must be greater than zero", nameof(batchSize));
            }

            _payload = payload;
            _batchSize = batchSize;
        }

        public IEnumerable<GraphitePayload> GetBatches()
        {
            if (_payload == null || !_payload.Points.Any())
            {
                return Enumerable.Empty<GraphitePayload>();
            }

            var batchedPayloads = new List<GraphitePayload>();
            var batchedPoints = Batch(_payload.Points);

            foreach (var points in batchedPoints)
            {
                var batchedPayload = new GraphitePayload();
                foreach (var point in points)
                {
                    batchedPayload.Add(point);
                }

                batchedPayloads.Add(batchedPayload);
            }

            return batchedPayloads;
        }

        private IEnumerable<ICollection<GraphitePoint>> Batch(ICollection<GraphitePoint> points)
        {
            if (points.Count <= _batchSize)
            {
                yield return points;

                yield break;
            }

            var section = new List<GraphitePoint>(_batchSize);

            foreach (var item in points)
            {
                section.Add(item);

                if (section.Count == _batchSize)
                {
                    yield return section.AsReadOnly();

                    section = new List<GraphitePoint>(_batchSize);
                }
            }

            if (section.Count > 0)
            {
                yield return section.AsReadOnly();
            }
        }
    }
}