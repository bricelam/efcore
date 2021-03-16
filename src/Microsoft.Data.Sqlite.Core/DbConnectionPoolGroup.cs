// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Sqlite
{
    // TODO: Can we use just pool directly?
    internal class DbConnectionPoolGroup
    {
        private const int PoolGroupStateActive = 1;
        private const int PoolGroupStateIdle = 2;
        private const int PoolGroupStateDisabled = 4;

        private DbConnectionPool _pool;
        private int _state = PoolGroupStateActive;

        public DbConnectionPoolGroup(
            SqliteConnectionStringBuilder connectionOptions,
            string connectionString,
            DbConnectionPoolGroupOptions? poolGroupOptions)
        {
            ConnectionOptions = connectionOptions;
            ConnectionString = connectionString;
            PoolGroupOptions = poolGroupOptions;
        }

        public SqliteConnectionStringBuilder ConnectionOptions { get; }

        public string ConnectionString { get; }

        public bool IsDisabled
            => _state == PoolGroupStateDisabled;

        public DbConnectionPoolGroupOptions? PoolGroupOptions { get; }

        public DbConnectionPool GetConnectionPool(SqliteConnectionFactory connectionFactory)
        {
            DbConnectionPool pool = null;

            if (PoolGroupOptions != null)
            {
                if (_pool == null)
                {
                    lock (this)
                    {
                        if (_pool == null)
                        {
                            var newPool = new DbConnectionPool(connectionFactory, this);

                            if (MarkPoolGroupAsActive())
                            {
                                newPool.Startup();
                                _pool = newPool;
                                pool = newPool;
                            }
                            else
                            {
                                newPool.Shutdown();
                            }
                        }
                        else
                        {
                            pool = _pool;
                        }
                    }
                }
                else
                {
                    pool = _pool;
                }
            }

            if (pool == null)
            {
                lock (this)
                {
                    MarkPoolGroupAsActive();
                }
            }

            return pool;
        }

        private bool MarkPoolGroupAsActive()
        {
            if (_state == PoolGroupStateIdle)
            {
                _state = PoolGroupStateActive;
            }

            return _state == PoolGroupStateActive;
        }
    }
}
