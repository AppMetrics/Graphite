// <copyright file="Constants.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using App.Metrics.Reporting;

namespace App.Metrics.Formatting.Graphite
{
    public static class Constants
    {
        public class GraphiteDefaults
        {
            public static readonly Dictionary<ApdexValueDataKeys, string> CustomApdexKeys = new Dictionary<ApdexValueDataKeys, string>
                                                                                            {
                                                                                                { ApdexValueDataKeys.Frustrating, "Frustrating" },
                                                                                                { ApdexValueDataKeys.Samples, "Samples" },
                                                                                                { ApdexValueDataKeys.Satisfied, "Satisfied" },
                                                                                                { ApdexValueDataKeys.Score, "Score" },
                                                                                                { ApdexValueDataKeys.Tolerating, "Tolerating" }
                                                                                            };

            public static readonly Dictionary<CounterValueDataKeys, string> CustomCounterDataKeys = new Dictionary<CounterValueDataKeys, string>
                                                                                                    {
                                                                                                        { CounterValueDataKeys.Total, "Total" },
                                                                                                        {
                                                                                                            CounterValueDataKeys.MetricSetItemSuffix,
                                                                                                            "-SetItem"
                                                                                                        },
                                                                                                        {
                                                                                                            CounterValueDataKeys.SetItemPercent,
                                                                                                            "Percent"
                                                                                                        }
                                                                                                    };

            public static readonly Dictionary<HistogramValueDataKeys, string> CustomHistogramDataKeys = new Dictionary<HistogramValueDataKeys, string>
                                                                                                        {
                                                                                                            { HistogramValueDataKeys.Count, "Count" },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserLastValue,
                                                                                                                "User-Last"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserMinValue,
                                                                                                                "User-Min"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.UserMaxValue,
                                                                                                                "User-Max"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.LastValue,
                                                                                                                "Last"
                                                                                                            },
                                                                                                            { HistogramValueDataKeys.Min, "Min" },
                                                                                                            { HistogramValueDataKeys.Max, "Max" },
                                                                                                            { HistogramValueDataKeys.Mean, "Mean" },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.Median,
                                                                                                                "Median"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.P75,
                                                                                                                "Percentile-75"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.P95,
                                                                                                                "Percentile-95"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.P98,
                                                                                                                "Percentile-98"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.P99,
                                                                                                                "Percentile-99"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.P999,
                                                                                                                "Percentile-999"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.Samples,
                                                                                                                "Samples"
                                                                                                            },
                                                                                                            {
                                                                                                                HistogramValueDataKeys.StdDev,
                                                                                                                "StdDev"
                                                                                                            },
                                                                                                            { HistogramValueDataKeys.Sum, "Sum" }
                                                                                                        };

            public static readonly Dictionary<MeterValueDataKeys, string> CustomMeterDataKeys = new Dictionary<MeterValueDataKeys, string>
                                                                                                {
                                                                                                    { MeterValueDataKeys.Count, "Total" },
                                                                                                    { MeterValueDataKeys.RateMean, "Rate-Mean" },
                                                                                                    { MeterValueDataKeys.SetItemPercent, "Percent" },
                                                                                                    {
                                                                                                        MeterValueDataKeys.MetricSetItemSuffix,
                                                                                                        "-SetItem"
                                                                                                    },
                                                                                                    { MeterValueDataKeys.Rate1M, "Rate-1-Min" },
                                                                                                    { MeterValueDataKeys.Rate5M, "Rate-5-Min" },
                                                                                                    { MeterValueDataKeys.Rate15M, "Rate-15-Min" }
                                                                                                };

            public static readonly Func<string, string, string> MetricNameFormatter = (metricContext, metricName) => string.IsNullOrWhiteSpace(metricContext)
                ? $"{metricName}".Replace(' ', '_').Replace('.', '_')
                : $"{metricContext}.{metricName.Replace(' ', '_').Replace('.', '_')}".Replace(' ', '_');
        }
    }
}