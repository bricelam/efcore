// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Diagnostics;

internal static class ActivityExtensions
{
    //public static void RecordException(this Activity activity, Exception ex)
    //    => activity.AddEvent(
    //        new ActivityEvent(
    //            "exception",
    //            tags: new()
    //            {
    //                { "exception.type", ex.GetType().FullName },
    //                { "exception.stacktrace", ex.ToString() },
    //                { "exception.message", ex.Message }
    //            }));
}
