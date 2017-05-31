// <copyright file="HttpPolicy.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using ClientConstants = App.Metrics.Extensions.Reporting.Graphite.Client.Constants;

namespace App.Metrics.Extensions.Reporting.Graphite
{
    public class HttpPolicy
    {
        public HttpPolicy()
        {
            FailuresBeforeBackoff = ClientConstants.DefaultFailuresBeforeBackoff;
            BackoffPeriod = ClientConstants.DefaultBackoffPeriod;
            Timeout = ClientConstants.DefaultTimeout;
        }

        public TimeSpan BackoffPeriod { get; set; }

        public int FailuresBeforeBackoff { get; set; }

        public TimeSpan Timeout { get; set; }
    }
}