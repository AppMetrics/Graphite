// <copyright file="ClientPolicy.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using ClientConstants = App.Metrics.Reporting.Graphite.Client.Constants;

namespace App.Metrics.Reporting.Graphite.Client
{
    public class ClientPolicy
    {
        public ClientPolicy()
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