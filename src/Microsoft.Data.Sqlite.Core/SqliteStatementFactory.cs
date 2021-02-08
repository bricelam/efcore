// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SQLitePCL;

using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    internal class SqliteStatementFactory : IDisposable
    {
        private const int Capacity = 15;

        private readonly sqlite3 _db;
        private readonly Dictionary<string, SqliteStatement> _cached = new(Capacity);
        private readonly Dictionary<int, int> _used = new();

        private int _minUsed = 3;

        public SqliteStatementFactory(sqlite3 db)
        {
            _db = db;
        }

        public SqliteStatement Create(string sql, Stopwatch timer, long timeout, out string tail)
        {
            if (_cached.Remove(sql, out var cachedStatement))
            {
                cachedStatement.Used++;
                cachedStatement.LastUsed = DateTime.UtcNow;

                tail = cachedStatement.Tail;

                return cachedStatement;
            }

            var flags = 0u;
            var hashCode = sql.GetHashCode();
            if (_used.Remove(hashCode, out var used))
            {
                used++;

                // TODO: Speculative. Better to re-prepare on cache?
                if (used >= _minUsed)
                {
                    flags |= SQLITE_PREPARE_PERSISTENT;
                }

                _used[hashCode] = used;
            }
            else
            {
                used = 1;

                _used.Add(hashCode, used);
            }

            int rc;
            sqlite3_stmt stmt;
            do
            {
                timer.Start();

                // TODO: Use Span overload
                string nextTail;
                while (IsBusy(rc = sqlite3_prepare_v3(_db, sql, flags, out stmt, out nextTail)))
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
            }
            // TODO: Cache invalid stmts?
            while (stmt.IsInvalid && tail.Length != 0);

            return new SqliteStatement(this, sql, stmt, tail, used);

            static bool IsBusy(int rc)
                => rc == SQLITE_LOCKED
                    || rc == SQLITE_BUSY
                    || rc == SQLITE_LOCKED_SHAREDCACHE;
        }

        public void Return(SqliteStatement statement)
        {
            // TODO: Also dispose if factory disposed? Is that situation possible?
            if (statement.Used < _minUsed)
            {
                var hashCode = statement.Sql.GetHashCode();
                if (!_used.TryAdd(hashCode, statement.Used))
                {
                    _used[hashCode] += statement.Used;
                }

                statement.Handle.Dispose();

                return;
            }

            if (_cached.Count == Capacity)
            {
                SqliteStatement? secondOldest = null;
                SqliteStatement? oldest = null;
                foreach (var cachedStatement in _cached.Values)
                {
                    if (cachedStatement.Sql == statement.Sql)
                    {
                        // TODO: Cache multiple? Use the same threshold? Seems unlikely.
                        cachedStatement.Used += statement.Used;
                        statement.Handle.Dispose();

                        return;
                    }

                    if (oldest == null
                        || oldest.Used > cachedStatement.Used
                        || (oldest.Used == cachedStatement.Used
                            && oldest.LastUsed > cachedStatement.LastUsed))
                    {
                        secondOldest = oldest;
                        oldest = cachedStatement;
                    }
                }

                _cached.Remove(oldest!.Sql);
                _minUsed = secondOldest!.Used;
            }

            sqlite3_reset(statement.Handle);
            sqlite3_clear_bindings(statement.Handle);

            _cached.Add(statement.Sql, statement);
        }

        public void Dispose()
        {
            foreach (var pair in _cached)
            {
                pair.Value.Handle.Dispose();
            }

            _cached.Clear();
        }
    }
}
