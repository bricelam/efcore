// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using SQLitePCL;

namespace Microsoft.Data.Sqlite
{
    internal class SqlitePooledStatement
    {
        public byte[] Key { get; set; }
        public sqlite3_stmt Value { get; set; }

        public int TailLength { get; set; }
        // TODO: Times retrieved
        //       Last retrieved on
    }
}
