// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.Data.Sqlite
{
    internal class DbConnectionPoolGroupOptions
    {
        public DbConnectionPoolGroupOptions(
            bool poolByIdentity,
            int minPoolSize,
            int maxPoolSize,
            int connectionTimeout,
            int loadBalanceTimeout,
            bool hasTransactionAffinity)
        {
        }
    }
}
