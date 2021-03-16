using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Microsoft.Data.Sqlite
{
    internal abstract class DbConnectionInternal
    {
        //private readonly bool _allowSetConnectionString;

        //protected DbConnectionInternal()
        //    : this(ConnectionState.Open, allowSetConnectionString: false)
        //{
        //}

        protected DbConnectionInternal(ConnectionState state, bool allowSetConnectionString)
        {
            State = state;
            //_allowSetConnectionString = allowSetConnectionString;

        }

        public ConnectionState State { get; }

        public void OpenConnection(SqliteConnection outerConnection, SqliteConnectionFactory connectionFactory)
        {
            connectionFactory.TryGetConnection(outerConnection, this, out var openConnection);
            connectionFactory.SetInnerConnectionEvent(outerConnection, openConnection);
        }

        //protected SqliteConnectionStringBuilder? ConnectionOptions { get; }

        //public virtual string DataSource
        //    => ConnectionOptions.DataSource;
    }
}
