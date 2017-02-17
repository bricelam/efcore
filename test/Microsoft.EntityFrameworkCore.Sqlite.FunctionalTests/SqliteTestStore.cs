// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Specification.Tests;
using System;
using System.Data.Common;

namespace Microsoft.EntityFrameworkCore.Sqlite.FunctionalTests
{
    public class SqliteTestStore : RelationalTestStore
    {
        private readonly SqliteConnection _lifetimeConnection;

        public SqliteTestStore(string connectionString)
        {
            if (new SqliteConnectionStringBuilder(connectionString).Mode == SqliteOpenMode.Memory)
            {
                _lifetimeConnection = new SqliteConnection(connectionString);
                _lifetimeConnection.Open();
            }

            Connection = new SqliteConnection(connectionString);
        }

        public static SqliteTestStore GetOrCreateShared(string name, Action initializeDatabase = null)
        {
            var store = new SqliteTestStore(
                new SqliteConnectionStringBuilder
                {
                    DataSource = name,
                    Cache = SqliteCacheMode.Shared
                }.ToString());
            store.CreateShared(nameof(SqliteTestStore) + name, initializeDatabase);

            return store;
        }

        public static SqliteTestStore CreateScratch()
            => new SqliteTestStore(CreateConnectionString(Guid.NewGuid().ToString()));

        public override string ConnectionString => Connection.ConnectionString;

        public override void OpenConnection()
        {
            Connection.Open();

            var command = Connection.CreateCommand();
            command.CommandText = "PRAGMA foreign_keys = 1;";
            command.ExecuteNonQuery();
        }

        public override DbConnection Connection { get; }

        public override void Dispose()
        {
            Connection.Dispose();
            _lifetimeConnection?.Dispose();

            base.Dispose();
        }

        public static string CreateConnectionString(string name)
            => new SqliteConnectionStringBuilder
            {
                DataSource = name,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Shared
            }.ToString();
    }
}
