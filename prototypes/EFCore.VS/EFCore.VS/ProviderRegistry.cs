using System.Linq;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

internal static class ProviderRegistry
{
    private static readonly (string name, Guid VsDataProvider, string invariant)[] _providers = new[]
    {
        ("Microsoft.EntityFrameworkCore.SqlServer", new Guid("8800600A-ADD9-47E8-81D2-1D13B5A09C13"), "Microsoft.Data.SqlClient"),
        ("Microsoft.EntityFrameworkCore.Sqlite", new Guid("796A79E8-2579-4375-9E12-03A9E0D1FC02"), "Microsoft.Data.Sqlite"),
        ("Npgsql.EntityFrameworkCore.PostgreSQL", new Guid("70ba90f8-3027-4aF1-9b15-37abbd48744c"), "Npgsql"),
        ("Oracle.EntityFrameworkCore", new Guid("9D8FDBB9-EE60-4787-B7AE-49831D34AD4B"), "Oracle.ManagedDataAccess.Client"),

        // TODO: DevArt

        // TODO: No DDEX provider
        //IBM.EntityFrameworkCore
        //MySql.EntityFrameworkCore
        //FirebirdSql.EntityFrameworkCore.Firebird
        //Pomelo.EntityFrameworkCore.MySql
        //EntityFrameworkCore.Jet
        //Google.Cloud.EntityFrameworkCore.Spanner
    };

    public static bool IsVsDataProviderSupported(Guid provider)
        => _providers.Any(p => p.VsDataProvider == provider);
}
