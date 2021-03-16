using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using SQLitePCL;
using static SQLitePCL.raw;

namespace Microsoft.Data.Sqlite
{
    class SqliteConnectionOpen : DbConnectionInternal
    {
        private sqlite3 _db;

        public override string DataSource
            => sqlite3_db_filename(_db, SqliteConnection.MainDatabaseName).utf8_to_string()
                ?? base.DataSource;
    }
}
