using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Data.Sqlite
{
    internal sealed class DbConnectionClosedNeverOpened : DbConnectionClosed
    {
        internal static readonly DbConnectionInternal SingletonInstance = new DbConnectionClosedNeverOpened();

        private DbConnectionClosedNeverOpened()
            : base(allowSetConnectionString: true)
        {
        }
    }
}
