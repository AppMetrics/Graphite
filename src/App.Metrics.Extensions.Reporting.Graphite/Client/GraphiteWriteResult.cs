// <copyright file="GraphiteWriteResult.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

namespace App.Metrics.Extensions.Reporting.Graphite.Client
{
    public struct GraphiteWriteResult
    {
        public GraphiteWriteResult(bool success)
        {
            Success = success;
            ErrorMessage = null;
        }

        public GraphiteWriteResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }

        public bool Success { get; }

        public static readonly GraphiteWriteResult SuccessResult = new GraphiteWriteResult(true);
    }
}