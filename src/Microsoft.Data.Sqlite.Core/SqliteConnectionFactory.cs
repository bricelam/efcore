using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Data.Sqlite
{
    internal sealed class SqliteConnectionFactory
    {
        public static readonly SqliteConnectionFactory SingletonInstance = new SqliteConnectionFactory();

        private Dictionary<string, DbConnectionPoolGroup> _connectionPoolGroups = new Dictionary<string, DbConnectionPoolGroup>();

        public bool TryGetConnection(
            SqliteConnection owningConnection,
            DbConnectionInternal oldConnection,
            out DbConnectionInternal? connection)
        {
            DbConnectionPoolGroup poolGroup;
            DbConnectionPool connectionPool;
            connection = null;

            var retriesLeft = 10;
            var timeBetweenRetriesMilliseconds = 1;

            do
            {
                connectionPool = GetConnectionPool(owningConnection);
                if (connectionPool == null)
                {
                    connection = CreateNonPooledConnection(owningConnection);
                }
                else
                {
                    connection = connectionPool.GetConnection(owningConnection);
                    if (connection == null)
                    {
                        if (connectionPool.IsRunning)
                        {
                            throw new InvalidOperationException("ADP_PooledOpenTimeout");
                        }
                        else
                        {
                            Thread.Sleep(timeBetweenRetriesMilliseconds);
                            timeBetweenRetriesMilliseconds *= 2;
                        }
                    }
                }
            }
            while (connection == null && retriesLeft-- > 0);

            if (connection == null)
            {
                throw new InvalidOperationException("ADP_PooledOpenTimeout");
            }
        }

        private DbConnectionPool GetConnectionPool(SqliteConnection owningObject)
        {
            var connectionPoolGroup = owningObject.PoolGroup;
            if (connectionPoolGroup.IsDisabled && connectionPoolGroup.PoolGroupOptions != null)
            {
                var connectionOptions = connectionPoolGroup.ConnectionOptions;
                connectionPoolGroup = GetConnectionPoolGroup(
                    connectionPoolGroup.ConnectionString,
                    connectionPoolGroup.PoolGroupOptions,
                    ref connectionOptions);

                owningObject.PoolGroup = connectionPoolGroup;
            }

            return connectionPoolGroup.GetConnectionPool(this);
        }

        private DbConnectionPoolGroup GetConnectionPoolGroup(
            string connectionString,
            DbConnectionPoolGroupOptions? poolOptions,
            ref SqliteConnectionStringBuilder? connectionOptions)
        {
            //DbConnectionPoolGroup connectionPoolGroup;
            var connectionPoolGroups = _connectionPoolGroups;
            if (!connectionPoolGroups.TryGetValue(connectionString, out var connectionPoolGroup)
                || (connectionPoolGroup.IsDisabled && connectionPoolGroup.PoolGroupOptions != null))
            {
                if (connectionOptions == null)
                {
                    connectionOptions = new SqliteConnectionStringBuilder(connectionString);

                    // TODO: Why not normalize before checking?
                    return GetConnectionPoolGroup(connectionOptions.ToString(), null, ref connectionOptions);
                }

                if (poolOptions == null)
                {
                    poolOptions = connectionPoolGroup != null
                        ? connectionPoolGroup.PoolGroupOptions
                        // TODO: Don't pool in-memory connections
                        : new DbConnectionPoolGroupOptions(
                            poolByIdentity: false,
                            minPoolSize: 0,
                            maxPoolSize: 100,
                            connectionTimeout: 15_000,
                            loadBalanceTimeout: 0,
                            hasTransactionAffinity: false);
                }

                lock (this)
                {
                    connectionPoolGroups = _connectionPoolGroups;
                    if (!connectionPoolGroups.TryGetValue(connectionString, out connectionPoolGroup))
                    {
                        var newConnectionPoolGroup = new DbConnectionPoolGroup(connectionOptions, connectionString, poolOptions);
                        //newConnectionPoolGroup.ProviderInfo = CreateConnectionPoolGroupProviderInfo(connectionOptions);

                        var newConnectionPoolGroups = new Dictionary<string, DbConnectionPoolGroup>(connectionPoolGroups.Count + 1);
                        foreach (var entry in connectionPoolGroups)
                        {
                            newConnectionPoolGroups.Add(entry.Key, entry.Value);
                        }

                        newConnectionPoolGroups.Add(connectionString, newConnectionPoolGroup);
                        connectionPoolGroup = newConnectionPoolGroup;
                        _connectionPoolGroups = newConnectionPoolGroups;
                    }
                }
            }
            else if (connectionOptions == null)
            {
                connectionOptions = connectionPoolGroup.ConnectionOptions;
            }

            return connectionPoolGroup;
        }

        private DbConnectionInternal CreateNonPooledConnection(SqliteConnection owningConnection)
        {
            DbConnectionPoolGroup poolGroup = owningConnection.PoolGroup;
            var newConnection = CreateConnection(
                poolGroup.ConnectionOptions,
                poolGroup.ConnectionString,
                owningConnection);
            if (newConnection != null)
            {
                newConnection.MakeNonPooledObject(owningConnection);
            }

            return newConnection;
        }
    }
}
