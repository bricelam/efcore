using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Microsoft.Data.Sqlite
{
    internal abstract class DbConnectionClosed : DbConnectionInternal
    {
        protected DbConnectionClosed()
            : base(ConnectionState.Closed, allowSetConnectionString: true)
        {

        }
    }
}
