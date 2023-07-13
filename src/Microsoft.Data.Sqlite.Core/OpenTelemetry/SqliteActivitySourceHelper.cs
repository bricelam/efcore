// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Data.Sqlite.OpenTelemetry;

/// <summary>
/// TODO.
/// </summary>
public sealed class SqliteActivitySourceHelper
{
    internal static readonly ActivitySource ActivitySource;
    internal static readonly string ActivityName;
    internal static readonly IReadOnlyDictionary<string, object?> CreationTags = new Dictionary<string, object?>
    {
        { "db.system", "sqlite" },
        { "server.address", "localhost" }
    };

    /// <summary>
    /// TODO.
    /// </summary>
    public static readonly string ActivitySourceName;

    static SqliteActivitySourceHelper()
    {
        var assemblyName = typeof(SqliteActivitySourceHelper).Assembly.GetName();
        var version = assemblyName.Version!;

        ActivitySourceName = assemblyName.Name!;
        ActivitySource = new(ActivitySourceName, version.ToString());
        ActivityName = ActivitySourceName + ".Execute";
    }
}
