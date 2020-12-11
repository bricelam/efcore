// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Data.Sqlite.Utilities;
using SQLitePCL;

using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    internal class SqliteStatementPool : IDisposable
    {
        // TODO: Allow same SQL to be cached multiple times?
        private readonly Dictionary<byte[], SqlitePooledStatement> _cache = new(new UTF8Comparer());
        private readonly sqlite3 _db;

        public SqliteStatementPool(sqlite3 db)
            => _db = db;

        public SqlitePooledStatement Get(ReadOnlySpan<byte> sql, Stopwatch timer, long timeout, out ReadOnlySpan<byte> tail)
        {
            // TODO: Avoid copy
            if (_cache.Remove(sql.ToArray(), out var entry))
            {
                tail = sql.Slice(sql.Length - entry.TailLength);

                return entry;
            }

            return new SqlitePooledStatement
            {
                Key = sql.ToArray(),
                Value = Create(sql, timer, timeout, out tail),
                TailLength = tail.Length
            };
        }

        public void Return(SqlitePooledStatement entry)
        {
            //sqlite3_clear_bindings(Handle);
            sqlite3_reset(entry.Value);

            // TODO: Evict old entries
            _cache.Add(entry.Key, entry);
        }

        public void Dispose()
        {
            foreach (var entry in _cache.Values)
            {
                entry.Value.Dispose();
            }

            _cache.Clear();
        }

        sqlite3_stmt Create(ReadOnlySpan<byte> sql, Stopwatch timer, long timeout, out ReadOnlySpan<byte> tail)
        {
            int rc;
            sqlite3_stmt stmt;

            timer.Start();

            ReadOnlySpan<byte> nextTail;
            // TODO: No SQLITE_PREPARE_PERSISTENT on first two times
            while (IsBusy(rc = sqlite3_prepare_v3(_db, sql, SQLITE_PREPARE_PERSISTENT, out stmt, out nextTail)))
            {
                if (timeout != 0L
                    && timer.ElapsedMilliseconds >= timeout)
                {
                    break;
                }

                Thread.Sleep(150);
            }

            timer.Stop();
            tail = nextTail;

            SqliteException.ThrowExceptionForRC(rc, _db);

            return stmt;
        }

        static bool IsBusy(int rc)
            => rc == SQLITE_LOCKED
                || rc == SQLITE_BUSY
                || rc == SQLITE_LOCKED_SHAREDCACHE;
    }
}
