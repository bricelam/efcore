// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Xunit;

namespace Microsoft.Data.Sqlite;

[Collection(nameof(OpenTelemetryTestCollection))]
public class OpenTelemetryTests
{
    [Fact]
    public void Execute_works()
    {
        var activities = new List<Activity>();
        using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSqliteInstrumentation()
            .AddInMemoryExporter(activities)
            .Build()!;

        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1";

        command.ExecuteNonQuery();

        var activity = Assert.Single(activities);
        Assert.Equal(":memory:", activity.DisplayName);

        // TODO: Assert
        Assert.Equal(5, activity.Tags.Count());
    }

    // TODO: Test errors
}
