// <copyright file="GraphitePayloadExtentions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Linq;
using System.Text;

namespace App.Metrics.Formatting.Graphite.Extensions
{
    public static class GraphitePayloadExtentions
    {
        public static string Format(this GraphitePayload payload, IGraphiteNameFormatter formatter)
        {
            if (formatter == null)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (var formatted in payload.SelectMany(formatter.Format))
            {
                sb.Append(formatted);
                sb.Append("\n");
            }

            return sb.ToString();
        }
    }
}
