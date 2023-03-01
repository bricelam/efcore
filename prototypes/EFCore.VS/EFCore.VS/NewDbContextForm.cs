using System.Data.Common;
using System.Windows.Forms;
using Microsoft.VisualStudio.Data.Core;
using Microsoft.VisualStudio.Data.Services;

namespace Microsoft.EntityFrameworkCore.VisualStudio;

public partial class NewDbContextForm : Form
{
    public NewDbContextForm()
        => InitializeComponent();

    private void button1_Click(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        var dialogFactory = (IVsDataConnectionDialogFactory)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataConnectionDialogFactory));
        var providerManager = (IVsDataProviderManager)ServiceProvider.GlobalProvider.GetService(typeof(IVsDataProviderManager));

        var dialog = dialogFactory.CreateConnectionDialog();
        dialog.AddSources(
            (_, providerKey) =>
            {
                var provider = providerManager.Providers[providerKey];
                if (provider.Technology != new Guid("77AB9A9D-78B9-4BA7-91AC-873F5338F1D2"))
                {
                    return false;
                }

                var invariantName = provider.GetProperty("InvariantName") as string;
                if (invariantName is null)
                {
                    return false;
                }

                DbProviderFactory providerFactory = null;
                try
                {
                    providerFactory = DbProviderFactories.GetFactory(invariantName);
                }
                catch
                {
                }
                if (providerFactory is null)
                {
                    // TODO: Can we do something with just the invariant name?
                    return false;
                }

                string connectionType;
                using (var connection = providerFactory.CreateConnection())
                {
                    connectionType = connection.GetType().FullName;
                }

                // TODO: Match to an EF provider
                return true;
            });
        dialog.ShowDialog();

        label1.Text = dialog.SafeConnectionString;
    }
}
