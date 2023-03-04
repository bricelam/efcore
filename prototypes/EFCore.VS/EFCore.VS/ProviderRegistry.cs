using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.Data.Core;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal static class ProviderRegistry
{
    private static readonly Guid _adoDotNetTechnology = new("77AB9A9D-78B9-4ba7-91AC-873F5338F1D2");

    private static readonly (string Name, string DbConnection)[] _providers = new[]
    {
        // TODO: Add known providers
        ("Microsoft.EntityFrameworkCore.SqlServer", "Microsoft.Data.SqlClient.SqlConnection"),
        ("Microsoft.EntityFrameworkCore.Sqlite", "Microsoft.Data.Sqlite.SqliteConnection"),
        ("Npgsql.EntityFrameworkCore.PostgreSQL", "Npgsql.NpgsqlConnection"),
        ("Oracle.EntityFrameworkCore", "Oracle.ManagedDataAccess.Client.OracleConnection"),
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

        return false;
    }
}
