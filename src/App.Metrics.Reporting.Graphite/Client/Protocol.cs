// <copyright file="Protocol.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

namespace App.Metrics.Reporting.Graphite.Client
{
    public enum Protocol
    {
        /// <summary>
        /// Send data using TCP
        /// </summary>
        Tcp,

        /// <summary>
        /// Send data using UDP
        /// </summary>
        Udp,

        /// <summary>
        /// Send data using Pickled
        /// </summary>
        Pickled
    }
}