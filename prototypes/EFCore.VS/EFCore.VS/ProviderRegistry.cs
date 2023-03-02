namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal static class ProviderRegistry
{

    private static readonly (string name, string connectionType)[] _providers = new[]
    {
        // TODO: Add known providers
        ("Microsoft.EntityFrameworkCore.SqlServer", "Microsoft.Data.SqlClient.SqlConnection"),
        ("Microsoft.EntityFrameworkCore.Sqlite", "Microsoft.Data.Sqlite.SqliteConnection"),
        ("Npgsql.EntityFrameworkCore.PostgreSQL", "Npgsql.NpgsqlConnection"),
        ("Oracle.EntityFrameworkCore", "Oracle.ManagedDataAccess.Client.OracleConnection"),
    };

    public static bool TryGetForConnectionType(string connectionType, out string providerName)
    {
        foreach (var provider in _providers)
        {
            if (provider.connectionType == connectionType)
            {
                providerName = provider.name;

                return true;
            }
        }

        providerName = null;

        return false;
    }
}
