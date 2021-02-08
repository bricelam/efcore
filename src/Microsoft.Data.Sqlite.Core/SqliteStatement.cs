// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using SQLitePCL;

namespace Microsoft.Data.Sqlite
{
    internal class SqliteStatement : IDisposable
    {
        private readonly SqliteStatementFactory _factory;

        public SqliteStatement(SqliteStatementFactory factory, string sql, sqlite3_stmt stmt, string tail, int used)
        {
            _factory = factory;
            Sql = sql;
            Handle = stmt;
            Tail = tail;
            Used = used;
        }

        public string Sql { get; }
        public sqlite3_stmt Handle { get; }
        public string Tail { get; }

        public int Used { get; set; }
        public DateTime LastUsed { get; set; }

        public void Dispose()
            => _factory.Return(this);
    }
}
