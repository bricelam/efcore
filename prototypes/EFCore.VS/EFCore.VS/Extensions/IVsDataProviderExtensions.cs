using System.Data.Common;

namespace Microsoft.VisualStudio.Data.Core;

internal static class IVsDataProviderExtensions
{
    public static string GetConnectionType(this IVsDataProvider provider)
    {
        var invariantName = (string)provider.GetProperty("InvariantName");
        DbProviderFactory providerFactory;
        try
        {
            providerFactory = DbProviderFactories.GetFactory(invariantName);
        }
        catch
        {
            return null;
        }

        using (var connection = providerFactory.CreateConnection())
        {
            return connection.GetType().FullName;
        }
    }
}
