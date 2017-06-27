// <copyright file="StringExtensions.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Diagnostics;

// ReSharper disable CheckNamespace
namespace System
    // ReSharper restore CheckNamespace
{
    internal static class StringExtensions
    {
        [DebuggerStepThrough]
        internal static bool IsMissing(this string value) { return string.IsNullOrWhiteSpace(value); }

        [DebuggerStepThrough]
        internal static bool IsPresent(this string value) { return !string.IsNullOrWhiteSpace(value); }
    }
}