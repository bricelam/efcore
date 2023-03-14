using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.Data.Core;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal static class ProviderRegistry
{
    private static readonly Guid _adoDotNetTechnology = new("77AB9A9D-78B9-4ba7-91AC-873F5338F1D2");

    private static readonly (string Name, string DbConnection)[] _providers = new[]
    {
        ("Microsoft.EntityFrameworkCore.SqlServer", "Microsoft.Data.SqlClient.SqlConnection"),
        ("Npgsql.EntityFrameworkCore.PostgreSQL", "Npgsql.NpgsqlConnection"),
        ("Pomelo.EntityFrameworkCore.MySql", "MySqlConnector.MySqlConnection"),
        ("Microsoft.EntityFrameworkCore.Sqlite", "Microsoft.Data.Sqlite.SqliteConnection"),
        ("Oracle.EntityFrameworkCore", "Oracle.ManagedDataAccess.Client.OracleConnection"),
        ("Devart.Data.Oracle.EFCore", "Devart.Data.Oracle.OracleConnection"),
        ("MySql.EntityFrameworkCore", "MySql.Data.MySqlClient.MySqlConnection"),
        ("Devart.Data.MySql.EFCore", "Devart.Data.MySql.MySqlConnection"),
        ("Devart.Data.PostgreSql.EFCore", "Devart.Data.PostgreSql.PgSqlConnection"),
        ("FirebirdSql.EntityFrameworkCore.Firebird", "FirebirdSql.Data.FirebirdClient.FbConnection"),
        ("IBM.EntityFrameworkCore", "IBM.Data.Db2.DB2Connection"),
        ("Google.Cloud.EntityFrameworkCore.Spanner", "Google.Cloud.Spanner.Data.SpannerConnection"),
        ("Devart.Data.SQLite.EFCore", "Devart.Data.SQLite.SQLiteConnection")
    };

    public static string[] GetEntityFrameworkCoreProviders()
        => _providers.Select(p => p.Name).ToArray();

    public static bool TryGetEntityFrameworkCoreProvider(IVsDataProvider provider, out string name)
    {
        name = null;

        if (provider.Technology != _adoDotNetTechnology)
        {
            return false;
        }

        var invariantName = provider.GetProperty("InvariantName") as string;
        if (invariantName is null)
        {
            return false;
        }

        DbProviderFactory providerFactory;
        try
        {
            providerFactory = DbProviderFactories.GetFactory(invariantName);
        }
        catch
        {
            return false;
        }

        string connectionTypeName;
        using (var connection = providerFactory.CreateConnection())
        {
            connectionTypeName = connection.GetType().FullName;
        }

        foreach (var item in _providers)
        {
            if (item.DbConnection == connectionTypeName)
            {
                name = item.Name;

                return true;
            }
        }

        // TODO: Use provider.GetProperty?
        return false;
    }
}
